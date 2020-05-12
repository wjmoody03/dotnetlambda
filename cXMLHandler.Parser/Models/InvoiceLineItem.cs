using System;
using System.Collections.Generic;
using System.Text;

namespace cXMLHandler.Parser.Models
{
    public class InvoiceLineItem
    {
        public int Quantity { get; set; }
        public decimal Price { get; set; }
        public int LineNumber { get; set; }
        public string ItemID { get; set; }
        public string Description { get; set; }

        public decimal SubtotalAmount {
            get
            {
                return Quantity * Price;
            }
        }

        public decimal GrossAmount
        {
            get
            {
                return Quantity * Price;
            }
        }

        public decimal NetAmount
        {
            get
            {
                return Quantity * Price;
            }
        }

    }
}
