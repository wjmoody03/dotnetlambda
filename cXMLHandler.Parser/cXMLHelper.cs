using cXMLHandler.Parser.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;
using System.Xml.Linq;

namespace cXMLHandler.Parser
{
    public class cXMLHelper
    {
        static string dateTimeFormat = "yyyy-MM-ddThh:mm:ssK";


        public static string CreateEnvelopeWithCredentials(XElement requestContent)
        {
            var envelope = new XElement("cXML",
                        new XAttribute("version", $"1.2.011"),
                        new XAttribute("payloadID", $"{DateTime.Now.ToString("yyyyMMddhhmmss")}.0.{new Random().Next(0,1000000)}@bargreen.com"),   //datetime.process id.random number@hostname
                        new XAttribute(XNamespace.Xml + "lang", "en-US"),
                        new XAttribute("timestamp", DateTime.Now.ToString(dateTimeFormat)),
                        new XElement("Header",
                            CXmlCredential("From","BARGREEN ELLINGSON"),
                            CXmlCredential("To","H.E.B"),
                            CXmlCredential("Sender","43556916", "BP/SYEdtFDXg/R1PqQsRQ==", "BARGREEN ELLINGSON")
                        ),
                        new XElement("Request",
                            requestContent
                        )
                    );

            string declaration = "<?xml version=\"1.0\" encoding=\"UTF-8\"?>";
            string docType = $"<!DOCTYPE cXML SYSTEM \"http://xml.cxml.org/schemas/cXML/1.2.011/cXML.dtd\">";
            string xml = $"{declaration}\r\n{docType}\r\n{envelope.ToString()}";
            return xml;
        }
        private static XElement CXmlCredential(string credentialType, string identity, string sharedSecret = null, string userAgent = null)
        {

            var body = new XElement(credentialType,
                new XElement("Credential", 
                    new XAttribute("domain", "NetworkId"),
                    new XElement("Identity",identity)
                )
            );            

            if (!string.IsNullOrWhiteSpace(sharedSecret))
            {
                var cred = body.Elements().First();
                cred.Add(new XElement("SharedSecret", sharedSecret));
            }
            if (!string.IsNullOrEmpty(userAgent))
            {
                body.Add(new XElement("UserAgent", userAgent));
            }
            return body;

        }

