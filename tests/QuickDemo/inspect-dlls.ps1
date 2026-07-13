$paths = @(
	"runtimes\win-x64\native\epanet2.dll",
	"runtimes\win-x86\native\epanet2.dll",
	"tests\QuickDemo\bin\Debug\net10.0\runtimes\win-x64\native\epanet2.dll",
	"tests\QuickDemo\bin\Debug\net10.0\runtimes\win-x86\native\epanet2.dll",
	"tests\QuickDemo\epanet2.dll",
	"tests\QuickDemo\rede.inp",
	"tests\QuickDemo\example.inp"
)

function Get-PeMachine($path) {
	if (-not (Test-Path $path)) { return $null }
	$fs = [System.IO.File]::OpenRead($path)
	$br = New-Object System.IO.BinaryReader($fs)
	try {
		$fs.Seek(0x3C, 'Begin') | Out-Null
		$pe = $br.ReadInt32()
		$fs.Seek($pe + 4, 'Begin') | Out-Null
		$machine = $br.ReadUInt16()
		switch ($machine) {
			0x014c { return 'x86 (I386)' }
			0x8664 { return 'x64 (AMD64)' }
			0x0200 { return 'Itanium (IA64)' }
			default { return ('Unknown 0x{0:X4}' -f $machine) }
		}
	}
	finally {
		$br.Close()
		$fs.Close()
	}
}

Write-Host "Inspecting candidate files for epanet2.dll and sample INP:`n"
foreach ($p in $paths) {
	Write-Host "---`nPath: $p"
	if (Test-Path $p) {
		$fi = Get-Item $p
		Write-Host "Exists: yes, Size=$($fi.Length) bytes, LastWrite=$($fi.LastWriteTime)"
		$machine = Get-PeMachine $p
		if ($machine) { Write-Host "Machine: $machine" }
		else { Write-Host "Binary type: not applicable or not PE" }

		$dumpbin = Get-Command dumpbin -ErrorAction SilentlyContinue
		if ($dumpbin) {
			Write-Host "Running dumpbin /DEPENDENTS $p"
			try { dumpbin /DEPENDENTS $p } catch { Write-Host "dumpbin failed: $_" }
		} else {
			Write-Host "dumpbin not available in PATH"
		}
	} else {
		Write-Host "Exists: no"
	}
}

Write-Host "`nProcess architecture: $(if ([Environment]::Is64BitProcess) { 'x64' } else { 'x86' })"
