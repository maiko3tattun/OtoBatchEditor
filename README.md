# oto.iniまとめて改造ましーん／OtoBatchEditor

## これはなに？
複数のoto.iniをまとめて編集するソフトです。
行の追加や削除、エイリアスの置換、数値の四捨五入などができます。

## ダウンロード
[https://github.com/maiko3tattun/OtoBatchEditor/releases](https://github.com/maiko3tattun/OtoBatchEditor/releases)  
Assetsを開き、Windowsならだいたいwin-x64.zip、Windows Armならwin-arm64.zipをダウンロードしてください。

## 起動方法
- Windows: zipファイルを展開し、「OtoBatchEditor.exe」を開くとスタンドアロンで起動します。
- setParamのpluginsフォルダにインストールするとsetParam内からプラグインとして起動できます。
- macOS版準備中。

## サポート環境
- OS: Windows10以降
- setParam: ver.4.0-b240804以降
- win-arm64は動作確認が取れていません。

## 使い方
- 左上の水色の枠にoto.ini、またはoto.iniが入ったフォルダをドラッグ＆ドロップすると、ファイル名が「oto.ini」と完全一致のもののみ下のリストに追加されます。
- 処理内容は起動直後の画面で選択できるほか、右上の「≡」からも選択できます。
- 処理時にoto.iniの書き換えが発生する場合は、oto.iniの隣にoto_backup.iniが作成されます。

## プリセットについて
- プリセットがあるすべてのページについては、デフォルトに戻すと前回値を復元のメニューがあります。
- プリセットは `C:\Users\<username>\AppData\Roaming\Maiko\OtoBatchEditor\Presets` に自動的に保存されます。
- プリセット名の編集や削除は実装していません。yamlのファイル名を書き換えたりyamlファイルを削除してください。
- 同名で保存すると確認なしで上書き保存されます。

## 利用規約
- 再配布は禁止です。
- 本ソフトを利用して発生した問題については一切責任を負いかねますのでご了承下さい。
- うまく動かない、バグを見つけた場合等は、できるだけご連絡ください。  
バグ報告ガイドライン：https://ameblo.jp/maiko3utau/entry-12822371321.html

## 更新履歴
同梱のreadme.txtに記載しています。
