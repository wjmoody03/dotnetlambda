using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace cXMLHandler
{
    public class M3Order
    {
        public string CustomerNumber { get; set; }
        public string CustomerPONumber { get; set; }
        public DateTime DeliveryDate { get; set; }
        public string OrderType { get; set; }
        public int DeliveryAddressID { get; set; }
        public string Facility { get; set; }

        public IEnumerable<M3OrderLine> OrderLines { get; set; }

        public static M3Order ParseCXMLOrder(string orderRequest, out string payloadId)
        {
            var doc = XDocument.Parse(orderRequest);

            payloadId = doc.Element("cXML").Attribute("payloadID").Value;
            var orderRequestElement = doc.Descendants("OrderRequest").First();

            var order = new M3Order()
            {
                //Facility = config.CustomerFacilityForM3Orders,
                CustomerPONumber = orderRequestElement.Element("OrderRequestHeader").Attribute("orderID").Value,
                //OrderType = "ST",
                OrderLines = orderRequestElement.Descendants("ItemOut").Select(ol => new M3OrderLine()
                {
                    ItemNumber = ol.Element("ItemID").Element("SupplierPartID").Value,
                    Quantity = decimal.Parse(ol.Attribute("quantity").Value),
                    Description = ol.Element("ItemDetail").Element("Description").Element("ShortName").Value,
                    Price = decimal.Parse(ol.Element("ItemDetail").Element("UnitPrice").Element("Money").Value)
                }).ToList()
            };

            return order;
        }

    }

    public class M3OrderLine
    {
        public string ItemNumber { get; set; }
        public decimal Quantity { get; set; }

        //these should go away after testing
        public string Description { get; set; }
        public decimal Price { get; set; }


    }

}
