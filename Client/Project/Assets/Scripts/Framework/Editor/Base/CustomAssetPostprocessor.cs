using UnityEngine;
using UnityEditor;
using FrameworkEditor.UI;

public class CustomAssetPostprocessor : AssetPostprocessor
{
    private void OnPreprocessTexture()
    {
        UIEditorUtil.OnPreprocessTexture(assetPath, assetImporter as TextureImporter);
    }
}