using System.Linq;
using System.Collections.Generic;
using PureMVC.Interfaces;
using PureMVC.Patterns.Mediator;

namespace HotFix.Core
{
    public class BaseMediator<T> : Mediator where T : BaseView, new()
    {
        public delegate void Handler(INotification notification);

        public new T ViewComponent { get; set; }

        protected Dictionary<string, Handler> _notificationDic = new Dictionary<string, Handler>();

        private Queue<INotification> _noticficationQueue = new Queue<INotification>();

        private string _notificationShow;
        private string _notificationHide;

        private CoroutineHandle _onUpdateHandle;

        public BaseMediator() : base()
        {
            OnBind();
        }

        public BaseMediator(string mediatorName, object viewComponent = null) : base(mediatorName, viewComponent)
        {
            OnBind();
        }

        public override void HandleNotification(INotification notification)
        {
            if (notification == null)
                return;

            if (!_notificationDic.TryGetValue(notification.Name, out Handler callback) || callback == null)
                return;

            if (notification.Name != _notificationShow &&
                notification.Name != _notificationHide &&
                (ViewComponent == null || !ViewComponent.HasCreateed))
            {
                _noticficationQueue.Enqueue(notification);
            }
            else
            {
                callback(notification);
            }
        }

        protected void RegisterNotification(string key, Handler handler)
        {
            _notificationDic.Add(key, handler);
        }

        protected void RemoveNotification(string key)
        {
            _notificationDic.Remove(key);
        }

        public override string[] ListNotificationInterests()
        {
            return _notificationDic.Keys.ToArray();
        }

        protected virtual void OnBind()
        {
            _notificationDic.Clear();

            var fullname = typeof(T).FullName;
            _notificationShow = fullname + "_SHOW";
            _notificationHide = fullname + "_HIDE";

            RegisterNotification(_notificationShow, OnShow);
            RegisterNotification(_notificationHide, OnHide);
        }

        private void OnShow(INotification notification)
        {
            if (ViewComponent != null)
            {
                OnShowExtend(notification);
                return;
            }
            ViewComponent = UIManager.Instance.ShowPanel<T>();
            _onUpdateHandle = CoroutineManager.RunCoroutine(OnUpdate(notification));
        }

        private void OnHide(INotification notification)
        {
            _noticficationQueue.Clear();
            CoroutineManager.KillCoroutines(_onUpdateHandle);

            OnHideExtend(notification);

            float delay = -1f;
            if (notification.Body != null)
            {
                delay = (float)notification.Body;
            }
            UIManager.Instance.HidePanel<T>(delay);
        }

        protected virtual void OnShowExtend(INotification notification)
        {

        }

        protected virtual void OnHideExtend(INotification notification)
        {

        }

        private IEnumerator<float> OnUpdate(INotification startNotification)
        {
            while (ViewComponent == null || !ViewComponent.HasCreateed || !ViewComponent.IsShowing)
                yield return CoroutineManager.WaitForOneFrame;

            OnShowExtend(startNotification);

            while (_noticficationQueue.Count > 0)
            {
                var notification = _noticficationQueue.Dequeue();

                _notificationDic[notification.Name](notification);
            }
        }
    }
}
