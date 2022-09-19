using System.IO;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;
using Framework;
using FrameworkEditor.ILRuntimeHelper;
using FrameworkEditor.UI;
using FrameworkEditor.AssetBundle;


namespace FrameworkEditor
{
    public class MenuItemExtend
    {
        private const string BASE = "Framework/";
        private const string TOOLS = "Tools/";
        private const string ASSET = "Assets/";
        private const string GAMEOBJECT = "GameObject/";

        private const string DATA = "Data/";
        private const string ILRT = "ILRT/";
        private const string UI = "UI/";
        private const string ASSETBUNDLE = "AssetBundle/";

        public const string SIMULATION_MODE = BASE + ASSETBUNDLE + "Simulation Mode";

        #region Framework Menu Item

        [MenuItem(BASE + DATA + "UserData")]
        private static void ShowUserDataWindow()
        {
            PlayerPrefsWindow.Instance.Open();
        }

        [MenuItem(BASE + DATA + "Update TableItem")]
        private static void OpenCreateNewAppWindow()
        {
            UpdateTableItemsWindow.Instance.Open();
        }

        [MenuItem(BASE + DATA + "Generate GameSetting")]
        private static void GenerateGameSetting()
        {
            var appdomain = ILRuntimeEditorUtil.GetDomain();
            var o = appdomain.Instantiate("HotFix.Core.GameSetting");
            FileUtil.CreateFile(PathConst.LocalSettingPath, PathConst.GAME_SETTING + ".json", o.ToJson());
        }

        [MenuItem(BASE + ILRT + "Generate CLR Binding Code by Analysis")]
        public static void GenerateCLRBindingByAnalysis()
        {
            ILRuntimeEditorUtil.GenerateCLRBindingByAnalysis();
        }

        [MenuItem(BASE + ILRT + "Remove CLR Binding Code")]
        public static void RemoveCLRBinding()
        {
            ILRuntimeEditorUtil.RemoveCLRBinding();
        }

        [MenuItem(BASE + ILRT + "Generate Cross Binding Adapter")]
        public static void GenerateCrossbindAdapter()
        {
            ILRuntimeEditorUtil.GenerateCrossbindAdapter();
        }

        [MenuItem(BASE + UI + "Create Atlas")]
        public static void CreateAtals()
        {
            var map = UIEditorUtil.SelectAtlas();

            var parent = new DirectoryInfo(PathConst.LocalAssets + "Image/");

            var children = parent.GetDirectories();

            foreach (var child in children)
            {
                UIEditorUtil.CreateSpriteAltas(map, child);
            }
        }

        [MenuItem(SIMULATION_MODE)]
        public static void ToggleSimulationMode()
        {
            EditorPrefs.SetBool(PathConst.kSimulateAssetBundles, !EditorPrefs.GetBool(PathConst.kSimulateAssetBundles, true));
        }

        [MenuItem(SIMULATION_MODE, true)]
        public static bool ToggleSimulationModeValidate()
        {
            Menu.SetChecked(SIMULATION_MODE, EditorPrefs.GetBool(PathConst.kSimulateAssetBundles, true));
            return true;
        }

        [MenuItem(BASE + ASSETBUNDLE + "Set Bundles")]
        public static void AssetsToBundles()
        {
            AssetBundleHelperEditor.AssetToBundle(Selection.activeObject);
        }

        #endregion

        #region Tools Menu Item

        [MenuItem(TOOLS + "GUIStyle")]
        public static void OpenGuiStyle()
        {
            GUIStyleViewer.Instance.Open();
        }

        [MenuItem(TOOLS + "切换横竖屏")]
        public static void ChangeScreenDirection()
        {
            var width = Screen.width;
            var height = Screen.height;
            var original = width > height ? "横屏" : "竖屏";

            GameViewUtil.SwitchOrientation();

            EditorApplication.delayCall += () =>
            {
                width = Screen.width;
                height = Screen.height;
                var target = width > height ? "横屏" : "竖屏";
                Log.Info(string.Format("由<color=green>{0}</color>切换为<color=green>{1}</color>", original, target));
            };
        }

        #endregion

        #region Asset Menu Item

        [MenuItem(ASSET + ASSETBUNDLE + "Set Bundles")]
        public static void AssetsToBundles1()
        {
            AssetBundleHelperEditor.AssetToBundle(Selection.activeObject);
        }

        #endregion

        #region GameObject Menu Item

        [MenuItem(GAMEOBJECT + UI + "Text With Background")]
        static public void AddLoopHorizontalScrollRect(MenuCommand menuCommand)
        {
            var parent = Selection.activeGameObject.transform;

            var background = new GameObject("Background").AddComponent<Image>();
            background.rectTransform.SetParent(parent);
            background.rectTransform.localPosition = Vector3.zero;
            background.rectTransform.localRotation = Quaternion.identity;
            background.rectTransform.localScale = Vector3.one;
            background.rectTransform.sizeDelta = new Vector2(100, 100);

            background.sprite = AssetDatabase.GetBuiltinExtraResource<Sprite>("UI/Skin/Background.psd");
            background.type = Image.Type.Sliced;

            var com = background.gameObject.AddComponent<BackgroundText>();
            com.padding = new RectOffset(5, 5, 5, 5);

            var content = new GameObject("Content").AddComponent<UIText>();
            content.RectTransform.SetParent(background.rectTransform);
            content.RectTransform.localPosition = Vector3.zero;
            content.RectTransform.localRotation = Quaternion.identity;
            content.RectTransform.localScale = Vector3.one;
            content.Component.color = Color.black;

            com.Initialize();

            EditorApplication.delayCall += () => Selection.activeGameObject = background.gameObject;
        }

        #endregion
    }
}
