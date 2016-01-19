using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;

namespace UOReader
{

    public class TileartImageOffset
    {
        public int Xstart, Ystart, Width, Height, offX, offY, Xend, Yend;

        public TileartImageOffset(int[] imgoff)
        {
            Xstart = imgoff[0];
            Ystart = imgoff[1];
            Xend = imgoff[2];
            Yend = imgoff[3];
            offX = imgoff[4];
            offY = imgoff[5];

            Width = Xend - Xstart;
            Height = Yend - Ystart;
        }
    }

    public class Tileart
    {
        public ushort header;
        public uint id;
        public uint nameIndex;
        public byte unk;
        public byte unk7;
        public float unk2;
        public float unk3;
        public int fixedZero;
        public int oldID;
        public float unk6;
        public int unk_type;
        public byte unk8;
        public int unk9, unk10;
        public float unk11, unk12;
        public int unk13, unk16;
        public long int_flags, int_flags_full;
        public TileFlag flags = TileFlag.None;
        public TileFlag flags2 = TileFlag.None;

        public TileartImageOffset offset2D;
        public TileartImageOffset offsetEC;
        private int[] imgoff2D = new int[6];
        private int[] imgoffEC = new int[6];
        //9_1
        public TileartPropItem[] props;
        //9_2
        public TileartPropItem[] props2;
        //9_3
        public StackAlias[] stackaliases;
        //9_4
        public AnimationAppearanceItem appearance;
        //9_5
        public SittingAnimationItem sittingAnimation;
        //9_6
        public RadarColItem radarCol;
        //9_7
        public TextureInfo[] textures = new TextureInfo[4];
        //9_8
        public EffectItem[] effects;

        private Tileart()
        {
        }

        public static Tileart readTileart(BinaryReader r)
        {
            Tileart t = new Tileart();
            t.header = r.ReadUInt16();//fixed 03
            t.nameIndex = r.ReadUInt32();//t.group = Enum.Parse(typeof(TileGroup), r.ReadUInt32().ToString());
            t.id = r.ReadUInt32();

            t.unk = r.ReadByte();
            t.unk7 = r.ReadByte();
            t.unk2 = r.ReadSingle();
            t.unk3 = r.ReadSingle();
            t.fixedZero = r.ReadInt32();
            t.oldID = r.ReadInt32();
            t.unk6 = r.ReadInt32();
            t.unk_type = r.ReadInt32();

            t.unk8 = r.ReadByte();
            t.unk9 = r.ReadInt32();
            t.unk10 = r.ReadInt32();
            t.unk11 = r.ReadSingle();
            t.unk12 = r.ReadSingle();
            t.unk13 = r.ReadInt32();

            t.int_flags = r.ReadInt64();
            t.int_flags_full = r.ReadInt64();

            t.flags = (TileFlag)Enum.Parse(typeof(TileFlag), t.int_flags.ToString());
            t.flags2 = (TileFlag)Enum.Parse(typeof(TileFlag), t.int_flags_full.ToString());

            t.unk16 = r.ReadInt32();

            //IMAGE OFFSET
            for (int i = 0; i < 6; ++i)
            {
                t.imgoffEC[i] = r.ReadInt32();
            }
            for (int i = 0; i < 6; ++i)
            {
                t.imgoff2D[i] = r.ReadInt32();
            }

            t.offsetEC = new TileartImageOffset(t.imgoffEC);
            t.offset2D = new TileartImageOffset(t.imgoff2D);

            try
            {
                //PROPERTIES
                t.props = ReadProps(t, r);
                t.props2 = ReadProps(t, r); //Repetition (?)
                                            //9_3
                ReadStackAliases(t, r);
                //9_4
                t.appearance = AnimationAppearanceItem.ReadAnimationAppearance(r);
                //9_5
                t.sittingAnimation = SittingAnimationItem.ReadSittingAnimation(r);
                //Radarcol
                t.radarCol = RadarColItem.ReadRadarCol(r);

                t.textures[0] = TextureInfo.readTextureInfo(r);
                t.textures[1] = TextureInfo.readTextureInfo(r);
                t.textures[2] = TextureInfo.readTextureInfo(r);
                t.textures[3] = TextureInfo.readTextureInfo(r);//New Format
                                                               //9_8
                ReadEffects(t, r);
            }
            catch
            {
            }
            return t;
        }

        private static TileartPropItem[] ReadProps(Tileart t, BinaryReader r)
        {
            byte count1 = r.ReadByte();
            TileartPropItem[] props = new TileartPropItem[count1];
            for (int i = 0; i < count1; ++i)
            {
                props[i] = TileartPropItem.ReadProp(r);
            }
            return props;
        }
        private static void ReadStackAliases(Tileart t, BinaryReader r)
        {
            int count3 = r.ReadInt32();
            t.stackaliases = new StackAlias[count3];
            for (int i = 0; i < count3; ++i)
            {
                t.stackaliases[i] = StackAlias.ReadStackAlias(r);
            }
        }

        private static void ReadEffects(Tileart t, BinaryReader r)
        {
            byte count = r.ReadByte();
            t.effects = new EffectItem[count];
            for (int i = 0; i < count; ++i)
            {
                t.effects[i] = EffectItem.ReadEffect(r);
            }
        }

        /*
		public static void DrawBorders(Bitmap cimg, bool isEC, Tileart tai, bool drawOffsets) {
			Graphics gr = Graphics.FromImage(cimg);
			Pen p = new Pen(Brushes.Pink);

			int Xstart, Ystart, Width, Height, offX, offY, Xend, Yend;
			if (isEC) {
				Xstart = tai.imgoffEC[0];
				Ystart = tai.imgoffEC[1];
				Xend = tai.imgoffEC[2];
				if (Xend == 0)
					Xend = tai.imgoffEC[3];
				Yend = tai.imgoffEC[3];
				offX = tai.imgoffEC[4];
				offY = tai.imgoffEC[5];
			} else {
				Xstart = tai.imgoff2D[0];
				Ystart = tai.imgoff2D[1];
				Xend = tai.imgoff2D[2];
				Yend = tai.imgoff2D[3];
				offX = tai.imgoff2D[4];
				offY = tai.imgoff2D[5];
			}

			Width = Xend - Xstart;
			Height = Yend - Ystart;

			if (drawOffsets) {
				if (offX > 0)
					Xstart += offX;
				else
					Width -= offX;
				if (offY > 0)
					Ystart += offY;
				else
					Height -= offY;
			}

			gr.DrawRectangle(p, new Rectangle(Xstart, Ystart, Width, Height));

			gr.Dispose();
			return;
		}
		*/
    }
}
