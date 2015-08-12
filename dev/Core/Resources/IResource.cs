using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace UltimaXNA.Core.Resources
{
    public interface IResource<T>
    {
        T GetResource(int resourceIndex);
    }
}
