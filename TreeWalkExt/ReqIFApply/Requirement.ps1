
$instruction = $inputObj.Property("ProcessingInstruction");
If($instruction -eq "ApplyTemplate") {	
	$componentXPath = $inputObj.Property("ParamValue")
	Write-Output "Template called: $componentXPath"
	$component_template = $OwnBM.getNodeByPath($componentXPath).Property("TemplatePath")
	$component_template = $Env:BASEDIR + $component_template
	$copy_schema = $Env:BASEDIR+"bin\STPL\"
	TreeWalkCon -i $component_template -o $projectPath -p "PSScript;T4Dir" -r FilteredTree -s $copy_schema -l Info
}