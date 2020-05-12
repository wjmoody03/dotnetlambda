using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Xml;
using System.Xml.Linq;
using System.Xml.Schema;
using System.Xml.Serialization;

namespace cXMLHandler.Parser
{
    public class XMLHelpers
    {
        //this serializes the cXML classes with the proper doctype and xml declarations
        public static string SerializeObject<T>(T obj, string cXMLVersion)
        {
            //first, just serialize the xml like normal
            byte[] bytes;
            var utf8NoBom = new UTF8Encoding(false);
            var settings = new XmlWriterSettings
            {
                Indent = true,
                Encoding = utf8NoBom
            };

            using (MemoryStream ms = new MemoryStream())
            {
                using (var xmlWriter = XmlWriter.Create(ms, settings))
                {
                    XmlSerializer xmlSer = new XmlSerializer(typeof(T));
                    xmlSer.Serialize(xmlWriter, obj);
                    bytes = ms.ToArray();
                }
            }

            //now we need to remove the namespace delarations
            var doc = XDocument.Parse(utf8NoBom.GetString(bytes));
            doc.Descendants().Attributes().Where(a => a.IsNamespaceDeclaration).Remove();

            foreach (var element in doc.Descendants())
            {
                element.Name = element.Name.LocalName;
            }
            var xmlWithoutNamespaces = doc.ToString();

            //now add in the expected declarations
            string declaration = "<?xml version=\"1.0\" encoding=\"UTF-8\"  standalone=\"no\"?>";
            string docType = $"<!DOCTYPE cXML SYSTEM \"http://xml.cxml.org/schemas/cXML/{cXMLVersion}/InvoiceDetail.dtd\">";

            xmlWithoutNamespaces = $"{declaration}\r\n{docType}\r\n{xmlWithoutNamespaces}";

            return xmlWithoutNamespaces;

        }

    }
}
