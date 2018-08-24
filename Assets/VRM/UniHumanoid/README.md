# UniHumanoid

Unity humanoid utility with bvh importer.

## License

* [license](./LICENSE.md)

## BVH Importer

Drop bvh file to Assets folder.
Then, AssetPostprocessor import bvh file and create prefab includes AnimationClip with Avatar.
Although avtar creation may fail, in that case you can create it manually later.

![bvh prefab](doc/bvh_prefab.png)

Instanciate prefab to scene.

![bvh gameobject](doc/bvh_gameobject.png)

That object can play. 

## Transfer humanoid pose to other humanoid

Set avatar to HumanPoseTransfer attached to bvh gameobject.

Instanciate target humanoid model from asset, For example fbx.
Attach HumanPoseTransfer to new human model and set to bvh HumanPoseTransfer's Source of bvh gameobject. 
![humanpose transfer target](doc/humanpose_transfer_inspector.png)

Then, Bvh animtion copy to new humanoid ! 
![humanpose transfer](doc/humanpose_transfer.png)

## BoneMapping

This script help create human avatar from GameObject hierarchy.
First, attach this script to GameObject that has Animator with HumanAvatar.

Next, setup below.

* model position is origin
* model look at +z orientation
* model root node rotation is Quatenion.identity
* Set hips bone.

press Guess bone mapping.
If fail to guess bone mapping, you can set bones manually.

Optional, press Ensure T-Pose.
Create avatar.

![bvh bone mapping](doc/bvh_bonemapping.png)

These humanoids imported by [UniGLTF](https://github.com/ousttrue/UniGLTF) and created human avatar by BoneMapping. 

![humanoid](doc/humanoid.gif)

