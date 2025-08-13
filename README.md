# oto.iniまとめて改造ましーん／OtoBatchEditor

## これはなに？
複数のoto.iniをまとめて編集するソフトです。
行の追加や削除、エイリアスの置換、数値の四捨五入などができます。

## サポート環境
- Windows: 10以降
- macOS: 10.15 Catalina以降
- setParam: ver.4.0-b240804以降
- `osx-arm64`, `win-arm64`は動作確認が取れていません

## ダウンロード
https://github.com/maiko3tattun/OtoBatchEditor/releases  
Assetsを開き、Windowsならだいたい`win-x64`、Windows Armなら`win-arm64`、Mac Apple Siliconなら`osx-arm64`、Mac Intelなら`osx-x64`をダウンロードしてください。

## 起動方法
- Windows: zipファイルを展開し、「OtoBatchEditor.exe」を開くとスタンドアロンで起動します。
  - setParamのpluginsフォルダにインストールするとsetParam内からプラグインとして起動できます。  
    原音設定中のoto.iniと起動後に読み込んだoto.iniを両方処理できます（oto-autoEstimation.iniがsetParamで原音設定中のデータです）
- macOS:  
1. zipファイルを展開し、「OtoBatchEditor.app」をApplicationフォルダにコピーする
2. ターミナルで`chmod +x /Applications/OtoBatchEditor.app/Contents/MacOS/OtoBatchEditor`を実行する
3. ターミナルで`xattr -rc /Applications/OtoBatchEditor.app`を実行する
4. OtoBatchEditor.appを開く

## 使い方
- 左上の水色の枠にoto.ini、またはoto.iniが入ったフォルダをドラッグ＆ドロップすると、ファイル名が「oto.ini」と完全一致のもののみ下のリストに追加されます。
- 処理内容は起動直後の画面で選択できるほか、右上の「≡」からも選択できます。
- 処理時にoto.iniの書き換えが発生する場合は、oto.iniの隣にoto_backup.iniが作成されます。別途「バックアップを作成」機能もあるので併せてお使いください。

## プリセットについて
- プリセットがあるすべてのページについては、デフォルトに戻すと前回値を復元のメニューがあります。
- プリセットは `C:\Users\<username>\AppData\Roaming\Maiko\OtoBatchEditor\Presets`, `/Users/<username>/Library/Application Support/Maiko/OtoBatchEditor/Presets` に自動的に保存されます。
- プリセット名の編集や削除は実装していません。yamlのファイル名を書き換えたりyamlファイルを削除してください。
- 同名で保存すると確認なしで上書き保存されます。

## 利用規約
- 再配布は禁止です。
- 本ソフトを利用して発生した問題については一切責任を負いかねますのでご了承下さい。
- うまく動かない、バグを見つけた場合等は、できるだけご連絡ください。  
バグ報告ガイドライン：https://ameblo.jp/maiko3utau/entry-12822371321.html

## 作者
まいこ  
Twitter: https://twitter.com/maiko3tattun  
HP: https://maiko3tattun.wixsite.com/mysite  
mail: maikotattun@yahoo.co.jp  

## 更新履歴（ver2.0以降）
2025/08/13 v1.99-beta1
- 何から何まで作り直した
- ほとんどのページにプリセットと前回値の保存を実装
- 「先頭子音を追加」「エイリアス前後のスペースを削除」「サイレント文字化け（濁点分離）を修正」「エラーチェック」機能を追加
- 「行を追加」に単独音を複製して[- ○]を作る機能を追加
- 「連続音にVCを追加」にカスタム機能を追加
- 「重複エイリアスの確認と削除」で重複エイリアスを2個以上残すときに番号を追加する機能を追加
- 「エイリアスを変換」でSuffixを書き換えないモードを追加
- 処理後に変更がなければ上書きしないよう改修
- デバッグモードの実装
