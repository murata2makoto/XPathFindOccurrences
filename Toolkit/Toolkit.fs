module OOXML.Toolkit

open System.Xml
open System.Xml.XPath
open System.Xml.Linq
open System.IO
open System.IO.Compression

let createXDocumentFromDocxFileName docxFileName partName : XDocument = 
    let directory = 
        System.Environment.GetEnvironmentVariable("TEMP")+"\\ooxml"
    let documentXml =
        directory + partName //"\\word\\document.xml"
    try
        Directory.Delete(directory, true)
    with 
    | :? DirectoryNotFoundException as e -> ()
    ZipFile.ExtractToDirectory(docxFileName, directory)
    
    use fsr = File.OpenRead(documentXml)
    let doc = XDocument.Load(fsr)
    fsr.Close()
    doc

let createDocXFromXDocument (doc: XDocument) docxFileName =
    let directory = 
        System.Environment.GetEnvironmentVariable("TEMP")+"\\ooxml"
    let documentXml =
        directory + "\\word\\document.xml"
    doc.Save(documentXml)
    try
        File.Delete(docxFileName)
    with 
    | :? System.IO.FileNotFoundException  -> ()
    ZipFile.CreateFromDirectory(directory, docxFileName)

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

let readXPaths inputFileName = 
    let enc = new System.Text.UTF8Encoding(true)
    let ifs = new FileStream(inputFileName, FileMode.Open)
    let sr = new StreamReader(ifs, enc)
    [while not(sr.EndOfStream) do 
        let line = sr.ReadLine()
        if line.Length > 5 && not(line.StartsWith('#')) then
            yield line]
