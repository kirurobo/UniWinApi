﻿using System;
using System.Collections.Generic;
using System.Linq;
using NUnit.Framework;
using UniGLTF;


namespace VRM
{

    public class VRMTextureEnumerateTests
    {
        /// <summary>
        /// Test uniqueness
        /// </summary>
        [Test]
        public void TextureEnumerationTest()
        {
            {
                var data = GltfData.CreateFromGltfDataForTest(
                    new glTF
                    {
                        images = new List<glTFImage>
                        {
                            new glTFImage
                            {
                                mimeType = "image/png",
                            }
                        },
                        textures = new List<glTFTexture>
                        {
                            new glTFTexture
                            {
                                name = "texture0",
                                source = 0,
                            }
                        },
                        materials = new List<glTFMaterial>
                        {
                            new glTFMaterial
                            {
                                pbrMetallicRoughness = new glTFPbrMetallicRoughness
                                {
                                    baseColorTexture = new glTFMaterialBaseColorTextureInfo
                                    {
                                        index = 0,
                                    }
                                }
                            },
                            new glTFMaterial
                            {
                                pbrMetallicRoughness = new glTFPbrMetallicRoughness
                                {
                                    baseColorTexture = new glTFMaterialBaseColorTextureInfo
                                    {
                                        index = 0,
                                    }
                                }
                            },
                        }
                    }
                );
                var vrm = new glTF_VRM_extensions
                {
                    materialProperties = new List<glTF_VRM_Material>
                    {
                        new glTF_VRM_Material
                        {
                            textureProperties = new Dictionary<string, int>
                            {
                                {"_MainTex", 0},
                            }
                        },
                        new glTF_VRM_Material
                        {
                            textureProperties = new Dictionary<string, int>
                            {
                                {"_MainTex", 0},
                            }
                        },
                     }
                };
                var items = new VrmTextureDescriptorGenerator(data, vrm).Get().GetEnumerable().ToArray();
                Assert.AreEqual(1, items.Length);
            }
        }

        [Test]
        public void TextureEnumerationInUnknownShader()
        {
            var data = GltfData.CreateFromGltfDataForTest(
                new glTF
                {
                    images = new List<glTFImage>
                    {
                        new glTFImage
                        {
                            mimeType = "image/png",
                        }
                    },
                    textures = new List<glTFTexture>
                    {
                        new glTFTexture
                        {
                            name = "texture0",
                            source = 0,
                        }
                    },
                    materials = new List<glTFMaterial>
                    {
                        new glTFMaterial
                        {
                            pbrMetallicRoughness = new glTFPbrMetallicRoughness
                            {
                                baseColorTexture = new glTFMaterialBaseColorTextureInfo
                                {
                                    index = 0,
                                }
                            }
                        },
                    }
                }
            );
            var vrm = new glTF_VRM_extensions
            {
                materialProperties = new List<glTF_VRM_Material>
                    {
                        new glTF_VRM_Material
                        {
                            shader = "UnknownShader",
                            textureProperties = new Dictionary<string, int>
                            {
                                {"_MainTex", 0},
                            }
                        },
                     }
            };

            // 2系統ある？
            Assert.IsTrue(VRMMToonMaterialImporter.TryCreateParam(data, vrm, 0, out VRMShaders.MaterialDescriptor matDesc));
            Assert.AreEqual(1, matDesc.TextureSlots.Count);

            var items = new VrmTextureDescriptorGenerator(data, vrm).Get().GetEnumerable().ToArray();
            Assert.AreEqual(1, items.Length);
        }
    }
}
