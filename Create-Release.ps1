$myDir = [IO.Path]::GetDirectoryName($MyInvocation.MyCommand.Definition)

$projectName = "BenchManager"
$mode = "Release"
$clrVersion = "4.0.30319"
$toolsVersion = "14.0"
$verbosity = "minimal"
$msbuild = "$env:SystemRoot\Microsoft.NET\Framework\v$clrVersion\MSBuild.exe"
$rootDir = "$myDir" # absolute
$solutionDir = "" # relative to root dir
$solutionFile = "$projectName.sln" # relative to solution dir
$releaseDir = "$rootDir\release" # absolute
$releaseFileName = "$projectName"

# Paths of build artifacts are relative to the solution dir
$buildArtifacts = @(
    "BenchLib\lib\Ionic.Zip.dll",
    "BenchLib\bin\$mode\Interop.*.dll",
    "BenchLib\bin\$mode\BenchLib.dll",
    "BenchDashboard\bin\$mode\ConEmu.WinForms.dll",
    "BenchDashboard\bin\$mode\BenchDashboard.exe",
    "BenchDashboard\bin\$mode\BenchDashboard.exe.config"
)

$taggedzipName = "$releaseDir\${releaseFileName}_$([DateTime]::Now.ToString("yyyy-MM-dd"))"
$suffix = 0
$taggedZipFile = "${taggedZipName}.zip"
while (Test-Path $taggedZipFile)
{
    $suffix++
    $taggedZipFile = "${taggedZipName}_${suffix}.zip"
}

cd "$rootDir\$solutionDir"

& $msbuild $solutionFile /v:$verbosity /tv:$toolsVersion /p:Configuration=$mode

if ($LastExitCode -ne 0)
{
    Write-Error "Build the solution failed."
    return
}

if (!(Test-Path $releaseDir)) { $_ = mkdir $releaseDir }

$stageDir = "$releaseDir\staging"
if (Test-Path $stageDir) { Remove-Item $stageDir -Force -Recurse }
$_ = mkdir $stageDir

foreach ($artifact in $buildArtifacts)
{
    cp "$rootDir\$solutionDir\$artifact" "$stageDir\"
}

cd "$rootDir"

$_ = [Reflection.Assembly]::LoadWithPartialName("System.IO.Compression.FileSystem")
[IO.Compression.ZipFile]::CreateFromDirectory($stageDir, $taggedZipFile, "Optimal", $False)

$zipFile = "$releaseDir\$releaseFileName.zip"
if (Test-Path $zipFile) { del $zipFile }
cp $taggedZipFile $zipFile

Write-Host ""
Write-Host "Finished."
