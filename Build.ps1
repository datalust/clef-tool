function Clean-Output
{
	if(Test-Path ./artifacts) { rm ./artifacts -Force -Recurse }
}

function Restore-Packages
{
	& nuget restore
}

function Update-AssemblyInfo($version)
{  
    $versionPattern = "[0-9]+(\.([0-9]+|\*)){3}"

    foreach ($file in ls ./src/*/Properties/AssemblyInfo.cs)  
    {     
        (cat $file) | foreach {  
                % {$_ -replace $versionPattern, "$version.0" }             
            } | sc -Encoding "UTF8" $file                                 
    }  
}

function Update-WixVersion($version)
{
    $defPattern = "define Version = ""0\.0\.0"""
	$def = "define Version = ""$version"""
    $product = ".\setup\Datalust.ClefTool.Setup\Product.wxs"

    (cat $product) | foreach {  
            % {$_ -replace $defPattern, $def }    
        } | sc -Encoding "UTF8" $product
}

function Execute-MSBuild
{
	& msbuild ./clef-tool.sln /t:Rebuild /p:Configuration=Release /p:Platform=x64
}

function Execute-Tests
{
    pushd ./test/Datalust.ClefTool.Tests

    & dotnet test -c Release
    if($LASTEXITCODE -ne 0) { exit 3 }

    popd
}

function Publish-Artifacts($version)
{
	mkdir ./artifacts
	mv ./setup/Datalust.ClefTool.Setup/bin/Release/clef.msi ./artifacts/clef-$version-pre.msi
}

Push-Location $PSScriptRoot

$version = @{ $true = $env:APPVEYOR_BUILD_VERSION; $false = "0.0.0" }[$env:APPVEYOR_BUILD_VERSION -ne $NULL];

Clean-Output
Restore-Packages
Update-AssemblyInfo($version)
Update-WixVersion($version)
Execute-MSBuild
Execute-Tests
Publish-Artifacts($version)

Pop-Location
