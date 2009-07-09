using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace UltimaXNA.Diagnostics
{
    /// <summary>
    /// Defines an event to notify status update information
    /// </summary>
    public interface IStatusNotifier
    {
        event EventHandler<StatusUpdateEventArgs> StatusUpdate;
    }
}
