module XPathFindOccurrences.Toolkit

open System.Xml
open System.Xml.XPath
open System.Xml.Linq
open System.IO
open System.IO.Compression

let createXDocumentFromDocxFileName docxFileName: XDocument = 
    let directory = 
        System.Environment.GetEnvironmentVariable("TEMP")+"\\ooxml"
    let documentXml =
        directory + "\\word\\document.xml"
    try
        Directory.Delete(directory, true)
    with 
    | :? DirectoryNotFoundException as e -> ()
    ZipFile.ExtractToDirectory(docxFileName, directory)
    use fsr = File.OpenRead(documentXml)
    XDocument.Load(fsr)

let getManager (doc: XDocument) =
  let nav = doc.CreateNavigator()
  let manager = new XmlNamespaceManager(nav.NameTable)
  manager.AddNamespace("w", 
    "http://schemas.openxmlformats.org/wordprocessingml/2006/main")
  manager

let createTextWriteFromOutputFileName outputFileName = 
    let enc = new System.Text.UTF8Encoding(true)
    let ofs = new FileStream(outputFileName, FileMode.Create)
    new StreamWriter(ofs, enc)