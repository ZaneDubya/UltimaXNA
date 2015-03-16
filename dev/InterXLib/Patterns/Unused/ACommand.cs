using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace InterXLib.Patterns.Commandable
{
    /// <summary>
    /// CURRENTLY UNUSED. Commands are an object-oriented replacement for callbacks.
    /// </summary>
    public abstract class ACommand
    {
        private object m_Entity;
        public object Entity
        {
            get { return m_Entity; }
        }

        public ACommand(object entity)
        {
            m_Entity = entity;
        }

        internal void Execute()
        {
            OnExecute();
        }

        internal void Undo()
        {
            OnUndo();
        }

        protected abstract void OnExecute();
        protected abstract void OnUndo();
    }
}
