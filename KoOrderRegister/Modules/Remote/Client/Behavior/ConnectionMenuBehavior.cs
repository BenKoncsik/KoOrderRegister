using KORCore.Modules.Database.Models;
using KORCore.Modules.Database.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KoOrderRegister.Modules.Remote.Client.Behavior
{
    public class ConnectionMenuBehavior : Behavior<MenuItem>
    {
        private IRemoteDatabase _remoteDatabase;
        public ConnectionMenuBehavior(IRemoteDatabase remotedatabase)
        {
            _remoteDatabase = remotedatabase;
        }
        public static readonly BindableProperty ConnectionDataProperty =
            BindableProperty.Create(
                nameof(ConnectionData),
                typeof(ConnectionDeviceData),
                typeof(ConnectionMenuBehavior),
                null);

        public ConnectionDeviceData ConnectionData
        {
            get => (ConnectionDeviceData)GetValue(ConnectionDataProperty);
            set => SetValue(ConnectionDataProperty, value);
        }

        public static readonly BindableProperty ReverseProperty =
            BindableProperty.Create(
                nameof(Reverse),
                typeof(bool),
                typeof(ConnectionMenuBehavior),
                false);

        public bool Reverse
        {
            get => (bool)GetValue(ReverseProperty);
            set => SetValue(ReverseProperty, value);
        }

        private MenuItem _associatedObject;

        protected override void OnAttachedTo(MenuItem bindable)
        {
            base.OnAttachedTo(bindable);
            _associatedObject = bindable;
            bindable.BindingContextChanged += OnBindingContextChanged;
        }

        protected override void OnDetachingFrom(MenuItem bindable)
        {
            base.OnDetachingFrom(bindable);
            bindable.BindingContextChanged -= OnBindingContextChanged;
            _associatedObject = null;
        }

        private void OnBindingContextChanged(object sender, EventArgs e)
        {
            if (_associatedObject == null)
                return;

            _associatedObject.BindingContext = ((MenuItem)sender).BindingContext;
            UpdateMenuVisibility();
        }

        private void UpdateMenuVisibility()
        {
            if (ConnectionData == null)
                return;

            bool isConnected = CheckIfConnected(ConnectionData, Reverse);
            _associatedObject.IsEnabled = isConnected;
        }

        private bool CheckIfConnected(ConnectionDeviceData connectionData, bool reverse)
        {
            bool result = _remoteDatabase.GetConnectedUrl().Equals(connectionData.Url + "/api");
            return reverse ? !result : result;
        }
    }
}
