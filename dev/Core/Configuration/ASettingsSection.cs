/***************************************************************************
 *   ASettingsSection.cs
 *   Copyright (c) 2015 UltimaXNA Development Team
 * 
 *   This program is free software; you can redistribute it and/or modify
 *   it under the terms of the GNU General Public License as published by
 *   the Free Software Foundation; either version 3 of the License, or
 *   (at your option) any later version.
 *
 ***************************************************************************/
#region usings
using System;
using System.ComponentModel;
using UltimaXNA.Core.ComponentModel;
#endregion

namespace UltimaXNA.Core.Configuration
{
    public abstract class ASettingsSection : NotifyPropertyChangedBase
    {
        public event EventHandler Invalidated;

        public override bool SetProperty<T>(ref T storage, T value)
        {
            if (!base.SetProperty<T>(ref storage, value))
                return false;

            INotifyPropertyChanged notifier = storage as INotifyPropertyChanged;

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

            return true;
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

        protected int Clamp(int value, int min, int max)
        {
            if (value < min)
                return min;
            if (value > max)
                return max;
            return value;
        }
    }
}