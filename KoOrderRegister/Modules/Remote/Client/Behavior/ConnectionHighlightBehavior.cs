using KoOrderRegister.Modules.Remote.Client.Service;
using KORCore.Modules.Database.Models;
using KORCore.Modules.Database.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KoOrderRegister.Modules.Remote.Client.Behavior
{
    public class ConnectionHighlightBehavior : Behavior<ListView>
    {
        private IRemoteDatabase _remoteDatabase;
        public ConnectionHighlightBehavior(IRemoteDatabase remotedatabase)
        {
            _remoteDatabase = remotedatabase;
        }
        protected override void OnAttachedTo(ListView bindable)
        {
            base.OnAttachedTo(bindable);
            bindable.ItemAppearing += OnItemAppearing;
            bindable.ItemDisappearing += OnItemDisappearing;
        }

        protected override void OnDetachingFrom(ListView bindable)
        {
            base.OnDetachingFrom(bindable);
            bindable.ItemAppearing -= OnItemAppearing;
            bindable.ItemDisappearing -= OnItemDisappearing;
        }

        private void OnItemAppearing(object sender, ItemVisibilityEventArgs e)
        {
            if (e.Item is ConnectionDeviceData connectionData)
            {
                bool isConnected = CheckIfConnected(connectionData);
                if (isConnected)
                {
                    var listView = sender as ListView;
                    var index = listView.ItemsSource.Cast<ConnectionDeviceData>().ToList().IndexOf(connectionData);
                    if (index >= 0)
                    {
                        var cell = listView.TemplatedItems[index] as ViewCell;
                        if (cell != null)
                        {
                            cell.View.BackgroundColor = Colors.Green;
                            cell.View.Background = Colors.Green;
                        }
                    }
                }
            }
        }

        private void OnItemDisappearing(object sender, ItemVisibilityEventArgs e)
        {
            if (e.Item is ConnectionDeviceData connectionData)
            {
                var listView = sender as ListView;
                var index = listView.ItemsSource.Cast<ConnectionDeviceData>().ToList().IndexOf(connectionData);
                if (index >= 0)
                {
                    var cell = listView.TemplatedItems[index] as ViewCell;
                    if (cell != null)
                    {
                        cell.View.BackgroundColor = Colors.Transparent;
                    }
                }
            }
        }

        private bool CheckIfConnected(ConnectionDeviceData connectionData)
        {
            return _remoteDatabase.GetConectedUrl().Equals(connectionData.Url + "/api");
        }
    }
}
