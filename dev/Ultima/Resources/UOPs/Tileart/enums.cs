using System;
using System.Collections.Generic;
using System.Text;

namespace UOReader
{
	/// <summary>
	/// An enumeration of 32 different tile flags.
	/// <seealso cref="ItemData" />
	/// <seealso cref="LandData" />
	/// </summary>
	[Flags]
	public enum TileFlag : ulong
	{
		/// <summary>
		/// Nothing is flagged.
		/// </summary>
		None = 0x00000000,
		/// <summary>
		/// Not yet documented.
		/// </summary>
		Background = 0x00000001,
		/// <summary>
		/// Not yet documented.
		/// </summary>
		Weapon = 0x00000002,
		/// <summary>
		/// Not yet documented.
		/// </summary>
		Transparent = 0x00000004,
		/// <summary>
		/// The tile is rendered with partial alpha-transparency.
		/// </summary>
		Translucent = 0x00000008,
		/// <summary>
		/// The tile is a wall.
		/// </summary>
		Wall = 0x00000010,
		/// <summary>
		/// The tile can cause damage when moved over.
		/// </summary>
		Damaging = 0x00000020,
		/// <summary>
		/// The tile may not be moved over or through.
		/// </summary>
		Impassable = 0x00000040,
		/// <summary>
		/// Not yet documented.
		/// </summary>
		Wet = 0x00000080,
		/// <summary>
		/// Unknown.
		/// </summary>
		Ignored = 0x00000100,
		/// <summary>
		/// The tile is a surface. It may be moved over, but not through.
		/// </summary>
		Surface = 0x00000200,
		/// <summary>
		/// The tile is a stair, ramp, or ladder.
		/// </summary>
		Bridge = 0x00000400,
		/// <summary>
		/// The tile is stackable
		/// </summary>
		Generic = 0x00000800,
		/// <summary>
		/// The tile is a window. Like <see cref="TileFlag.NoShoot" />, tiles with this flag block line of sight.
		/// </summary>
		Window = 0x00001000,
		/// <summary>
		/// The tile blocks line of sight.
		/// </summary>
		NoShoot = 0x00002000,
		/// <summary>
		/// For single-amount tiles, the string "a " should be prepended to the tile name.
		/// </summary>
		ArticleA = 0x00004000,
		/// <summary>
		/// For single-amount tiles, the string "an " should be prepended to the tile name.
		/// </summary>
		ArticleAn = 0x00008000,
		/// <summary>
		/// Not yet documented.
		/// </summary>
		ArticleThe = 0x0000C000,
		/// <summary>
		/// Not yet documented.
		/// </summary>
		Mongen = 0x00010000,
		/// <summary>
		/// The tile becomes translucent when walked behind. Boat masts also have this flag.
		/// </summary>
		Foliage = 0x00020000,
		/// <summary>
		/// Only gray pixels will be hued
		/// </summary>
		PartialHue = 0x00040000,
		/// <summary>
		/// Unknown.
		/// </summary>
		UseNewArt = 0x00080000,
		/// <summary>
		/// The tile is a map--in the cartography sense. Unknown usage.
		/// </summary>
		Map = 0x00100000,
		/// <summary>
		/// The tile is a container.
		/// </summary>
		Container = 0x00200000,
		/// <summary>
		/// The tile may be equiped.
		/// </summary>
		Wearable = 0x00400000,
		/// <summary>
		/// The tile gives off light.
		/// </summary>
		LightSource = 0x00800000,
		/// <summary>
		/// The tile is animated.
		/// </summary>
		Animation = 0x01000000,
		/// <summary>
		/// Gargoyles can fly over
		/// </summary>
		HoverOver = 0x02000000,
		/// <summary>
		/// Unknown.
		/// </summary>
		ArtUsed = 0x04000000,
		/// <summary>
		/// Not yet documented.
		/// </summary>
		Armor = 0x08000000,
		/// <summary>
		/// The tile is a slanted roof.
		/// </summary>
		Roof = 0x10000000,
		/// <summary>
		/// The tile is a door. Tiles with this flag can be moved through by ghosts and GMs.
		/// </summary>
		Door = 0x20000000,
		/// <summary>
		/// Not yet documented.
		/// </summary>
		StairBack = 0x40000000,
		/// <summary>
		/// Not yet documented.
		/// </summary>
		StairRight = 0x80000000,
		//NEW!!!
		/// <summary>
		/// Not yet documented.
		/// 0x1
		/// </summary>
		NoHouse = 0x100000000,
		/// <summary>
		/// Not yet documented.
		/// 0x2
		/// </summary>
		NoDraw = 0x00200000000,
		/// <summary>
		/// Not yet documented.
		/// 0x4
		/// </summary>
		Unused1 = 0x00400000000,
		/// <summary>
		/// Not yet documented.
		/// 0x8
		/// </summary>
		AlphaBlend = 0x00800000000,
		/// <summary>
		/// Not yet documented.
		/// </summary>
		NoShadow = 0x01000000000,
		/// <summary>
		/// Not yet documented.
		/// </summary>
		PixelBleed = 0x02000000000,
		/// <summary>
		/// Not yet documented.
		/// 0x40
		/// </summary>
		Unused2 = 0x04000000000,
		/// <summary>
		/// Not yet documented.
		/// 0x80
		/// </summary>
		PlayAnimOnce = 0x08000000000,
		/// <summary>
		/// Not yet documented.
		/// </summary>
		MultiMovable = 0x10000000000
	}


	public enum TileArtProperties {
		//Used in tiledata bin sub1-2
		Weight = 0,
		Quality = 1,
		Quantity = 2,
		Height = 3,
		Value = 4,
		AcVc = 5,
		Slot = 6,
		off_C8 = 7,
		Appearance = 8,
		Race = 9,
		Gender = 10,
		Paperdoll = 11
	}

	public enum EffectsID {
		EFFECT00 = 0,
		EFFECT01 = 1,
		EFFECT02 = 2,
		EFFECT07 = 7,
		EFFECT10 = 10,
		EFFECT11 = 11,
		EFFECT12 = 12,
		EFFECT15 = 15,
		EFFECT16 = 16,
		EFFECT17 = 17
	}
}
