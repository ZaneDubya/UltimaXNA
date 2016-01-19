using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace UOReader {
	public class TextureImageInfo{
		public uint textureIDX; //String dictionary index of the texture path
		public byte unk4;
		public float repetition;//How many times the texture should be replicated in a single image
		public int unk6;
		public int unk7;
		public TextureImageInfo() {
		}
	}

	public class TextureInfo {
		public byte texturePresent;
		public byte unk1;
		public int shaderNameIDX;
		public byte texturesCount;
		public TextureImageInfo[] texturesArray;
		public uint count2;
		public int[] unk8;
		public uint count3;
		public float[] unk9;

		private TextureInfo() {
		}

		public static TextureInfo readTextureInfo(BinaryReader r) {
			TextureInfo t = new TextureInfo();
			t.texturePresent = r.ReadByte();
			
			if (t.texturePresent != 0) {
				t.unk1 = r.ReadByte();
				t.shaderNameIDX = r.ReadInt32();

				t.texturesCount = r.ReadByte();
				t.texturesArray = new TextureImageInfo[t.texturesCount];

				for (int i = 0; i < t.texturesCount; ++i) {
					t.texturesArray[i] = new TextureImageInfo();
					t.texturesArray[i].textureIDX = r.ReadUInt32();//String Dictionary Offset
					t.texturesArray[i].unk4 = r.ReadByte();
					t.texturesArray[i].repetition = r.ReadSingle();//Float
					t.texturesArray[i].unk6 = r.ReadInt32();
					t.texturesArray[i].unk7 = r.ReadInt32();
				}
				t.count2 = r.ReadUInt32();
				t.unk8 = new int[t.count2];
				for (int i = 0; i < t.count2; ++i) {
					t.unk8[i] = r.ReadInt32();
				}
				t.count3 = r.ReadUInt32();
				t.unk9 = new float[t.count3];
				for (int i = 0; i < t.count3; ++i) {
					t.unk9[i] = r.ReadSingle();
				}
			}
			return t;
		}

	}
}
