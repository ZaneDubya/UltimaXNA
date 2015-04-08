#region Usings

using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using UltimaXNA.Diagnostics;

#endregion

namespace UltimaXNA.Data
{
    public abstract class SettingsBase
    {
        private readonly SettingsFile _file;

        protected SettingsBase(SettingsFile file)
        {
            _file = file;
        }

        protected abstract string Name
        {
            get;
        }

        protected void SetValue<T>(T value, [CallerMemberName] string propertyName = null)
        {
            Guard.Requires<ArgumentNullException>(!string.IsNullOrWhiteSpace(propertyName), "propertyName");

            // ReSharper disable once ExplicitCallerInfoArgument
            var notify = GetValue(default(T), propertyName) as INotifyPropertyChanged;

            if(notify != null)
            {
                notify.PropertyChanged -= OnSettingPropertyChanged;
            }

            _file.SetValue(Name, char.ToLowerInvariant(propertyName[0]) + propertyName.Substring(1), value);

            notify = value as INotifyPropertyChanged;

            if(notify != null)
            {
                notify.PropertyChanged += OnSettingPropertyChanged;
            }
        }

        protected T GetValue<T>(T defaultValue, [CallerMemberName] string propertyName = null)
        {
            Guard.Requires<ArgumentNullException>(!string.IsNullOrWhiteSpace(propertyName), "propertyName");
            return _file.GetValue(Name, char.ToLowerInvariant(propertyName[0]) + propertyName.Substring(1), defaultValue);
        }

        private void OnSettingPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            _file.InvalidateDirty();
        }
    }
}