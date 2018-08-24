using System.IO;
using System.Text;
using UnityEngine;


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
            var animator=context.Root.AddComponent<Animator>();

            BuildHierarchy(context.Root.transform, context.Bvh.Root, 1.0f);

            var minY = 0.0f;
            foreach (var x in context.Root.transform.Traverse())
            {
                if (x.position.y < minY)
                {
                    minY = x.position.y;
                }
            }

            var toMeter = 1.0f / (-minY);
            Debug.LogFormat("minY: {0} {1}", minY, toMeter);
            foreach (var x in context.Root.transform.Traverse())
            {
                x.localPosition *= toMeter;
            }

            // foot height to 0
            var hips = context.Root.transform.GetChild(0);
            hips.position = new Vector3(0, -minY * toMeter, 0);

            //
            // create AnimationClip
            //
            context.Animation = BvhAnimation.CreateAnimationClip(context.Bvh, toMeter);
            context.Animation.name = context.Root.name;
            context.Animation.legacy = true;
            context.Animation.wrapMode = WrapMode.Loop;

            var animation = context.Root.AddComponent<Animation>();
            animation.AddClip(context.Animation, context.Animation.name);
            animation.clip = context.Animation;
            animation.Play();

            var boneMapping = context.Root.AddComponent<BoneMapping>();
            boneMapping.Bones[(int)HumanBodyBones.Hips] = hips.gameObject;
            boneMapping.GuessBoneMapping();
            var description = AvatarDescription.Create();
            BoneMapping.SetBonesToDescription(boneMapping, description);
            context.Avatar = description.CreateAvatar(context.Root.transform);
            context.Avatar.name = "Avatar";
            context.AvatarDescription = description;
            animator.avatar = context.Avatar;

            var humanPoseTransfer = context.Root.AddComponent<HumanPoseTransfer>();
            humanPoseTransfer.Avatar = context.Avatar;

            // create SkinnedMesh for bone visualize
            var renderer = SkeletonMeshUtility.CreateRenderer(animator);
            context.Material = new Material(Shader.Find("Standard"));
            renderer.sharedMaterial = context.Material;
            context.Mesh = renderer.sharedMesh;
            context.Mesh.name = "box-man";
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
