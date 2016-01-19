using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace UOReader {
	public class RadarColItem {
		public byte R;
		public byte G;
		public byte B;
		public byte A;

		public RadarColItem() {
		}

		public static RadarColItem ReadRadarCol(BinaryReader r) {
			RadarColItem rc = new RadarColItem();

			rc.R = r.ReadByte();
			rc.G = r.ReadByte();
			rc.B = r.ReadByte();
			rc.A = r.ReadByte();

			return rc;
		}

		public static string PrintRadarColInfo(RadarColItem rc) {
			StringBuilder sb = new StringBuilder();
			sb.AppendLine("RadarCol\n\tR" + rc.R + " G" + rc.G + " B" + rc.B + " A" + rc.A);
			sb.AppendLine();
			return sb.ToString();
		}
	}
}
