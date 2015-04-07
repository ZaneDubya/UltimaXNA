using UltimaXNA.Core.Input.Windows;
using Microsoft.Xna.Framework;
using System.Collections.Generic;
using UltimaXNA.Core.Input;

namespace InterXLib.Patterns.MVC
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
