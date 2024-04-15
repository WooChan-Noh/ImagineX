# ImagineX
+ このプロジェクトの目的は、生成型AIを簡単に使うことです。
  + 音声コマンドをもとに、生成型AIを活用して絵を生成します。
+ UI design was done by designer from _TinyGem_
+ 韓国語のみです。
## Overview
### What it does
+ 音声を入力として受け取り、韓国語テキストに変換します。
+ 韓国語テキストを英語テキストに変換します。
+ 生成AIを活用します。 (REST通信)
+ 生成されたイメージを表示します。
### Test Environment (Only available in VR)
+ Meta Quest2
+ Meta Quest Pro 
### I used this tech
+ ETRI : SST (voice to text) [API](https://aiopen.etri.re.kr/guide/Recognition)
+ Papago : 번역 (kor to eng) [API](https://developers.naver.com/docs/papago/README.md)
+ Karlo : 생성형 AI [API](https://developers.kakao.com/product/karlo)
+ Oculus SDK and API : Follow official guide [Official guide](https://developer.oculus.com/documentation/unity/unity-gs-overview/), [Controller API](https://developer.oculus.com/documentation/unity/unity-ovrinput/)
## Learn more
+ VR開発環境設定が必要です。
> + "Oculus Integration" Package Import (Asset Store)    
> + Project Settings - XR Plug-in Management install -> Oculus check      
> + Project Settings - Player - Minimum API Level : Android 10.0 (Level 29) check    
> + Project Settings - Oculus - Fix and Apply all recommend    
> + Build Settings - Texture Compression : ASTC change    
> + Meta Quest device dev mode on (moblie app)    
> + In MQHD app, OVR Monitor Metrics install    
> + Complete software updates for the device and controllers    
> + Apps you need [Oculus](https://www.meta.com/kr/ko/quest/setup/?utm_source=www.meta.com&utm_medium=oculusredirect), [MQHD](https://developer.oculus.com/downloads/package/oculus-developer-hub-win)
+ UIの実装については説明しません(重要ではありません)。
  + イメージの生成中に表示されるローディングアニメーションと、シェーダースクリプトを利用したぼかし効果があります。
+ 全部で4つの美術館があります。
+ 各美術館は全て同じ機能(イメージ生成)のみです。
+ 美術館以外の機能(オプション、マニュアルなど)は実装されていません。
+ キャラクターの視点切り替えとスクリーンショット機能があります。
+ カメラがキャラクターに沿って移動します。
+ 一人称視点ではキャラクターをレンダリングしません。
### Controls
+ 左スティック：キャラクター移動
+ 右スティック：カメラ角度調整
+ スティッククリック：キャラクターの視点変換(3人称ではRaycastは動作しません)
+ トリガーボタン : 額縁とインタラクション
+ Bボタン：スクリーンショット
+ Yボタン : UI
+ A,Xボタン : キャラクター回転(3人称モードで)
### How to use
1.フォトフレームを指し示し、トリガーボタンを押すと録音が開始されます。
2.ボタンを押している間、録音が行われます。
3.フォトフレームを指し示し、トリガーボタンを離すと録画が終了します。
4.イメージが生成されると、フォトフレームに表示されます。
+ 片手または両手で操作することができます。
### Process
+ フレーム単位でオブジェクトのタグを確認します。
+ インタラクション(Raycast)を検出すると、オブジェクトのタグが変更され、録音が開始されます。
+ オーディオクリップは実際に押した分だけ録音されるのではなく、必ず20秒のオーディオクリップが生成されます。
 + Unityは、オーディオクリップを生成した後のサイズ変更はサポートしていません。
 + そのため、インタラクションが行われる時間を測定して、実際の録音時間として活用します。
+ 実際は20秒の録音を行い、インタラクションが終了した瞬間に録音を強制的に終了し、オーディオクリップの音声データを実際の録音時間分だけ取り込みます。
+ この時、取り込む音声データは非常に小さなfloat値です。(unity) ETRIはshort intデータを要求するため、正規化を行います。
+ その後、base64で暗号化してETRIに送信します。
+ ETRIから変換された韓国語テキストを受け取ったら、すぐにPapagoに送信します。
+ Papagoから結果を受け取ったら、すぐにKarloに送信します。
 + `Draw API.cs` はプロンプトの長さを確認し、base64で暗号化してkarloに送信します。
+ karloは、base64で暗号化され、PILで前処理されたbyteデータを結果として返します。
+ `DrawPicture.cs` はbase64を復号化し、PILの前処理をC#で再度行います。
+ 最後に、フォトフレームオブジェクトのマテリアルに合わせてイメージのサイズと倍率を調整します。
+ すべての処理はコルーチンを利用して非同期で行われます。
### Completed Issue(Exception handling)
+ 非常に短い時間記録された場合（1秒以下 - 「非常に短い時間」は開発者が設定することができます。）
+ 何も記録されていない場合
+ 2つのフォトフレームを同時に記録しようとした場合。
+ 変換されたプロンプトが長すぎる場合(最大256文字)
+ 録画の端が切り捨てられた場合
+ 録音時間が長すぎる場合
+ 録音中に他のコントローラーのインタラクションボタンを押すと、録音が強制的に終了する場合。
+ Karloから受け取った絵が表示されない場合。
+ オブジェクトマテリアルの接続エラー。
### Caution
+ 声が小さすぎたり、発音がつぶれていると認識率が低下します。
+ 韓国語のみサポートします。
+ **ETRI API要件 : short int, max 20 seconds, 16000Hz**.
+ 短い録音は認識されません： 1秒以下
+ 扇情的またはネガティブなイメージの生成に注意してください：Karloの内蔵フィルタリング機能がonになっていますが、完璧ではありません。
+ 終止音が ㄱ, ㄷ, ㅂ の場合、認識率が低下します。
### Scripts
+ `RecordVoice.cs` 
オブジェクトのタグを確認し、インタラクションが検出されたら録音を開始します。
録音が完了すると、オーディオクリップを `VoiceAPI.cs` に送信します。
録音時の例外処理機能を担当します。
+ `VoiceAPI.cs`
ETRI と通信するための前処理を行い、通信します(正規化、Base64)。
結果を受け取るとすぐにPapagoと通信し、結果を `DrawAPI.cs` に直接送信します。
+ `DrawAPI.cs`
通信仕様を確認し、Karloと通信します。
完了したら、生成されたイメージを `DrawPicture.cs` に送信します。
+ `DrawPicture.cs` 
Karloの結果を後処理し、オブジェクトのマテリアルに合わせてリスケーリングして適用します。
+ `MyScreenCapture.cs`
スクリーンショットはいつでも使用できます。
+ `HandRayControll.cs` 
Oculus SDKを使用して、Rayの開始位置をコントローラの位置にします。
Rayのレンダリング、色の変更、インタラクションするオブジェクトのタグを制御します（エラー防止タグを含む）。
+ `CharacterManager.cs 
キャラクターをレンダリングし、カメラとキャラクターオブジェクトの位置を変更するトランスフォーム情報を持っています。
+ `CharacterControl.cs` 
`Character Controller` コンポーネントを使用してキャラクターを動かします。
VRコントローラの入力に基づいてキャラクターを動かします。
PC版テスト用のキーボードコードも含まれています。
+ `CameraManager.cs `
キャラクターの位置に応じてカメラの位置を設定します。
一人称、三人称視点に合わせてカメラの位置を設定します。
3人称視点でカメラがマップを貫通しないように処理します。
+ `FollowCamera.cs` 
カメラがキャラクターを追いかけるようにします。
## Known Issue
+ Raycastが正しく機能せず、プロセスに時間がかかる、またはフリーズする場合：MacBookでのみ発生します。
+ **フォトフレームの外でインタラクションを完了するとエラーが発生します。**
 + フォトフレーム内でトリガーボタンを押して録画を開始し、フレームの外でトリガーボタンを離すと、20秒(最大録画時間)の間、強制的に録画が行われます。
 + この間、他のフォトフレームで録画を開始することはできず、その録画が終わるまで待つ必要があります。
 + トリガーボタンを押して録画を開始すると、トリガーボタンを離さない限り、Rayがフォトフレームの外に出ても構いません。
 + ただし、録画を終了するためには、必ずRayでフォトフレームを指差した状態でトリガーボタンを離す必要があります。
 + 録音終了条件：フォトフレームを指差した状態でトリガーボタンを離す。
 + 録画中に誤ってフォトフレームの外でトリガーボタンを離した場合、次のような解決方法があります。
 1. **待つ** : 当該録音プロセスが終わるまで待ってから再録音します。
 2.フォトフレームの外でトリガーボタンを押した後、フォトフレームを指してトリガーボタンを離します。
> 録画が進行中の場合、録画を終了する以外、フォトフレームとの全てのインタラクションが不可能になります。
> だから、フォトフレームの**外で**トリガーボタンを押さなければ認識され、その後、正常に録画を終了することができます。
---
![Screenshot (1)](https://github.com/WooChan-Noh/ImagineX/assets/103042258/def88683-38a0-422f-a6b3-0861f06d261f)
![Screenshot (2)](https://github.com/WooChan-Noh/ImagineX/assets/103042258/fb5da3ca-ee05-42db-a413-24a2f2d1674e)
![UI (2)](https://github.com/WooChan-Noh/ImagineX/assets/103042258/54832acc-ee9f-478a-b0bb-fc09592a33cc)
![test1](https://github.com/WooChan-Noh/ImagineX/assets/103042258/f4421a38-c78d-4df8-aa0f-b4d7258dfe88)
![test2](https://github.com/WooChan-Noh/ImagineX/assets/103042258/5d0c4538-6993-4f01-ac3e-9b363d7e9e84)

