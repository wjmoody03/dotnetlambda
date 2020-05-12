using cXMLHandler.Parser.StatusUpdateRequest;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Xunit;

namespace cXMLHandler.Tests.Parser.StatusUpdateRequest
{
    public class StatusUpdateRequestGeneratorTests
    {
        [Fact]
        public void status_update_request_is_generated()
        {
            var xml = StatusUpdateRequestGenerator.GenerateOkStatusRequest("2000001368.80.1561657766366@vroozi.com");

        }
    }
}
