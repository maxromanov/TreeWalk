

[TreeWalk.Logging]::Info("Processing business requirement SpecObject")

If($InputObj.Property("Kind") -eq "ClientContext") {
	[TreeWalk.Logging]::Info("Creating client business model")
	$client_path = $Env:BASEDIR+$InputObj.Property("ParamValue")
	$ClientBM = New-Object  -TypeName "TreeWalk.ArchimateModel" -ArgumentList $client_path

}

If($InputObj.Property("Kind") -eq "ProductRequest") {
	[TreeWalk.Logging]::Info("Querying client service requested")
	$Srvc = $OwnBM.getNodeByPath( $InputObj.Property("ParamValue"))
}

If($InputObj.Property("Kind") -eq "ProductObject") {
	[TreeWalk.Logging]::Info("Querying client object - scope")
	$ClientObj = $ClientBM.getNodeByPath( $InputObj.Property("ParamValue"))
}

If( ($ClientBM -ne $null) -and ( $Srvc -ne $null ) -and ( $ClientObj -ne $null ) ) {
    [TreeWalk.Logging]::Info("Ready for Init Project")
	$projectPath = $Env:BASEDIR+"workspace\projects\"+$ClientBM.Name + "\" + $ClientObj.Name+ "\"
    $projectName = $ClientObj.Name
    $InitProjectScript =  $Env:BASEDIR+"\work\service\ops\init\init_project.ps1"
	Invoke-Expression -Command $InitProjectScript
}