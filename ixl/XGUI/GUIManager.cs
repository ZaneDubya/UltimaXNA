using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using InterXLib.XGUI.Rendering;
using InterXLib.Input.Windows;
using InterXLib.Display;
using Microsoft.Xna.Framework;

namespace InterXLib.XGUI
{
    public class GUIManager
    {
        // To add: 
        //  * CursorSprite
        //  * 
        private AElement m_KeyboardFocusControl = null;
        public AElement KeyboardFocusControl
        {
            get { return m_KeyboardFocusControl; }
            set
            {
                if (value != m_KeyboardFocusControl)
                {
                    // inform control that it loses focus?
                    m_KeyboardFocusControl = value;
                    // inform control that is gains focus?
                }
            }
        }

        private AElement m_MouseOverControl = null;
        public AElement MouseOverControl
        {
            get { return m_MouseOverControl; }
            set
            {
                if (value != m_MouseOverControl)
                {
                    if (m_MouseOverControl != null)
                    {
                        m_MouseOverControl.IsMouseOver = false;
                        if (m_MouseOverControl.GetController().GetType().IsSubclassOf(typeof(AElementController)))
                            ((AElementController)m_MouseOverControl.GetController()).OnMouseExit();
                    }
                    m_MouseOverControl = value;
                    if (m_MouseOverControl != null)
                    {
                        m_MouseOverControl.IsMouseOver = true;
                        if (m_MouseOverControl.GetController().GetType().IsSubclassOf(typeof(AElementController)))
                            ((AElementController)m_MouseOverControl.GetController()).OnMouseEnter();
                    }
                }
            }
        }

        public Dictionary<string, Skin> Skins = new Dictionary<string,Skin>();
        public Dictionary<string, YSpriteFont> Fonts = new Dictionary<string, YSpriteFont>();
        public string DefaultSkinName, DefaultFontName;

        private AScreen m_CurrentScreen = null;
        public AScreen CurrentScreen
        {
            get { return m_CurrentScreen; }
            set
            {
                m_CurrentScreen = value;
                if (m_CurrentScreen != null)
                    m_CurrentScreen.Initialize(this);
            }
        }

        public void AddSkin(string name, Skin skin)
        {
            Skins.Add(name, skin);
        }

        public void AddFont(string name, YSpriteFont font)
        {
            Fonts.Add(name, font);
        }

        public YSpriteFont GetFont(string name)
        {
            YSpriteFont font = null;
            if (Fonts.TryGetValue(name, out font))
                return font;
            return Fonts[DefaultFontName];
        }

        internal void Update(double totalTime, double frameTime)
        {
            if (m_CurrentScreen != null)
                m_CurrentScreen.Update(totalTime, frameTime);
        }

        internal void Draw(YSpriteBatch spritebatch, double frameTime)
        {
            spritebatch.GUIClipRect = new Rectangle(0, 0, 1024, 768);
            if (m_CurrentScreen != null)
                m_CurrentScreen.GetView().Draw(spritebatch, frameTime);
        }

        internal void ReceiveKeyboardInput(List<InputEventKeyboard> events)
        {
            if (m_KeyboardFocusControl != null)
                ((AElementController)m_KeyboardFocusControl.GetController()).ReceiveKeyboardInput(events);
        }

        internal void ReceiveMouseInput(Point MousePosition, List<InputEventMouse> events)
        {
            List<AElement> mouseOverControls = null;

            if (m_CurrentScreen != null)
            {
                if (m_CurrentScreen.GetElementsAtPoint(MousePosition, true, out mouseOverControls))
                {
                    MouseOverControl = mouseOverControls[0];
                    foreach (InputEventMouse e in events)
                        ((AElementController)MouseOverControl.GetController()).ReceiveMouseInput(MousePosition, events);
                }
                else
                {
                    MouseOverControl = null;
                }
            }

            // clear mouse down elements if there is a mouse up event.
            foreach (InputEventMouse e in events)
                if (e.EventType == MouseEvent.Up)
                    InformMouseDownElementsThatMouseIsUp(e);

            
        }

        public void AnnounceElementWantsKeyboardInput(AElement e)
        {

        }

        public void AnnounceElementGivesUpKeyboardInput(AElement e)
        {

        }

        protected void InformMouseDownElementsThatMouseIsUp(InputEventMouse e)
        {
            if (m_MouseDownElements == null || m_MouseDownElements.Count == 0)
                return;

            List<InputEventMouse> list = new List<InputEventMouse>(1) {e};

            for (int i = 0; i < m_MouseDownElements.Count; i++)
            {
                Tuple<AElement, MouseButton> md = m_MouseDownElements[i];
                if (md.Item2 == e.Button)
                {
                    m_MouseDownElements.RemoveAt(i);
                    i--;
                    ((AElementController)md.Item1.GetController()).ReceiveMouseInput(e.Position, list);
                }
            }
        }

        List<Tuple<AElement, MouseButton>> m_MouseDownElements;
        internal void AnnounceElementOnMouseDown(AElement e, MouseButton b)
        {
            if (m_MouseDownElements == null)
                m_MouseDownElements = new List<Tuple<AElement, MouseButton>>();
            m_MouseDownElements.Add(new Tuple<AElement, MouseButton>(e, b));

            if (b == MouseButton.Left)
            {
                if (e.HandlesKeyboardInput)
                    KeyboardFocusControl = e;
            }
        }

        internal bool AnnounceElementOnMouseUp(AElement e, MouseButton b)
        {
            bool control_has_no_other_mouse_downs = true;
            if (m_MouseDownElements == null || m_MouseDownElements.Count == 0)
                return control_has_no_other_mouse_downs;
            
            for (int i = 0; i < m_MouseDownElements.Count; i++)
            {
                Tuple<AElement, MouseButton> md = m_MouseDownElements[i];
                if (md.Item1 == e)
                {
                    if (md.Item2 == b)
                    {
                        m_MouseDownElements.RemoveAt(i);
                        i--;
                    }
                    else
                    {
                        control_has_no_other_mouse_downs = false;
                    }
                }
            }
            return control_has_no_other_mouse_downs;
        }

    }
}
