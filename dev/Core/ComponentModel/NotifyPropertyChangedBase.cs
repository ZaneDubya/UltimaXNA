/***************************************************************************
 *   NotifyPropertyChangedBase.cs
 *   Copyright (c) 2015 UltimaXNA Development Team
 * 
 *   This program is free software; you can redistribute it and/or modify
 *   it under the terms of the GNU General Public License as published by
 *   the Free Software Foundation; either version 3 of the License, or
 *   (at your option) any later version.
 *
 ***************************************************************************/

using System;
using System.Web.Script.Serialization;

namespace UltimaXNA.Core.ComponentModel
{
    public abstract class NotifyPropertyChangedBase
    {
        [ScriptIgnore]
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
            PropertyChanged?.Invoke(this, null);
        }

        protected float Clamp(float value, float min, float max)
        {
            if (value < min)
                return min;
            if (value > max)
                return max;
            return value;
        }
    }
}
