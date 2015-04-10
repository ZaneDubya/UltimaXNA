#region Usings

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using UltimaXNA.Core.ComponentModel;
using UltimaXNA.Core.Diagnostics;

#endregion

namespace UltimaXNA.Configuration
{
    public abstract class SettingsSectionBase : NotifyPropertyChangedBase
    {
        protected SettingsSectionBase()
        {

        }

        public event EventHandler Invalidated;

        protected override void SetPropertyOverride<T>(ref T storage, object value, string propertyName)
        {
            base.SetPropertyOverride<T>(ref storage, value, propertyName);

            var notifier = storage as INotifyPropertyChanged;

            if (notifier != null)
            {
                // Stop listening to the old value since it is no longer part of
                // the settings section.
                notifier.PropertyChanged -= onSectionPropertyChanged;
            }

            notifier = value as INotifyPropertyChanged;

            if (notifier != null)
            {
                // Start listening to the new value 
                notifier.PropertyChanged += onSectionPropertyChanged;
            }
        }

        void onSectionPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            onInvalidated();
        }

        private void onInvalidated()
        {
            EventHandler handler = Invalidated;

            if (handler != null)
            {
                handler(this, EventArgs.Empty);
            }
        }
    }
}