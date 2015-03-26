using InterXLib.Input.Windows;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using UltimaXNA.UltimaEntities;
using UltimaXNA.UltimaPackets.Client;
using UltimaXNA.UltimaWorld.View;
using UltimaXNA.UltimaVars;

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
        private Vector2 m_dragOffset;
        private AEntity m_DraggingEntity;

        // keyboard movement variables.
        private double m_PauseBeforeKeyboardMovementMS = 0;
        private const double c_PauseBeforeKeyboardMovementMS = 50d;

        public void Update(double frameMS)
        {
            if (UltimaVars.EngineVars.InWorld && !UltimaEngine.UserInterface.IsModalControlOpen && m_Model.Client.IsConnected)
            {
                // always parse keyboard. (Is it possible there are some situations in which keyboard input is blocked???)
                InternalParseKeyboard(frameMS);

                // In all cases, where we are moving and the move button was released, stop moving.
                if (ContinuousMouseMovementCheck &&
                    UltimaEngine.Input.HandleMouseEvent(MouseEvent.Up, UltimaVars.EngineVars.MouseButton_Move))
                {
                    ContinuousMouseMovementCheck = false;
                }

                // If 1. The mouse is over the world (not over UI) and
                //    2. The cursor is not blocking input, then interpret mouse input.
                if (!UltimaEngine.UserInterface.IsMouseOverUI && !m_Model.Cursor.IsHoldingItem)
                    InternalParseMouse(frameMS);

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
            if (distanceFromCenterOfScreen >= 150.0f || UltimaVars.SettingVars.AlwaysRun)
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

        void onInteractButton(InputEventMouse e, AEntity overEntity, Vector2 overEntityPoint)
        {
            if (e.EventType == MouseEvent.Down)
            {
                // prepare to pick this item up.
                m_DraggingEntity = overEntity;
                m_dragOffset = overEntityPoint;
            }
            else if (e.EventType == MouseEvent.Click)
            {
                if (overEntity is Ground)
                {
                    // no action.
                }
                else if (overEntity is StaticItem)
                {
                    // pop up name of item.
                    overEntity.AddOverhead(MessageType.Label, "<outline>" + overEntity.Name, 0, 0);
                    Model.StaticManager.AddStaticThatNeedsUpdating(overEntity as StaticItem);
                }
                else if (overEntity is Item)
                {
                    // request context menu
                    UltimaInteraction.SingleClick(overEntity);
                }
                else if (overEntity is Mobile)
                {
                    // request context menu
                    UltimaInteraction.SingleClick(overEntity);
                }
            }
            else if (e.EventType == MouseEvent.DoubleClick)
            {
                if (overEntity is Ground)
                {
                    // no action.
                }
                else if (overEntity is StaticItem)
                {
                    // no action.
                }
                else if (overEntity is Item)
                {
                    // request context menu
                    UltimaInteraction.DoubleClick(overEntity);
                }
                else if (overEntity is Mobile)
                {
                    // Send double click packet.
                    // Set LastTarget == targeted Mobile.
                    // If in WarMode, set Attacking == true.
                    UltimaInteraction.DoubleClick(overEntity);
                    UltimaVars.EngineVars.LastTarget = overEntity.Serial;
                    if (UltimaVars.EngineVars.WarMode)
                    {
                        m_Model.Client.Send(new AttackRequestPacket(overEntity.Serial));
                    }
                }
            }
            else if (e.EventType == MouseEvent.DragBegin)
            {
                if (overEntity is Ground)
                {
                    // no action.
                }
                else if (overEntity is StaticItem)
                {
                    // no action.
                }
                else if (overEntity is Item)
                {
                    // attempt to pick up item.
                    UltimaInteraction.PickupItem((Item)overEntity, new Point((int)m_dragOffset.X, (int)m_dragOffset.Y));
                }
                else if (overEntity is Mobile)
                {
                    // drag off a status gump for this mobile.
                }
            }

            e.Handled = true;
        }

        void InternalParseMouse(double frameMS)
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
                    if (e.EventType == MouseEvent.Click)
                    {
                        InternalQueueSingleClick(e, IsometricRenderer.MouseOverObject, IsometricRenderer.MouseOverObjectPoint);
                        continue;
                    }
                    else if (e.EventType == MouseEvent.DoubleClick)
                    {
                        ClearQueuedClick();
                    }
                    onInteractButton(e, IsometricRenderer.MouseOverObject, IsometricRenderer.MouseOverObjectPoint);
                }
                else
                {
                    // no handler for other buttons.
                }
            }

            InternalCheckQueuedClick(frameMS);
        }

        #region QueuedClicks
        // Legacy Client waits about 0.5 seconds before sending a click event when you click in the world.
        // This allows time for the player to potentially double-click on an object.
        // If the player does so, this will cancel the single-click event.
        private bool m_QueuedEvent_InQueue = false;
        private InputEventMouse m_QueuedEvent = null;
        private AEntity m_QueuedEntity = null;
        private double m_QueuedEvent_DequeueAt = 0d;
        private Vector2 m_QueuedEntityPosition;

        private void ClearQueuedClick()
        {
            m_QueuedEvent_InQueue = false;
            m_QueuedEvent = null;
            m_QueuedEntity = null;
        }

        private void InternalCheckQueuedClick(double frameMS)
        {
            if (m_QueuedEvent_InQueue)
            {
                m_QueuedEvent_DequeueAt -= frameMS;
                if (m_QueuedEvent_DequeueAt <= 0d)
                {
                    onInteractButton(m_QueuedEvent, m_QueuedEntity, m_QueuedEntityPosition);
                    ClearQueuedClick();
                }
            }
        }

        private void InternalQueueSingleClick(InputEventMouse e, AEntity overEntity, Vector2 overEntityPoint)
        {
            m_QueuedEvent_InQueue = true;
            m_QueuedEntity = overEntity;
            m_QueuedEntityPosition = overEntityPoint;
            m_QueuedEvent_DequeueAt = EngineVars.SecondsForDoubleClick * 1000d;
            m_QueuedEvent = e;
        }
        #endregion

        void InternalParseKeyboard(double frameMS)
        {
            // all names mode
            EngineVars.AllLabels = (UltimaEngine.Input.IsShiftDown && UltimaEngine.Input.IsCtrlDown);

            // Warmode toggle:
            if (UltimaEngine.Input.HandleKeyboardEvent(KeyboardEventType.Down, WinKeys.Tab, false, false, false))
            {
                if (UltimaVars.EngineVars.WarMode)
                    m_Model.Client.Send(new RequestWarModePacket(false));
                else
                    m_Model.Client.Send(new RequestWarModePacket(true));
            }

            // movement with arrow keys if the player is not moving and the mouse isn't moving the player.
            if (!ContinuousMouseMovementCheck)
            {
                Mobile m = (Mobile)EntityManager.GetPlayerObject();
                bool up = UltimaEngine.Input.IsKeyDown(WinKeys.Up);
                bool left = UltimaEngine.Input.IsKeyDown(WinKeys.Left);
                bool right = UltimaEngine.Input.IsKeyDown(WinKeys.Right);
                bool down = UltimaEngine.Input.IsKeyDown(WinKeys.Down);
                if (!m.IsMoving && (up | left | right | down))
                {
                    // Allow a short span of time (50ms) to get all the keys pressed.
                    // Otherwise, when moving diagonally, we would only get the first key
                    // in most circumstances and the second key a frame or two later - but
                    // too late, we would already be moving in the non-diagonal direction :(
                    m_PauseBeforeKeyboardMovementMS -= frameMS;
                    if (m_PauseBeforeKeyboardMovementMS <= 0)
                    {
                        if (up)
                        {
                            if (left)
                                m.PlayerMobile_Move(Direction.West);
                            else if (UltimaEngine.Input.IsKeyDown(WinKeys.Right))
                                m.PlayerMobile_Move(Direction.North);
                            else
                                m.PlayerMobile_Move(Direction.Up);
                        }
                        else if (down)
                        {
                            if (left)
                                m.PlayerMobile_Move(Direction.South);
                            else if (right)
                                m.PlayerMobile_Move(Direction.East);
                            else
                                m.PlayerMobile_Move(Direction.Down);
                        }
                        else
                        {
                            if (left)
                                m.PlayerMobile_Move(Direction.Left);
                            else if (right)
                                m.PlayerMobile_Move(Direction.Right);
                        }
                    }
                }
                else
                {
                    m_PauseBeforeKeyboardMovementMS = c_PauseBeforeKeyboardMovementMS;
                }
            }

            // debug variables.
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
