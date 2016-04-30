$myDir = [IO.Path]::GetDirectoryName($MyInvocation.MyCommand.Definition)

$msbuild = "$env:SystemRoot\Microsoft.NET\Framework\v4.0.30319\MSBuild.exe"
$solutionFile = "BenchManager.sln"
$mode = "Release"
$releaseDir = "$myDir\release"
$tagedZipFile = "$releaseDir\BenchManager_$([DateTime]::Now.ToString("yyyy-MM-dd")).zip"
$releaseFile = "$releaseDir\BenchManager.zip"

cd $myDir

& $msbuild $solutionFile /v:minimal /p:Configuration=$mode

if ($LastExitCode -ne 0) {
    Write-Error "Build the solution failed."
    return
}

if (!(Test-Path $releaseDir)) { $_ = mkdir $releaseDir }

$stageDir = "$releaseDir\staging"
if (Test-Path $stageDir) { Remove-Item $stageDir -Force -Recurse }
$_ = mkdir $stageDir

cp "$myDir\BenchLib\lib\Ionic.Zip.dll" "$stageDir\"
cp "$myDir\BenchLib\bin\$mode\Interop.*.dll" "$stageDir\"
cp "$myDir\BenchLib\bin\$mode\BenchLib.dll" "$stageDir\"
cp "$myDir\BenchDashboard\bin\$mode\ConEmu.WinForms.dll" "$stageDir\"
cp "$myDir\BenchDashboard\bin\$mode\BenchDashboard.exe" "$stageDir\"
cp "$myDir\BenchDashboard\bin\$mode\BenchDashboard.exe.config" "$stageDir\"

$_ = [Reflection.Assembly]::LoadWithPartialName("System.IO.Compression.FileSystem")
[IO.Compression.ZipFile]::CreateFromDirectory($stageDir, $tagedZipFile, "Optimal", $False)

if (Test-Path $releaseFile) { del $releaseFile }
cp $tagedZipFile $releaseFile

Write-Host "Finished."
