#
# Merge-Json.ps1
#
function Merge-Json {
    <#
    .SYNOPSIS
     Merges input json file into output one 
    .DESCRIPTION
     Will rewrite output
    .EXAMPLE
    # Simple use
      New-Merge-Json -InputJson ="in.json" -OutputJson="out.json"
    #>	
    [CmdletBinding()]
    param
    (
        [Parameter(Mandatory=$True)]
        [string]$InputJson,

		[Parameter(Mandatory=$True)]
		[string]$OutputJson

    )

	[TreeWalk.Runner]::Defaults
	[TreeWalk.Runner]::runnerType = "Tree"    
	[TreeWalk.Runner]::outputProcessorType = "Merge"

	$InputFullPath = ""

	if(![System.IO.Path]::IsPathRooted($InputJson)) {
		$InputFullPath = "$Env:BASEPATH\$InputJson"
	} else {
		$InputFullPath = "$InputJson"
	}

	$OutFullPath = ""

	if( ![System.IO.Path]::IsPathRooted($OutputJson) ) {
		$OutFullPath = "$Env:BASEPATH\$OutputJson"
	} else { 
		$OutFullPath = "$OutputJson"
	}

	[TreeWalk.Runner]::Run($InputFullPath,$OutFullPath,"")
	 
}