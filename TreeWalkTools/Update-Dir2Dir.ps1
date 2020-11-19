#
# Update-Dir2Dir.ps1
#
function Update-Dir2Dir {
    <#
    .SYNOPSIS
     Merges template directory into target one 
    .DESCRIPTION
     Will update TargetDir
    .EXAMPLE
    # Simple use
      Update-Dir2Dir -TargetDir="C:\work\target" -TemplateDir="C:\work\Template"
    #>	
    [CmdletBinding()]
    param
    (
        [Parameter(Mandatory=$True)]
        [string]$TargetDir,

		[Parameter(Mandatory=$True)]
		[string]$TemplateDir

    )
	
	[TreeWalk.Runner]::Defaults
	[TreeWalk.Runner]::runnerType = "FilteredTree"    
	[TreeWalk.Runner]::outputProcessorType = "PSScript"
	
	$InputFullPath = ""
	$CurDir = (pwd).Path		

	if(![System.IO.Path]::IsPathRooted($TemplateDir)) {		
		$InputFullPath = "$CurDir\$TemplateDir\"		
	}
	else {
		$InputFullPath = "$TemplateDir\"
	}	

	$OutFullPath = ""
	if( ![System.IO.Path]::IsPathRooted($TargetDir) ) {
		$OutFullPath = "$CurDir\$TargetDir\"
	}
	else { 
		$OutFullPath = "$TargetDir\"
	}
	if(![System.IO.Directory]::Exists($OutFullPath)) {
		[System.IO.Directory]::CreateDirectory($OutFullPath)
	}
	Write-Verbose "TreeWalk.Runner  $InputFullPath  $OutFullPath  $TreeWalkPath\FUPD\ "
	[TreeWalk.Runner]::Run($InputFullPath,$OutFullPath,"$TreeWalkPath\FUPD\")
		 
}