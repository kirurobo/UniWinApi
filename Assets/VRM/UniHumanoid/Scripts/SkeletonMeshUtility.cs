using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace UniHumanoid
{
    public static class SkeletonMeshUtility
    {
        class MeshBuilder
        {
            List<Vector3> m_positioins = new List<Vector3>();
            List<int> m_indices = new List<int>();
            List<BoneWeight> m_boneWeights = new List<BoneWeight>();

            public void AddBone(Vector3 head, Vector3 tail, int boneIndex, float width=0.05f)
            {
                var dir = (tail - head).normalized;
                var xaxis = Vector3.Cross(dir, Vector3.forward).normalized;
                AddBox((head+tail)*0.5f, 
                    xaxis*width, 
                    (tail-head)*0.5f, 
                    Vector3.forward*width, 
                    boneIndex);
            }

            void AddBox(Vector3 center, Vector3 xaxis, Vector3 yaxis, Vector3 zaxis, int boneIndex)
            {
                AddQuad(
                    center - yaxis - xaxis - zaxis,
                    center - yaxis + xaxis - zaxis,
                    center - yaxis + xaxis + zaxis,
                    center - yaxis - xaxis + zaxis,
                    boneIndex);
                AddQuad(
                    center + yaxis - xaxis - zaxis,
                    center + yaxis + xaxis - zaxis,
                    center + yaxis + xaxis + zaxis,
                    center + yaxis - xaxis + zaxis,
                    boneIndex, true);
                AddQuad(
                    center - xaxis - yaxis - zaxis,
                    center - xaxis + yaxis - zaxis,
                    center - xaxis + yaxis + zaxis,
                    center - xaxis - yaxis + zaxis,
                    boneIndex, true);
                AddQuad(
                    center + xaxis - yaxis - zaxis,
                    center + xaxis + yaxis - zaxis,
                    center + xaxis + yaxis + zaxis,
                    center + xaxis - yaxis + zaxis,
                    boneIndex);
                AddQuad(
                    center - zaxis - xaxis - yaxis,
                    center - zaxis + xaxis - yaxis,
                    center - zaxis + xaxis + yaxis,
                    center - zaxis - xaxis + yaxis,
                    boneIndex, true);
                AddQuad(
                    center + zaxis - xaxis - yaxis,
                    center + zaxis + xaxis - yaxis,
                    center + zaxis + xaxis + yaxis,
                    center + zaxis - xaxis + yaxis,
                    boneIndex);
            }

            void AddQuad(Vector3 v0, Vector3 v1, Vector3 v2, Vector3 v3, int boneIndex, bool reverse=false)
            {
                var i = m_positioins.Count;
                m_positioins.Add(v0);
                m_positioins.Add(v1);
                m_positioins.Add(v2);
                m_positioins.Add(v3);

                var bw = new BoneWeight
                {
                    boneIndex0=boneIndex,
                    weight0=1.0f,
                };
                m_boneWeights.Add(bw);
                m_boneWeights.Add(bw);
                m_boneWeights.Add(bw);
                m_boneWeights.Add(bw);

                if (reverse)
                {
                    m_indices.Add(i + 3);
                    m_indices.Add(i + 2);
                    m_indices.Add(i + 1);

                    m_indices.Add(i + 1);
                    m_indices.Add(i);
                    m_indices.Add(i + 3);
                }
                else
                {
                    m_indices.Add(i);
                    m_indices.Add(i + 1);
                    m_indices.Add(i + 2);

                    m_indices.Add(i + 2);
                    m_indices.Add(i + 3);
                    m_indices.Add(i);
                }
            }

            public Mesh CreateMesh()
            {
                var mesh = new Mesh();
                mesh.SetVertices(m_positioins);
                mesh.boneWeights = m_boneWeights.ToArray();
                mesh.triangles = m_indices.ToArray();
                mesh.RecalculateNormals();
                mesh.RecalculateBounds();
                return mesh;
            }
        }

        struct BoneHeadTail
        {
            public HumanBodyBones Head;
            public HumanBodyBones Tail;

            public BoneHeadTail(HumanBodyBones head, HumanBodyBones tail)
            {
                Head = head;
                Tail = tail;
            }
        }

        static BoneHeadTail[] Bones = new BoneHeadTail[]
        {
            new BoneHeadTail(HumanBodyBones.Hips, HumanBodyBones.Spine),
            new BoneHeadTail(HumanBodyBones.Spine, HumanBodyBones.Chest),
            new BoneHeadTail(HumanBodyBones.Chest, HumanBodyBones.Neck),
            new BoneHeadTail(HumanBodyBones.Neck, HumanBodyBones.Head),

            // Head

            new BoneHeadTail(HumanBodyBones.LeftShoulder, HumanBodyBones.LeftUpperArm),
            new BoneHeadTail(HumanBodyBones.LeftUpperArm, HumanBodyBones.LeftLowerArm),
            new BoneHeadTail(HumanBodyBones.LeftLowerArm, HumanBodyBones.LeftHand),

            new BoneHeadTail(HumanBodyBones.LeftUpperLeg, HumanBodyBones.LeftLowerLeg),
            new BoneHeadTail(HumanBodyBones.LeftLowerLeg, HumanBodyBones.LeftFoot),

            // Toe

            new BoneHeadTail(HumanBodyBones.RightShoulder, HumanBodyBones.RightUpperArm),
            new BoneHeadTail(HumanBodyBones.RightUpperArm, HumanBodyBones.RightLowerArm),
            new BoneHeadTail(HumanBodyBones.RightLowerArm, HumanBodyBones.RightHand),

            new BoneHeadTail(HumanBodyBones.RightUpperLeg, HumanBodyBones.RightLowerLeg),
            new BoneHeadTail(HumanBodyBones.RightLowerLeg, HumanBodyBones.RightFoot),

            // Toe
        };

        public static SkinnedMeshRenderer CreateRenderer(Animator animator)
        {
            //var bodyBones = (HumanBodyBones[])Enum.GetValues(typeof(HumanBodyBones));
            var bones = animator.transform.Traverse().ToList();

            var builder = new MeshBuilder();
            foreach(var headTail in Bones)
            {
                var head = animator.GetBoneTransform(headTail.Head);
                var tail = animator.GetBoneTransform(headTail.Tail);
                if (head!=null && tail!=null)
                {
                    builder.AddBone(head.position,  tail.position, bones.IndexOf(head));
                }
            }

            var mesh = builder.CreateMesh();
            mesh.bindposes = bones.Select(x =>
                            x.worldToLocalMatrix * animator.transform.localToWorldMatrix).ToArray();
            var renderer = animator.gameObject.AddComponent<SkinnedMeshRenderer>();
            renderer.bones = bones.ToArray();
            renderer.rootBone = animator.GetBoneTransform(HumanBodyBones.Hips);
            renderer.sharedMesh = mesh;
            var bounds = new Bounds(Vector3.zero, mesh.bounds.size);
            renderer.localBounds = bounds;
            return renderer;
        }
    }
}
