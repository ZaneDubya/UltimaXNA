using InterXLib.Input.Windows;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using UltimaXNA.Entity;
using UltimaXNA.UltimaData;
using UltimaXNA.UltimaPackets.Client;

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
        bool _ContinuousMoveCheck = false;
        const int _TimeHoveringBeforeTipMS = 1000;

        // make sure we drag the correct object variables
        Vector2 _dragOffset;
        AMapObject _dragObject;

        // Are we asking for a target?
        TargetTypes _TargettingType = TargetTypes.Nothing;
        bool isTargeting
        {
            get
            {
                if (_TargettingType == TargetTypes.Nothing)
                    return false;
                else
                    return true;
            }
        }

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
                parseKeyboard();
                parseMouse();

                // PickType is the kind of objects that will show up as the 'MouseOverObject'
                if (UltimaEngine.UserInterface.IsMouseOverUI)
                    IsometricRenderer.PickType = PickTypes.PickNothing;
                else
                    IsometricRenderer.PickType = PickTypes.PickEverything;

                // Set the cursor direction
                UltimaVars.EngineVars.CursorDirection = Utility.DirectionFromVectors(new Vector2(400, 300), UltimaEngine.Input.MousePosition.ToVector2());

                // Show a popup tip if we have hovered over this item for X seconds.
                if (UltimaEngine.Input.MouseStationaryTimeMS >= _TimeHoveringBeforeTipMS)
                    if (IsometricRenderer.MouseOverObject != null)
                        createHoverLabel(IsometricRenderer.MouseOverObject);

                // Changed to leverage movementFollowsMouse interface option -BERT
                if (_ContinuousMoveCheck)
                    doMovement();

                // Show our target's name
                createHoverLabel(UltimaVars.EngineVars.LastTarget);
            }
        }

        void doMovement()
        {
            // Get the move direction and add the Running offset if the Cursor is far enough away.
            Direction moveDirection = UltimaVars.EngineVars.CursorDirection;
            float distanceFromCenterOfScreen = Vector2.Distance(UltimaEngine.Input.MousePosition.ToVector2(), new Vector2(400, 300));
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
                _ContinuousMoveCheck = true;
            }
            else if (e.EventType == MouseEvent.Up)
            {
                // If the movement mouse button has been released, stop moving.
                _ContinuousMoveCheck = false;
            }

            e.Handled = true;
        }

        void onTargetingButton(AMapObject worldObject)
        {
            if (worldObject == null)
                return;

            // If isTargeting is true, then the target cursor is active and we are waiting for the player to target something.
            // If not, then we are just clicking the mouse and we need to find out if something is under the mouse cursor.
            switch (_TargettingType)
            {
                case TargetTypes.Object:
                    // Select Object
                    IsometricRenderer.PickType = PickTypes.PickStatics | PickTypes.PickObjects;
                    mouseTargetingEventObject(worldObject);
                    break;
                case TargetTypes.Position:
                    // Select X, Y, Z
                    IsometricRenderer.PickType = PickTypes.PickStatics | PickTypes.PickObjects;
                    mouseTargetingEventObject(worldObject); // mouseTargetingEventXYZ(mouseOverObject);
                    break;
                case TargetTypes.MultiPlacement:
                    // select X, Y, Z
                    mouseTargetingEventXYZ(worldObject);
                    break;
                default:
                    throw new Exception("Unknown targetting type!");
            }
        }

        void onInteractButton(InputEventMouse e)
        {
            AMapObject overObject = (e.EventType == MouseEvent.DragBegin) ? _dragObject : IsometricRenderer.MouseOverObject;
            Vector2 overObjectOffset = (e.EventType == MouseEvent.DragBegin) ? _dragOffset : IsometricRenderer.MouseOverObjectPoint;

            if (e.EventType == MouseEvent.Down)
            {
                _dragObject = overObject;
                _dragOffset = overObjectOffset;
            }

            if (overObject == null)
                return;

            if (isTargeting && e.EventType == MouseEvent.Click)
            {
                // Special case: targeting
                onTargetingButton(overObject);
            }
            else if (UltimaEngine.UltimaUI.Cursor.IsHolding && e.EventType == MouseEvent.Up)
            {
                // Special case: if we're holding anything, drop it.
                checkDropItem();
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
                    Item entity = (Item)overObject.OwnerEntity;
                    // single click = tool tip
                    // double click = use / open
                    // click and drag = pick up
                    switch (e.EventType)
                    {
                        case MouseEvent.Click:
                            // tool tip
                            UltimaInteraction.SingleClick(entity);
                            break;
                        case MouseEvent.DoubleClick:
                            UltimaInteraction.DoubleClick(entity);
                            break;
                        case MouseEvent.DragBegin:
                            UltimaInteraction.PickUpItem(entity, (int)overObjectOffset.X, (int)overObjectOffset.Y);
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

        void checkDropItem()
        {
            AMapObject mouseoverObject = IsometricRenderer.MouseOverObject;

            if (mouseoverObject != null)
            {
                int x, y, z;

                if (mouseoverObject is MapObjectMobile || mouseoverObject is MapObjectCorpse)
                {
                    // special case, attempt to give this item.
                    return;
                }
                else if (mouseoverObject is MapObjectItem || mouseoverObject is MapObjectStatic)
                {
                    x = (int)mouseoverObject.Position.X;
                    y = (int)mouseoverObject.Position.Y;
                    z = (int)mouseoverObject.Z;
                    if (mouseoverObject is MapObjectStatic)
                    {
                        ItemData itemData = UltimaData.TileData.ItemData[mouseoverObject.ItemID & 0x3FFF];
                        z += itemData.Height;
                    }
                    else if (mouseoverObject is MapObjectItem)
                    {
                        z += UltimaData.TileData.ItemData[mouseoverObject.ItemID].Height;
                    }
                }
                else if (mouseoverObject is MapObjectGround)
                {
                    x = (int)mouseoverObject.Position.X;
                    y = (int)mouseoverObject.Position.Y;
                    z = (int)mouseoverObject.Z;
                }
                else
                {
                    // over text?
                    return;
                }
                UltimaInteraction.DropItemToWorld(UltimaEngine.UltimaUI.Cursor.HoldingItem, x, y, z);
            }
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
            // If the mouse is over the world, then interpret mouse input:
            if (!UltimaEngine.UserInterface.IsMouseOverUI)
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
        }

        void parseKeyboard()
        {
            List<InputEventKeyboard> events = UltimaEngine.Input.GetKeyboardEvents();
            foreach (InputEventKeyboard e in events)
            {
                // If we are targeting, cancel the target cursor if we hit escape.
                if (isTargeting)
                    if ((e.EventType == KeyboardEventType.Press) && e.KeyCode == WinKeys.Escape)
                        mouseTargetingCancel();

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

        public void MouseTargeting(TargetTypes targetingType, int cursorID)
        {
            setTargeting(targetingType);
        }

        void setTargeting(TargetTypes targetingType)
        {
            _TargettingType = targetingType;
            // Set the UserInterface's cursor to a targetting cursor. If multi, tell the cursor which multi.
            UltimaEngine.UltimaUI.Cursor.IsTargeting = true;
            // Stop continuous movement.
            _ContinuousMoveCheck = false;
        }

        void clearTargeting()
        {
            // Clear our target cursor.
            _TargettingType = TargetTypes.Nothing;
            UltimaEngine.UltimaUI.Cursor.IsTargeting = false;
        }

        void mouseTargetingCancel()
        {
            // Send the cancel target message back to the server.
            m_Model.Client.Send(new TargetCancelPacket());
            clearTargeting();
        }

        void mouseTargetingEventXYZ(AMapObject selectedObject)
        {
            // Send the targetting event back to the server!
            int modelNumber = 0;
            Type type = selectedObject.GetType();
            if (type == typeof(MapObjectStatic))
            {
                modelNumber = selectedObject.ItemID;
            }
            else
            {
                modelNumber = 0;
            }
            // Send the target ...
            m_Model.Client.Send(new TargetXYZPacket((short)selectedObject.Position.X, (short)selectedObject.Position.Y, (short)selectedObject.Z, (ushort)modelNumber));
            // ... and clear our targeting cursor.
            clearTargeting();
        }

        void mouseTargetingEventObject(AMapObject selectedObject)
        {
            // If we are passed a null object, keep targeting.
            if (selectedObject == null)
                return;
            Serial serial = selectedObject.OwnerSerial;
            // Send the targetting event back to the server
            if (serial.IsValid)
            {
                m_Model.Client.Send(new TargetObjectPacket(serial));
            }
            else
            {
                int modelNumber = 0;
                Type type = selectedObject.GetType();
                if (type == typeof(MapObjectStatic))
                {
                    modelNumber = selectedObject.ItemID;
                }
                else
                {
                    modelNumber = 0;
                }
                m_Model.Client.Send(new TargetXYZPacket((short)selectedObject.Position.X, (short)selectedObject.Position.Y, (short)selectedObject.Z, (ushort)modelNumber));
            }

            // Clear our target cursor.
            clearTargeting();
        }
    }
}
