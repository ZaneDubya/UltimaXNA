using System;
using System.Collections.Generic;
using System.Linq;

namespace UltimaXNA.Patterns.IoC
{
    public sealed class MultiRegisterOptions
    {
        private IEnumerable<RegisterOptions> _registerOptions;

        public MultiRegisterOptions(IEnumerable<RegisterOptions> registerOptions)
        {
            _registerOptions = registerOptions;
        }

        public MultiRegisterOptions AsMultiInstance()
        {
            _registerOptions = executeOnAllRegisterOptions(ro => ro.AsMultiInstance());
            return this;
        }

        public MultiRegisterOptions AsSingleton()
        {
            _registerOptions = executeOnAllRegisterOptions(ro => ro.AsSingleton());
            return this;
        }

        private IEnumerable<RegisterOptions> executeOnAllRegisterOptions(Func<RegisterOptions, RegisterOptions> action)
        {
            return _registerOptions.Select(action).ToList();
        }
    }
}