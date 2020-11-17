
$src = $inputObj.Property("FullName")
$dst = $outputObj.getOutputPath() + $inputObj.getPath()

$src_info = [io.FileInfo]($src)

If(!(test-path $dst -PathType Leaf ))
{
   Write-Output "Copy file :"
   Write-Output "Src : $src "
   Write-Output "Dst : $dst "
   cp $src_file $dst_file 
   $dst_info = [io.FileInfo]($dst)
   If($dst_info.Exists) {
	 $dst_info.CreationTime = $src_info.CreationTime
     $dst_info.LastWriteTime = $src_info.LastWriteTime
     $dst_info.Attributes = $src_info.Attributes
     $dst_info.SetAccessControl($src_info.GetAccessControl())
   }    
}
Else {
    $dst_info = [io.FileInfo]($dst)    
    Write-Output "Update file :"
    Write-Output "Src : $src "
    Write-Output "Dst : $dst "
    cp $src $dst
    $dst_info = [io.FileInfo]($dst)    
    If($dst_info.Exists) {
	     $dst_info.CreationTime = $src_info.CreationTime
         $dst_info.LastWriteTime = $src_info.LastWriteTime
         $dst_info.Attributes = $src_info.Attributes
         $dst_info.SetAccessControl($src_info.GetAccessControl())
    }
}