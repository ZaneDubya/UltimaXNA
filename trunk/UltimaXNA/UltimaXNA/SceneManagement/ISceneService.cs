using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;

namespace UltimaXNA.SceneManagement
{
    public interface ISceneService : IDrawable, IGameComponent
    {
        IScene CurrentScene { get; set; }
    }
}
