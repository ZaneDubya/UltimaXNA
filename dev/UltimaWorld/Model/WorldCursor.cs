using UltimaXNA.Entity;
using UltimaXNA.Rendering;
using UltimaXNA.UltimaGUI;
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
            base.Update();
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
            UltimaInteraction.OnPickupItem += OnPickUpItem;
            UltimaInteraction.OnClearHolding += OnClearHolding;
        }

        private void InternalUnregisterInteraction()
        {
            UltimaInteraction.OnPickupItem -= OnPickUpItem;
            UltimaInteraction.OnClearHolding -= OnClearHolding;
        }

        // ======================================================================
        // Pickup/Drop/Hold item routines
        // ======================================================================

        private Item m_HoldingItem = null;
        private Point2D m_HoldingItemOffset = Point2D.Zero;

        private bool IsHoldingItem
        {
            get { return m_HoldingItem != null; }
        }

        private void OnPickUpItem(Item item, int x, int y)
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
                m_HoldingItem = item;
                m_HoldingItemOffset = new Point2D(x, y);
            }
        }

        private void OnClearHolding()
        {
            m_HoldingItem = null;
        }
    }
}
