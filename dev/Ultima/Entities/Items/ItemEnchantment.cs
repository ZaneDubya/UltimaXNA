namespace UltimaXNA.UltimaEntities
{
    struct ItemEnchantment
    {
        int EnchantmentType;
        int EnchantedBySerial;

        ItemEnchantment(int nType, int nBySerial)
        {
            EnchantmentType = nType;
            EnchantedBySerial = nBySerial;
        }
    }
}