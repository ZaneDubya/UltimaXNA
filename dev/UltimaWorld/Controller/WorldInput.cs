using InterXLib.Input.Windows;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using UltimaXNA.Entity;
using UltimaXNA.UltimaPackets.Client;
using UltimaXNA.UltimaWorld.View;

namespace UltimaXNA.UltimaWorld.Controller
{
    class WorldInput
    {
        public WorldInput(WorldModel model)
        {
            m_Model = model;
        }

        private WorldModel m_Model;

        // mouse input variables
        bool m_ContinuousMoveCheck = false;
        public bool ContinuousMouseMovementCheck
        {
            set { m_ContinuousMoveCheck = value; }
        }

        const int m_TimeHoveringBeforeTipMS = 1000;

        // make sure we drag the correct object variables
        Vector2 m_dragOffset;
        AMapObject m_dragObject;

        public void Update(double frameMS)
        {
            // input for debug variables.
            List<InputEventKeyboard> keyEvents = UltimaEngine.Input.GetKeyboardEvents();
            foreach (InputEventKeyboard e in keyEvents)
            {
                // debug flags
                if ((e.EventType == KeyboardEventType.Press) && (e.KeyCode == WinKeys.D) && e.Control)
                {
                    if (!e.Alt)
                    {

                        if (!UltimaVars.DebugVars.Flag_ShowDataRead)
                            UltimaVars.DebugVars.Flag_ShowDataRead = true;
                        else
                        {
                            if (!UltimaVars.DebugVars.Flag_BreakdownDataRead)
                                UltimaVars.DebugVars.Flag_BreakdownDataRead = true;
                            else
                            {
                                UltimaVars.DebugVars.Flag_ShowDataRead = false;
                                UltimaVars.DebugVars.Flag_BreakdownDataRead = false;
                            }
                        }
                    }
                    else
                    {
                        Diagnostics.Dynamic.InvokeDebug();
                    }
                    e.Handled = true;
                }

                // fps limiting
                if ((e.EventType == KeyboardEventType.Press) && (e.KeyCode == WinKeys.F) && e.Alt)
                {
                    if (!e.Control)
                        UltimaVars.DebugVars.Flag_DisplayFPS = Utility.ToggleBoolean(UltimaVars.DebugVars.Flag_DisplayFPS);
                    else
                        UltimaVars.EngineVars.LimitFPS = Utility.ToggleBoolean(UltimaVars.EngineVars.LimitFPS);
                    e.Handled = true;
                }

                // mouse enabling
                if ((e.EventType == KeyboardEventType.Press) && (e.KeyCode == WinKeys.M) && e.Alt)
                {
                    UltimaVars.EngineVars.MouseEnabled = Utility.ToggleBoolean(UltimaVars.EngineVars.MouseEnabled);
                    e.Handled = true;
                }
            }

            doUpdate();
        }

        void doUpdate()
        {
            if (UltimaVars.EngineVars.InWorld && !UltimaEngine.UserInterface.IsModalControlOpen && m_Model.Client.IsConnected)
            {
                // always parse keyboard. (Is it possible there are some situations in which keyboard input is blocked???)
                parseKeyboard();

                // If 1. The mouse is over the world (not over UI) and
                //    2. The cursor is not blocking input, then interpret mouse input.
                if (!UltimaEngine.UserInterface.IsMouseOverUI && !m_Model.Cursor.IsHoldingItem)
                    parseMouse();

                // PickType is the kind of objects that will show up as the 'MouseOverObject'
                if (UltimaEngine.UserInterface.IsMouseOverUI)
                    IsometricRenderer.PickType = PickTypes.PickNothing;
                else
                    IsometricRenderer.PickType = PickTypes.PickEverything;

                // Set the cursor direction
                UltimaVars.EngineVars.CursorDirection = Utility.DirectionFromPoints(new Point (400, 300), UltimaEngine.Input.MousePosition);

                // Show a popup tip if we have hovered over this item for X seconds.
                if (UltimaEngine.Input.MouseStationaryTimeMS >= m_TimeHoveringBeforeTipMS)
                    if (IsometricRenderer.MouseOverObject != null)
                        createHoverLabel(IsometricRenderer.MouseOverObject);

                // Changed to leverage movementFollowsMouse interface option -BERT
                if (m_ContinuousMoveCheck)
                    doMovement();

                // Show our target's name
                createHoverLabel(UltimaVars.EngineVars.LastTarget);
            }
        }

        void doMovement()
        {
            // Get the move direction and add the Running offset if the Cursor is far enough away.
            Direction moveDirection = UltimaVars.EngineVars.CursorDirection;
            float distanceFromCenterOfScreen = Utility.DistanceBetweenTwoPoints(new Point(400, 300), UltimaEngine.Input.MousePosition);
            if (distanceFromCenterOfScreen >= 150.0f)
                moveDirection |= Direction.Running;

            // Tell the player to Move.
            Mobile m = (Mobile)EntityManager.GetPlayerObject();
            m.PlayerMobile_Move(moveDirection);
        }

        void onMoveButton(InputEventMouse e)
        {
            if (e.EventType == MouseEvent.Down)
            {
                // keep moving as long as the move button is down.
                m_ContinuousMoveCheck = true;
            }
            else if (e.EventType == MouseEvent.Up)
            {
                // If the movement mouse button has been released, stop moving.
                m_ContinuousMoveCheck = false;
            }

            e.Handled = true;
        }

