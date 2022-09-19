using UnityEngine;
using DG.Tweening;

namespace HotFix
{
    /// <summary>
    /// UI面板
    /// </summary>
    public class UIPanel : UIComponent
    {
        /// <summary>
        /// Panel 动画
        /// </summary>
        public enum UIAnimationMode
        {
            /// <summary>
            /// 无
            /// </summary>
            None,
            /// <summary>
            /// 缩放
            /// </summary>
            Scale,
            /// <summary>
            /// 左移
            /// </summary>
            LeftMove,
            /// <summary>
            /// 右移
            /// </summary>
            RightMove,
        }

        /// <summary>
        /// open type
        /// </summary>
        protected UIAnimationMode _openMode = UIAnimationMode.None;
        /// <summary>
        /// open duration
        /// </summary>
        protected float _openTime = 0f;
        /// <summary>
        /// open ease
        /// </summary>
        protected Ease _openEase = Ease.Linear;

        /// <summary>
        /// close type
        /// </summary>
        protected UIAnimationMode _closeMode = UIAnimationMode.None;
        /// <summary>
        /// close duration
        /// </summary>
        protected float _closeTime = 0f;
        /// <summary>
        /// close ease
        /// </summary>
        protected Ease _closeEase = Ease.Linear;

        public UIPanel() { }

        /// <summary>
        /// 构造方法
        /// </summary>
        /// <param name="go"></param>
        public UIPanel(GameObject go) : base(go)
        {

        }

        /// <summary>
        /// 
        /// </summary>
        protected override void OnEnable()
        {
            base.OnEnable();

            PlayOpenAniamtion();
        }

        /// <summary>
        /// 
        /// </summary>
        protected override void OnDisable()
        {
            PlayCloseAniamtion(base.OnDisable);
        }

        /// <summary>
        /// 播放现实动画
        /// </summary>
        private void PlayOpenAniamtion()
        {
            if (_openMode == UIAnimationMode.None)
                return;

            rectTransform.DOKill();

            switch (_openMode)
            {
                case UIAnimationMode.Scale:
                    rectTransform.localScale = Vector3.zero;
                    rectTransform.DOScale(Vector3.one, _openTime).SetEase(_openEase);
                    break;
                case UIAnimationMode.LeftMove:
                    break;
                case UIAnimationMode.RightMove:
                    break;
            }
        }

        /// <summary>
        /// 播放现实动画
        /// </summary>
        private void PlayCloseAniamtion(TweenCallback callback)
        {
            if (_closeMode == UIAnimationMode.None)
            {
                callback();
                return;
            }

            rectTransform.DOKill();

            switch (_closeMode)
            {
                case UIAnimationMode.Scale:
                    rectTransform.localScale = Vector3.one;
                    rectTransform.DOScale(Vector3.zero, _closeTime).SetEase(_closeEase).OnComplete(callback);
                    break;
                case UIAnimationMode.LeftMove:
                    callback();
                    break;
                case UIAnimationMode.RightMove:
                    callback();
                    break;
            }
        }
    }
}
