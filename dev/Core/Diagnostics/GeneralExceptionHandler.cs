/***************************************************************************
 *   GeneralExceptionHandler.cs
 *   Copyright (c) 2015 UltimaXNA Development Team
 * 
 *   This program is free software; you can redistribute it and/or modify
 *   it under the terms of the GNU General Public License as published by
 *   the Free Software Foundation; either version 3 of the License, or
 *   (at your option) any later version.
 *
 ***************************************************************************/
using System;
using UltimaXNA.Core.Diagnostics.Tracing;

namespace UltimaXNA.Core.Diagnostics
{
    public class GeneralExceptionHandler
    {
        private static GeneralExceptionHandler m_instance;

        public static GeneralExceptionHandler Instance
        {
            get { return m_instance ?? (m_instance = new GeneralExceptionHandler()); }
            set { m_instance = value; }
        }

        public void OnError(Exception e)
        {
            Tracer.Error(e);
            OnErrorOverride(e);
        }
        
        protected virtual void OnErrorOverride(Exception e)
        {
        }
    }
}