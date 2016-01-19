using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace UOReader {
	public class EffectItem {
		public EffectsID effectID;

		public string effectData;

		public EffectItem() {
		}

		public static EffectItem ReadEffect(BinaryReader r) {
			EffectItem e = new EffectItem();

			e.effectID = (EffectsID)r.ReadInt32();

			/*StringBuilder sb = new StringBuilder();
			switch (e.effectID) {
				case EffectsID.EFFECT00: EffectsCollection.Effect0(r, sb); break;
				case EffectsID.EFFECT01: EffectsCollection.Effect1(r, sb); break;
				case EffectsID.EFFECT02: EffectsCollection.Effect2(r, sb); break;
				case EffectsID.EFFECT07: EffectsCollection.Effect7(r, sb); break;
				case EffectsID.EFFECT10: EffectsCollection.Effect10(r, sb); break;
				case EffectsID.EFFECT11: EffectsCollection.Effect11(r, sb); break;
				case EffectsID.EFFECT12: EffectsCollection.Effect12(r, sb); break;
				case EffectsID.EFFECT15: EffectsCollection.Effect15(r, sb); break;
				case EffectsID.EFFECT16: EffectsCollection.Effect16(r, sb); break;
				case EffectsID.EFFECT17: EffectsCollection.Effect17(r, sb); break;
				default: sb.Append("not implemented yet"); break;
			}
			e.effectData = sb.ToString();*/
			return e;
		}

		public static string PrintEffect(EffectItem e) {
			StringBuilder sb = new StringBuilder();
			sb.AppendLine(e.effectID.ToString());
			sb.AppendLine(e.effectData);

			return sb.ToString();
		}
	}
}
