using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Xml;
using System.Xml.Schema;

namespace cXMLTester
{
    public class XMLValidator
    {

        private XMLValidationResult result = new XMLValidationResult();
        public class XMLValidationResult
        {
            public bool IsValid { get { return Errors == null; } }
            public string Errors { get; set; }
        }

        public XMLValidationResult ValidateXML(string xml)
        {
            //get an xsd reader
            //string xsd = new StreamReader(System.Reflection.Assembly.GetExecutingAssembly().GetManifestResourceStream("cXMLTester.XMLSchemas.InvoiceDetail.1.2.025.InvoiceDetail.1.2.024.xsd")).ReadToEnd();
            string xsd = File.ReadAllText(@"C:\Users\jmoody\source\repos\cXMLTester\cXMLTester\XMLSchemas\InvoiceDetail\1.2.025\InvoiceDetail.1.2.025.xsd");
            XmlReaderSettings xsdSettings = new XmlReaderSettings();
            xsdSettings.DtdProcessing = DtdProcessing.Parse;
            StringReader xsdSR = new StringReader(xsd);
            XmlReader xsdReader = XmlReader.Create(xsdSR,xsdSettings);


            // Set the validation settings.
            XmlReaderSettings settings = new XmlReaderSettings();
            settings.ValidationType = ValidationType.Schema;
            settings.DtdProcessing = DtdProcessing.Parse;
            settings.Schemas.Add(null, xsdReader);
            settings.ValidationFlags |= XmlSchemaValidationFlags.ProcessInlineSchema;
            settings.ValidationFlags |= XmlSchemaValidationFlags.ProcessSchemaLocation;
            settings.ValidationFlags |= XmlSchemaValidationFlags.ReportValidationWarnings;
            settings.ValidationEventHandler += new ValidationEventHandler(ValidationCallBack);

            // Create the XmlReader object.
            StringReader sr = new StringReader(xml);
            XmlReader reader = XmlReader.Create(sr, settings);

            // Parse the file. 
            while (reader.Read()) ;
            return result;

        }
        // Display any warnings or errors.
        private void ValidationCallBack(object sender, ValidationEventArgs args)
        {

            result.Errors += "\tValidation error: " + args.Message;

        }
    }
}
