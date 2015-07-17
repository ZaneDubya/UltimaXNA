using System.Collections.Generic;
using Microsoft.Xna.Framework;
using UltimaXNA.Core.Input;
using UltimaXNA.Core.Input.Windows;

namespace UltimaXNA.Core.Patterns.MVC
{
    /// <summary>
    /// Abstract Controller - receives input, interacts with state of model.
    /// </summary>
    public abstract class AController
    {
        protected AModel Model;

        public AController(AModel parent_model)
        {
            Model = parent_model;
        }

        public virtual void ReceiveKeyboardInput(List<InputEventKeyboard> events)
        {

        }

        public virtual void ReceiveMouseInput(Point MousePosition, List<InputEventMouse> events)
        {

        }

        public virtual void ReceiveInputActions(ActionBinder actions)
        {

        }
    }
}
