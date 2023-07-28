module Toolkit.ReadWrite
open System.Xml
open System.Xml.XPath
open System.Xml.Linq
open System.IO
open System.IO.Compression
open OOXMLNamespaces

let tempDir = "j:\Temp"; //System.Environment.GetEnvironmentVariable("TEMP")

let extractDocx docxFileName =
//DOCXファイル全体を展開
    let directory = 
        tempDir+"\\ooxml"
    try
        Directory.Delete(directory, true)
    with 
    | :? DirectoryNotFoundException as e -> ()
    ZipFile.ExtractToDirectory(docxFileName, directory)

    
let createXDocumentFromExtractedDocx partName : XDocument = 
//DOCXファイルが展開されているディレクトリから指定したpartをパースしてXDocumentを返す
    let directory = 
        tempDir+"\\ooxml"
    let documentXml =
        directory + partName //"\\word\\document.xml"
    use fsr = File.OpenRead(documentXml)
    let doc = XDocument.Load(fsr)
    fsr.Close()
    doc

let createXDocumentFromDocxFileName docxFileName partName : XDocument = 
//DOCXファイル全体を展開したうえで、指定したpartからなるXDocumentを返す
    extractDocx docxFileName
    createXDocumentFromExtractedDocx partName

let createDocxFromExtractedDocx docxFileName =
//ディレクトリからDOCXファイルを生成する
    let directory = 
        tempDir+"\\ooxml"
    try
        File.Delete(docxFileName)
    with 
    | :? System.IO.FileNotFoundException  -> ()
    ZipFile.CreateFromDirectory(directory, docxFileName)

let replaceXDocumentInExtractedDocx partName (doc: XDocument) = 
//DOCXファイルが展開されているディレクトリの、指定したpartにXDocumentを格納する
    let directory = 
        tempDir+"\\ooxml"
    let documentXml =
        directory + partName //"\\word\\document.xml"
    doc.Save(documentXml, SaveOptions.OmitDuplicateNamespaces)

let createDocXFromXDocument docxFileName partName (doc: XDocument)  =
//DOCXファイル全体が展開されているという仮定で、指定したpartをXDocumentで置き換えて、DOCXファイルを作る
    replaceXDocumentInExtractedDocx partName doc
    createDocxFromExtractedDocx docxFileName

let getManager (doc: XDocument) =
  let nav = doc.CreateNavigator()
  let manager = new XmlNamespaceManager(nav.NameTable)
  manager.AddNamespace("w", wmlNs)
  manager.AddNamespace("mc",mceNs)
  manager.AddNamespace("wp", wpNs)
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
        let line = sr.ReadLine().Trim()
        if line.Length > 0 && not(line.StartsWith('#')) then
            yield line]
