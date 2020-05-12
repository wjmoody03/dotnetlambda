using System;
using System.Collections.Generic;
using System.Text;
using System.Xml.Linq;

namespace cXMLHandler.Parser.ConfirmationRequests
{
    /*
     * A document is one of the following types, specified by the type attribute of the ConfirmationHeader element:
        “accept,” “allDetail,” “detail,” “backordered,” “except,” “reject,” “requestToPay,” and “replace.” With
        a type of “detail”, you can update portions of a purchase order, such as prices, quantities, and delivery dates,
        reject portions, and add tax and shipping information. Only the line items mentioned are changed. With a type of
        “allDetail”, you can update all information of specified line items without rejecting or accepting the order. You
        can apply the confirmation to the entire order request using the types “accept”, “reject”, and “except”.
        “allDetail” and “detail” update individual lines, they do not accept or reject the entire order.
        A ConfirmationRequest with type=”requestToPay” invokes a payment service where the network hub
        requests a payment service provider to perform a point of sale transaction against the PCard listed in the purchase
        order and return the status of the transaction. The network hub then sends the transaction status back to the
        supplier in a StatusUpdateRequest document.
     */


    public static class ConfirmationGenerator
    {
        const string dateFormat = "yyyy-MM-ddThh:mm:ssK";
        public static string CreateConfirmationRequestFromOrderRequest(string orderRequestXml)
        {
            var doc = XDocument.Parse(orderRequestXml);

            var request = new XElement("ConfirmationRequest",
                new XElement("ConfirmationHeader",
                    new XAttribute("type", "accept"), //indicates that we're accepting the entire PO as sent
                    new XAttribute("noticeDate", DateTime.Now.ToString(dateFormat)), //required
                    new XAttribute("confirmID", "") //this is up to us to generate. secondary to payloadId.
                ),
                new XElement("OrderReference",
                    new XAttribute("orderID", ""),
                    new XElement("DocumentReference",
                        new XAttribute("payloadID", "")
                    )
                )
            );

            var envelope = new XElement("cXML",
                        new XAttribute("version", $"1.2.011"),
                        new XAttribute("payloadID", $""),
                        new XAttribute(XNamespace.Xml + "lang", "en"),
                        new XAttribute("timestamp", DateTime.Now.ToString(dateFormat)),
                        new XElement("Header",
                            new XElement("From",
                                new XElement("Credential",
                                    new XElement("Identity","")
                                )
                            ),
                            new XElement("To",
                                new XElement("Credential",
                                    new XElement("Identity", "")
                                )
                            ),
                            new XElement("Sender",
                                new XElement("Credential",
                                    new XElement("Identity", ""),
                                    new XElement("SharedSecret","")
                                ),
                                new XElement("UserAgent","")
                            )
                        ),
                        new XElement("Request",
                            request
                        )
                    );



            return envelope.ToString();
        }
    }
}


/*
 * THESE CAN ALL BE OPTIONALLY ADDED TO THE ConfirmationHeader
 * 
 *                     //new XAttribute("invoiceID",""), //optional: we don't know yet, so leave it out
                    //new XElement("Shipping",
                    //    new XElement("Money",
                    //        new XAttribute("currency","USD")
                    //    ),
                    //    new XElement("Description",
                    //        new XAttribute(XNamespace.Xml + "lang", "en-US"),
                    //        "Shipping comments here"
                    //    )
                    //),
                    //new XElement("Tax",
                    //    new XElement("Money",
                    //        new XAttribute("currency", "USD")
                    //    ),
                    //    new XElement("Description",
                    //        new XAttribute(XNamespace.Xml + "lang", "en-US"),
                    //        "Shipping comments here"
                    //    )
                    //),
                    //new XElement("Total",
                    //    new XElement("Money",
                    //        new XAttribute("currency", "USD")
                    //    ),
                    //    "totalamount"
                    //),
                    //new XElement("Contact",
                    //    new XAttribute("role","shipFrom"),
                    //    new XElement("Name",
                    //        new XAttribute(XNamespace.Xml + "lang", "en-US"),
                    //        "Contact name here"
                    //    ),
                    //    new XElement("PostalAddress",
                    //        new XElement("Street", ""),
                    //        new XElement("City", ""),
                    //        new XElement("State", ""),
                    //        new XElement("PostalCode", ""),
                    //        new XElement("Country",
                    //            new XAttribute("isoCountryCode", "US"),
                    //            "US"
                    //        )
                    //    ),
                    //    new XElement("Phone",
                    //        new XElement("TelephoneNumber",
                    //            new XElement("CountryCode",
                    //                new XAttribute("isoCountryCode","US"),
                    //                "US"),
                    //            new XElement("AreaOrCityCode",""),
                    //            new XElement("Number","")
                    //        )
                    //    )
                    //),
                    //new XElement("Comments",
                    //    new XAttribute(XNamespace.Xml + "lang", "en-US"),
                    //    "Insert comments here"
                    //)
 * 
 */
