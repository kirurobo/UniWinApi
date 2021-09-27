# An Example project for UniWinApi

> UniWinApi is an old project.  
> ***See my new repository [UniWindowController](https://github.com/kirurobo/uniwindowcontroller).***  
> It works on both Mac and Windows.  
---


## What's UniWinApi

[UniWinApi](https://github.com/kirurobo/UniWinApiAsset) is a library using Windows APIs. It allows us the following operations which are not available normary in Unity. 

* Move the window
* Resize the window
* Maximize the window
* Minimize the window
* **Make the window transparent** (Make the window non-rectangle and unframed)
* **Accept file drop**
* **Open file dialog** (Test implementation, single file only)
* Move the cursor pointer
* Send mouse button events


## What's this repository

This repository contains a project for a desktop mascot style VRM viewer as an application example of using UniWinApi.
[![UniWinApi VRM viewer](http://i.ytimg.com/vi/cq2g-hIGlAs/mqdefault.jpg)](https://youtu.be/cq2g-hIGlAs "UniWinApi VRM viewer v0.4.0 beta")  
Image video [YouTube](https://youtu.be/cq2g-hIGlAs)


## Download


* There is a pre-built VRM viewer 'UniWinApiVrmViewer' in [Releases](https://github.com/kirurobo/UniWinApi/releases) 
* [Latest UniWinApi core asset package](https://github.com/kirurobo/UniWinApi/releases/tag/v0.5.0)
<details>
  <summary>Archives</summary>
  
* [Ver.0.6.0 Added Allow Drop From Lower Privilege setting](https://github.com/kirurobo/UniWinApi/releases/tag/v0.6.0)
* [Ver.0.5.0 Added layered window mode](https://github.com/kirurobo/UniWinApi/releases/tag/v0.5.0)
* [Ver.0.4.0-beta](https://github.com/kirurobo/UniWinApi/releases/tag/v0.4.0beta)
* [Ver.0.3.3 Updated to UniVRM 0.44](https://github.com/kirurobo/UniWinApi/releases/tag/v0.3.3)
* [Ver.0.3.2 Added Looking at cursor](https://github.com/kirurobo/UniWinApi/releases/tag/v0.3.2)
* [Ver.0.3.1 Enable transparent on startup](https://github.com/kirurobo/UniWinApi/releases/tag/v0.3.1)
* [Ver.0.3.0 Added rotation and translation for light](https://github.com/kirurobo/UniWinApi/releases/tag/v0.3.0)
* [Ver.0.2.3 Imported UniVRM 0.42. Set FOV at 10deg.](https://github.com/kirurobo/UniWinApi/releases/tag/v0.2.3)
* [Ver.0.2.2 Set the light color white](https://github.com/kirurobo/UniWinApi/releases/tag/v0.2.2)
* [Ver.0.2.1 Updated the shader](https://github.com/kirurobo/UniWinApi/releases/download/v0.2.1/UniWinApiVrmViewer_x64_v0.2.1.zip)
* [Ver.0.2.0 Firtst release](https://github.com/kirurobo/UniWinApi/releases/download/v0.2.0/UniWinApiVrmViewer_x64.zip)

</details>



## License

UniWinApiAsset is CC0, but several other projects are used in this VRM viewer.

* CC0
  * [UniWinApiAsset](http://github.com/kirurobo/UniWinApiAsset)
    * The asset in the .unitypackage except for the VRM viewer.
    * The source code is maintained in another repository.

* MIT License
  * [UniVRM](https://github.com/vrm-c/UniVRM/) by [VRM consortium](https://vrm-consortium.org/)
  * [VRMLoaderUI](https://github.com/m2wasabi/VRMLoaderUI/) by [えむにわ @m2wasabi](https://twitter.com/m2wasabi)

* Other
  * [HumanoidCollider](https://github.com/yuzu-unity/HumanoidCollider) / [Article on Qiita](https://qiita.com/Yuzu_Unity/items/b645ecb76816b4f44cf9)
 by [ゆず @Yuzu_Unity](https://twitter.com/Yuzu_Unity)
  * UI-Default-ZWrite in the CustomShaders folder

## System requirements

* 2018.4.20 or lator
* Windows 10


## Usage

If you just want to try to run the VRM viewer, extract the downloaded zip file and run UniWinApiVrmViewer.exe.  
After launching, drag and drop your VRM model and the model will be displayed.

To use UniWinApi-vXXXXX.unitypackage without the VRM viewer project (which is the main part of UniWinApi), watch a [tutorial](https://github.com/kirurobo/UniWinApi/blob/master/docs/index.md).

To build the VRM viewer, clone the repository and open it in Unity's editor.
* I used to use Unity-chan's animation, but since we excluded that, you can open it without preparing other assets separately.
