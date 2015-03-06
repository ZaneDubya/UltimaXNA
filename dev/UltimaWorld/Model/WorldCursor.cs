using InterXLib.Input.Windows;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using UltimaXNA.Entity;
using UltimaXNA.Rendering;
using UltimaXNA.UltimaGUI;
using UltimaXNA.UltimaGUI.Controls;
using UltimaXNA.UltimaPackets.Client;

namespace UltimaXNA.UltimaWorld.Model
{
    class WorldCursor : UltimaCursor
    {
        private WorldModel m_Model;

        public WorldCursor(WorldModel model)
            : base()
        {
            m_Model = model;
            InternalRegisterInteraction();
        }

        public override void Update()
        {
            if (IsHoldingItem && UltimaEngine.UserInterface.IsMouseOverUI && UltimaEngine.Input.HandleMouseEvent(MouseEvent.Up, MouseButton.Left))
            {
                Control target = UltimaEngine.UserInterface.MouseOverControl;
                // attempt to drop the item onto an interface. The only acceptable targets for dropping items are:
                // 1. ItemGumplings that represent containers (like a bag icon)
                // 2. Gumps that represent open Containers (GumpPicContainers, e.g. an open GumpPic of a chest)
                // 3. Paperdolls for my character.
                // 4. Backpack gumppics (seen in paperdolls).
                if ((target is ItemGumpling && ((ItemGumpling)target).Item.ItemData.IsContainer) ||
                    (target is GumpPic))
                {
                    DropItemToContainer((Container)((ItemGumpling)target).Item);
                }
                else if (target is GumpPicContainer)
                {
                    int x = (int)UltimaEngine.Input.MousePosition.X - HeldItemOffset.X - (target.X + target.Owner.X);
                    int y = (int)UltimaEngine.Input.MousePosition.Y - HeldItemOffset.Y - (target.Y + target.Owner.Y);
                    DropItemToContainer((Container)((GumpPicContainer)target).Item, x, y);
                }
                else if (target is ItemGumplingPaperdoll || (target is GumpPic && ((GumpPic)target).IsPaperdoll))
                {
                    WearHeldItem();
                }
                else if (target is GumpPicBackpack)
                {
                    DropItemToContainer((Container)((GumpPicBackpack)target).BackpackItem);
                }
            }
            base.Update(); // reset image
        }

        public override void Dispose()
        {
            base.Dispose();
        }

        protected override void BeforeDraw(SpriteBatchUI spritebatch, Point2D position)
        {

        }

        // ======================================================================
        // Interaction routines
        // ======================================================================

        private void InternalRegisterInteraction()
        {
            UltimaInteraction.OnPickupItem += PickUpItem;
            UltimaInteraction.OnClearHolding += ClearHolding;
        }

        private void InternalUnregisterInteraction()
        {
            UltimaInteraction.OnPickupItem -= PickUpItem;
            UltimaInteraction.OnClearHolding -= ClearHolding;
        }

        // ======================================================================
        // Pickup/Drop/Hold item routines
        // ======================================================================

        private Item m_HeldItem = null;
        internal Item HeldItem
        {
            get { return m_HeldItem; }
        }

        private Point2D m_HeldItemOffset = Point2D.Zero;
        internal Point2D HeldItemOffset
        {
            get { return m_HeldItemOffset; }
        }

        internal bool IsHoldingItem
        {
            get { return m_HeldItem != null; }
        }

        private void PickUpItem(Item item, int x, int y)
        {
            // make sure we can pick up the item before actually picking it up!
            if (item.PickUp())
            {
                // let the server know we're picking up the item. If the server says we can't pick it up, it will send us a cancel pick up message.
                // TEST: what if we can pick something up and drop it in our inventory before the server has a chance to respond?
                m_Model.Client.Send(new PickupItemPacket(item.Serial, item.Amount));

                // if the item is in a container, remove it from that container.
                if (item.Parent != null)
                {
                    if (item.Parent is Container)
                        ((Container)item.Parent).RemoveItem(item.Serial);
                    item.Parent = null;
                }

                // set our local holding item variables.
                m_HeldItem = item;
                m_HeldItemOffset = new Point2D(x, y);
            }
        }

        private void DropItemToContainer(Container container)
        {
            // get random coords and drop the item there.
            Rectangle bounds = UltimaData.ContainerData.GetData(container.ItemID).Bounds;
            int x = Utility.RandomValue(bounds.Left, bounds.Right);
            int y = Utility.RandomValue(bounds.Top, bounds.Bottom);
            DropItemToContainer(container, x, y);
        }

        private void DropItemToContainer(Container container, int x, int y)
        {
            Rectangle containerBounds = UltimaData.ContainerData.GetData(container.ItemID).Bounds;
            Texture2D itemTexture = UltimaData.ArtData.GetStaticTexture(HeldItem.DisplayItemID);
            if (x < containerBounds.Left) x = containerBounds.Left;
            if (x > containerBounds.Right - itemTexture.Width) x = containerBounds.Right - itemTexture.Width;
            if (y < containerBounds.Top) y = containerBounds.Top;
            if (y > containerBounds.Bottom - itemTexture.Height) y = containerBounds.Bottom - itemTexture.Height;
            m_Model.Client.Send(new DropItemPacket(HeldItem.Serial, (ushort)x, (ushort)y, 0, 0, container.Serial));
            ClearHolding();
        }

        private void WearHeldItem()
        {
            m_Model.Client.Send(new DropToLayerPacket(HeldItem.Serial, 0x00, EntityManager.MySerial));
            ClearHolding();
        }

        private void ClearHolding()
        {
            m_HeldItem = null;
        }
    }
}
