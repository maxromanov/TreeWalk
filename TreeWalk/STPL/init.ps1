
$env:Path += ";"+$Env:BASEDIR+"bin"

if(!$inputObj) { 
	throw "No input object passed for init script!"
}

if(!$outputObj) {
	throw "No output object passed for init script!"
}

$TemplatePath = $inputObj.Property("FullName")
$ProjectPath  = $outputObj.getOutputPath()
$ProjectDirInfo = [System.IO.DirectoryInfo]($ProjectPath)
$ProjectName = $ProjectDirInfo.Name

Write-Output "Project: $ProjectName"


