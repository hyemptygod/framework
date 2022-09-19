using HotFix.Core;

namespace HotFix.Game
{
    public class GameFacade : BaseFacade<GameFacade>
    {
        public override void Start()
        {
            base.Start();

            BaseView.HideCallback = HidePanel;
        }

        public static void ShowPanel<TPanel>(object body = null) where TPanel : BaseView, new()
        {
            Instance.SendNotification(typeof(TPanel).FullName + "_SHOW", body);
        }

        public static void HidePanel<TPanel>(float delay = -1) where TPanel : BaseView, new()
        {
            HidePanel(typeof(TPanel), delay);
        }

        public static void HidePanel(System.Type t, float delay = -1)
        {
            Instance.SendNotification(t.FullName + "_HIDE", delay);
        }
    }
}
