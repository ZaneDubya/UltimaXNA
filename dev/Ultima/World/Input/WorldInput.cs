/***************************************************************************
 *   WorldInput.cs
 *   Copyright (c) 2015 UltimaXNA Development Team
 *   
 *   This program is free software; you can redistribute it and/or modify
 *   it under the terms of the GNU General Public License as published by
 *   the Free Software Foundation; either version 3 of the License, or
 *   (at your option) any later version.
 *
 ***************************************************************************/
#region usings
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using UltimaXNA.Configuration;
using UltimaXNA.Core.Input;
using UltimaXNA.Core.Input.Windows;
using UltimaXNA.Core.Network;
using UltimaXNA.Core.UI;
using UltimaXNA.Ultima.Data;
using UltimaXNA.Ultima.Network.Client;
using UltimaXNA.Ultima.UI.Controls;
using UltimaXNA.Ultima.World.Entities;
using UltimaXNA.Ultima.World.Entities.Items;
using UltimaXNA.Ultima.World.Entities.Mobiles;
using UltimaXNA.Ultima.UI.WorldGumps;
#endregion

namespace UltimaXNA.Ultima.World.Input
{
    /// <summary>
    /// Handles all the mouse input when the mouse is over the world.
    /// </summary>
    internal class WorldInput
    {
        private const double c_PauseBeforeMouseMovementMS = 105d;
        private const double c_PauseBeforeKeyboardFacingMS = 55d; // a little more than three frames @ 60fps.
        private const double c_PauseBeforeKeyboardMovementMS = 125d; // a little more than seven frames @ 60fps.
        private bool m_ContinuousMouseMovementCheck;

        private AEntity m_DraggingEntity;

        private INetworkClient m_Network;
        private UserInterfaceService m_UserInterface;
        private InputManager m_Input;

        // keyboard movement variables.
        private double m_PauseBeforeKeyboardMovementMS;
        private double m_TimeSinceMovementButtonPressed;
        private Vector2 m_dragOffset;

        protected WorldModel World
        {
            get;
            private set;
        }

        public WorldInput(WorldModel world)
        {
            World = world;

            m_Network = ServiceRegistry.GetService<INetworkClient>();
            m_UserInterface = ServiceRegistry.GetService<UserInterfaceService>();
            m_Input = ServiceRegistry.GetService<InputManager>();

            MousePick = new MousePicking();
        }

        public MousePicking MousePick
        {
            get;
            private set;
        }

        public bool ContinuousMouseMovementCheck
        {
            get { return m_ContinuousMouseMovementCheck; }
            set
            {
                if(m_ContinuousMouseMovementCheck != value)
                {
                    m_ContinuousMouseMovementCheck = value;
                    if(m_ContinuousMouseMovementCheck)
                    {
                        m_UserInterface.AddInputBlocker(this);
                    }
                    else
                    {
                        m_UserInterface.RemoveInputBlocker(this);
                    }
                }
            }
        }

        public bool IsMouseOverUI
        {
            get
            {
                if (m_UserInterface.IsMouseOverUI)
                {
                    AControl over = m_UserInterface.MouseOverControl;
                    return !(over is WorldViewport);
                }
                return false;
            }
        }

        public bool IsMouseOverWorld
        {
            get
            {
                if (m_UserInterface.IsMouseOverUI)
                {
                    AControl over = m_UserInterface.MouseOverControl;
                    return (over is WorldViewport);
                }
                return false;
            }
        }

        public Point MouseOverWorldPosition
        {
            get
            {
                WorldViewport world = ServiceRegistry.GetService<WorldViewport>();
                Point mouse = new Point(m_Input.MousePosition.X - world.ScreenX, m_Input.MousePosition.Y - world.ScreenY);
                return mouse;
            }
        }

        public void Dispose()
        {
            m_UserInterface.RemoveInputBlocker(this);
        }

