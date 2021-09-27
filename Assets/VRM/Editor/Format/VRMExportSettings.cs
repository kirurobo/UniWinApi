﻿using System;
using UniGLTF;
using UnityEngine;

namespace VRM
{
    [Serializable]
    public class VRMExportSettings : ScriptableObject
    {

        /// <summary>
        /// エクスポート時に強制的にT-Pose化する
        /// </summary>
        [Tooltip("Option")]
        public bool ForceTPose = false;

        /// <summary>
        /// エクスポート時にヒエラルキーの正規化を実施する
        /// </summary>
        [Tooltip("Require only first time")]
        public bool PoseFreeze = true;

        /// <summary>
        /// BlendShapeのシリアライズにSparseAccessorを使う
        /// </summary>
        [Tooltip("Use sparse accessor for blendshape. This may reduce vrm size")]
        public bool UseSparseAccessor = false;

        /// <summary>
        /// BlendShapeのPositionのみをエクスポートする
        /// </summary>
        [Tooltip("UniVRM-0.54 or later can load it. Otherwise fail to load")]
        public bool OnlyBlendshapePosition = false;

        /// <summary>
        /// エクスポート時にBlendShapeClipから参照されないBlendShapeを削除する
        /// </summary>
        [Tooltip("Remove blendshape that is not used from BlendShapeClip")]
        public bool ReduceBlendshape = false;

        /// <summary>
        /// skip if BlendShapeClip.Preset == Unknown
        /// </summary>
        [Tooltip("Remove blendShapeClip that preset is Unknown")]
        public bool ReduceBlendshapeClip = false;

        /// <summary>
        /// Export時に頂点バッファをsubmeshで分割する。GLTF互換性
        /// </summary>
        [Tooltip("Divide vertex buffer. For more gltf compatibility")]
        public bool DivideVertexBuffer = false;

        public GltfExportSettings MeshExportSettings => new GltfExportSettings
        {
            UseSparseAccessorForMorphTarget = UseSparseAccessor,
            ExportOnlyBlendShapePosition = OnlyBlendshapePosition,
            DivideVertexBuffer = DivideVertexBuffer,
        };

        public GameObject Root { get; set; }
    }
}
