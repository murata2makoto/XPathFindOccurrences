module 共通XPath定義

let secTitleQuery = "w:p[
                  .//w:r and
                  (starts-with(w:pPr/w:pStyle/@w:val, \"Heading\")
                  or
                  (w:pPr/w:pStyle/@w:val = \"ISOClause1\")
                  or
                  (w:pPr/w:pStyle/@w:val = \"1\")
                  or
                  (w:pPr/w:pStyle/@w:val = \"21\")
                  or
                  (w:pPr/w:pStyle/@w:val = \"31\")
                  or
                  (w:pPr/w:pStyle/@w:val = \"41\")
                  or
                  (w:pPr/w:pStyle/@w:val = \"50\")
                  or
                  (w:pPr/w:pStyle/@w:val = \"51\")
                  or
                  (w:pPr/w:pStyle/@w:val = \"Appendix1\")
                  or
                  (w:pPr/w:pStyle/@w:val = \"Appendix2\")
                  or
                  (w:pPr/w:pStyle/@w:val = \"Appendix3\")
                  or
                  (w:pPr/w:pStyle/@w:val = \"Appendix4\")
                  or
                  (w:pPr/w:pStyle/@w:val = \"Appendix5\")
                  or
                  (w:pPr/w:pStyle/@w:val = \"Appendix6\")
                  )
                  ]"
                  
 (*                
                  
                  "w:p[
                  .//w:r and
                  ((starts-with(w:pPr/w:pStyle/@w:val, \"Heading\")
                  or
                  (w:pPr/w:pStyle/@w:val = \"ISOClause1\")
                  or
                  (w:pPr/w:pStyle/@w:val = \"1\")
                  or
                  (w:pPr/w:pStyle/@w:val = \"21\")
                  or
                  (w:pPr/w:pStyle/@w:val = \"31\")
                  or
                  (w:pPr/w:pStyle/@w:val = \"41\")
                  or
                  (w:pPr/w:pStyle/@w:val = \"50\")
                  or
                  (w:pPr/w:pStyle/@w:val = \"51\")
                  or
                  (w:pPr/w:pStyle/@w:val = \"Appendix1\")
                  or
                  (w:pPr/w:pStyle/@w:val = \"Appendix2\")
                  or
                  (w:pPr/w:pStyle/@w:val = \"Appendix3\")
                  or
                  (w:pPr/w:pStyle/@w:val = \"Appendix4\")
                  or
                  (w:pPr/w:pStyle/@w:val = \"Appendix5\")
                  or
                  (w:pPr/w:pStyle/@w:val = \"Appendix6\")
                  )
                  ]"

                  *) 