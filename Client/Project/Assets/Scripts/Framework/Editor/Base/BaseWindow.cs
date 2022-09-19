using UnityEngine;
using UnityEditor;

namespace FrameworkEditor
{
    public enum WindowType
    {
        Normal,
        DropDown,
        AuxWindow,
        Notification,
        Popup,
        Tab,
        Utility
    }

    public abstract class BaseWindow<T> : EditorWindow where T : BaseWindow<T>
    {
        private static T _instance;
        public static T Instance
        {
            get
            {
                if(_instance == null)
                {
                    _instance = EditorWindow.GetWindow<T>();
                    _instance.Init();
                }
                return _instance;
            }
        }

        protected abstract string TitleName { get; }
        protected abstract WindowType WindowType { get; }

        private void Init()
        {
            _instance.titleContent = new GUIContent(TitleName);
            //_instance.position = Position;
        }

        public void Open(params object[] args)
        {
            var null_args = args == null || args.Length <= 0;
            switch (WindowType)
            {
                case WindowType.Normal:
                    if (null_args)
                        _instance.Show();
                    else
                        _instance.Show((bool)args[0]);
                    break;
                case WindowType.DropDown:
                    var button_rect = null_args ? new Rect(0,0,100,100) : (Rect)args[0];
                    _instance.ShowAsDropDown(button_rect, _instance.position.size);
                    break;
                case WindowType.AuxWindow:
                    _instance.ShowAuxWindow();
                    break;
                case WindowType.Notification:
                    _instance.ShowNotification(new GUIContent(null_args ? "Test" : args[0].ToString()));
                    break;
                case WindowType.Popup:
                    _instance.ShowPopup();
                    break;
                case WindowType.Tab:
                    _instance.ShowTab();
                    break;
                case WindowType.Utility:
                    _instance.ShowUtility();
                    break;

            }
        }

        protected virtual void OnGUI()
        {

        }
        
    }
}
