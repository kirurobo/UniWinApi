﻿using System;
using System.Collections.Generic;
using UnityEngine;


namespace UniGLTF.MeshUtility
{
    public static class BoneMeshEraser
    {
        private struct ExcludeBoneIndex
        {
            public readonly bool Bone0;
            public readonly bool Bone1;
            public readonly bool Bone2;
            public readonly bool Bone3;

            public ExcludeBoneIndex(bool bone0, bool bone1, bool bone2, bool bone3)
            {
                Bone0 = bone0;
                Bone1 = bone1;
                Bone2 = bone2;
                Bone3 = bone3;
            }
        }

        [Serializable]
        public struct EraseBone
        {
            public Transform Bone;
            public bool Erase;

            public override string ToString()
            {
                return Bone.name + ":" + Erase;
            }
        }

        static int ExcludeTriangles(int[] triangles, BoneWeight[] bws, int[] exclude)
        {
            int count = 0;
            if (bws != null && bws.Length > 0)
            {
                for (int i = 0; i < triangles.Length; i += 3)
                {
                    var a = triangles[i];
                    var b = triangles[i + 1];
                    var c = triangles[i + 2];

                    {
                        var bw = bws[a];
                        var eb = AreBoneContains(ref exclude, bw.boneIndex0, bw.boneIndex1, bw.boneIndex2, bw.boneIndex3);
                        if (bw.weight0 > 0 && eb.Bone0) continue;
                        if (bw.weight1 > 0 && eb.Bone1) continue;
                        if (bw.weight2 > 0 && eb.Bone2) continue;
                        if (bw.weight3 > 0 && eb.Bone3) continue;
                    }
                    {
                        var bw = bws[b];
                        var eb = AreBoneContains(ref exclude, bw.boneIndex0, bw.boneIndex1, bw.boneIndex2, bw.boneIndex3);
                        if (bw.weight0 > 0 && eb.Bone0) continue;
                        if (bw.weight1 > 0 && eb.Bone1) continue;
                        if (bw.weight2 > 0 && eb.Bone2) continue;
                        if (bw.weight3 > 0 && eb.Bone3) continue;
                    }
                    {
                        var bw = bws[c];
                        var eb = AreBoneContains(ref exclude, bw.boneIndex0, bw.boneIndex1, bw.boneIndex2, bw.boneIndex3);
                        if (bw.weight0 > 0 && eb.Bone0) continue;
                        if (bw.weight1 > 0 && eb.Bone1) continue;
                        if (bw.weight2 > 0 && eb.Bone2) continue;
                        if (bw.weight3 > 0 && eb.Bone3) continue;
                    }

                    triangles[count++] = a;
                    triangles[count++] = b;
                    triangles[count++] = c;
                }
            }

            return count;
        }

        private static ExcludeBoneIndex AreBoneContains(ref int[] exclude, int boneIndex0, int boneIndex1,
            int boneIndex2, int boneIndex3)
        {
            var b0 = false;
            var b1 = false;
            var b2 = false;
            var b3 = false;
            for (int i = 0; i < exclude.Length; i++)
            {
                if (exclude[i] == boneIndex0)
                {
                    b0 = true;
                    continue;
                }

                if (exclude[i] == boneIndex1)
                {
                    b1 = true;
                    continue;
                }

                if (exclude[i] == boneIndex2)
                {
                    b2 = true;
                    continue;
                }

                if (exclude[i] == boneIndex3)
                {
                    b3 = true;
                }
            }

            return new ExcludeBoneIndex(b0, b1, b2, b3);
        }

        public static Mesh CreateErasedMesh(Mesh src, int[] eraseBoneIndices)
        {
            /*
            Debug.LogFormat("{0} exclude: {1}", 
                src.name,
                String.Join(", ", eraseBoneIndices.Select(x => x.ToString()).ToArray())
                );
            */
            var mesh = new Mesh();
            mesh.name = src.name + "(erased)";

#if UNITY_2017_3_OR_NEWER
            mesh.indexFormat = src.indexFormat;
#endif

            mesh.vertices = src.vertices;
            mesh.normals = src.normals;
            mesh.uv = src.uv;
            mesh.tangents = src.tangents;
            mesh.boneWeights = src.boneWeights;
            mesh.bindposes = src.bindposes;
            mesh.subMeshCount = src.subMeshCount;
            for (int i = 0; i < src.subMeshCount; ++i)
            {
                var indices = src.GetIndices(i);
                var count = ExcludeTriangles(indices, mesh.boneWeights, eraseBoneIndices);
                var dst = new int[count];
                Array.Copy(indices, 0, dst, 0, count);
                mesh.SetIndices(dst, MeshTopology.Triangles, i);
            }

            return mesh;
        }

        public static IEnumerable<Transform> Ancestor(this Transform t)
        {
            yield return t;

            if (t.parent != null)
            {
                foreach (var x in Ancestor(t.parent))
                {
                    yield return x;
                }
            }
        }
    }
}
