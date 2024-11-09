$currentDirectory = (Get-Item .).FullName -replace "\\", "/"
Write-Output "(command `"netload`" `"$currentDirectory/bin/Debug/GMEPElectricalResidential.dll`")" | Out-File "load_dll.lsp"