        public void Update(double frameMS)
        {
            if (WorldModel.IsInWorld && !m_UserInterface.IsModalControlOpen && m_Network.IsConnected)
            {
                // always parse keyboard. (Is it possible there are some situations in which keyboard input is blocked???)
                InternalParseKeyboard(frameMS);

                // In all cases, where we are moving and the move button was released, stop moving.
                if(ContinuousMouseMovementCheck &&
                   m_Input.HandleMouseEvent(MouseEvent.Up, Settings.World.Mouse.MovementButton))
                {
                    ContinuousMouseMovementCheck = false;
                }

                // If 1. The mouse is over the world (not over UI) and
                //    2. The cursor is not blocking input, then interpret mouse input.
                if(IsMouseOverWorld && !World.Cursor.IsHoldingItem)
                {
                    InternalParseMouse(frameMS);
                }

                // PickType is the kind of objects that will show up as the 'MouseOverObject'
                if(IsMouseOverWorld)
                {
                    MousePick.PickOnly = PickType.PickEverything;
                    MousePick.Position = MouseOverWorldPosition;
                }
                else
                {
                    MousePick.PickOnly = PickType.PickNothing;
                }
                
                doMouseMovement(frameMS);
            }
        }

        private void doMouseMovement(double frameMS)
        {
            // if the move button is pressed, change facing and move based on mouse cursor direction.
            if(ContinuousMouseMovementCheck)
            {
                ResolutionConfig resolution = Settings.World.PlayWindowGumpResolution;
                Point centerScreen = new Point(resolution.Width / 2, resolution.Height / 2);
                Direction mouseDirection = DirectionHelper.DirectionFromPoints(centerScreen, MouseOverWorldPosition);

                m_TimeSinceMovementButtonPressed += frameMS;

                if(m_TimeSinceMovementButtonPressed >= c_PauseBeforeMouseMovementMS)
                {
                    // Get the move direction.
                    Direction moveDirection = mouseDirection;

                    // add the running flag if the mouse cursor is far enough away from the center of the screen.
                    float distanceFromCenterOfScreen = Utility.DistanceBetweenTwoPoints(centerScreen, MouseOverWorldPosition);

                    if (distanceFromCenterOfScreen >= 150.0f || Settings.World.AlwaysRun)
                    {
                        moveDirection |= Direction.Running;
                    }

                    // Tell the player to Move.
                    Mobile m = (Mobile)WorldModel.Entities.GetPlayerEntity();
                    m.PlayerMobile_Move(moveDirection);
                }
                else
                {
                    // Get the move direction.
                    Direction facing = mouseDirection;

                    Mobile m = (Mobile)WorldModel.Entities.GetPlayerEntity();
                    if(m.Facing != facing)
                    {
                        // Tell the player entity to change facing to this direction.
                        m.PlayerMobile_ChangeFacing(facing);
                        // reset the time since the mouse cursor was pressed - allows multiple facing changes.
                        m_TimeSinceMovementButtonPressed = 0d;
                    }
                }
            }
            else
            {
                m_TimeSinceMovementButtonPressed = 0d;
                // Tell the player to stop moving.
                Mobile m = (Mobile)WorldModel.Entities.GetPlayerEntity();
                m.PlayerMobile_Move(Direction.Nothing);
            }
        }

