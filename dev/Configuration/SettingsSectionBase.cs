#region Usings
using System;
using System.ComponentModel;
using UltimaXNA.Core.ComponentModel;
#endregion

namespace UltimaXNA.Configuration
{
    public abstract class SettingsSectionBase : NotifyPropertyChangedBase
    {
        protected SettingsSectionBase()
        {

        }

        public event EventHandler Invalidated;

        internal void OnDeserialized()
        {
            UpdateVersionValues();
        }

        protected virtual void UpdateVersionValues()
        {
            
        }

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