# UniHumanoid

Unity humanoid utility with bvh importer.

## License

* [MIT](./LICENSE.md)

## BVH files

* https://sites.google.com/a/cgspeed.com/cgspeed/motion-capture
* http://mocapdata.com/
* http://www.thetrailerspark.com/download/Mocap/Packed/EYES-JAPAN/BVH/

## BVH Asset Importer

Drop bvh file to Assets folder.
Then, AssetPostprocessor import bvh file.

* create a hierarchy prefab 
* create a humanoid Avatar
* create a legacy mode AnimationClip
* create a skinned mesh for preview

![bvh prefab](doc/assets.png)

Instanciate prefab to scene.

![bvh gameobject](doc/mesh.png)

That object can play. 


## BVH Runtime Importer

```cs
var context = new UniHumanoid.ImporterContext
{
    Path = path
};
UniHumanoid.BvhImporter.Import(context);
GameObject root = context.Root;
```

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

