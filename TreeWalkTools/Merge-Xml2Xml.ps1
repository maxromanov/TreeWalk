#
# Merge-Xml2Xml.ps1
#
function Merge-Xml2Xml {
    <#
    .SYNOPSIS
     Merges input xml file into output one 
    .DESCRIPTION
     Will rewrite output
    .EXAMPLE
    # Simple use
      New-Merge-Xml2Xml -InputXml="in.xml" -OutputXml="out.xml"
    #>	
    [CmdletBinding()]
    param
    (
        [Parameter(Mandatory=$True)]
        [string]$InputXml,

		[Parameter(Mandatory=$True)]
		[string]$OutputXml

    )

	[TreeWalk.Runner]::Defaults
	[TreeWalk.Runner]::runnerType = "Tree"    
	[TreeWalk.Runner]::outputProcessorType = "Merge"

	$InputFullPath = ""

	if(![System.IO.Path]::IsPathRooted($InputXml)) {
		$InputFullPath = "$Env:BASEPATH\$InputXml"
	} else {
		$InputFullPath = "$InputXml"
	}

	$OutFullPath = ""

	if( ![System.IO.Path]::IsPathRooted($OutputXml) ) {
		$OutFullPath = "$Env:BASEPATH\$OutputXml"
	} else { 
		$OutFullPath = "$OutputXml"
	}

	[TreeWalk.Runner]::Run($InputFullPath,$OutFullPath,"")
	 
}