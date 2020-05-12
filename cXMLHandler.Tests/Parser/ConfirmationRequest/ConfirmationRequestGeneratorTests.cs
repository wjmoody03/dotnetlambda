using cXMLHandler.Parser.ConfirmationRequests;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using Xunit;

namespace cXMLHandler.Tests.Parser.ConfirmationRequest
{
    public class ConfirmationRequestGeneratorTests
    {
        string sampleOrderRequestXml;

        public ConfirmationRequestGeneratorTests()
        {
            sampleOrderRequestXml = new StreamReader(
                    Assembly
                    .GetAssembly(typeof(ConfirmationRequestGeneratorTests))
                    .GetManifestResourceStream("cXMLHandler.Tests.Parser.ConfirmationRequest.SampleOrderRequest.xml")
                ).ReadToEnd();
        }

        [Fact]
        public void confirmation_request_can_be_generated_from_order_request()
        {
            var xml = ConfirmationGenerator.CreateConfirmationRequestFromOrderRequest(sampleOrderRequestXml);   
        }
    }
}
