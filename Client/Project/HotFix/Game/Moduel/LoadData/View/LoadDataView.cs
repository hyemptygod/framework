using HotFix.Core;
using UnityEngine;
using UnityEngine.UI;
using HotFix.Base;

namespace HotFix.Game
{
    [UIPanel("start/loaddataview", "LoadDataView", (int)DisplayLevel.UI)]
    public class LoadDataView : BaseView
    {
        [UIField("Text")]
        private Text _titleTxt;

        [UIField("Progress/Value")]
        private Image _progressImg;

        private bool _hasCompleted;

        protected override void Awake()
        {
            base.Awake();

            var uiimg = gameObject.QuickGetComponent<UIImage>();
            uiimg[TriggerType.Click] += data =>
            {
                if (!_hasCompleted)
                    return;
                Hide(0);
                GameFacade.ShowPanel<MainView>();
            };
        }

        public void RefreshTitle(ProgressInfo p)
        {
            _progressImg.fillAmount = p.Progress;
            _titleTxt.text = p.title;
        }

        public void Complete()
        {
            _hasCompleted = true;
            _titleTxt.text = "完成";
            _progressImg.transform.parent.gameObject.SetActive(false);
        }
    }
}
