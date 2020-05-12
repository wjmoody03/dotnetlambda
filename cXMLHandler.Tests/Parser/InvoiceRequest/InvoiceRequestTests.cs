using cXMLHandler.Parser;
using cXMLHandler.Parser.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Xunit;

namespace cXMLHandler.Tests.Parser.InvoiceRequest
{
    public class InvoiceRequestTests
    {
        [Fact]
        public void generate_invoice_hawthorn()
        {
            var invoice = new Invoice()
            {
                Currency = "USD",
                CustomerOrderNumber = "20010000",
                CustomerOrderReference = "HSLG110032839",
                InvoiceDate = DateTime.Today,
                InvoiceID = "Sample_123",
                ShippingAmount = 0,
                TaxAmount = 0,
                InvoiceLines = new List<InvoiceLineItem>()
                 {
                     new InvoiceLineItem()
                     {
                         Description = "Diversified Ceramics AE01WH Shirred Egg, White, Ceramic - 8 oz",
                         ItemID = "DIVAE01WH",
                         LineNumber = 1,
                         Price = 176.42M,
                         Quantity = 1
                     },
                     new InvoiceLineItem()
                     {
                         Description = "Chef Revival 700-BRT White Towel, Cotton - 16\" x 19\" *Discon*",
                         ItemID = "BVU700BRT24",
                         LineNumber = 2,
                         Price = 12.59M,
                         Quantity = 1
                     },
                    new InvoiceLineItem()
                     {
                         Description = "Rubbermaid FGQ63006 BL00 Hygen Microfiber Glass/Mirror Cloth - 16\" x 16\"",
                         ItemID = "RUBQ63006",
                         LineNumber = 3,
                         Price = 6.48M,
                         Quantity = 1
                     },
                    new InvoiceLineItem()
                     {
                         Description = "Cardinal L5755 Chef & Sommelier Sequence Cooler Glass - 16 oz",
                         ItemID = "CARL5755",
                         LineNumber = 4,
                         Price = 47.23M,
                         Quantity = 1
                     },
                    new InvoiceLineItem()
                     {
                         Description = "Anchor Hocking 77796 Clarisse Cooler Glass - 16 oz",
                         ItemID = "ANC77796",
                         LineNumber = 5,
                         Price = 153.99M,
                         Quantity = 1
                     }
                 }
            };

            var xml = cXMLHelper.cXMLInvoice(invoice);
            Console.Write(xml);
        }

        [Fact]
        public void generate_invoice_heb()
        {
            var invoice = new Invoice()
            {
                Currency = "USD",
                CustomerOrderNumber = "2000001368",
                CustomerOrderReference = "HSLG110032839",
                InvoiceDate = DateTime.Today,
                InvoiceID = "Sample_123",
                ShippingAmount = 0,
                TaxAmount = 7.17M,
                InvoiceLines = new List<InvoiceLineItem>()
                 {
                     new InvoiceLineItem()
                     {
                         Description = "PER QUOTE SMALLWARES",
                         ItemID = "1206488",
                         LineNumber = 1,
                         Price = 0.01M,
                         Quantity = 3
                     },
                     new InvoiceLineItem()
                     {
                         Description = "LID SALT & PEPPER SHAKER SS TOPS",
                         ItemID = "123275",
                         LineNumber = 2,
                         Price = 7.2M,
                         Quantity = 2
                     },
                    new InvoiceLineItem()
                     {
                         Description = "PAN 1/4 SIZE 4IN DEEP STEAM TABLE PAN STAINLESS STEEL",
                         ItemID = "115478",
                         LineNumber = 3,
                         Price = 14.5M,
                         Quantity = 5
                     }
                 }
            };

            var xml = cXMLHelper.cXMLInvoice(invoice);
            Console.Write(xml);
        }
    }
}
