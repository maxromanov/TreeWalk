
$PackageMgMt = Get-Module PackageManagement
if(!$PackageMgMt) {
	Import-Module PackageManagement -RequiredVersion 1.2.2
}


$TreeWalkPackage = Get-Package TreeWalk

if(!$TreeWalkPackage) {
	Install-Package TreeWalk -Verbose  -Scope CurrentUser -ProviderName NuGet -Source "https://www.nuget.org/api/v2/" -Force -SkipDependencies
}
$DllPath = [System.IO.Path]::GetDirectoryName($TreeWalkPackage.Source) + "\lib\net461\TreeWalk.dll"
Add-Type -Path $DllPath 

. "$PSScriptRoot\Merge-Xml2Xml.ps1"


Export-ModuleMember -Function 'Merge-Xml2Xml'
