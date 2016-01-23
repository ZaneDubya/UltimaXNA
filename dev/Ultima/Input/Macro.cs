namespace UltimaXNA.Ultima.Input
{
    /// <summary>
    /// A single macro action.
    /// </summary>
    public class Macro
    {
        public readonly MacroType Type;

        private bool m_IsInteger = true;
        private int m_ValueInteger = -1;
        private string m_ValueString = null;

        public string Name
        {
            get
            {
                return Macros.GetMacroName(this);
            }
        }

        public Macro(MacroType type)
        {
            Type = type;
        }

        public Macro(MacroType type, int value)
            : this(type)
        {
            m_ValueInteger = value;
            m_IsInteger = true;
        }

        public Macro(MacroType type, string value)
            : this(type)
        {
            m_ValueString = value;
            m_IsInteger = false;
        }

        public int ValueInteger
        {
            set
            {
                m_IsInteger = true;
                m_ValueInteger = value;
            }
            get
            {
                if (m_IsInteger)
                    return m_ValueInteger;
                else
                    return 0;
            }
        }

        public string ValueString
        {
            set
            {
                m_IsInteger = false;
                m_ValueString = value;
            }
            get
            {
                return m_ValueString;
            }
        }

        public bool IsInteger
        {
            get
            {
                return m_IsInteger;
            }
        }

        public override string ToString()
        {
            return string.Format("{0} ({1})", Name, (m_IsInteger ? m_ValueInteger.ToString() : m_ValueString));
        }
    }
}
