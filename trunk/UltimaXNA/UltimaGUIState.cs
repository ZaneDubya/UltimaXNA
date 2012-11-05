using System.Collections.Generic;
using UltimaXNA.Input;
using UltimaXNA.UltimaGUI;
using UltimaXNA.GUI;
using UltimaXNA.Graphics;
using UltimaXNA.UltimaGUI.Controls;

namespace UltimaXNA
{
    public class UltimaGUIState
    {
        UltimaCursor m_Cusor = null;
        public UltimaCursor Cursor { get { return m_Cusor; } }

        public UltimaGUIState()
        {
            m_Cusor = new UltimaCursor();
        }

        public void Draw()
        {
            // Draw the cursor
            m_Cusor.Draw(UltimaEngine.UserInterface.SpriteBatch, UltimaEngine.Input.MousePosition);
        }

        public void Update()
        {
            if (UltimaEngine.UserInterface.MouseOverControl != null && Cursor.IsHolding)
            {
                Control target = UltimaEngine.UserInterface.MouseOverControl;
                List<InputEventM> events = UltimaEngine.Input.GetMouseEvents();
                foreach (InputEventM e in events)
                {
                    if (e.EventType == MouseEvent.Up && e.Button == MouseButton.Left)
                    {
                        // dropped an item we were holding.
                        if (target is ItemGumpling && ((ItemGumpling)target).Item.ItemData.Container)
                            UltimaInteraction.DropItemToContainer(Cursor.HoldingItem, (Entity.Container)((ItemGumpling)target).Item);
                        else if (target is GumpPicContainer)
                        {
                            int x = (int)UltimaEngine.Input.MousePosition.X - Cursor.HoldingOffset.X - (target.X + target.Owner.X);
                            int y = (int)UltimaEngine.Input.MousePosition.Y - Cursor.HoldingOffset.Y - (target.Y + target.Owner.Y);
                            UltimaInteraction.DropItemToContainer(Cursor.HoldingItem, (Entity.Container)((GumpPicContainer)target).Item, x, y);
                        }
                    }
                }
            }
        }

        public MsgBox MsgBox(string msg, MsgBoxTypes type)
        {
            // pop up an error message, modal.
            MsgBox msgbox = new MsgBox(msg, type);
            UltimaEngine.UserInterface.AddControl(msgbox, 0, 0);
            return msgbox;
        }
    }
}
