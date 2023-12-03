# ImagineX (in progress)
+ The purpose of this project to make generative AI easy to use. (Illustration)
+ UI design was done by designer from _TinyGem_
+ Only available in Korean
+ This project contain paid assets. (Map, UI effect)
## Overview
### What it does
+ Voice as input and converts them into kor text 
+ Translate kor text to eng(as prompt)
+ Communicate with generative AI
+ Show the generated picture
### Test Environment (Only available in VR)
+ Meta Quest2
+ Meta Quest Pro 
### I used this tech
+ ETRI : SST (voice to text) [API](https://aiopen.etri.re.kr/guide/Recognition)
+ Papago : Translate (kor to eng) [API](https://developers.naver.com/docs/papago/README.md)
+ Karlo : Generative AI [API](https://developers.kakao.com/product/karlo)
+ Oculus SDK and API : Follow official guide [Official guide](https://developer.oculus.com/documentation/unity/unity-gs-overview/), [Controller API](https://developer.oculus.com/documentation/unity/unity-ovrinput/)
## Learn more
+ Need to set preferences for VR development in Unity
> + "Oculus Integration" Package Import (Asset Store)    
> + Project Settings - XR Plug-in Management install -> Oculus check      
> + Project Settings - Player - Minimum API Level : Android 10.0 (Level 29) check    
> + Project Settings - Oculus - Fix and Apply all recommend    
> + Build Settings - Texture Compression : ASTC change    
> + Meta Quest device dev mode on (moblie app)    
> + In MQHD app, OVR Monitor Metrics install    
> + Complete software updates for the device and controllers    
> + Apps you need [Oculus](https://www.meta.com/kr/ko/quest/setup/?utm_source=www.meta.com&utm_medium=oculusredirect), [MQHD](https://developer.oculus.com/downloads/package/oculus-developer-hub-win)
+ I don't explane UI implementation (it's not important)
  + Add a loading animation and use a shader script for a blur effect
+ There are 4 museum scenes in total.(These are all demo scenes from paid assets)
+ Each scene does same thing (picture creation)
+ Except for the museum scene, such as the `main menu` and `options`, is not completely implemented (including UI).
+ Currently, only the museum interior is partially implemented (essential features and behaviors are complete, but the UI is not yet complete)
+ Character perspective switching and screenshot functionality is present
+ Camera follow the character
+ First-person perspective does not rendering the character.
### Controls
+ Left stick: Character movement
+ Right stick: Camera angle
+ Left/Right stick click (thumb pressed) : Change Character perspective. In Third-person mode, you can't interact with the picture frame. (Rays disappear)
+ Trigger button: Interact with the picture frame
+ B button : Screenshot
+ Y button : UI
+ A,X button : Turn around character (**Only Third-person mode**)
### How to use
1. Point to the picture frame and press the trigger button to start recording.
2. Recording will continue while the button is pressed. (The trigger button must be press while recording)
3. Release the trigger button while pointing at the picture frame to end the recording.
4. If a picture is created, it is automatically displayed in the frame.
+ Interaction can be done with one or both hands.
### Process
+ Checking the tag of the frame object in `Update`
+ Triggers an interaction (ray collision detection), the frame object's tag is changed and recording starts
+ The audio clip is not actually recorded as long as you press it, but is fixed at a maximum of 20 seconds.
  + Because Unity doesn't allow you to change the time of an audio clip once it's created.
+ So we measure the time while the trigger is pressed, and use as actual recording time
+ What we're actually doing is recording 20 seconds unconditionally, forcing it to end the moment you release the trigger, and then resizing audio clip.
+ The audio clips recorded in Unity have `float` values that are too small, so they are normalized to fit the `short int` values required by **ETRI API**
+ This is then encrypted with base64 and sent to **ETRI**
+ Voice converted to kor text and sent directly to **Papago**.
+ We then receive the translation (prompt) and send it directly to the **Karlo**.
+ Draw API script checks the length of the prompt (**Karlo prompt max 256**) and converts it to base64 to confirm the communication specification.
+ Karlo then sends the image bytes, which are base64 encrypted and preprocessed with PIL.
+ The DrawScript decrypts the base64 and preprocesses the PIL back to C#.
+ Finally, it resizes and scales the picture to fit the size of the frame object material.
+ All of this happens in a single step(recording -> convert to kor text -> translation -> karlo -> apply to material),    
and it's all coroutine and asynchronous.
### Completed Issue(Exception handling)
+ Recorded for a very short time (1 second or less - "very short time" can be set by the developer.)
+ If nothing was recorded
+ Trying to record two frames at the same time
+ The converted prompt too long
+ The end of the recording is cut off
+ Recording too long
+ Pressing the interaction button with the other controller while recording forces the recording to end.
+ The picture received from the Karlo is not displayed correctly
+ Object material connection errors.
### Caution
+ Beware of too small a voice or muffled pronunciation
+ Doesn't work if nothing is recorded
+ Only support Korean
+ **ETRI API requirements: short int, max 20 seconds, 16000Hz**
+ Short recordings will not be ignored : 1 second or less 
+ Beware of creating sensationalized or negative images : Karlo's built-in filtering is turned on, but it's not perfect.
+ Low recognition when the last word ends in [ㄱ, ㄷ, ㅂ].
### Scripts
+ `RecordVoice.cs`    
`Update` checks for tags and starts recording when an interaction is detected.    
When it finishes recording, it sends the audio clip to the `VoiceAPI.cs`.    
Includes functions for exception handling 
+ `VoiceAPI.cs`    
Preprocessing tasks for sending to **ETRI**, and communicate with ETRI.(normalized, ToBase64)    
It then immediately communicates with **Papago** to get the prompt, which are then sent directly to `DrawAPI.cs`    
+ `DrawAPI.cs`    
Checks the conditions for communication, and communicates with **Karlo** to receive the picture.    
Send results to the `DrawPicture.cs` when finished
+ `DrawPicture.cs`     
Post-process the results from Karlo, rescale them to match the object materials, and apply them.
+ `MyScreenCapture.cs`    
You can always use screenshots, but the character turning around only works in third-person mode.    
+ `HandRayControll.cs`    
using the Oculus SDK, set up a Ray with the controller's location as the origin.    
Controll Ray is rendered or not, color change, and the tag of the interacted object.    
Tags are included to prevent errors.
+ `CharacterManager.cs`    
Rendering the character and has Transform information to change the position of the camera and character object.
+ `CharacterControll.cs`    
Move the character using the `Character Controller` component    
Based on VR controller input.    
Keyboard code is also included for testing the PC version.
+ `CameraManager.cs`    
Sets the position of the camera based on the character's location.    
Position the camera to match the perspective.    
For third-person, using Ray to don't cross the map object 
+ `FollowCamera.cs`    
Make the camera follow the character
## Known Issue
+ Ray doesn't work properly and Processes take a long time or get stuck - **Only happens on MacBook**
+ **If you finish an interaction outside the frame**
  + If you press the trigger button in the frame to start recording, and then release the trigger button **outside the frame**, the recording is forced for 20 seconds (maximum recording time).
  + During this time, you will not be able to record another picture frame and will have to wait for it to finish processing.
  + Once you start recording, you can move Ray outside the frame as long as the trigger button is pressed until recording ends
  + but you must release the trigger button **while pointing at the frame** to end the recording.
  + Because recording END condition is the trigger button released **while pointed at the frame**
    + If you accidentally release the trigger button outside of the frame while recording, you have two options
      1. **Wait** for the recording process to end for that frame and re-record.
      2. Press the trigger button on the **outside of the frame**, point to the frame **while holding** the trigger button, and release the trigger button    
      > + If a recording is in progress, all interaction with the picture frame is **blocked** except to end the recording    
      > + So you must press the trigger button on the **outside of the picture frame**, hold it down, point to the picture frame, and release the trigger button to end the recording.
---
![Screenshot (1)](https://github.com/WooChan-Noh/ImagineX/assets/103042258/def88683-38a0-422f-a6b3-0861f06d261f)
![Screenshot (2)](https://github.com/WooChan-Noh/ImagineX/assets/103042258/fb5da3ca-ee05-42db-a413-24a2f2d1674e)
![UI (2)](https://github.com/WooChan-Noh/ImagineX/assets/103042258/54832acc-ee9f-478a-b0bb-fc09592a33cc)
![test1](https://github.com/WooChan-Noh/ImagineX/assets/103042258/f4421a38-c78d-4df8-aa0f-b4d7258dfe88)
![test2](https://github.com/WooChan-Noh/ImagineX/assets/103042258/5d0c4538-6993-4f01-ac3e-9b363d7e9e84)

