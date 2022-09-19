using UnityEngine;
using UnityEngine.U2D;
using UnityEditor;
using UnityEditor.U2D;
using Framework;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace FrameworkEditor.UI
{
    public class UIEditorUtil
    {
        private static string _imgPath = "Assets/GameAssets/Image/";

        public static void OnPreprocessTexture(string assetPath, TextureImporter textureImporter)
        {
            if (textureImporter == null)
                return;

            if (assetPath.StartsWith(_imgPath))
            {
                textureImporter.textureType = TextureImporterType.Sprite;
                textureImporter.mipmapEnabled = false;
                textureImporter.spriteImportMode = SpriteImportMode.Single;
                textureImporter.spritePackingTag = assetPath.Substring(0, assetPath.LastIndexOf("/")).Replace(_imgPath, "");
            }

            var setting = textureImporter.GetPlatformTextureSettings("iPhone");
            //android使用etc2 ,window使用dxt，都使用默认的不需要另外设置
            if (!setting.format.ToString().StartsWith("ASTC_"))
            {
                var targetFormat = TextureImporterFormat.ASTC_5x5;
                if (textureImporter.DoesSourceTextureHaveAlpha() || textureImporter.alphaSource == TextureImporterAlphaSource.FromGrayScale)
                    targetFormat = TextureImporterFormat.ASTC_5x5;
                if (targetFormat != setting.format)
                {
                    setting.overridden = true;
                    setting.format = targetFormat;
                    textureImporter.SetPlatformTextureSettings(setting);
                }
            }
        }

        /// <summary>
        /// 获取所有的atlas
        /// </summary>
        /// <returns></returns>
        public static Dictionary<string, SpriteAtlas> SelectAtlas()
        {
            var atlaspath = PathConst.LocalAssets + "Atlas/";
            FileUtil.CreateDir(atlaspath);
            var files = EditorUtil.GetFiles(atlaspath);
            var atlasMap = new Dictionary<string, SpriteAtlas>(files.Count);
            foreach (var file in files)
            {
                var asset = AssetDatabase.LoadAssetAtPath<SpriteAtlas>(file);
                atlasMap.Add(asset.name, asset);
            }
            return atlasMap;
        }

        /// <summary>
        /// 创建新的atlas
        /// </summary>
        /// <param name="map"></param>
        /// <param name="info"></param>
        public static void CreateSpriteAltas(Dictionary<string, SpriteAtlas> map, DirectoryInfo info)
        {
            var name = info.Name;

            var create = false;
            if (!map.TryGetValue(name, out SpriteAtlas atlas) || atlas == null)
            {
                atlas = new SpriteAtlas();
                map.Add(name, atlas);
                create = true;
            }

            var files = EditorUtil.GetFiles(info.FullName);
            var sprites = new List<Object>();
            var packables = atlas.GetPackables();
            for (int i = 0; i < files.Count; i++)
            {
                var sprite = AssetDatabase.LoadAssetAtPath<Sprite>(files[i]);
                if (!packables.Contains(sprite))
                    sprites.Add(sprite);
            }

            atlas.Add(sprites.ToArray());

            if (create)
                AssetDatabase.CreateAsset(atlas, "Assets/GameAssets/Atlas/" + name + ".spriteatlas");

            EditorUtility.SetDirty(atlas);
            AssetDatabase.Refresh();
        }
    }
}
