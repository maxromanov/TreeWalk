﻿# Import-Module -Name "$PSScriptRoot\TreeWalkTools.psm1" -Verbose


# $TreeWalkPackage = Get-Package TreeWalk

# if(!$TreeWalkPackage) {
#	Install-Package TreeWalk -Verbose  -Scope CurrentUser -SkipDependencies
# }

# $DllPath = [System.IO.Path]::GetDirectoryName($TreeWalkPackage.Source) + "\lib\net461\TreeWalk.dll"

# Add-Type -Path $DllPath 


# New-ScriptFileInfo -Path "$PSScriptRoot\Merge-Xml2Xml.ps1" -Version "1.0.0" -Author "maskym.a.romanov@gmail.com" -Description "Merge input XML into output Xml"  -CompanyName "Individual" `
#   -Copyright "Gnu GPL v3" 


# Update-ModuleManifest -Path .\TreeWalkTools\TreeWalkTools.psd1 -ProjectUri "https://github.com/maxromanov/TreeWalk" -LicenseUri "https://raw.githubusercontent.com/maxromanov/TreeWalk/master/LICENSE"

# Publish-Module -Name "C:\workspace\orgs\github\TreeWalk\TreeWalkTools" -NuGetApiKey "oy2hgsze3ps7g2dzx6sqkweuaa6slphefdethscpkjs4ou"