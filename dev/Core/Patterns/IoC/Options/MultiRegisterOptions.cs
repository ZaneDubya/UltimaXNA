using System;
using System.Collections.Generic;
using System.Linq;

namespace UltimaXNA.Patterns.IoC
{
    public sealed class MultiRegisterOptions
    {
        private IEnumerable<RegisterOptions> m_registerOptions;

        public MultiRegisterOptions(IEnumerable<RegisterOptions> registerOptions)
        {
            m_registerOptions = registerOptions;
        }

        public MultiRegisterOptions AsMultiInstance()
        {
            m_registerOptions = executeOnAllRegisterOptions(ro => ro.AsMultiInstance());
            return this;
        }

        public MultiRegisterOptions AsSingleton()
        {
            m_registerOptions = executeOnAllRegisterOptions(ro => ro.AsSingleton());
            return this;
        }

        private IEnumerable<RegisterOptions> executeOnAllRegisterOptions(Func<RegisterOptions, RegisterOptions> action)
        {
            return m_registerOptions.Select(action).ToList();
        }
    }
}