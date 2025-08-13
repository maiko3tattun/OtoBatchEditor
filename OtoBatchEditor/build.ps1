# -----------------------------
$AppName = "OtoBatchEditor"
$PublishDir = "OtoBatchEditor\bin\Release"
$RIDs = @{
    "win-x64" = "Windows x64"
    "win-arm64" = "Windows ARM64"
    "osx-x64" = "macOS x64"
    "osx-arm64" = "macOS ARM64"
}

foreach ($rid in $RIDs.Keys) {
    Write-Host "Publishing $($RIDs[$rid])..."
    $outputDir = "$PublishDir\$rid"
    dotnet publish -c Release -r $rid --self-contained true -p:PublishSingleFile=true -p:DebugType=None -o $outputDir
    Write-Host "Output: $outputDir"
}

Write-Host "All 4 builds completed."

# -----------------------------------
# macOS .app バンドル作成（x64とARM64）
foreach ($macRid in @("osx-x64", "osx-arm64")) {
    $Executable = "$PublishDir\$macRid\$AppName"
    $AppBundleDir = "$PublishDir\$AppName-$macRid.app\Contents"
    New-Item -ItemType Directory -Force -Path "$AppBundleDir\MacOS"
    New-Item -ItemType Directory -Force -Path "$AppBundleDir\Resources"

    Copy-Item $Executable "$AppBundleDir\MacOS"
    Copy-Item "$AppName\Info.plist" $AppBundleDir

    Write-Host ".app bundle created: $AppBundleDir"
    Write-Host "Next step on macOS: chmod +x $AppBundleDir\MacOS\$AppName"
}
# -----------------------------

# .csprojとInfo.plistのバージョンを書き換える

# これを実行：
# OtoBatchEditor\build.ps1

# Macではインストール後実行前にターミナルでコマンドを叩く必要がある
# chmod +x /Applications/OtoBatchEditor.app/Contents/MacOS/OtoBatchEditor