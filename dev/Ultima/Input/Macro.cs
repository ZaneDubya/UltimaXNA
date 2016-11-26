namespace UltimaXNA.Ultima.Input
{
    /// <summary>
    /// A single macro that performs one thing.
    /// </summary>
    public class Macro
    {
        public readonly MacroType Type;

        ValueTypes m_ValueType =  ValueTypes.None;
        int m_ValueInteger = -1;
        string m_ValueString;

        public Macro(MacroType type)
        {
            Type = type;
        }

        public Macro(MacroType type, int value)
            : this(type)
        {
            m_ValueInteger = value;
            m_ValueType = ValueTypes.Integer;
        }

        public Macro(MacroType type, string value)
            : this(type)
        {
            m_ValueString = value;
            m_ValueType = ValueTypes.String;
        }

        public int ValueInteger
        {
            set
            {
                m_ValueType =  ValueTypes.Integer;
                m_ValueInteger = value;
            }
            get
            {
                if (m_ValueType == ValueTypes.Integer)
                    return m_ValueInteger;
                return 0;
            }
        }

        public string ValueString
        {
            set
            {
                m_ValueType = ValueTypes.String;
                m_ValueString = value;
            }
            get
            {
                if (m_ValueType == ValueTypes.String)
                    return m_ValueString;
                return null;
            }
        }

        public ValueTypes ValueType
        {
            get
            {
                return m_ValueType;
            }
        }

        public override string ToString()
        {
            string value = (m_ValueType == ValueTypes.None ? string.Empty : (m_ValueType == ValueTypes.Integer ? m_ValueInteger.ToString() : m_ValueString));
            return string.Format("{0} ({1})", Type.ToString(), value);
        }

        public enum ValueTypes
        {
            None,
            Integer,
            String
        }
    }
}
