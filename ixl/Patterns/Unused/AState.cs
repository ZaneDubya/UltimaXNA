using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace InterXLib.Patterns
{
    /// <summary>
    /// CURRENTLY UNUSED. States represent the current updatable state of an object.
    /// </summary>
    public abstract class AState
    {
        private object m_Entity;
        public object Entity
        {
            get { return m_Entity; }
        }

        public AState(object entity)
        {
            m_Entity = entity;
        }

        internal void Enter()
        {
            OnEnter();
        }

        internal void Exit()
        {
            OnExit();
        }

        internal void Update(double frameTime)
        {
            switch (m_State)
            {
                case StateStatus.None:
                    // do nothing
                    break;
                case StateStatus.Entering:
                    OnUpdateActive(frameTime);
                    break;
                case StateStatus.Active:
                    OnUpdateEntering(frameTime);
                    break;
                case StateStatus.Exiting:
                    OnUpdateExiting(frameTime);
                    break;
            }
        }

        private StateStatus m_State = StateStatus.None;

        protected virtual void OnEnter() { }
        protected virtual void OnExit() { }
        protected virtual void OnUpdateActive(double frameTime) { }
        protected virtual void OnUpdateEntering(double frameTime) { }
        protected virtual void OnUpdateExiting(double frameTime) { }

        enum StateStatus
        {
            None,
            Entering,
            Active,
            Exiting,
        }
    }
}
