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

        string BucketName { get; set; }

        public OrdersController(IConfiguration configuration, ILogger<OrdersController> logger, IAmazonS3 s3Client)
        {
            this.Logger = logger;
            this.S3Client = s3Client;

            this.BucketName = configuration[Startup.AppS3BucketKey];
            if(string.IsNullOrEmpty(this.BucketName))
            {
                logger.LogCritical("Missing configuration for S3 bucket. The AppS3Bucket configuration must be set to a S3 bucket.");
                throw new Exception("Missing configuration for S3 bucket. The AppS3Bucket configuration must be set to a S3 bucket.");
            }

            logger.LogInformation($"Configured to use bucket {this.BucketName}");
        }

        [HttpPost]
        public async Task Post()
        {
            // Copy the request body into a seekable stream required by the AWS SDK for .NET.
            var seekableStream = new MemoryStream();
            await this.Request.Body.CopyToAsync(seekableStream);
            seekableStream.Position = 0;

            string key = DateTime.Now.ToString("yyyyMMddhhmmss");
            var putRequest = new PutObjectRequest
            {
                BucketName = this.BucketName,
                Key = key,
                InputStream = seekableStream
            };

            try
            {
                
                var response = await this.S3Client.PutObjectAsync(putRequest);
                Logger.LogInformation($"Uploaded object {key} to bucket {this.BucketName}. Request Id: {response.ResponseMetadata.RequestId}");

                var orderResponse = new XElement("cXML",
                        new XAttribute("version", $"1.2.011"),
                        new XAttribute("payloadID", $"order_{key}"),
                        new XAttribute(XNamespace.Xml + "lang", "en"),
                        new XAttribute("timestamp", DateTime.Now.ToString("yyyy-MM-ddThh:mm:ssK")),
                        new XElement("Response",
                            new XElement("Status",
                                new XAttribute("code", "200"),
                                new XAttribute("text", "success")
                            )
                        )
                    );

                string declaration = "<?xml version=\"1.0\" encoding=\"UTF-8\"  standalone=\"no\"?>";
                string docType = $"<!DOCTYPE cXML SYSTEM \"http://xml.cxml.org/schemas/cXML/1.2.0.11/cXML.dtd\">";
                string xml = $"{declaration}\r\n{docType}\r\n{orderResponse.ToString()}";

                this.Response.StatusCode = 200;
                this.Response.ContentType = "text/xml";
                this.Response.StatusCode = (int)HttpStatusCode.OK;                
                var utf8bytes = System.Text.Encoding.UTF8.GetBytes(xml);
                var utf16bytes = System.Text.Encoding.Convert(Encoding.UTF8, Encoding.Unicode, utf8bytes);
                this.Response.ContentLength = utf16bytes.Length;
                this.Response.Body.Write(utf16bytes, 0, utf16bytes.Length);
            }
            catch (AmazonS3Exception e)
            {
                this.Response.StatusCode = (int)e.StatusCode;
                var writer = new StreamWriter(this.Response.Body);
                writer.Write(e.Message);
            }
        }

    }
}