        private void doKeyboardMovement(double frameMS)
        {
            if(m_PauseBeforeKeyboardMovementMS < c_PauseBeforeKeyboardMovementMS)
            {
                if(m_Input.HandleKeyboardEvent(KeyboardEventType.Up, WinKeys.Up, false, false, false))
                {
                    m_PauseBeforeKeyboardMovementMS = 0;
                }
                if(m_Input.HandleKeyboardEvent(KeyboardEventType.Up, WinKeys.Down, false, false, false))
                {
                    m_PauseBeforeKeyboardMovementMS = 0;
                }
                if(m_Input.HandleKeyboardEvent(KeyboardEventType.Up, WinKeys.Left, false, false, false))
                {
                    m_PauseBeforeKeyboardMovementMS = 0;
                }
                if(m_Input.HandleKeyboardEvent(KeyboardEventType.Up, WinKeys.Right, false, false, false))
                {
                    m_PauseBeforeKeyboardMovementMS = 0;
                }
            }

            Mobile m = (Mobile)WorldModel.Entities.GetPlayerEntity();
            bool up = m_Input.IsKeyDown(WinKeys.Up);
            bool left = m_Input.IsKeyDown(WinKeys.Left);
            bool right = m_Input.IsKeyDown(WinKeys.Right);
            bool down = m_Input.IsKeyDown(WinKeys.Down);
            if(up | left | right | down)
            {
                // Allow a short span of time (50ms) to get all the keys pressed.
                // Otherwise, when moving diagonally, we would only get the first key
                // in most circumstances and the second key a frame or two later - but
                // too late, we would already be moving in the non-diagonal direction :(
                m_PauseBeforeKeyboardMovementMS += frameMS;
                if(m_PauseBeforeKeyboardMovementMS >= c_PauseBeforeKeyboardFacingMS)
                {
                    Direction facing = Direction.Up;
                    if(up)
                    {
                        if(left)
                        {
                            facing = Direction.West;
                        }
                        else if(m_Input.IsKeyDown(WinKeys.Right))
                        {
                            facing = Direction.North;
                        }
                        else
                        {
                            facing = Direction.Up;
                        }
                    }
                    else if(down)
                    {
                        if(left)
                        {
                            facing = Direction.South;
                        }
                        else if(right)
                        {
                            facing = Direction.East;
                        }
                        else
                        {
                            facing = Direction.Down;
                        }
                    }
                    else
                    {
                        if(left)
                        {
                            facing = Direction.Left;
                        }
                        else if(right)
                        {
                            facing = Direction.Right;
                        }
                    }

                    // only send messages if we're not moving.
                    if(!m.IsMoving)
                    {
                        if(m_PauseBeforeKeyboardMovementMS >= c_PauseBeforeKeyboardMovementMS)
                        {
                            m.PlayerMobile_Move(facing);
                        }
                        else
                        {
                            if(m.Facing != facing)
                            {
                                m.PlayerMobile_ChangeFacing(facing);
                            }
                        }
                    }
                }
            }
            else
            {
                m_PauseBeforeKeyboardMovementMS = 0;
            }
        }

        private void onMoveButton(InputEventMouse e)
        {
            if(e.EventType == MouseEvent.Down)
            {
                // keep moving as long as the move button is down.
                ContinuousMouseMovementCheck = true;
            }
            else if(e.EventType == MouseEvent.Up)
            {
                // If the movement mouse button has been released, stop moving.
                ContinuousMouseMovementCheck = false;
            }

            e.Handled = true;
        }

        private void onInteractButton(InputEventMouse e, AEntity overEntity, Vector2 overEntityPoint)
        {
            if(e.EventType == MouseEvent.Down)
            {
                // prepare to pick this item up.
                m_DraggingEntity = overEntity;
                m_dragOffset = overEntityPoint;
            }
            else if(e.EventType == MouseEvent.Click)
            {
                if(overEntity is Ground)
                {
                    // no action.
                }
                else if(overEntity is StaticItem)
                {
                    // pop up name of item.
                    overEntity.AddOverhead(MessageTypes.Label, "<outline>" + overEntity.Name, 0, 0);
                    WorldModel.Statics.AddStaticThatNeedsUpdating(overEntity as StaticItem);
                }
                else if(overEntity is Item)
                {
                    // request context menu
                    World.Interaction.SingleClick(overEntity);
                }
                else if(overEntity is Mobile)
                {
                    // request context menu
                    World.Interaction.SingleClick(overEntity);
                }
            }
            else if(e.EventType == MouseEvent.DoubleClick)
            {
                if(overEntity is Ground)
                {
                    // no action.
                }
                else if(overEntity is StaticItem)
                {
                    // no action.
                }
                else if(overEntity is Item)
                {
                    // request context menu
                    World.Interaction.DoubleClick(overEntity);
                }
                else if(overEntity is Mobile)
                {
                    // Send double click packet.
                    // Set LastTarget == targeted Mobile.
                    // If in WarMode, set Attacking == true.
                    World.Interaction.DoubleClick(overEntity);
                    World.Interaction.LastTarget = overEntity.Serial;

                    if (WorldModel.Entities.GetPlayerEntity().Flags.IsWarMode)
                    {
                        m_Network.Send(new AttackRequestPacket(overEntity.Serial));
                    }
                }
            }
            else if(e.EventType == MouseEvent.DragBegin)
            {
                if(overEntity is Ground)
                {
                    // no action.
                }
                else if(overEntity is StaticItem)
                {
                    // no action.
                }
                else if(overEntity is Item)
                {
                    // attempt to pick up item.
                    World.Interaction.PickupItem((Item)overEntity, new Point((int)m_dragOffset.X, (int)m_dragOffset.Y));
                }
                else if(overEntity is Mobile)
                {
                    // drag off a status gump for this mobile.
                    MobileHealthTrackerGump gump = new MobileHealthTrackerGump(overEntity as Mobile);
                    m_UserInterface.AddControl(gump, e.X - 77, e.Y - 30);
                    m_UserInterface.AttemptDragControl(gump, new Point(e.X, e.Y), true);
                }
            }

            e.Handled = true;
        }

