using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Framework.Start
{
    /// <summary>
    /// 加载资源的UI面板
    /// </summary>
    public class DownloadNewAssetView : MonoBehaviour
    {
        [SerializeField]
        private Image _progressImg;
        [SerializeField]
        private Text _sizeTxt;
        [SerializeField]
        private Text _totalSizeTxt;

        /// <summary>
        /// 加载完成后的回调
        /// </summary>
        public Action Callback { get; set; }

        /// <summary>
        /// 检查资源
        /// </summary>
        /// <returns></returns>
        public IEnumerator<float> CheckAsset()
        {
            var _assetVersion = new AssetVersionHelper();

            yield return CoroutineManager.WaitUntilDone(_assetVersion.Check());

            Log.Info("Update :" + _assetVersion.NeedUpdate);

            if (_assetVersion.NeedUpdate)
            {
                Log.Info("size:" + SizeUtil.GetSize(_assetVersion.UpdateSize));

                _assetVersion.UnZip();

                _totalSizeTxt.text = SizeUtil.GetSize(_assetVersion.UpdateSize);

                while (!_assetVersion.Success)
                {
                    _progressImg.fillAmount = _assetVersion.Current.progress;
                    _sizeTxt.text = SizeUtil.GetSize(_assetVersion.Current.size);
                    yield return CoroutineManager.WaitForOneFrame;
                }
                _progressImg.fillAmount = 1f;
            }

            gameObject.SetActive(false);

            Callback();
        }
    }
}

