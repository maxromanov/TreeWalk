

$template = $inputObj.Property("FullName")
$dataFile = [io.Path]::ChangeExtension($template,"ntpldata")

$ParamsObject = Get-Content -Raw -Path $dataFile | ConvertFrom-Json
$DataTable = @{}
$calc_expr = ""
$var_value = ""

$outFileName = ""

$ParamsObject.PSObject.Properties | ForEach-Object {
	$calc_expr = $_.Value
	$var_value = Invoke-Expression -Command $calc_expr   
	$DataTable[$_.Name] = $var_value
	If($_.Name -eq "OutputPath") {
		$outFileName = $var_value
	}
}

$ResultFile = $ProjectPath + [io.Path]::ChangeExtension($inputObj.Name,".ntpljson")

$DataTable | ConvertTo-Json | Out-File -FilePath $ResultFile

$outPath = $ProjectPath + $outFileName

nustasheext $template $ResultFile $outFileName