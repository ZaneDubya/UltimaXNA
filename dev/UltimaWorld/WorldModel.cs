using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using InterXLib.Patterns.MVC;

namespace UltimaXNA.UltimaWorld
{
    class WorldModel : AModel
    {
        private EntityManager m_Entities;
        public EntityManager Entities
        {
            get { return m_Entities; }
        }

        public WorldModel()
        {
            m_Entities = new EntityManager();
        }

        public override void Update(double totalTime, double frameTime)
        {
            throw new NotImplementedException();
        }

        protected override AController CreateController()
        {
            throw new NotImplementedException();
        }

        protected override AView CreateView()
        {
            throw new NotImplementedException();
        }
    }
}
