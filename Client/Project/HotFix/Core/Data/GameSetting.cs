using System;
using UnityEngine;

namespace HotFix.Core
{
    /// <summary>
    /// 游戏的基本配置
    /// </summary>
    [Serializable]
    public class GameSetting
    {
        /// <summary>
        /// 是否有服务器
        /// </summary>
        public bool UseServer = false;
        /// <summary>
        /// 服务器的地址
        /// </summary>
        public string ServerURL = "http://www.huoyunxs.com";
    }
}
