**General**

An invoice is initially generated in Word format and subsequently converted to PDF. A Word template with document variables is utilised. The values for this invoice are sourced from a CSV or an Excel file. The selection of the invoice record is accomplished via a filter.

A clean architecture approach is adopted to ensure the product remains easily extensible. The source is dynamically ingested into the project. It must be possible to change the source or the Word template without necessitating any code modifications. For this purpose, a mapping is required from the source to the application and another mapping from the application to the template. These mappings are specified in the application's configuration file.

A console application is selected, capable of execution on both Windows and Linux platforms.

**Technical Details**

The source file is read by a Repository, which translates the invoice record into a dictionary. Subsequently, mapping is applied. The values from the source file are added to the domain entity invoice, using the mapping from the configuration file. To facilitate ease of use, this configuration is added in a flat version within the configuration file. There is a section in the configuration containing the mapping from the source to the domain object. Within the application settings, there is a section named InvoiceSettings, which contains general settings for the invoice, as well as SourceMapping and DestinationMapping sections. Within these sections, the mapping takes place. Initially, a mapping is created from the DataSource (in the application Repository) to the Invoice in the application, followed by a mapping from the application object to the DocVariables in the WordTemplate.

The InputSource can be either CSV or Excel, necessitating the creation of two repositories. Various invoice generators can be added as destinations. These require the name of the template and the Invoice object. The initial delivery includes an InvoiceWordGenerator and an InvoicePdfGenerator. To minimise dependencies, services are utilised. These are crafted to the smallest logically possible unit of functionality. For instance, the InvoicePdfGenerator will also utilise the InvoiceDocumentBuilder service. The standard document format for the application is a Word document, as we commence with a Word template. Consequently, the InvoicePdfGenerator will employ this service, as it is possible to generate a PDF directly from a Word document. Open XML SDK is utilised as this library is cross-platform compatible.

**Standard Values**

In the application configuration, standard values such as ExpireDays (14), DefaultVatRate (21.00), DefaultCurrency (EUR), and OutputFormat [pdf or docx] can be set. Expire days may also be included in the source. The default value from the configuration is only used if it is not present in the source. Invoice records have a VatRate. If this is not specified, the VatRate from the application file is used. A VatRate can explicitly be set to “0.00” for cases where no VAT is applicable to the invoice record. The total VAT is displayed on the invoice. There is a general VAT percentage for the invoice, which is only displayed (in %) if all invoice records have the same VAT format.

If the OutputFormat does not exist, PDF is used as the default output format. The file type is determined based on the MIME type of the file. Accordingly, GetRepository will return the service required to read the file.

**Summary of Use Cases**

There are two use cases:

1.  The first reads the application file based on the MIME type of the file. This file is ingested, followed by mapping to make the application Invoice object available. This then calls the InvoiceService.GetLatestInvoice().
2.  The second use case generates an invoice: GenerateLatestInvoice(Invoice, outputMapping, outputFormat = pdf).

**Use Cases**

1.  **GetInvoiceLatestInvoice.cs** (invoiceSettings)
2.  **GenerateLatestInvoice** (Invoice, outputMapping, outputFormat = pdf)

By these measures, we ensure a seamless and efficient process for generating and managing invoices within the application.

**Building the invoice**

Based on a Word template, the invoice mapping and the invoice object a Word document will be generated. The PDF creator will call the DocumentCreator to parse this file as PDF.

Input for the DocumentGenerator should be only the document template and a flat dictionary with all DocumentVariables that have to be replaced.

ConfigurationMapper is another class. It’s input will be the invoice object and the output mapping configuration. While mapping recursive through the output configuration nodes the values will be taken for the invoice object. A value can be a decimal or otherwise a text. A null value will be parsed as empty string to the output dictionary that is finally used to replace the values in the document generator.

var invoice = lastInvoice;  
var documentVariables = ConfigurationMapper.GetDocumentVariables(invoice);  
var filename = DocumentGenerator.CreateInvoice(documentVariables, FileFormat fileFormat = FileFormat.PDF)  
  
I think that the ConfigurationMapper should later be use for ConfigurationMapper.GetInvoice(rawInvoiceData);

Currently the configuration mapper that is creating the invoice based on the rawinvoice data is generating json and then parsing it to an object. This is overload. The configuration mapper should just recursively go through the configuration tree and assign the entity properties of invoice directly.

In the full application an entity object is generated based on a node in the configuration ConfigToEntity and also EntityToDocumentDictionary.

I think this needs to be a ConfigParser class. ConfigParser.Get\<Invoice\>(IConfigurationElement configurationElement, Dictionary\<string, string\> data)

TemplateValues tv = ConfigParser.GetDictionary(IConfigurationElement configurationElement, object invoice);

**ConfigurationMapper.GetDocumentVariables(invoice) =\>** ConfigParser.GetDictionary(invoiceSettings.outputMapping, invoice);

Simple approach:

var rawInvoiceData = sourceReader.GetData();  
var latestInvoice = dataReader.GetLatestInvoice(rawInvoiceData);  
var filename = InvoiceGenerator.CreateInvoice(latestInvoice, filetype);

UseCases:

Invoice GetLatestInvoice(rawInvoiceData);

return ConfigParser.Get\<Invoice\>(rawInvoiceData);

String CreateInvoice(invoice, filetype);

var documentVariables = ConfigParser.GetDocumentVariables(IConfigurationTree);

return WordProcessor.Create(documentTemplate, documentVariables, filetype)
