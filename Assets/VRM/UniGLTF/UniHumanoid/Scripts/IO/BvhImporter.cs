using System.IO;
using System.Text;
using UnityEngine;
using System.Linq;
using System;

namespace UniHumanoid
{
    public static class BvhImporter
    {
        public static void Import(ImporterContext context)
        {
            //
            // parse
            //
            context.Source = File.ReadAllText(context.Path, Encoding.UTF8);
            context.Bvh = Bvh.Parse(context.Source);
            Debug.LogFormat("parsed {0}", context.Bvh);

            //
            // build hierarchy
            //
            context.Root = new GameObject(Path.GetFileNameWithoutExtension(context.Path));

            BuildHierarchy(context.Root.transform, context.Bvh.Root, 1.0f);

            var hips = context.Root.transform.GetChild(0);
            var estimater = new BvhSkeletonEstimator();
            var skeleton = estimater.Detect(hips.transform);
            var description = AvatarDescription.Create();
            //var values= ((HumanBodyBones[])Enum.GetValues(typeof(HumanBodyBones)));
            description.SetHumanBones(skeleton.ToDictionary(hips.Traverse().ToArray()));

            //
            // scaling. reposition
            //
            float scaling = 1.0f;
            {
                //var foot = animator.GetBoneTransform(HumanBodyBones.LeftFoot);
                var foot = hips.Traverse().Skip(skeleton.GetBoneIndex(HumanBodyBones.LeftFoot)).First();
                var hipHeight = hips.position.y - foot.position.y;
                // hips height to a meter
                scaling = 1.0f / hipHeight;
                foreach (var x in context.Root.transform.Traverse())
                {
                    x.localPosition *= scaling;
                }

                var scaledHeight = hipHeight * scaling;
                hips.position = new Vector3(0, scaledHeight, 0); // foot to ground
            }

            //
            // avatar
            //
            context.Avatar = description.CreateAvatar(context.Root.transform);
            context.Avatar.name = "Avatar";
            context.AvatarDescription = description;
            var animator = context.Root.AddComponent<Animator>();
            animator.avatar = context.Avatar;

            //
            // create AnimationClip
            //
            context.Animation = BvhAnimation.CreateAnimationClip(context.Bvh, scaling);
            context.Animation.name = context.Root.name;
            context.Animation.legacy = true;
            context.Animation.wrapMode = WrapMode.Loop;

            var animation = context.Root.AddComponent<Animation>();
            animation.AddClip(context.Animation, context.Animation.name);
            animation.clip = context.Animation;
            animation.Play();

            var humanPoseTransfer = context.Root.AddComponent<HumanPoseTransfer>();
            humanPoseTransfer.Avatar = context.Avatar;

            // create SkinnedMesh for bone visualize
            var renderer = SkeletonMeshUtility.CreateRenderer(animator);
            context.Material = new Material(Shader.Find("Standard"));
            renderer.sharedMaterial = context.Material;
            context.Mesh = renderer.sharedMesh;
            context.Mesh.name = "box-man";

            context.Root.AddComponent<BoneMapping>();
        }

        static void BuildHierarchy(Transform parent, BvhNode node, float toMeter)
        {
            var go = new GameObject(node.Name);
            go.transform.localPosition = node.Offset.ToXReversedVector3() * toMeter;
            go.transform.SetParent(parent, false);

            //var gizmo = go.AddComponent<BoneGizmoDrawer>();
            //gizmo.Draw = true;

            foreach (var child in node.Children)
            {
                BuildHierarchy(go.transform, child, toMeter);
            }
        }
    }
}