        private void InternalParseMouse(double frameMS)
        {
            List<InputEventMouse> events = m_Input.GetMouseEvents();
            foreach (InputEventMouse e in events)
            {
                if (e.Button == Settings.World.Mouse.MovementButton)
                {
                    onMoveButton(e);
                }
                else if (e.Button == Settings.World.Mouse.InteractionButton)
                {
                    if(e.EventType == MouseEvent.Click)
                    {
                        InternalQueueSingleClick(e, MousePick.MouseOverObject, MousePick.MouseOverObjectPoint);
                        continue;
                    }
                    if(e.EventType == MouseEvent.DoubleClick)
                    {
                        ClearQueuedClick();
                    }
                    onInteractButton(e, MousePick.MouseOverObject, MousePick.MouseOverObjectPoint);
                }
            }

            InternalCheckQueuedClick(frameMS);
        }

        private void InternalParseKeyboard(double frameMS)
        {
            // all names mode
            WorldView.AllLabels = (m_Input.IsShiftDown && m_Input.IsCtrlDown);

            // Warmode toggle:
            if(m_Input.HandleKeyboardEvent(KeyboardEventType.Down, WinKeys.Tab, false, false, false))
            {
                m_Network.Send(new RequestWarModePacket(!WorldModel.Entities.GetPlayerEntity().Flags.IsWarMode));
            }

            // movement with arrow keys if the player is not moving and the mouse isn't moving the player.
            if(!ContinuousMouseMovementCheck)
            {
                doKeyboardMovement(frameMS);
            }

            // debug variables.
            if(m_Input.HandleKeyboardEvent(KeyboardEventType.Press, WinKeys.D, false, false, true))
            {
                if(!Settings.Debug.ShowDataRead)
                {
                    Settings.Debug.ShowDataRead = true;
                }
                else
                {
                    if(!Settings.Debug.ShowDataReadBreakdown)
                    {
                        Settings.Debug.ShowDataReadBreakdown = true;
                    }
                    else
                    {
                        Settings.Debug.ShowDataRead = false;
                        Settings.Debug.ShowDataReadBreakdown = false;
                    }
                }
            }

            // FPS limiting
            if(m_Input.HandleKeyboardEvent(KeyboardEventType.Press, WinKeys.F, false, true, false))
            {
                Settings.Game.IsFixedTimeStep = !Settings.Game.IsFixedTimeStep;
            }

            // Display FPS
            if(m_Input.HandleKeyboardEvent(KeyboardEventType.Press, WinKeys.F, false, true, true))
            {
                Settings.Debug.ShowFps = !Settings.Debug.ShowFps;
            }

            // Mouse enable / disable
            if(m_Input.HandleKeyboardEvent(KeyboardEventType.Press, WinKeys.M, false, true, false))
            {
                Settings.World.Mouse.IsEnabled = !Settings.World.Mouse.IsEnabled;
            }
        }

        #region QueuedClicks

        // Legacy Client waits about 0.5 seconds before sending a click event when you click in the world.
        // This allows time for the player to potentially double-click on an object.
        // If the player does so, this will cancel the single-click event.
        private AEntity m_QueuedEntity;
        private Vector2 m_QueuedEntityPosition;
        private InputEventMouse m_QueuedEvent;
        private double m_QueuedEvent_DequeueAt;
        private bool m_QueuedEvent_InQueue;

        private void ClearQueuedClick()
        {
            m_QueuedEvent_InQueue = false;
            m_QueuedEvent = null;
            m_QueuedEntity = null;
        }

        private void InternalCheckQueuedClick(double frameMS)
        {
            if(m_QueuedEvent_InQueue)
            {
                m_QueuedEvent_DequeueAt -= frameMS;
                if(m_QueuedEvent_DequeueAt <= 0d)
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
            m_QueuedEvent_DequeueAt = Settings.World.Mouse.DoubleClickMS;
            m_QueuedEvent = e;
        }

        #endregion
    }
}