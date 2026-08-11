# -----------------------------
$AppName = "OtoBatchEditor"
$PublishDir = "OtoBatchEditor\bin\Release"
$RIDs = @{
    "win-x64"   = "Windows x64"
    "win-arm64" = "Windows ARM64"
    "osx-x64"   = "macOS x64"
    "osx-arm64" = "macOS ARM64"
}

# -----------------------------------
# バージョン情報の自動取得 (.csproj 優先、なければ Info.plist)
$CsprojPath = "$AppName\$AppName.csproj"
$InfoPlistPath = "$AppName\Info.plist"
$versionRaw = ""

# 1. .csproj から <Version> を取得
if (Test-Path $CsprojPath) {
    [xml]$csprojXml = Get-Content $CsprojPath
    $versionRaw = $csprojXml.Project.PropertyGroup.Version
}

# 2. .csproj で取れなかった場合は Info.plist の CFBundleShortVersionString を参照
if ([string]::IsNullOrWhiteSpace($versionRaw) -and (Test-Path $InfoPlistPath)) {
    [xml]$plistXml = Get-Content $InfoPlistPath
    $keyNode = $plistXml.SelectSingleNode("//key[text()='CFBundleShortVersionString']")
    if ($keyNode) {
        $versionRaw = $keyNode.NextSibling.InnerText
    }
}

# 万が一どちらからも取得できなかった場合のデフォルト値
if ([string]::IsNullOrWhiteSpace($versionRaw)) {
    $versionRaw = "2.0.0"
}

# バージョンの整形 ("2.0.0" -> "v200" に変換)
$versionTag = "v" + ($versionRaw -replace '\.', '')

Write-Host "Detected Version: $versionRaw -> Tag: $versionTag"

# -----------------------------
# ビルド & パブリッシュ
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
    New-Item -ItemType Directory -Force -Path "$AppBundleDir\MacOS" | Out-Null
    New-Item -ItemType Directory -Force -Path "$AppBundleDir\Resources" | Out-Null

    Copy-Item "$SourceDir\*" "$AppBundleDir\MacOS" -Recurse -Force
    Copy-Item "$AppName\Info.plist" $AppBundleDir -Force
    Copy-Item "$AppName\Assets\MaikoTools.icns" "$AppBundleDir\Resources" -Force

    Write-Host ".app bundle created: $AppBundleDir"
}

# -----------------------------------
# Zip アーカイブの作成
Write-Host "Creating Zip archives..."

foreach ($rid in $RIDs.Keys) {
    # 出力ファイル名 (例: OtoBatchEditor-v200-win-x64.zip)
    $zipFileName = "$AppName-$versionTag-$rid.zip"
    $zipPath = "$PublishDir\$zipFileName"

    # 既に同名のZipが存在する場合は事前に削除
    if (Test-Path $zipPath) {
        Remove-Item $zipPath -Force
    }

    if ($rid.StartsWith("osx-")) {
        # macOS: 作成した .app バンドル自体を Zip 化
        $targetPath = "$PublishDir\$rid-app\$AppName.app"
        Compress-Archive -Path $targetPath -DestinationPath $zipPath -Force
    } else {
        # Windows: 出力フォルダの中身を Zip 化
        $targetPath = "$PublishDir\$rid\*"
        Compress-Archive -Path $targetPath -DestinationPath $zipPath -Force
    }

    Write-Host "Zip created: $zipPath"
}

Write-Host "All process finished successfully!"
# -----------------------------

# .csprojとInfo.plistのバージョンを書き換える

# これを実行：
# OtoBatchEditor\build.ps1

# Macではインストール後実行前にターミナルでコマンドを叩く必要がある
# chmod +x /Applications/OtoBatchEditor.app/Contents/MacOS/OtoBatchEditor
# xattr -rc /Applications/OtoBatchEditor.app