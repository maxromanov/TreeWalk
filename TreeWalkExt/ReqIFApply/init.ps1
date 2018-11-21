
 $env:Path += ";"+$Env:BASEDIR+"bin"

 $ReqIF = $InputObj
 $OwnModelPath = $Env:BASEDIR+"work\strategy\dev.digitaltwins.info.archimate"
 $OwnBM = New-Object  -TypeName "TreeWalk.ArchimateModel" -ArgumentList $OwnModelPath

 $ClientBMPath = $ReqIF.getNodeByPath("/ReqIF/Specification[@Name='BusinessRequirements']/SpecObject[@Kind='ClientContext']").Property("ParamValue")


 $ClientBM = $null
 $Srv      = $null
 $ClientObj = $null
