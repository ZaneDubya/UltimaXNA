namespace UltimaXNA.Ultima.Login.Data {
    class CreateCharacterData {
        public bool HasSkillData;
        public int[] Attributes = new int[3];
        public int[] SkillIndexes = new int[3];
        public int[] SkillValues = new int[3];

        public bool HasAppearanceData;
        public string Name;
        public int Gender, HairStyleID, FacialHairStyleID;
        public int SkinHue, HairHue, FacialHairHue;
        public int ShirtColor, PantsColor;
    }
}
