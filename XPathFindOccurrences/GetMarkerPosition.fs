module GetMarkerPosition

let private ignoreCase = System.StringComparison.CurrentCultureIgnoreCase

let getMarkerPosition (xpath: string) startP (evalue: string) = 
    if xpath.Contains("Example",ignoreCase) then
        if startP then evalue.IndexOf("[Example:")
        else evalue.IndexOf("end example") + 11
                        - evalue.Length
    elif xpath.Contains("Note", ignoreCase) then
        if startP then evalue.IndexOf("[Note:")
        else evalue.IndexOf("end note") + 8
                        - evalue.Length
    elif xpath.Contains("Guidance", ignoreCase) then
        if startP then evalue.IndexOf("[Guidance:")
        else evalue.IndexOf("end guidance") + 12
                        - evalue.Length
    elif xpath.Contains("Rationale", ignoreCase) then
        if startP then evalue.IndexOf("[Rationale:")
        else evalue.IndexOf("end rationale") + 13
                        - evalue.Length
    else failwith "hen"