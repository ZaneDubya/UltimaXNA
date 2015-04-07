using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace UltimaXNA.Core.Patterns
{
    public class Properties
    {
        private List<IProperty> m_Props = new List<IProperty>();

        private IProperty InternalGet(string name)
        {
            name = name.ToLower(); // name must be lower. enforced.
            foreach (IProperty p in m_Props)
            {
                if (p.Name == name)
                {
                    return p;
                }
            }
            return null;
        }

        public T Get<T>(string name)
        {
            IProperty p = InternalGet(name);
            if (p == null) // does not contain.
                return default(T);
            else if (p.Type == typeof(T)) // contains, right type.
                return ((Property<T>)p).Value;
            else // contains, wrong type.
                return default(T);
        }

        public bool Has(string name)
        {
            if (InternalGet(name) != null)
                return true;
            return false;
        }

        public void Clear(string name)
        {
            IProperty p = InternalGet(name);
            if (p != null)
                m_Props.Remove(p);
        }

        public void Set<T>(string name, T value)
        {
            IProperty p = InternalGet(name);
            if (p == null)
                m_Props.Add(new Property<T>(name, value));
            else if (p.Type == typeof(T))
                ((Property<T>)p).Value = value;
            else
            {
                m_Props.Remove(p);
                m_Props.Add(new Property<T>(name, value));
            }
        }

        public void Add<T>(string name, T value)
        {
            IProperty p = InternalGet(name);
            if (p == null)
                m_Props.Add(new Property<T>(name, value));
            else if (p.Type == typeof(T))
            {
                dynamic a = ((Property<T>)p).Value;
                dynamic b = value;
                ((Property<T>)p).Value = a + b;
            }
            else
            {
                m_Props.Remove(p);
                m_Props.Add(new Property<T>(name, value));
            }
        }
    }

    public interface IProperty
    {
        string Name { get; }
        Type Type { get; }
    }

    public class Property<T> : IProperty
    {
        private string m_Name;

        public string Name
        {
            get { return m_Name; }
        }

        public Type Type
        {
            get { return typeof(T); }
        }

        public T Value;

        public Property(string name, T value)
        {
            m_Name = name.ToLower(); // name must be lower. enforced.
            Value = value;
        }
    }
}
