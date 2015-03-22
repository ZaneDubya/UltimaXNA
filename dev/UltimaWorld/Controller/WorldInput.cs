using InterXLib.Input.Windows;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using UltimaXNA.Entity;
using UltimaXNA.UltimaPackets.Client;
using UltimaXNA.UltimaWorld.View;

namespace UltimaXNA.UltimaWorld.Controller
{
    /// <summary>
    /// Handles all the mouse input when the mouse is over the world.
    /// </summary>
    class WorldInput
    {
        private WorldModel m_Model;

        public WorldInput(WorldModel model)
        {
            m_Model = model;
        }

        // mouse input variables
        private bool m_ContinuousMouseMovementCheck = false;
        public bool ContinuousMouseMovementCheck
        {
            get
            {
                return m_ContinuousMouseMovementCheck;
            }
            set
            {
                if (m_ContinuousMouseMovementCheck != value)
                {
                    m_ContinuousMouseMovementCheck = value;
                    if (m_ContinuousMouseMovementCheck == true)
                        UltimaEngine.UserInterface.AddInputBlocker(this);
                    else
                        UltimaEngine.UserInterface.RemoveInputBlocker(this);
                }

            }
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

                // In all cases, where we are moving and the move button was released, stop moving.
                if (ContinuousMouseMovementCheck &&
                    UltimaEngine.Input.HandleMouseEvent(MouseEvent.Up, UltimaVars.EngineVars.MouseButton_Move))
                {
                    ContinuousMouseMovementCheck = false;
                }

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
            AEntity overEntity = IsometricRenderer.MouseOverObject;

            if (e.EventType == MouseEvent.Down)
            {
                // prepare to pick this item up.
                m_DraggingEntity = IsometricRenderer.MouseOverObject;
                m_dragOffset = IsometricRenderer.MouseOverObjectPoint;
            }
            else if (e.EventType == MouseEvent.Click)
            {
                AEntity entity = IsometricRenderer.MouseOverObject;
                if (entity is Ground)
                {
                    // no action.
                }
                else if (entity is StaticItem)
                {
                    // pop up name of item.
                }
                else if (entity is Item)
                {
                    // request context menu
                    UltimaInteraction.SingleClick(entity);
                }
                else if (entity is Mobile)
                {
                    // request context menu
                    UltimaInteraction.SingleClick(entity);
                }
            }
            else if (e.EventType == MouseEvent.DoubleClick)
            {
                AEntity entity = IsometricRenderer.MouseOverObject;
                if (entity is Ground)
                {
                    // no action.
                }
                else if (entity is StaticItem)
                {
                    // no action.
                }
                else if (entity is Item)
                {
                    // request context menu
                    UltimaInteraction.DoubleClick(entity);
                }
                else if (entity is Mobile)
                {
                    // Send double click packet.
                    // Set LastTarget == targeted Mobile.
                    // If in WarMode, set Attacking == true.
                    UltimaInteraction.DoubleClick(entity);
                    UltimaVars.EngineVars.LastTarget = entity.Serial;
                    if (UltimaVars.EngineVars.WarMode)
                    {
                        m_Model.Client.Send(new AttackRequestPacket(entity.Serial));
                    }
                }
            }
            else if (e.EventType == MouseEvent.DragBegin)
            {
                AEntity entity = IsometricRenderer.MouseOverObject;
                if (entity is Ground)
                {
                    // no action.
                }
                else if (entity is StaticItem)
                {
                    // no action.
                }
                else if (entity is Item)
                {
                    // attempt to pick up item.
                    UltimaInteraction.PickupItem((Item)entity, new Point((int)m_dragOffset.X, (int)m_dragOffset.Y));
                }
                else if (entity is Mobile)
                {
                    // drag off a status gump for this mobile.
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