        void onInteractButton(InputEventMouse e)
        {
            AMapObject overObject = (e.EventType == MouseEvent.DragBegin) ? m_dragObject : IsometricRenderer.MouseOverObject;
            Vector2 overObjectOffset = (e.EventType == MouseEvent.DragBegin) ? m_dragOffset : IsometricRenderer.MouseOverObjectPoint;

            if (e.EventType == MouseEvent.Down)
            {
                m_dragObject = overObject;
                m_dragOffset = overObjectOffset;
            }

            if (overObject == null)
                return;

            if (m_Model.Cursor.IsTargeting && e.EventType == MouseEvent.Click)
            {
                // Special case: targeting
                // handled by cursor class.
            }
            else
            {
                // standard interaction actions ...
                if (overObject is MapObjectGround)
                {
                    // we can't interact with ground tiles.
                }
                else if (overObject is MapObjectStatic)
                {
                    // clicking a should pop up the name of the static.
                    if (e.EventType == MouseEvent.Click)
                    {

                    }
                }
                else if (overObject is MapObjectItem)
                {
                    Item item = (Item)overObject.OwnerEntity;
                    // single click = tool tip
                    // double click = use / open
                    // click and drag = pick up
                    switch (e.EventType)
                    {
                        case MouseEvent.Click:
                            // tool tip
                            UltimaInteraction.SingleClick(item);
                            break;
                        case MouseEvent.DoubleClick:
                            UltimaInteraction.DoubleClick(item);
                            break;
                        case MouseEvent.DragBegin:
                            UltimaInteraction.PickupItem(item, new Point((int)overObjectOffset.X, (int)overObjectOffset.Y));
                            break;
                    }
                }
                else if (overObject is MapObjectMobile)
                {
                    Mobile entity = (Mobile)overObject.OwnerEntity;
                    // single click = tool tip; if npc = request context sensitive menu
                    // double click = set last target; if is human open paper doll; if ridable ride; if self and riding, dismount;
                    // click and drag = pull off status bar
                    switch (e.EventType)
                    {
                        case MouseEvent.Click:
                            // tool tip
                            UltimaInteraction.SingleClick(entity);
                            if (UltimaVars.EngineVars.WarMode)
                                m_Model.Client.Send(new AttackRequestPacket(entity.Serial));
                            else
                                m_Model.Client.Send(new RequestContextMenuPacket(entity.Serial));
                            break;
                        case MouseEvent.DoubleClick:
                            UltimaInteraction.DoubleClick(entity);
                            UltimaVars.EngineVars.LastTarget = entity.Serial;
                            break;
                        case MouseEvent.DragBegin:
                            // pull off status bar
                            break;
                    }
                }
                else if (overObject is MapObjectCorpse)
                {
                    Corpse entity = (Corpse)overObject.OwnerEntity;
                    // click and drag = nothing
                    // single click = tool tip
                    // double click = open loot window.
                }
                else if (overObject is MapObjectText)
                {
                    // clicking on text should somehow indicate the person speaking.
                }
                else
                {
                    throw new Exception("Unknown object type in onInteractButtonDown()");
                }
            }

            e.Handled = true;
        }

        void createHoverLabel(AMapObject mapObject)
        {
            if (mapObject.OwnerSerial.IsValid)
            {
                // this object is an entity of some kind.
                createHoverLabel(mapObject.OwnerSerial);
            }
            else if (mapObject is MapObjectStatic)
            {
                // since statics have no entity object, we can't easily create a label for them at the moment.
                // surely this will be fixed.
            }
        }

        void createHoverLabel(Serial serial)
        {
            if (!serial.IsValid)
                return;

            BaseEntity e = EntityManager.GetObject<BaseEntity>(serial, false);

            if (e is Mobile)
            {
                Mobile m = (Mobile)e;
                m.AddOverhead(MessageType.Label, m.Name, 3, m.NotorietyHue);
            }
            else if (e is Corpse)
            {
                // Currently corpse entities do not Update() so they will not show their label.
            }
            else if (e is Item)
            {
                // Currently item entities do not Update() so they will not show their label.
            }
        }

        void parseMouse()
        {
            List<InputEventMouse> events = UltimaEngine.Input.GetMouseEvents();
            foreach (InputEventMouse e in events)
            {
                if (e.Button == UltimaVars.EngineVars.MouseButton_Move)
                {
                    onMoveButton(e);
                }
                else if (e.Button == UltimaVars.EngineVars.MouseButton_Interact)
                {
                    onInteractButton(e);
                }
                else
                {
                    // no handler for this button.
                }
            }
        }

        void parseKeyboard()
        {
            List<InputEventKeyboard> events = UltimaEngine.Input.GetKeyboardEvents();
            foreach (InputEventKeyboard e in events)
            {
                // Toggle for war mode:
                if (e.EventType == KeyboardEventType.Down && e.KeyCode == WinKeys.Tab)
                {
                    if (UltimaVars.EngineVars.WarMode)
                        m_Model.Client.Send(new RequestWarModePacket(false));
                    else
                        m_Model.Client.Send(new RequestWarModePacket(true));
                }

                // toggle for All Names:
                /*if (InputState.IsKeyDown(Keys.LeftShift) && InputState.IsKeyDown(Keys.LeftControl))
                {
                    List<Mobile> mobiles = EntitiesCollection.GetObjectsByType<Mobile>();
                    foreach (Mobile m in mobiles)
                    {
                        if (m.Name != string.Empty)
                        {
                            m.AddOverhead(MessageType.Label, m.Name, 3, m.NotorietyHue);
                        }
                    }
                }*/
            }
        }
    }
}
