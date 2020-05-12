using System;
using System.Collections.Generic;
using System.Text;
using System.Xml.Linq;

namespace cXMLHandler.Parser.InvoiceRequest
{
    class InvoiceRequestGenerator
    {
        public static string GenerateInvoiceRequest(string originalPayloadId)
        {
            var update = new XElement("StatusUpdateRequest",
                new XElement("DocumentReference", new XAttribute("payloadID", originalPayloadId)),
                new XElement("Status",
                    new XAttribute("code", "200"),
                    new XAttribute("text", "OK"),
                    new XAttribute(XNamespace.Xml + "lang", "en-US"),
                    "Processed"
                )
            );

            return cXMLHelper.CreateEnvelopeWithCredentials(update);
        }
    }
}
