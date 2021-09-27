﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using NUnit.Framework;
using UnityEngine;
using VRMShaders;

namespace UniGLTF
{
    public class GltfLoadTests
    {
        static IEnumerable<FileInfo> EnumerateGltfFiles(DirectoryInfo dir)
        {
            if (dir.Name == ".git")
            {
                yield break;
            }

            foreach (var child in dir.EnumerateDirectories())
            {
                foreach (var x in EnumerateGltfFiles(child))
                {
                    yield return x;
                }
            }

            foreach (var child in dir.EnumerateFiles())
            {
                switch (child.Extension.ToLower())
                {
                    case ".gltf":
                    case ".glb":
                        yield return child;
                        break;
                }
            }
        }

        static void Message(string path, Exception exception)
        {
            while (exception.InnerException != null)
            {
                exception = exception.InnerException;
            }

            if (exception is UniGLTFNotSupportedException ex)
            {
                Debug.LogWarning($"LoadError: {path}: {ex}");
            }
            else
            {
                Debug.LogError($"LoadError: {path}");
                Debug.LogException(exception);
            }
        }

        static Byte[] Export(GameObject root)
        {
            var gltf = new glTF();
            using (var exporter = new gltfExporter(gltf, new GltfExportSettings()))
            {
                exporter.Prepare(root);
                exporter.Export(new GltfExportSettings(), new EditorTextureSerializer());
                return gltf.ToGlbBytes();
            }
        }

        // Unsolved Animation Export issue
        //
        // QuaternionToEuler: Input quaternion was not normalized
        //
        static string[] Skip = new string[]
        {
            "BrainStem",
            "RiggedSimple"
        };

        static void RuntimeLoadExport(FileInfo gltf, int subStrStart)
        {
            GltfData data = null;
            try
            {
                data = new AmbiguousGltfFileParser(gltf.FullName).Parse();
            }
            catch (Exception ex)
            {
                Debug.LogError($"ParseError: {gltf}");
                Debug.LogException(ex);
            }

            using (var loader = new ImporterContext(data))
            {
                try
                {
                    var loaded = loader.Load();
                    if (loaded == null)
                    {
                        Debug.LogWarning($"root is null: ${gltf}");
                        return;
                    }

                    if (Skip.Contains(gltf.Directory.Parent.Name))
                    {
                        // Export issue:
                        // skip
                        return;
                    }

                    Export(loaded.gameObject);
                }
                catch (Exception ex)
                {
                    Message(gltf.FullName.Substring(subStrStart), ex);
                }

            }
        }

        /// <summary>
        /// Extract をテスト
        /// </summary>
        /// <param name="gltf"></param>
        /// <param name="root"></param>
        static void EditorLoad(FileInfo gltf, int subStrStart)
        {
            GltfData data = null;
            try
            {
                data = new AmbiguousGltfFileParser(gltf.FullName).Parse();
            }
            catch (Exception ex)
            {
                Debug.LogError($"ParseError: {gltf}");
                Debug.LogException(ex);
            }

            // should unique
            var gltfTextures = new GltfTextureDescriptorGenerator(data).Get().GetEnumerable()
                .Select(x => x.SubAssetKey)
                .ToArray();
            var distinct = gltfTextures.Distinct().ToArray();
            Assert.True(gltfTextures.Length == distinct.Length);
            Assert.True(gltfTextures.SequenceEqual(distinct));
        }

        [Test]
        public void GltfSampleModelsTests()
        {
            var env = System.Environment.GetEnvironmentVariable("GLTF_SAMPLE_MODELS");
            if (string.IsNullOrEmpty(env))
            {
                return;
            }
            var root = new DirectoryInfo($"{env}/2.0");
            if (!root.Exists)
            {
                return;
            }

            foreach (var gltf in EnumerateGltfFiles(root))
            {
                RuntimeLoadExport(gltf, root.FullName.Length);

                EditorLoad(gltf, root.FullName.Length);
            }
        }

        // [Test]
        public void GltfSampleModelsTest_BrainStem()
        {
            var env = System.Environment.GetEnvironmentVariable("GLTF_SAMPLE_MODELS");
            if (string.IsNullOrEmpty(env))
            {
                return;
            }
            var root = new DirectoryInfo($"{env}/2.0");
            if (!root.Exists)
            {
                return;
            }

            // foreach (var gltf in EnumerateGltfFiles(root))
            {
                var gltf = new FileInfo(Path.Combine(root.FullName, "BrainStem/glTF-Binary/BrainStem.glb"));
                RuntimeLoadExport(gltf, root.FullName.Length);

                EditorLoad(gltf, root.FullName.Length);
            }
        }

        [Test]
        public void GltfSampleModelsTest_DamagedHelmet()
        {
            var env = System.Environment.GetEnvironmentVariable("GLTF_SAMPLE_MODELS");
            if (string.IsNullOrEmpty(env))
            {
                return;
            }
            var root = new DirectoryInfo($"{env}/2.0");
            if (!root.Exists)
            {
                return;
            }

            {
                var path = Path.Combine(root.FullName, "DamagedHelmet/glTF-Binary/DamagedHelmet.glb");
                var data = new AmbiguousGltfFileParser(path).Parse();

                var matDesc = new GltfMaterialDescriptorGenerator().Get(data, 0);
                Assert.AreEqual("Standard", matDesc.ShaderName);
                Assert.AreEqual(5, matDesc.TextureSlots.Count);
                var (key, value) = matDesc.EnumerateSubAssetKeyValue().First();
                Assert.AreEqual(new SubAssetKey(typeof(Texture2D), "texture_0"), key);
            }
        }
    }
}
