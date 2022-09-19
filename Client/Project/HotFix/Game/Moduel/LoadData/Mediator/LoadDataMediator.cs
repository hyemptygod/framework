using PureMVC.Interfaces;
using HotFix.Core;
using HotFix.Base;

namespace HotFix.Game
{
    public class LoadDataMediator : BaseMediator<LoadDataView>
    {
        protected override void OnBind()
        {
            base.OnBind();

            RegisterNotification(EventList.LOADDATA_VIEW_REFRESH, OnRefresh);
            RegisterNotification(EventList.LOADDATA_VIEW_COMPLETE, OnComplete);
        }

        private void OnRefresh(INotification notification)
        {
            ViewComponent.RefreshTitle(notification.Body as ProgressInfo);
        }

        private void OnComplete(INotification notification)
        {
            ViewComponent.Complete();
        }
    }
}
