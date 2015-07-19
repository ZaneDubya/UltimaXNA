/***************************************************************************
 *   ITracer.cs
 *   Copyright (c) 2015 UltimaXNA Development Team
 * 
 *   This program is free software; you can redistribute it and/or modify
 *   it under the terms of the GNU General Public License as published by
 *   the Free Software Foundation; either version 3 of the License, or
 *   (at your option) any later version.
 *
 ***************************************************************************/

using System;

namespace UltimaXNA.Core.Diagnostics.Tracing
{
    public interface ITracer
    {
        void Critical(Exception ex);
        void Critical(Exception ex, string message, params object[] args);
        void Error(Exception ex);
        void Error(Exception ex, string message, params object[] args);
        void Warn(Exception ex);
        void Warn(Exception ex, string message, params object[] args);
        void Warn(string message, params object[] args);
        void Verbose(string message, params object[] args);
        void Info(string message, params object[] args);
    }
}