#
# This is a PowerShell Unit Test file.
# You need a unit test framework such as Pester to run PowerShell Unit tests. 
# You can download Pester from https://go.microsoft.com/fwlink/?LinkID=534084
#

Describe "Test Module Load" {

	Context "Package Exists" {
		Import-Module -Name "$PSScriptRoot\TreeWalkTools.psm1"
		$Module = Get-Module TreeWalkTools
		It "Should Return" {
		    $Module | Should -Not -BeNullOrEmpty
		}
	}
}