        //use this tool to validate output: https://punchoutcommerce.com/tools/xml-validator
        public static string cXMLInvoice(Invoice invoice)
        {
            var sampleInvoice = new cxml1_2_025.cXML() { timestamp = invoice.InvoiceDate.ToString(dateTimeFormat), payloadID = invoice.InvoiceID, lang = "en-US" };

            var invoiceHeader = new cxml1_2_025.Header()
            {
                From = new cxml1_2_025.From() { Credential = GetCredential("BARGREEN ELLINGSON") },
                To = new cxml1_2_025.To() { Credential = GetCredential("H.E.B") },
                Sender = new cxml1_2_025.Sender() { Credential = GetCredential("43556916", sharedSecret: "BP/SYEdtFDXg/R1PqQsRQ=="), UserAgent = "BARGREEN ELLINGSON" },
            };

            var request = new cxml1_2_025.Request()
            {
                deploymentMode = cxml1_2_025.RequestDeploymentMode.test,
                Item = new cxml1_2_025.InvoiceDetailRequest()
                {
                    InvoiceDetailRequestHeader = new cxml1_2_025.InvoiceDetailRequestHeader()
                    {
                        invoiceID = invoice.InvoiceID,
                        purpose = cxml1_2_025.InvoiceDetailRequestHeaderPurpose.standard,
                        operation = cxml1_2_025.InvoiceDetailRequestHeaderOperation.@new,
                        invoiceDate = invoice.InvoiceDate.ToString(dateTimeFormat),
                        invoiceOrigin = cxml1_2_025.InvoiceDetailRequestHeaderInvoiceOrigin.supplier,
                        DocumentReference = new cxml1_2_025.DocumentReference() { payloadID = "payloadIdOfAssociatedPO" },
                        InvoicePartner = new cxml1_2_025.InvoicePartner[] {
                            MakeInvoicePartner("issuerOfInvoice","BARGREEN ELLINGSON", address1: "6626 Tacoma Mall Blvd suite b", city:"Tacoma", state:"WA", zip:"98409",isoCountryCode:"US"),
                            MakeInvoicePartner("billTo","HEB Corporate", address1:"646 S FLORES ST", city:"SAN ANTONIO", state:"TX", zip:"78204-1219",isoCountryCode:"US", email:"info@heb.com"),
                            //MakeInvoicePartner("shipTo","HEB Corporate", address1:"646 S FLORES ST", city:"SAN ANTONIO", state:"TX", zip:"78204-1219",isoCountryCode:"US", email:"info@heb.com")
                        },
                        InvoiceDetailShipping = new cxml1_2_025.InvoiceDetailShipping()
                        {
                            shippingDate = DateTime.Today.ToString(dateTimeFormat),
                            Contact1 = new cxml1_2_025.Contact[]{
                                MakeContact("shipFrom","BARGREEN ELLINGSON"),
                                MakeContact("shipTo","you")
                            }                            
                        },
                        //Extrinsic = new cxml1_2_025.Extrinsic[]
                        //{
                        //    new cxml1_2_025.Extrinsic(){ name = "n/a"}
                        //},
                        Items = new object[]
                        {
                            new cxml1_2_025.PaymentTerm(){ payInNumberOfDays = "30"}
                        },
                        InvoiceDetailHeaderIndicator = new cxml1_2_025.InvoiceDetailHeaderIndicator() { isHeaderInvoice = new cxml1_2_025.InvoiceDetailHeaderIndicatorIsHeaderInvoice() },
                        InvoiceDetailLineIndicator = new cxml1_2_025.InvoiceDetailLineIndicator()
                        {
                            isTaxInLine = new cxml1_2_025.InvoiceDetailLineIndicatorIsTaxInLine()
                        }
                    },
                    Items = new object[]
                    {
                        new cxml1_2_025.InvoiceDetailOrder(){
                          InvoiceDetailOrderInfo = new cxml1_2_025.InvoiceDetailOrderInfo()
                          {
                              Items = new object[]
                              {
                                  new cxml1_2_025.OrderReference()
                                  {
                                       orderID = invoice.CustomerOrderNumber,
                                       DocumentReference = new cxml1_2_025.DocumentReference(){ payloadID = invoice.InvoiceID },
                                       orderDate = DateTime.Today.ToString(dateTimeFormat)
                                  },
                                  new cxml1_2_025.OrderIDInfo()
                                  {
                                       orderID = invoice.CustomerOrderNumber, 
                                       orderDate = invoice.OriginalOrderDate.ToString(dateTimeFormat)
                                  },
                                  new cxml1_2_025.SupplierOrderInfo(){
                                        orderID = invoice.CustomerOrderNumber
                                  }
                              }
                          },
                          Items = invoice.InvoiceLines.Select(l => new cxml1_2_025.InvoiceDetailItem()
                            {
                                InvoiceDetailItemReference = new cxml1_2_025.InvoiceDetailItemReference()
                                {
                                     Description = new cxml1_2_025.Description(){ lang="en-US", Text = new string[]{l.Description } },
                                     ItemID = new cxml1_2_025.ItemID(){
                                         SupplierPartID = l.ItemID,
                                         SupplierPartAuxiliaryID = new cxml1_2_025.SupplierPartAuxiliaryID(){ Any = TextNodes(l.ItemID)}
                                     },
                                     lineNumber = l.LineNumber.ToString()
                                },
                                //GrossAmount = HasMoney<cxml1_2_025.GrossAmount>(invoice.Currency, l.GrossAmount),
                                invoiceLineNumber = l.LineNumber.ToString(),
                                //NetAmount = HasMoney<cxml1_2_025.NetAmount>(invoice.Currency, l.NetAmount),
                                quantity = l.Quantity.ToString(),
                                SubtotalAmount = HasMoney<cxml1_2_025.SubtotalAmount>(invoice.Currency,l.SubtotalAmount),
                                UnitPrice =  HasMoney<cxml1_2_025.UnitPrice>(invoice.Currency,l.Price),
                                UnitOfMeasure = "EA"

                            }).ToArray()
                        }
                    },
                    InvoiceDetailSummary = new cxml1_2_025.InvoiceDetailSummary()
                    {
                        SubtotalAmount = HasMoney<cxml1_2_025.SubtotalAmount>(invoice.Currency, invoice.SubtotalAmount),
                        NetAmount = HasMoney<cxml1_2_025.NetAmount>(invoice.Currency, invoice.NetAmount),
                        ShippingAmount = HasMoney<cxml1_2_025.ShippingAmount>(invoice.Currency, invoice.ShippingAmount),
                        GrossAmount = HasMoney<cxml1_2_025.GrossAmount>(invoice.Currency, invoice.GrossAmount),
                        DueAmount = HasMoney<cxml1_2_025.DueAmount>(invoice.Currency, invoice.DueAmount),
                        Tax = new cxml1_2_025.Tax()
                        {
                            Description = new cxml1_2_025.Description() { lang = "en-US", Text = new string[] { "Tax" } },
                            Money = new cxml1_2_025.Money() { currency = invoice.Currency, Value = invoice.TaxAmount.ToString() }, //this is the sum of all different kinds of taxes applied to the order
                            TaxDetail = new cxml1_2_025.TaxDetail[] //this should contain an item for each specific tax type. eg, county, city and state sales tax. use tax, etc. 
                            {
                                new cxml1_2_025.TaxDetail(){
                                    Description = new cxml1_2_025.Description() { lang = "en-US", Text= new string[] { "Tax" } },
                                    TaxAmount = HasMoney<cxml1_2_025.TaxAmount>(invoice.Currency, invoice.TaxAmount),
                                    purpose="tax",
                                    category = "sales"
                                }
                            },
                            Extrinsic = new cxml1_2_025.Extrinsic[]
                            {
                                new cxml1_2_025.Extrinsic(){ name = "geocode"}
                            }
                        }
                    }
                }
            };

            sampleInvoice.Items = new object[] { invoiceHeader, request };
            return XMLHelpers.SerializeObject<cxml1_2_025.cXML>(sampleInvoice, "1.2.025");
        }

