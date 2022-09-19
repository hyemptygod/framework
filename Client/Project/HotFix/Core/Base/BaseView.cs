

namespace HotFix.Core
{
    public class BaseView : UIPanel
    {
        public static System.Action<System.Type, float> HideCallback { get; set; }

        public BaseView() : base()
        {

        }

        protected void Hide(float delay = -1)
        {
            if (_closeMode != UIAnimationMode.None && delay >= 0)
                delay += _closeTime;
            HideCallback(GetType(), delay);
        }
    }
}
