using System.ComponentModel;
using System.Runtime.CompilerServices;
using System;

namespace UltimaXNA.Core.ComponentModel
{
    public abstract class NotifyPropertyChangedBase
    {
        public EventHandler PropertyChanged;

        public virtual bool SetProperty<T>(ref T storage, T value)
        {
            if (EqualityHelper.IsEqual(storage, value))
            {
                return false;
            }

            storage = value;

            OnPropertyChanged();

            return true;
        }

        protected virtual void OnPropertyChanged()
        {
            EventHandler handler = PropertyChanged;
            if(handler != null)
            {
                handler(this, null);
            }
        }
    }
}
