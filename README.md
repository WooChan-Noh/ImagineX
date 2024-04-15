[Read Me - English](https://github.com/WooChan-Noh/ImagineX/blob/main/ReadMeEng.md)    
[Read Me - Japanese](https://github.com/WooChan-Noh/ImagineX/blob/main/ReadMeJp.md)
# ImagineX
+ 이 프로젝트의 목적은 생성형 AI를 쉽게 사용하는 것입니다.
  + 음성 명령을 바탕으로 생성형 AI를 활용하여 그림을 생성합니다. 
+ UI design was done by designer from _TinyGem_
+ 한국어만 가능합니다.
+ 유로 에셋이 포함되어 있습니다. (Map, UI effect)
## Overview
### What it does
+ 음성을 입력으로 받아 한국어 텍스트로 변환합니다.
+ 한국어 텍스트를 영어 텍스트로 바꿉니다.
+ 생성 AI를 활용합니다. (REST통신)
+ 생성된 그림을 표시합니다.
### Test Environment (Only available in VR)
+ Meta Quest2
+ Meta Quest Pro 
### I used this tech
+ ETRI : SST (voice to text) [API](https://aiopen.etri.re.kr/guide/Recognition)
+ Papago : 번역 (kor to eng) [API](https://developers.naver.com/docs/papago/README.md)
+ Karlo : 생성형 AI [API](https://developers.kakao.com/product/karlo)
+ Oculus SDK and API : Follow official guide [Official guide](https://developer.oculus.com/documentation/unity/unity-gs-overview/), [Controller API](https://developer.oculus.com/documentation/unity/unity-ovrinput/)
## Learn more
+ VR 개발 환경 세팅이 필요합니다.
> + "Oculus Integration" Package Import (Asset Store)    
> + Project Settings - XR Plug-in Management install -> Oculus check      
> + Project Settings - Player - Minimum API Level : Android 10.0 (Level 29) check    
> + Project Settings - Oculus - Fix and Apply all recommend    
> + Build Settings - Texture Compression : ASTC change    
> + Meta Quest device dev mode on (moblie app)    
> + In MQHD app, OVR Monitor Metrics install    
> + Complete software updates for the device and controllers    
> + Apps you need [Oculus](https://www.meta.com/kr/ko/quest/setup/?utm_source=www.meta.com&utm_medium=oculusredirect), [MQHD](https://developer.oculus.com/downloads/package/oculus-developer-hub-win)
+ UI 구현에 대해서는 설명하지 않습니다.(중요하지 않음)
  + 그림이 생성되는 동안 표시될 로딩 애니메이션과 셰이더 스크립트를 활용한 블러 효과가 있습니다.
+ 총 4개의 미술관이 있습니다.
+ 각각 미술관은 모두 같은 기능(그림 생성)만 합니다.
+ 미술관을 제외한 기능(옵션, 설명서 등)은 구현되지 않았습니다.
+ 캐릭터 시점 전환 및 스크린샷 기능이 있습니다.
+ 카메라가 캐릭터를 따라 이동합니다.
+ 1인칭 시점에서는 캐릭터를 렌더링하지 않습니다.
### Controls
+ 왼쪽 스틱 : 캐릭터 이동
+ 오른쪽 스틱 : 카메라 각도 조절
+ 스틱 클릭 : 캐릭터 시점 변환 (3인칭에서는 Raycast는 동작하지 않습니다)
+ 트리거 버튼 : 액자와 상호작용
+ B 버튼 : 스크린샷
+ Y 버튼 : UI
+ A,X 버튼 : 캐릭터 회전(3인칭 모드)
### How to use
1. 액자를 가리키고 트리거 버튼을 누르면 녹음이 시작됩니다.
2. 버튼을 누르고 있는 동안 녹음이 진행됩니다.
3. 액자를 가리키고 트리거 버튼을 떼면 녹음이 종료됩니다.
4. 사진이 생성되면 액자에 표시됩니다.
+ 한 손이나 양 손으로 상호작용 할 수 있습니다.
### Process
+ 프레임 단위로 오브젝트의 태그를 확인합니다.
+ 상호 작용(Raycast)를 감지하면 오브젝트의 태그가 변경되고 녹음이 시작됩니다.
+ 오디오 클립은 실제로 누르는 만큼 녹음되는 것이 아니라 무조건 20초짜리 오디오 클립이 생성됩니다.
  + 유니티는 오디오 클립을 생성한 후 크기 변경을 지원하지 않습니다.
  + 그래서 상호 작용이 일어나는 시간을 측정하여 실제 녹음 시간으로 활용합니다.
+ 실제로는 무조건 20초 녹음을 진행하고, 상호 작용이 끝나는 순간 녹음을 강제로 종료한 다음 오디오 클립의 음성 데이터를 실제 녹음 시간만큼만 가져옵니다.
+ 이 때, 가져오는 음성 데이터는 매우 작은 float 값입니다.(유니티) ETRI는 short int 데이터를 요구하기 때문에 정규화를 진행합니다.
+ 이후 base64로 암호화하여 ETRI로 전송합니다.
+ ETRI로부터 변환된 한국어 텍스트를 받으면, 즉시 Papago로 전송합니다.
+ Papago로부터 결과를 받으면, 즉시 Karlo로 전송합니다.
  + `Draw API.cs`는 프롬프트 길이를 확인하고 base64로 암호화하여 karlo에게 전송합니다.
+ karlo는 base64로 암호화되고 PIL로 전처리된 byte 데이터를 결과로 반환합니다.
+ `DrawPicture.cs`는 base64를 복호화하고, PIL전처리 과정을 C#으로 다시 수행합니다.
+ 마지막으로, 액자 오브젝트 머터리얼에 맞게 그림의 크기와 배율을 조정합니다.
+ 모든 과정은 코루틴을 활용하여 비동기로 진행됩니다.
### Completed Issue(Exception handling)
+ 매우 짧은 시간 동안 녹음된 경우 (1초 이하 - "매우 짧은 시간"은 개발자가 설정할 수 있음).
+ 아무것도 녹화되지 않은 경우
+ 두 액자를 동시에 녹음하려고 시도하는 경우
+ 변환된 프롬프트가 너무 긴 경우(최대256자)
+ 녹음 끝부분이 잘린 경우
+ 너무 오래 녹음한 경우
+ 녹음 중 다른 컨트롤러의 상호작용 버튼을 누르면 녹음이 강제로 종료되는 경우
+ Karlo에서 받은 그림이 나타나지 않는 경우
+ 오브젝트 머터리얼 연결 오류.
### Caution
+ 목소리가 너무 작거나 발음이 뭉개지면 인식률이 떨어집니다.
+ 한국어만 지원합니다.
+ **ETRI API 요구 사항 : short int, max 20 seconds, 16000Hz**
+ 짧은 녹음은 인식되지 않습니다 : 1초 이하
+ 선정적이거나 부정적인 이미지 생성에 주의하세요 : Karlo의 내장 필터링 기능이 켜져 있지만 완벽하지 않습니다.
+ 종성이 ㄱ, ㄷ, ㅂ일 때 인식률이 떨어집니다.
### Scripts
+ `RecordVoice.cs`    
오브젝트의 태그를 확인하고 상호작용이 감지되면 녹음을 시작합니다.
녹음이 완료되면 오디오 클립을 `VoiceAPI.cs`로 보냅니다.
녹음 시 예외 처리 기능을 담당합니다.
+ `VoiceAPI.cs`    
ETRI와 통신하기 위한 전처리 작업을 수행하고 통신합니다. (정규화, Base64).
결과를 받으면 바로 파파고와 통신하고, 결과를 `DrawAPI.cs`로 바로 전송합니다.
+ `DrawAPI.cs`    
통신 규격을 확인하고 Karlo와 통신합니다.
완료되면 생성된 결과를 `DrawPicture.cs`로 전송합니다.
+ `DrawPicture.cs`     
Karlo 결과를 후처리하고 오브젝트 재질에 맞게 리스케일링한 후 적용합니다.
+ `MyScreenCapture.cs`    
스크린샷은 언제든지 사용할 수 있습니다.
+ `HandRayControll.cs`    
오큘러스 SDK를 사용하여 Ray의 시작 지점을 컨트롤러의 위치로 합니다.
Ray의 렌더링 여부, 색상 변경, 상호 작용하는 오브젝트의 태그를 제어합니다. (오류 방지 태그 포함)
+ `CharacterManager.cs`    
캐릭터를 렌더링하고 카메라와 캐릭터 오브젝트의 위치를 변경하는 트랜스폼 정보를 가지고 있습니다.
+ `CharacterControll.cs`    
`Character Controller` 컴포넌트를 사용하여 캐릭터를 움직입니다.
VR 컨트롤러 입력을 기반으로 캐릭터를 움직입니다.
PC 버전 테스트를 위한 키보드 코드도 포함되어 있습니다
+ `CameraManager.cs`    
캐릭터의 위치에 따라 카메라의 위치를 설정합니다.
1인칭, 3인칭 시점에 맞게 카메라 위치를 설정합니다.
3인칭 시점에서 카메라가 맵을 뚫고 지나가지 않게 처리합니다.
+ `FollowCamera.cs`    
카메라가 캐릭터를 따라다니게 합니다
## Known Issue
+ Raycast가 제대로 작동하지 않고 프로세스가 오래 걸리거나 멈추는 경우 : MacBook에서만 발생합니다.
+ **액자 밖에서 상호작용을 완료하면 오류가 발생합니다.**
  + 액자 안에서 트리거 버튼을 눌러서 녹음을 시작한 다음, 액자 밖에서 트리거 버튼을 떼면 20초(최대 녹음 시간)동안 강제로 녹음이 진행됩니다.
  + 이 동안 다른 액자에서 녹음을 시작할 수 없으며 해당 녹음이 끝날 때까지 기다려야 합니다.
  + 트리거 버튼을 눌러서 녹음을 시작하면, 트리거 버튼을 떼지 않는 한 Ray가 액자 밖으로 빠져나가도 상관없습니다.
  + 하지만 녹음을 종료하기 위해서는, 반드시 Ray로 액자를 가리킨 상태에서 트리거 버튼을 떼야 합니다.
  + 녹음 종료 조건 : 액자를 가리킨 상태에서 트리거 버튼을 놓기
    + 녹음 중에 실수로 액자 밖에서 트리거 버튼을 놓았다면, 다음과 같은 해결 방법이 있습니다.
      1. **기다리기** : 해당 녹음 프로세스가 끝날 때 까지 기다렸다가 다시 녹음합니다.
      2. 액자 밖에서 트리거 버튼을 누른 다음, 액자를 가리키고 트리거 버튼을 떼기 
      > + 녹음이 진행중인 경우, 녹음을 종료하는 것 외에는 액자와의 모든 상호 작용이 불가능하게 됩니다. 
      > + 그래서 액자 **밖에서** 트리거 버튼을 눌러야 인식이 되고, 이후에 정상적으로 녹음을 종료하면 됩니다.
---
![Screenshot (1)](https://github.com/WooChan-Noh/ImagineX/assets/103042258/def88683-38a0-422f-a6b3-0861f06d261f)
![Screenshot (2)](https://github.com/WooChan-Noh/ImagineX/assets/103042258/fb5da3ca-ee05-42db-a413-24a2f2d1674e)
![UI (2)](https://github.com/WooChan-Noh/ImagineX/assets/103042258/54832acc-ee9f-478a-b0bb-fc09592a33cc)
![test1](https://github.com/WooChan-Noh/ImagineX/assets/103042258/f4421a38-c78d-4df8-aa0f-b4d7258dfe88)
![test2](https://github.com/WooChan-Noh/ImagineX/assets/103042258/5d0c4538-6993-4f01-ac3e-9b363d7e9e84)

