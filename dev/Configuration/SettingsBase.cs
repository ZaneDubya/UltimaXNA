using System;
using System.Runtime.CompilerServices;
using UltimaXNA.Diagnostics;

namespace UltimaXNA.Data
{
    public abstract class SettingsBase
    {
        private readonly SettingsFile _file;

        protected SettingsBase(SettingsFile file)
        {
            _file = file;
        }

        protected abstract string Name { get; }

        protected void SetValue<T>(T value, [CallerMemberName] string propertyName = null)
        {
            Guard.Requires<ArgumentNullException>(!string.IsNullOrWhiteSpace(propertyName), "propertyName");
            _file.SetValue(Name, char.ToLowerInvariant(propertyName[0]) + propertyName.Substring(1), value);
        }

        protected T GetValue<T>([CallerMemberName] string propertyName = null)
        {
            Guard.Requires<ArgumentNullException>(!string.IsNullOrWhiteSpace(propertyName), "propertyName");
            return _file.GetValue<T>(Name, char.ToLowerInvariant(propertyName[0]) + propertyName.Substring(1));
        }

        protected T GetValue<T>(T defaultValue, [CallerMemberName] string propertyName = null)
        {
            Guard.Requires<ArgumentNullException>(!string.IsNullOrWhiteSpace(propertyName), "propertyName");
            return _file.GetValue(Name, char.ToLowerInvariant(propertyName[0]) + propertyName.Substring(1), defaultValue);
        }
    }
}