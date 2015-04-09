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
        private readonly SettingsFile m_file;

        protected SettingsBase(SettingsFile file)
        {
            m_file = file;
        }

        protected abstract string Name
        {
            get;
        }

        protected virtual string Comments
        {
            get { return string.Empty; }
        }

        protected void SetValue<T>(T value, string comment = null, [CallerMemberName] string propertyName = null)
        {
            Guard.Requires<ArgumentNullException>(!string.IsNullOrWhiteSpace(propertyName), "propertyName");

            // ReSharper disable once ExplicitCallerInfoArgument
            INotifyPropertyChanged notify = GetValue(default(T), propertyName) as INotifyPropertyChanged;

            if(notify != null)
            {
                notify.PropertyChanged -= OnSettingPropertyChanged;
            }

            m_file.SetValue(Name, char.ToLowerInvariant(propertyName[0]) + propertyName.Substring(1), value, Comments, comment);

            notify = value as INotifyPropertyChanged;

            if(notify != null)
            {
                notify.PropertyChanged += OnSettingPropertyChanged;
            }
        }

        protected T GetValue<T>(T defaultValue, [CallerMemberName] string propertyName = null)
        {
            Guard.Requires<ArgumentNullException>(!string.IsNullOrWhiteSpace(propertyName), "propertyName");
            return m_file.GetValue(Name, char.ToLowerInvariant(propertyName[0]) + propertyName.Substring(1), defaultValue);
        }

        private void OnSettingPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            m_file.InvalidateDirty();
        }
    }
}