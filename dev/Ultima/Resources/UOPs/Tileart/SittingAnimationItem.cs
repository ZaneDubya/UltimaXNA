using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace UOReader {
	public class SittingAnimationItem {
		public byte HasSittingAnimation;
		public int unk1;
		public int unk2;
		public int unk3;
		public int unk4;

		public SittingAnimationItem() {
		}
		public static SittingAnimationItem ReadSittingAnimation(BinaryReader r) {
			SittingAnimationItem sa = new SittingAnimationItem();
			sa.HasSittingAnimation = r.ReadByte();

			if (sa.HasSittingAnimation != 0)
			{
				sa.unk1 = r.ReadInt32();
				sa.unk2 = r.ReadInt32();
				sa.unk3 = r.ReadInt32();
				sa.unk4 = r.ReadInt32();
			}
			return sa;
		}
		public static string PrintSittingAnimation(SittingAnimationItem sa) {
			StringBuilder sb = new StringBuilder();
			sb.AppendLine("ChairRelated\t" + sa.HasSittingAnimation + " sitting data count(0:1)");
			if (sa.HasSittingAnimation != 0) {
				sb.AppendLine("Unk\t" + sa.unk1 + " " + sa.unk2 + " " + sa.unk3 + " " + sa.unk4);
			}
			return sb.ToString();
		}
	}
}
