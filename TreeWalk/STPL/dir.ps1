# making directories

$src_dir = $inputObj.getPath()
$dst_dir = $outputObj.getOutputPath() + $src_dir

If(!(test-path $dst_dir))
{
	  Write-Output "mkdir: $dst_dir"
      New-Item -ItemType Directory -Force -Path $dst_dir
}

