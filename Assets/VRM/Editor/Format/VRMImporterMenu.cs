﻿using System.IO;
using UnityEditor;
using UnityEngine;
using UniGLTF;
using System;
using System.Collections.Generic;
using System.Linq;
using VRMShaders;
using Object = UnityEngine.Object;

namespace VRM
{
    public static class VRMImporterMenu
    {
        [MenuItem(VRMVersion.MENU + "/Import", priority = 1)]
        static void ImportMenu()
        {
            var path = EditorUtility.OpenFilePanel("open vrm", "", "vrm");
            if (string.IsNullOrEmpty(path))
            {
                return;
            }

            if (Application.isPlaying)
            {
                ImportRuntime(path);
                return;
            }

            if (path.StartsWithUnityAssetPath())
            {
                Debug.LogWarningFormat("disallow import from folder under the Assets");
                return;
            }

            var prefabPath = EditorUtility.SaveFilePanel("save prefab", "Assets", Path.GetFileNameWithoutExtension(path), "prefab");
            if (string.IsNullOrEmpty(path))
            {
                return;
            }

            ImportAsset(path, UnityPath.FromFullpath(prefabPath));
        }

        static void ImportRuntime(string path)
        {
            // load into scene
            var data = new GlbFileParser(path).Parse();

            using (var context = new VRMImporterContext(data))
            {
                var loaded = context.Load();
                loaded.EnableUpdateWhenOffscreen();
                loaded.ShowMeshes();
                Selection.activeGameObject = loaded.gameObject;
            }
        }

        static void ImportAsset(string path, UnityPath prefabPath)
        {
            if (!prefabPath.IsUnderAssetsFolder)
            {
                Debug.LogWarningFormat("out of asset path: {0}", prefabPath);
                return;
            }

            // import as asset
            // var prefabPath = UnityPath.FromUnityPath(prefabPath);
            var data = new GlbFileParser(path).Parse();

            Action<IEnumerable<UnityPath>> onCompleted = texturePaths =>
            {
                //
                // after textures imported
                //
                var map = texturePaths
                    .Select(x => x.LoadAsset<Texture2D>())
                    .Where(x => x != null)
                    .ToDictionary(x => new SubAssetKey(x), x => x as Object);

                using (var context = new VRMImporterContext(data, map))
                {
                    var editor = new VRMEditorImporterContext(context, prefabPath);
                    foreach (var textureInfo in editor.TextureDescriptorGenerator.Get().GetEnumerable())
                    {
                        VRMShaders.TextureImporterConfigurator.Configure(textureInfo, context.TextureFactory.ExternalTextures);
                    }
                    var loaded = context.Load();
                    editor.SaveAsAsset(loaded);
                }
            };

            using (var context = new VRMImporterContext(data))
            {
                var editor = new VRMEditorImporterContext(context, prefabPath);
                editor.ConvertAndExtractImages(onCompleted);
            }
        }
    }
}