        public static XmlNode[] TextNodes(params string[] text)
        {
            return text.Select(t => TextNode(t)).ToArray();
        }

        public static XmlNode TextNode(string text)
        {
            XmlDocument doc = new XmlDocument();
            var node = doc.CreateNode(XmlNodeType.Text, null, null);
            node.InnerText = text;
            return node;
        }

        public static cxml1_2_025.Credential[] GetCredential(string identity, string domain = "NetworkID", string sharedSecret = null)
        {

            var cred = new cxml1_2_025.Credential()
            {
                domain = domain,
                Identity = new cxml1_2_025.Identity
                {
                    Any = new XmlNode[] { TextNode(identity) }
                }
            };
            if (sharedSecret != null)
            {
                cred.Item = new cxml1_2_025.SharedSecret() { Any = TextNodes(sharedSecret) };
            }
            return new cxml1_2_025.Credential[]
            {
               cred
            };
        }

        public static T HasMoney<T>(string ccy, decimal amount) where T : cxml1_2_025.IHasMoney, new()
        {
            return new T() { Money = new cxml1_2_025.Money() { currency = ccy, Value = amount.ToString() } };
        }

        public static cxml1_2_025.Contact MakeContact(string role, string name, string phoneNumber = null, string areaCode = null, string phoneCountryCode = null, string isoCountryCode = null,
    string address1 = null, string address2 = null, string city = null, string state = null, string zip = null, string email = null)
        {

            var contact = new cxml1_2_025.Contact()
            {
                Name = new cxml1_2_025.Name() { Value = name, lang = "en-US" },
                role = role
            };

            if (email != null)
            {
                contact.Email = new cxml1_2_025.Email[]
                {
                    new cxml1_2_025.Email(){ Value=email }
                };
            }

            if (phoneNumber != null)
            {
                contact.Phone = new cxml1_2_025.Phone[]{
                                        new cxml1_2_025.Phone() {
                                            name = name,
                                            TelephoneNumber =  new cxml1_2_025.TelephoneNumber() {
                                                Number = phoneNumber,
                                                AreaOrCityCode = areaCode,
                                                CountryCode = new cxml1_2_025.CountryCode(){ Value = phoneCountryCode, isoCountryCode = isoCountryCode}
                                            }
                                        }
                                    };
            }

            if (address1 != null)
            {
                contact.PostalAddress = new cxml1_2_025.PostalAddress[]
                {
                    new cxml1_2_025.PostalAddress()
                    {
                         Street = new string[]{ address1, address2 },
                         City = city,
                         State = state,
                         PostalCode = zip,
                         Country = new cxml1_2_025.Country(){ isoCountryCode = isoCountryCode }
                    }
                };
            }

            return contact;
        }

        public static cxml1_2_025.InvoicePartner MakeInvoicePartner(string role, string name, string phoneNumber = null, string areaCode = null, string phoneCountryCode = null, string isoCountryCode = null,
            string address1 = null, string address2 = null, string city = null, string state = null, string zip = null, string email = null)
        {

            var contact = MakeContact(role, name, phoneNumber, areaCode, phoneCountryCode, isoCountryCode, address1, address2, city, state, zip, email);

            var partner = new cxml1_2_025.InvoicePartner() { Contact = contact };

            return partner;
        }
    }
}
