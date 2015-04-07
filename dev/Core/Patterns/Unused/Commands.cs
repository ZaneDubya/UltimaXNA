using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace InterXLib.Patterns.Commandable
{
    /// <summary>
    /// CURRENTLY UNUSED. 
    /// </summary>
    public class Commands
    {
        private List<ACommand> m_Commands;

        public int Count
        {
            get { return m_Commands.Count; }
        }

        public Commands()
        {
            m_Commands = new List<ACommand>();
        }

        public void AddCommand(ACommand command)
        {
            m_Commands.Add(command);
            command.Execute();
        }

        public void UndoOne()
        {
            if (m_Commands.Count > 0)
            {
                m_Commands[Count - 1].Undo();
            }
        }

        public void UndoAll()
        {
            while (Count > 0)
            {
                UndoOne();
            }
        }

        public void Update()
        {
            if (m_State != null)
            {
                // m_State.Update();
            }
        }

        private AState m_State;
        public AState State
        {
            get
            {
                return m_State;
            }
            set
            {
                if (m_State != null)
                {
                    m_State.Exit();
                    m_State = null;
                }
                m_State = value;
                m_State.Enter();
            }
        }
    }
}
