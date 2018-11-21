
function Replace-Line($template)
{
    return [regex]::Replace(  $template, '\{\{\$(?<tokenName>\w+)\}\}',  {
            param($match)
            $tokenName = $match.Groups['tokenName'].Value
			$eval = '$tokenValue=$'+$tokenName
			$res = Invoke-Expression -Command $eval 
		    return $tokenValue
        })
}

function Replace-File ($in_filename ) {
	foreach($line in Get-Content $in_filename) {
	    Replace-Line $line
	}
}


function cp_txt_replace($src_file, $dst_file ) {
	$ext = [System.IO.Path]::GetExtension($src_file)
	if( $ext.Equals(".xml") -or $ext.Equals(".txt") ) {
		Write-Output "Replacing $ext "
		Replace-File $src_file | Set-Content -Path $dst_file
	}
	else {
		cp $src_file $dst_file
	}
}

#  Copy of file adopted
$src = $inputObj.Property("FullName")
$dst = $outputObj.getOutputPath() + $inputObj.getPath()

$src_info = [io.FileInfo]($src)

If(!(test-path $dst -PathType Leaf ))
{
   Write-Output "Copy file :"
   Write-Output "Src : $src "
   Write-Output "Dst : $dst "
   cp_txt_replace $src $dst
   $dst_info = [io.FileInfo]($dst)
   If($dst_info.Exists) {
	 $dst_info.CreationTime = $src_info.CreationTime
     $dst_info.LastWriteTime = $src_info.LastWriteTime
     $dst_info.Attributes = $src_info.Attributes
     $dst_info.SetAccessControl($src_info.GetAccessControl())
   }    
}

$dst_info = [io.FileInfo]($dst)

If($src_info.LastWriteTime -gt $dst_info.LastWriteTime)
{
   Write-Output "Update file :"
   Write-Output "Src : $src "
   Write-Output "Dst : $dst "
   cp_txt_replace $src $dst
   $dst_info = [io.FileInfo]($dst)
   If($dst_info.Exists) {
	 $dst_info.CreationTime = $src_info.CreationTime
     $dst_info.LastWriteTime = $src_info.LastWriteTime
     $dst_info.Attributes = $src_info.Attributes
     $dst_info.SetAccessControl($src_info.GetAccessControl())
   }    
}
Else {
	Write-Output "Skip file dst_time: $($dst_info.LastWriteTime) src_time: $($src_info.LastWriteTime)"
}

