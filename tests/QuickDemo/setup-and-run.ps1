param(
	[string]$SourcePath = "",
	[string]$InpPath = "",
	[int]$ExpectedNodes = 0,
	[int]$ExpectedLinks = 0
)

function Copy-DllForArch {
	param($src, $arch)
	$dest = Join-Path -Path (Resolve-Path "..\.." -Relative) -ChildPath "runtimes\$arch\native"
	if (-not (Test-Path $dest)) { New-Item -ItemType Directory -Path $dest -Force | Out-Null }
	Copy-Item -Path $src -Destination (Join-Path $dest "epanet2.dll") -Force
	Write-Host "Copied $src to $dest"
}

# Determine arch
$arch = if ([IntPtr]::Size -eq 8) { 'win-x64' } else { 'win-x86' }
Write-Host "Detected process architecture: $arch"

if ([string]::IsNullOrWhiteSpace($SourcePath)) {
	Write-Host "No SourcePath specified. Please provide path to epanet2.dll or place it into runtimes\$arch\native manually.";
} else {
	if (-not (Test-Path $SourcePath)) { Write-Error "SourcePath not found: $SourcePath"; exit 2 }
	Copy-DllForArch -src $SourcePath -arch $arch
}

$proj = "QuickDemo.csproj"
$projPath = Join-Path -Path (Get-Location) -ChildPath $proj
if (-not (Test-Path $projPath)) { Write-Error "Project not found: $projPath"; exit 3 }

$runInp = if ([string]::IsNullOrWhiteSpace($InpPath)) { "example.inp" } else { $InpPath }

$cmd = "dotnet run --project $projPath -- $runInp"
if ($ExpectedNodes -gt 0 -and $ExpectedLinks -gt 0) { $cmd += " $ExpectedNodes $ExpectedLinks" }

Write-Host "Running: $cmd"
iex $cmd
