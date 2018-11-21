
./call_gate.ps1

If( !$inputObj.HasProperty("title_semantic")) {
    call_gate($inputObj.Property("title"))
}

If( !$inputObj.HasProperty("description_semantic") ) {
   call_gate($inputObj.Property("description"))
}