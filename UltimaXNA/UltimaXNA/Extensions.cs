using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel.Design;
using Microsoft.Xna.Framework;

namespace UltimaXNA
{
    public static class GameServiceContainerExtensions
    {
        public static T GetService<T>(this GameServiceContainer service)
        {
            return service.GetService<T>(false);
        }

        public static T GetService<T>(this GameServiceContainer service, bool throwExceptionIfNotFound)
        {
            object o = service.GetService(typeof(T));

            if (o is T)
            {
                return (T)o;
            }
            else if (throwExceptionIfNotFound)
            {
                throw new ServiceNotFoundException(typeof(T).Name);
            }

            return default(T);
        }

        public static void AddService<T>(this GameServiceContainer service, T item)
        {
            service.AddService(typeof(T), item);
        }
    }

    public class ServiceNotFoundException : Exception
    {
        public ServiceNotFoundException(string serviceName)
            : base(String.Format("{0} Service is required and was not found in the Game.Services container", serviceName))
        {

        }
    }
}
