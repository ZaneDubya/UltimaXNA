using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace UOReader {
	public class StackAlias {
		public int amount;
		public int amountid;
		public StackAlias() {
		}

		public static StackAlias ReadStackAlias(BinaryReader r) {
			StackAlias s = new StackAlias();
			s.amount = r.ReadInt32();
			s.amountid = r.ReadInt32();
			return s;
		}

		public static string PrintStackAlias(StackAlias s,int i) {
			StringBuilder sb = new StringBuilder();

			sb.AppendLine("#" + i + "\tAmount: " +  s.amount + " - ID: " + s.amountid);

			return sb.ToString();
		}
	}
}
