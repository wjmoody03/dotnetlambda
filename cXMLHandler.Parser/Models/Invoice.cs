using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace cXMLHandler.Parser.Models
{
    public class Invoice
    {
        public string InvoiceID { get; set; }
        public DateTime InvoiceDate { get; set; }
        public DateTime OriginalOrderDate { get; set; }
        public string CustomerOrderReference { get; set; }
        public string CustomerOrderNumber { get; set; }
        public string Currency { get; set; }
        public decimal TaxAmount { get; set; }
        public decimal ShippingAmount { get; set; }
        public IEnumerable<InvoiceLineItem> InvoiceLines { get; set; }

        public decimal SubtotalAmount
        {
            get
            {
                return InvoiceLines.Sum(l => l.SubtotalAmount);
            }
        }

        public decimal GrossAmount
        {
            get
            {
                return SubtotalAmount;
            }
        }

        public decimal NetAmount
        {
            get
            {
                return GrossAmount + TaxAmount + ShippingAmount;
            }
        }

        public decimal DueAmount
        {
            get
            {
                return NetAmount;
            }
        }
    }
}
