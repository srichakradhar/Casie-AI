# Student Helper Unity 3D Chatbot

## Hack-a-Roo Spring 2021 Submission

### Team Ethnic Gems
* Srichakradhar Reddy
* Rohit Reddy

# Links

* [Demo](https://youtu.be/IPoA1alUIWI)
* [Watson Unity SDK](https://github.com/IBM/unity-sdk)
* [Unity Core SDK](https://github.com/IBM/unity-sdk-core)

## Flow
* User interacts in augmented reality and gives voice commands such as "Walk Forward".
* The phone microphone picks up the voice command and the running application sends it to Watson Speech-to-Text.
* Watson Speech-to-Text converts the audio to text and returns it to the running application on the phone.
* The application sends the text to Watson Assistant. Watson assistant returns the recognized intent "Forward". The intent triggers an animation state event change.
* The application sends the response from Watson Assistant to Watson Text-to-Speech.
* Watson Text-to-Speech converts the text to audio and returns it to the running application on the phone.
* The application plays the audio response and waits for the next voice command.

## Included components

* [IBM Watson Assistant](https://www.ibm.com/watson/developercloud/conversation.html): Create a chatbot with a program that conducts a conversation via auditory or textual methods.
* [IBM Watson Speech-to-Text](https://www.ibm.com/watson/developercloud/speech-to-text.html): Converts audio voice into written text.
* [IBM Watson Text-to-Speech](https://www.ibm.com/watson/developercloud/speech-to-text.html): Converts written text into audio.

## Featured technologies

* [Unity](https://unity3d.com/): A cross-platform game engine used to develop video games for PC, consoles, mobile devices and websites.
* [AR Foundation](https://docs.unity3d.com/Packages/com.unity.xr.arfoundation@1.0/manual/index.html): A Unity package for AR functionality to create augmented reality experiences.

# Steps

1. [Prerequisites](#1-before-you-begin)
2. [IBM Cloud services](#2-create-ibm-cloud-services)
3. [Building and Running](#3-building-and-running)

## 1. Prerequisites

* [IBM Cloud Account](http://ibm.biz/Bdimr6)
* [Unity](https://unity3d.com/get-unity/download)

## 2. IBM Cloud services

In [IBM Cloud](https://cloud.ibm.com/):

1. [Speech-To-Text](https://cloud.ibm.com/catalog/speech-to-text/) service instance.
2. [Text-to-Speech](https://cloud.ibm.com/catalog/text-to-speech/) service instance.
3. [Assistant](https://cloud.ibm.com/catalog/services/conversation/) service instance.

## 3. Building and Running

> Note: This has been compiled and tested using Unity 2018.3.0f2 and Watson SDK for Unity 3.1.0 (2019-04-09) & Unity Core SDK 0.2.0 (2019-04-09).

The directories for unity-sdk and unity-sdk-core are blank within the Assets directory, placeholders for where the SDKs should be. Either delete these blank directories or move the contents of the SDKs into the directories after the following commands.

1. Download the [Watson SDK for Unity](https://github.com/watson-developer-cloud/unity-sdk) or perform the following:

`git clone https://github.com/watson-developer-cloud/unity-sdk.git`

Make sure you are on the 3.1.0 tagged branch.

2. Download the [Unity Core SDK](https://github.com/IBM/unity-sdk-core) or perform the following:

`git clone https://github.com/IBM/unity-sdk-core.git`

Make sure you are on the 0.2.0 tagged branch.

3. Open Unity and inside the project launcher select the ![Open](doc/source/images/unity_open.png?raw=true) button.
4. If prompted to upgrade the project to a newer Unity version, do so.
5. Follow [these instructions](https://github.com/watson-developer-cloud/unity-sdk#getting-the-watson-sdk-and-adding-it-to-unity) to add the Watson SDK for Unity downloaded in step 1 to the project.
6. Follow [these instructions](https://github.com/watson-developer-cloud/unity-sdk#configuring-your-service-credentials) to create your Speech To Text, Text to Speech, and Watson Assistant services and find your credentials using [IBM Cloud](https://cloud.ibm.com)

**Please note, the following instructions include scene changes and game objects have been added or replaced for AR Foundation.**

7. In the Unity Hierarchy view, click to expand under `AR Default Plane`, click `DefaultAvatar`. If you are not in the Main scene, click `Scenes` and `Main` in your Project window, then find the game objects listed above.
8. In the Inspector you will see Variables for `Speech To Text`, `Text to Speech`, and `Assistant`. If you are using US-South or Dallas, you can leave the `Assistant URL`, `Speech to Text URL`, and `Text To Speech URL` blank, taking on the default value as shown in the WatsonLogic.cs file. If not, please provide the URL values listed on the Manage page for each service in IBM Cloud.
9. Fill out the `Assistant Id`, `Assistant IAM Apikey`, `Speech to Text Iam Apikey`, `Text to Speech Iam Apikey`. All Iam Apikey values are your API key or token, listed under the URL on the Manage page for each service.

!["Unity Editor enter credentials"](doc/source/images/UnityEditorUpdated.png?raw=true)

### Building for iOS
Build steps for iOS have been tested with iOS 11+ and Xcode 10.2.1.

1. To Build for iOS and deploy to your phone, you can _File_ -> _Build_ Settings (Ctrl + Shift +B) and click Build.
2. When prompted you can name your build.
3. When the build is completed, open the project in Xcode by clicking on `Unity-iPhone.xcodeproj`.
4. Follow [steps](https://help.apple.com/xcode/mac/current/#/dev60b6fbbc7) to sign your app. Note - you must have an Apple Developer Account.
5. Connect your phone via USB and select it from the target device list at the top of Xcode. Click the play button to run it.
6. Alternately, connect the phone via USB and _File_-> _Build and Run_ (or Ctrl+B).

### Building for Android
Build steps for Android have been tested with Pie on a Pixel 2 device with Android Studio 3.4.1.

1. To Build for Android and deploy to your phone, you can _File_ -> _Build_ Settings (Ctrl + Shift +B) and click Switch Platform.
2. The project will reload in Unity. When done, click Build.
3. When prompted you can name your build.
4. When the build is completed, install the APK on your emulator or device.
5. Open the app to run.

# Troubleshooting

AR features are only available on iOS 11+ and can not run on an emulator/simulator. Be sure to check your player settings to target minimum iOS device of 11, and your Xcode deployment target (under deployment info) to be 11 also.

In order to run the app you will need to sign it. Follow steps [here](https://help.apple.com/xcode/mac/current/#/dev60b6fbbc7).

Mojave updates may adjust security settings and block microphone access in Unity. If Watson Speech to Text appears to be in a ready and listening state but not hearing audio, make sure to check your security settings for microphone permissions. For more information: https://support.apple.com/en-us/HT209175.

You may need the [ARCore APK](https://github.com/google-ar/arcore-android-sdk/releases) for your Android emulator. This pattern has been tested with ARCore SDK v1.9.0 on a Pixel 2 device running Pie.
