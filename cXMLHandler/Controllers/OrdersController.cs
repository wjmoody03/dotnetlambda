using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

using Amazon.S3;
using Amazon.S3.Model;

using Newtonsoft.Json;
using System.Xml.Linq;
using System.Net;
using System.Text;
using Amazon.SimpleEmail;

namespace cXMLHandler.Controllers
{
    /// <summary>
    /// ASP.NET Core controller accepting cXML purchase orders from customers.
    /// </summary>
    [Route("api/[controller]")]
    public class OrdersController : Controller
    {
        IAmazonS3 S3Client { get; set; }
        ILogger Logger { get; set; }
        IEnvironmentSettings Settings { get; set; }
        IAmazonSimpleEmailService EmailClient { get; set; }

        public OrdersController(IConfiguration configuration, ILogger<OrdersController> logger, IAmazonS3 s3Client, IEnvironmentSettings settings, IAmazonSimpleEmailService emailClient)
        {
            this.Logger = logger;
            this.S3Client = s3Client;
            this.Settings = settings;
            this.EmailClient = emailClient;
        }

        [HttpPost]
        public async Task Post()
        {
            var cxml = new StreamReader(this.Request.Body).ReadToEnd();

            await SaveCXMLToS3(cxml);

            await EmailCXMLContents(cxml);

            var orderResponse = new XElement("cXML",
                    new XAttribute("version", $"1.2.011"),
                    new XAttribute("payloadID", $"{DateTime.Now.ToString("yyyyMMddhhmmss.1.fff")}@bargreen.com"),
                    new XAttribute(XNamespace.Xml + "lang", "en"),
                    new XAttribute("timestamp", DateTime.Now.ToString("yyyy-MM-ddThh:mm:ssK")),
                    new XElement("Response",
                        new XElement("Status",
                            new XAttribute("code", "201"),
                            new XAttribute("text", "Accepted")
                        )
                    )
                );

            string declaration = "<?xml version=\"1.0\" encoding=\"UTF-8\"  standalone=\"no\"?>";
            string docType = $"<!DOCTYPE cXML SYSTEM \"http://xml.cXML.org/schemas/cXML/1.2.011/cXML.dtd\">";
            string xml = $"{declaration}\r\n{docType}\r\n{orderResponse.ToString()}";

            this.Response.ContentType = "text/xml";
            this.Response.StatusCode = (int)HttpStatusCode.Created;
            var utf8bytes = System.Text.Encoding.UTF8.GetBytes(xml);
            this.Response.ContentLength = utf8bytes.Length;
            this.Response.Body.Write(utf8bytes, 0, utf8bytes.Length);

        }

        private async Task SaveCXMLToS3(string cXML)
        {
            string key = $"{Settings.BucketPrefix}/{DateTime.Now.ToString("yyyyMMddhhmmss")}";
            var putRequest = new PutObjectRequest
            {
                BucketName = Settings.S3BucketName,
                Key = key,
                ContentBody = cXML
            };

            try
            {
                var response = await this.S3Client.PutObjectAsync(putRequest);
                Logger.LogInformation($"Uploaded object {key} to bucket {Settings.S3BucketName}. Request Id: {response.ResponseMetadata.RequestId}");
            }
            catch (AmazonS3Exception e)
            {
                this.Response.StatusCode = (int)e.StatusCode;
                var writer = new StreamWriter(this.Response.Body);
                writer.Write(e.Message);
            }
        }

        private async Task EmailCXMLContents(string cxml)
        {
            await Email.SendEmail(EmailClient, Logger, "cXML Order Received", "cxmlorders@bargreen.io", new List<string>() { "jmoody@bargreen.io" }, cxml);
        }
    }
}
