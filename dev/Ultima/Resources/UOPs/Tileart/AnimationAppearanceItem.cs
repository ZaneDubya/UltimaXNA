using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace UOReader {
	public class AnimationAppearanceItemSub {
		public byte subval;
		public AnimationAppearanceItemSub1 sub1;
		public AnimationAppearanceItemSub2[] sub2;
		public AnimationAppearanceItemSub() {
		}
	}

	public class AnimationAppearanceItemSub1 {
		public byte unk1;
		public int unk2;
		public AnimationAppearanceItemSub1() {
		}
	}
	
	public class AnimationAppearanceItemSub2 {
		public int unk1;
		public int unk2;
		public AnimationAppearanceItemSub2() {
		}
	}

	public class AnimationAppearanceItem {
		public int count4;
		public AnimationAppearanceItemSub[] sub;

		public AnimationAppearanceItem() {
		}

		public static AnimationAppearanceItem ReadAnimationAppearance(BinaryReader r) {
			AnimationAppearanceItem aa = new AnimationAppearanceItem();

			aa.count4 = r.ReadInt32();

			aa.sub = new AnimationAppearanceItemSub[aa.count4];
			for (int i = 0; i < aa.count4; ++i) {
				aa.sub[i] = new AnimationAppearanceItemSub();
				aa.sub[i].subval = r.ReadByte();
				
				if (aa.sub[i].subval != 0) {
					if (aa.sub[i].subval == 1) {
						aa.sub[i].sub1 = new AnimationAppearanceItemSub1();
						aa.sub[i].sub1.unk1 = r.ReadByte();
						aa.sub[i].sub1.unk2 = r.ReadInt32();
					}
				} else {
					uint subcount1 = r.ReadUInt32();
					aa.sub[i].sub2 = new AnimationAppearanceItemSub2[subcount1];

					for (int j = 0; j < aa.sub[i].sub2.Length; ++j) {
						aa.sub[i].sub2[j] = new AnimationAppearanceItemSub2();
						aa.sub[i].sub2[j].unk1 = r.ReadInt32();
						aa.sub[i].sub2[j].unk2 = r.ReadInt32();
					}
				}
			}
			return aa;
		}

		public static string PrintAnimationAppearance(AnimationAppearanceItem aa) {
			StringBuilder sb = new StringBuilder();

			sb.AppendLine("C4\t" + aa.count4);
			for (int i = 0; i < aa.count4; ++i) {
				sb.AppendLine("val\t" + aa.sub[i].subval + " animation appearance filter");
				if (aa.sub[i].subval != 0) {
					if (aa.sub[i].subval == 1) {
						sb.AppendLine("\t\t" + aa.sub[i].sub1.unk1 + " " + aa.sub[i].sub1.unk2);
					}
				} else {
					sb.AppendLine("\tSubC\t" + aa.sub[i].sub2.Length);
					for (int j = 0; j < aa.sub[i].sub2.Length; ++j) {
						sb.AppendLine("\t\t" + aa.sub[i].sub2[j].unk1 + " " + aa.sub[i].sub2[j].unk2);
					}
				}
			}

			return sb.ToString();
		}
	}
}
