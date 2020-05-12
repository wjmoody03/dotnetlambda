using Amazon.SimpleEmail;
using Amazon.SimpleEmail.Model;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading.Tasks;

namespace cXMLHandler
{
    public class Email
    {
        public class Attachment
        {
            public byte[] Contents { get; set; }
            public string FileName { get; set; }
        }

        public class EmailAddress
        {
            public string Name { get; set; }
            public string Email { get; set; }
        }
        public static async Task SendEmail(IAmazonSimpleEmailService client, ILogger logger, string subject, string fromAddress, List<string> toAddresses, string htmlBody)
        {
            var sendRequest = new SendEmailRequest()
            {
                Source = fromAddress,
                Destination = new Destination()
                {
                    ToAddresses = toAddresses
                },
                Message = new Message()
                {
                    Subject = new Content(subject),
                    Body = new Body() { Html = new Content(htmlBody) }
                }
            };
            var mailId = Guid.NewGuid().ToString().Substring(0, 4).ToLower(); //this ensures that we can track the email in the logs in case the log messages aren't contiguous
            logger.LogInformation($"Sending email via ses. mailid={mailId} Recipients={string.Join(",", toAddresses)}");
            var response = await client.SendEmailAsync(sendRequest).ConfigureAwait(false);
            logger.LogInformation($"Completed sending email via ses. mailid={mailId}");
        }
    }
}
