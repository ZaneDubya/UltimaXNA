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
        private WorldModel m_Model;

        public WorldInput(WorldModel model)
        {
            m_Model = model;
        }

        // mouse input variables
        public bool ContinuousMouseMovementCheck
        {
            get;
            set;
        }

        // make sure we drag the correct object variables
        Vector2 m_dragOffset;
        AEntity m_DraggingEntity;

        public void Update(double frameMS)
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
                UltimaVars.EngineVars.CursorDirection = Utility.DirectionFromPoints(new Point(400, 300), UltimaEngine.Input.MousePosition);

                // Changed to leverage movementFollowsMouse interface option
                if (ContinuousMouseMovementCheck)
                    doMovement();
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
                ContinuousMouseMovementCheck = true;
            }
            else if (e.EventType == MouseEvent.Up)
            {
                // If the movement mouse button has been released, stop moving.
                ContinuousMouseMovementCheck = false;
            }

            e.Handled = true;
        }

        void onInteractButton(InputEventMouse e)
        {
            AEntity overEntity = (e.EventType == MouseEvent.DragBegin) ? m_DraggingEntity : IsometricRenderer.MouseOverObject;
            Vector2 overObjectOffset = (e.EventType == MouseEvent.DragBegin) ? m_dragOffset : IsometricRenderer.MouseOverObjectPoint;

            if (e.EventType == MouseEvent.Down)
            {
                m_DraggingEntity = overEntity;
                m_dragOffset = overObjectOffset;
            }

            if (overEntity == null)
                return;

            if (m_Model.Cursor.IsTargeting && e.EventType == MouseEvent.Click)
            {
                // Special case: targeting
                // handled by cursor class.
            }
            else
            {
                // standard interaction actions ...
                if (overEntity is Ground)
                {
                    // we can't interact with ground tiles.
                }
                else if (overEntity is StaticItem)
                {
                    // clicking a should pop up the name of the static.
                    if (e.EventType == MouseEvent.Click)
                    {

                    }
                }
                else if (overEntity is Item)
                {
                    Item item = (Item)overEntity;
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
                else if (overEntity is Mobile)
                {
                    Mobile entity = (Mobile)overEntity;
                    // single click = tool tip; if npc = request context sensitive menu
                    // double click = set last target; if is human open paper doll; if ridable ride; if self and riding, dismount;
                    // click and drag = pull off status bar
                    switch (e.EventType)
                    {
                        case MouseEvent.Click:
                            // tool tip
                            UltimaInteraction.SingleClick(entity);
                            if (UltimaVars.EngineVars.WarMode)
                            {
                                m_Model.Client.Send(new AttackRequestPacket(entity.Serial));
                            }
                            else
                            {
                                // m_Model.Client.Send(new RequestContextMenuPacket(entity.Serial));
                            }
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
                else if (overEntity is Corpse)
                {
                    Corpse entity = (Corpse)overEntity;
                    // click and drag = nothing
                    // single click = tool tip
                    // double click = open loot window.
                }
                /*else if (overEntity is MapObjectText)
                {
                    // clicking on text should somehow indicate the person speaking.
                }*/
                else
                {
                    throw new Exception("Unknown object type in onInteractButtonDown()");
                }
            }

            e.Handled = true;
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
                    // no handler for other buttons.
                }
            }
        }

        void parseKeyboard()
        {
            // Warmode toggle:
            if (UltimaEngine.Input.HandleKeyboardEvent(KeyboardEventType.Down, WinKeys.Tab, false, false, false))
            {
                if (UltimaVars.EngineVars.WarMode)
                    m_Model.Client.Send(new RequestWarModePacket(false));
                else
                    m_Model.Client.Send(new RequestWarModePacket(true));
            }

            if (UltimaEngine.Input.HandleKeyboardEvent(KeyboardEventType.Press, WinKeys.D, false, false, true))
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

            // FPS limiting
            if (UltimaEngine.Input.HandleKeyboardEvent(KeyboardEventType.Press, WinKeys.F, false, true, false))
            {
                UltimaVars.EngineVars.LimitFPS = Utility.ToggleBoolean(UltimaVars.EngineVars.LimitFPS);
            }

            // Display FPS
            if (UltimaEngine.Input.HandleKeyboardEvent(KeyboardEventType.Press, WinKeys.F, false, true, true))
            {
                UltimaVars.DebugVars.Flag_DisplayFPS = Utility.ToggleBoolean(UltimaVars.DebugVars.Flag_DisplayFPS);
            }

            // Mouse enable / disable
            if (UltimaEngine.Input.HandleKeyboardEvent(KeyboardEventType.Press, WinKeys.M, false, true, false))
            {
                UltimaVars.EngineVars.MouseEnabled = Utility.ToggleBoolean(UltimaVars.EngineVars.MouseEnabled);
            }
        }
    }
}
