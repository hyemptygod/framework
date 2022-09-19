using Framework.Resource;
using System.Collections;
using UnityEngine;

namespace Framework.Core
{
    /// <summary>
    /// 游戏管理器
    /// </summary>
    public class GameManager : Manager<GameManager>
    {
        /// <summary>
        /// 开启流程
        /// </summary>
        protected override void Init()
        {

        }

        /// <summary>
        /// 开始游戏
        /// </summary>
        public IEnumerator Start()
        {
            gameObject.AddComponent<ResourceManager>();
            while (!ResourceManager.Instance.InitializeSuccess)
                yield return 0;

            GameObject.FindWithTag("UICanvas").AddComponent<UIManager>();

            gameObject.AddComponent<ILRuntimeMgr>();
            while (!ILRuntimeMgr.Instance.Inited)
                yield return 0;
        }
    }
}
