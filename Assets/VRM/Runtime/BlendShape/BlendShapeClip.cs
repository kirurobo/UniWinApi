﻿using System;
using UnityEngine;


namespace VRM
{
    [Serializable]
    public struct BlendShapeBinding : IEquatable<BlendShapeBinding>
    {
        public String RelativePath;
        public int Index;
        public float Weight;

        public override string ToString()
        {
            return string.Format("{0}[{1}]=>{2}", RelativePath, Index, Weight);
        }

        public bool Equals(BlendShapeBinding other)
        {
            return string.Equals(RelativePath, other.RelativePath) && Index == other.Index && Weight.Equals(other.Weight);
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            return obj is BlendShapeBinding && Equals((BlendShapeBinding)obj);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                var hashCode = (RelativePath != null ? RelativePath.GetHashCode() : 0);
                hashCode = (hashCode * 397) ^ Index;
                hashCode = (hashCode * 397) ^ Weight.GetHashCode();
                return hashCode;
            }
        }
    }

    [Serializable]
    public struct MaterialValueBinding : IEquatable<MaterialValueBinding>
    {
        public String MaterialName;
        public String ValueName;
        public Vector4 TargetValue;
        public Vector4 BaseValue;

        public bool Equals(MaterialValueBinding other)
        {
            return string.Equals(MaterialName, other.MaterialName) && string.Equals(ValueName, other.ValueName) && TargetValue.Equals(other.TargetValue) && BaseValue.Equals(other.BaseValue);
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            return obj is MaterialValueBinding && Equals((MaterialValueBinding)obj);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                var hashCode = (MaterialName != null ? MaterialName.GetHashCode() : 0);
                hashCode = (hashCode * 397) ^ (ValueName != null ? ValueName.GetHashCode() : 0);
                hashCode = (hashCode * 397) ^ TargetValue.GetHashCode();
                hashCode = (hashCode * 397) ^ BaseValue.GetHashCode();
                return hashCode;
            }
        }
    }

    [CreateAssetMenu(menuName = "VRM/BlendShapeClip")]
    public class BlendShapeClip : ScriptableObject
    {
#if UNITY_EDITOR
        /// <summary>
        /// Preview 用のObject参照
        /// </summary>
        [SerializeField]
        GameObject m_prefab;
        public GameObject Prefab
        {
            set { m_prefab = value; }
            get
            {
                if (m_prefab == null)
                {
                    var assetPath = UnityEditor.AssetDatabase.GetAssetPath(this);
                    if (!string.IsNullOrEmpty(assetPath))
                    {
                        // if asset is subasset of prefab
                        m_prefab = UnityEditor.AssetDatabase.LoadAssetAtPath<GameObject>(assetPath);
                        if (m_prefab == null)
                        {
                            var parent = UniGLTF.UnityPath.FromAsset(this).Parent;
                            var prefabPath = parent.Parent.Child(parent.FileNameWithoutExtension + ".prefab");
                            m_prefab = UnityEditor.AssetDatabase.LoadAssetAtPath<GameObject>(prefabPath.Value);
                        }
                    }
                }
                return m_prefab;
            }
        }
#endif

        /// <summary>
        /// BlendShapePresetがUnknown場合の識別子
        /// </summary>
        [SerializeField]
        public string BlendShapeName = "";

        /// <summary>
        /// BlendShapePresetを識別する。Unknownの場合は、BlendShapeNameで識別する
        /// </summary>
        [SerializeField]
        public BlendShapePreset Preset;

        /// <summary>
        /// BlendShapeClipに対応するBlendShapeKey
        /// </summary>
        /// <returns></returns>
        public BlendShapeKey Key => BlendShapeKey.CreateFromClip(this);

        /// <summary>
        /// BlendShapeに対する参照(index ベース)
        /// </summary>
        /// <value></value>
        [SerializeField]
        public BlendShapeBinding[] Values = new BlendShapeBinding[] { };

        /// <summary>
        /// マテリアルに対する参照(名前ベース)
        /// </summary>
        /// <value></value>
        [SerializeField]
        public MaterialValueBinding[] MaterialValues = new MaterialValueBinding[] { };

        /// <summary>
        /// UniVRM-0.45: trueの場合、このBlendShapeClipは0と1の間の中間値を取らない。四捨五入する
        /// </summary>
        [SerializeField]
        public bool IsBinary;
    }
}
