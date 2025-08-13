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
    $SourceDir = "$PublishDir\$macRid"
    $AppBundleDir = "$PublishDir\$macRid-app\$AppName.app\Contents"
    New-Item -ItemType Directory -Force -Path "$AppBundleDir\MacOS"
    New-Item -ItemType Directory -Force -Path "$AppBundleDir\Resources"

    Copy-Item "$SourceDir\*" "$AppBundleDir\MacOS" -Recurse -Force
    Copy-Item "$AppName\Info.plist" $AppBundleDir -Force

    Write-Host ".app bundle created: $AppBundleDir"
}
# -----------------------------

# .csprojとInfo.plistのバージョンを書き換える

# これを実行：
# OtoBatchEditor\build.ps1

# Macではインストール後実行前にターミナルでコマンドを叩く必要がある
# chmod +x /Applications/OtoBatchEditor.app/Contents/MacOS/OtoBatchEditor
# xattr -rc /Applications/OtoBatchEditor.app