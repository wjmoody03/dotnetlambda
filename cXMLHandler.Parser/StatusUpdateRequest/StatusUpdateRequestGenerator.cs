using System;
using System.Collections.Generic;
using System.Text;
using System.Xml.Linq;

namespace cXMLHandler.Parser.StatusUpdateRequest
{
    public static class StatusUpdateRequestGenerator
    {
        /*
         *  <StatusUpdateRequest>
                <DocumentReference
                    payloadID="0c300508b7863dcclb_14999"/>
                <Status code="200" text="OK" xml:lang="en-US">Forwarded
                    to supplier</Status>
            </StatusUpdateRequest>
         */

        public static string GenerateOkStatusRequest(string originalPayloadId)
        {
            var update = new XElement("StatusUpdateRequest",
                new XElement("DocumentReference", new XAttribute("payloadID",originalPayloadId)),
                new XElement("Status", 
                    new XAttribute("code","200"),
                    new XAttribute("text","OK"),
                    new XAttribute(XNamespace.Xml + "lang", "en-US"),
                    "Processed"
                )
            );

            return cXMLHelper.CreateEnvelopeWithCredentials(update);
        }

    }
}
