$ErrorActionPreference = 'Stop'

$framework = 'net6.0'

function Clean-Output
{
	if(Test-Path ./artifacts) { rm ./artifacts -Force -Recurse }
}

function Restore-Packages
{
	& dotnet restore
}

function Execute-Tests
{
    & dotnet test ./test/Datalust.ClefTool.Tests/Datalust.ClefTool.Tests.csproj -c Release /p:Configuration=Release /p:Platform=x64 /p:VersionPrefix=$version
    if($LASTEXITCODE -ne 0) { exit 3 }
}

function Create-ArtifactDir
{
	mkdir ./artifacts
}

function Publish-Archives($version)
{
	$rids = @("linux-x64", "linux-musl-x64", "linux-arm64", "osx-x64", "win-x64", "osx-arm64")
	foreach ($rid in $rids) {
	    $tfm = $framework
	    
		& dotnet publish ./src/Datalust.ClefTool/Datalust.ClefTool.csproj -c Release -f $tfm -r $rid --self-contained `
		    /p:VersionPrefix=$version /p:PublishSingleFile=true /p:PublishReadyToRun=true

		if($LASTEXITCODE -ne 0) { exit 4 }

		# Make sure the archive contains a reasonable root filename
		mv ./src/Datalust.ClefTool/bin/Release/$tfm/$rid/publish/ ./src/Datalust.ClefTool/bin/Release/$tfm/$rid/clef-$version-$rid/

		if ($rid.StartsWith("win-")) {
			& ./build/7-zip/7za.exe a -tzip ./artifacts/clef-$version-$rid.zip ./src/Datalust.ClefTool/bin/Release/$tfm/$rid/clef-$version-$rid/
			if($LASTEXITCODE -ne 0) { exit 5 }

			# Back to the original directory name
			mv ./src/Datalust.ClefTool/bin/Release/$tfm/$rid/clef-$version-$rid/ ./src/Datalust.ClefTool/bin/Release/$tfm/$rid/publish/			
		} else {
			& ./build/7-zip/7za.exe a -ttar clef-$version-$rid.tar ./src/Datalust.ClefTool/bin/Release/$tfm/$rid/clef-$version-$rid/
			if($LASTEXITCODE -ne 0) { exit 5 }

			# Back to the original directory name
			mv ./src/Datalust.ClefTool/bin/Release/$tfm/$rid/clef-$version-$rid/ ./src/Datalust.ClefTool/bin/Release/$tfm/$rid/publish/
			
			& ./build/7-zip/7za.exe a -tgzip ./artifacts/clef-$version-$rid.tar.gz clef-$version-$rid.tar
			if($LASTEXITCODE -ne 0) { exit 6 }

			rm clef-$version-$rid.tar
		}
	}
}

function Publish-DotNetTool($version)
{	
	# Tool packages have to target a single non-platform-specific TFM; doing this here is cleaner than attempting it in the CSPROJ directly
	& dotnet pack ./src/Datalust.ClefTool/Datalust.ClefTool.csproj -c Release --output ./artifacts /p:VersionPrefix=$version /p:TargetFrameworks=$framework
    if($LASTEXITCODE -ne 0) { exit 7 }
}

Push-Location $PSScriptRoot

$version = @{ $true = $env:APPVEYOR_BUILD_VERSION; $false = "99.99.99" }[$env:APPVEYOR_BUILD_VERSION -ne $NULL];
Write-Output "Building version $version"

Clean-Output
Create-ArtifactDir
Restore-Packages
Publish-Archives($version)
Publish-DotNetTool($version)
Execute-Tests

Pop-Location
