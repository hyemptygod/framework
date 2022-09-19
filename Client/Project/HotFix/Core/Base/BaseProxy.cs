using PureMVC.Patterns.Proxy;

namespace HotFix.Core
{
    public class BaseProxy : Proxy
    {
        public BaseProxy() : base()
        {
            Initialize();
        }

        public BaseProxy(string proxyName, object data = null) : base(proxyName, data)
        {
            Initialize();
        }

        public override void OnRegister()
        {
            base.OnRegister();

            OnRegisterNetProtocol();
        }

        public override void OnRemove()
        {
            base.OnRemove();

            Clear();
        }

        protected virtual void Initialize()
        {

        }

        protected virtual void OnRegisterNetProtocol()
        {

        }

        protected virtual void Clear()
        {

        }
    }
}
