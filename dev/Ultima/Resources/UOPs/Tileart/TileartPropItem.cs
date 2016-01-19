using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace UOReader {
	public class TileartPropItem {
		public TileArtProperties prop;
		public int value;
		public TileartPropItem() {

		}

		public static TileartPropItem ReadProp(BinaryReader r) {
			TileartPropItem p = new TileartPropItem();
			p.prop = (TileArtProperties)r.ReadByte();
			p.value = r.ReadInt32();
			return p;
		}
	}
}
