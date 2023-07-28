module Toolkit.OOXMLNamespaces

open System.Xml.Linq

let wmlNs = "http://schemas.openxmlformats.org/wordprocessingml/2006/main"
let wml = XNamespace.Get wmlNs

let mceNs = "http://schemas.openxmlformats.org/markup-compatibility/2006"
let mce = XNamespace.Get mceNs

let wpcNs ="http://schemas.microsoft.com/office/word/2010/wordprocessingCanvas"
let wpc = XNamespace.Get wpcNs

let wpNs="http://schemas.openxmlformats.org/drawingml/2006/wordprocessingDrawing"
let wp = XNamespace.Get wpNs

