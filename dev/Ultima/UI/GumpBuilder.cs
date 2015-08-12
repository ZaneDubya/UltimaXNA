/***************************************************************************
 *   GumpBuilder.cs
 *   Copyright (c) 2015 UltimaXNA Development Team
 *   
 *   This program is free software; you can redistribute it and/or modify
 *   it under the terms of the GNU General Public License as published by
 *   the Free Software Foundation; either version 3 of the License, or
 *   (at your option) any later version.
 *
 ***************************************************************************/
#region usings
using System;
using UltimaXNA.Core.Diagnostics.Tracing;
using UltimaXNA.Core.Resources;
using UltimaXNA.Ultima.UI.Controls;
#endregion

namespace UltimaXNA.Ultima.UI
{
    static class GumpBuilder
    {
        public static void BuildGump(Gump gump, string[] gumpPieces, string[] gumpLines)
        {
            int currentGUMPPage = 0;
            int currentRadioGroup = 0;

            // get the resource provider
            IResourceProvider provider = ServiceRegistry.GetService<IResourceProvider>();

            for (int i = 0; i < gumpPieces.Length; i++)
            {
                string[] gumpParams = gumpPieces[i].Split(' ');
                switch (gumpParams[0])
                {
                    case "button":
                        // Button [x] [y] [released-id] [pressed-id] [quit] [page-id] [return-value]
                        // [released-id] and [pressed-id] specify the buttongraphic.
                        // If pressed check for [return-value].
                        // Use [page-id] to switch between pages and [quit]=1/0 to close the gump.
                        gump.AddControl(new Button(gump, gumpParams), currentGUMPPage);
                        break;
                    case "buttontileart":
                        // ButtonTileArt [x] [y] [released-id] [pressed-id] [quit] [page-id] [return-value] [tilepic-id] [hue] [tile-x] [tile-y]
                        //  Adds a button to the gump with the specified coordinates and tilepic as graphic.
                        // [tile-x] and [tile-y] define the coordinates of the tile graphic and are relative to [x] and [y]. 
                        gump.AddControl(new Button(gump, int.Parse(gumpParams[1]), int.Parse(gumpParams[2]), int.Parse(gumpParams[3]), int.Parse(gumpParams[4]),
                            (ButtonTypes)int.Parse(gumpParams[5]), int.Parse(gumpParams[6]), int.Parse(gumpParams[7])), currentGUMPPage);
                        gump.AddControl(new StaticPic(gump, int.Parse(gumpParams[1]) + int.Parse(gumpParams[10]), int.Parse(gumpParams[2]) + int.Parse(gumpParams[11]),
                            int.Parse(gumpParams[8]), int.Parse(gumpParams[9])), currentGUMPPage);
                        break;
                    case "checkertrans":
                        // CheckerTrans [x] [y] [width] [height]
                        // Creates a transparent rectangle on position [x,y] using [width] and [height].
                        gump.AddControl(new CheckerTrans(gump, gumpParams), currentGUMPPage);
                        break;
                    case "croppedtext":
                        // CroppedText [x] [y] [width] [height] [color] [text-id]
                        // Adds a text field to the gump. gump is similar to the text command, but the text is cropped to the defined area.
                        gump.AddControl(new CroppedText(gump, gumpParams, gumpLines), currentGUMPPage);
                        (gump.LastControl as CroppedText).Hue = 1;
                        break;
                    case "gumppic":
                        // GumpPic [x] [y] [id] hue=[color]
                        // Adds a graphic to the gump, where [id] ist the graphic id of an item. For example use InsideUO to get them. Optionaly there is a color parameter.
                        gump.AddControl(new GumpPic(gump, gumpParams), currentGUMPPage);
                        break;
                    case "gumppictiled":
                        // GumpPicTiled [x] [y] [width] [height] [id]
                        // Similar to GumpPic, but the gumppic is tiled to the given [height] and [width].
                        gump.AddControl(new GumpPicTiled(gump, gumpParams), currentGUMPPage);
                        break;
                    case "htmlgump":
                        // HtmlGump [x] [y] [width] [height] [text-id] [background] [scrollbar]
                        // Defines a text-area where Html-commands are allowed.
                        // [background] and [scrollbar] can be 0 or 1 and define whether the background is transparent and a scrollbar is displayed.
                        gump.AddControl(new HtmlGumpling(gump, gumpParams, gumpLines), currentGUMPPage);
                        break;
                    case "page":
                        // Page [Number]
                        // Specifies which page to define. Page 0 is the background thus always visible.
                        currentGUMPPage = Int32.Parse(gumpParams[1]);
                        break;
                    case "resizepic":
                        // ResizePic [x] [y] [gump-id] [width] [height]
                        // Similar to GumpPic but the pic is automatically resized to the given [width] and [height].
                        gump.AddControl(new ResizePic(gump, gumpParams), currentGUMPPage);
                        break;
                    case "text":
                        // Text [x] [y] [color] [text-id]
                        // Defines the position and color of a text (data) entry.
                        gump.AddControl(new TextLabel(gump, gumpParams, gumpLines), currentGUMPPage);
                        break;
                    case "textentry":
                        // TextEntry [x] [y] [width] [height] [color] [return-value] [default-text-id]
                        // Defines an area where the [default-text-id] is displayed.
                        // The player can modify gump data. To get gump data check the [return-value].
                        gump.AddControl(new TextEntry(gump, gumpParams, gumpLines), currentGUMPPage);
                        break;
                    case "textentrylimited":
                        // TextEntryLimited [x] [y] [width] [height] [color] [return-value] [default-text-id] [textlen]
                        // Similar to TextEntry but you can specify via [textlen] the maximum of characters the player can type in.
                        gump.AddControl(new TextEntry(gump, gumpParams, gumpLines), currentGUMPPage);
                        break;
                    case "tilepic":
                        // TilePic [x] [y] [id]
                        // Adds a Tilepicture to the gump. [id] defines the tile graphic-id. For example use InsideUO to get them.
                        gump.AddControl(new StaticPic(gump, gumpParams), currentGUMPPage);
                        break;
                    case "tilepichue":
                        // TilePicHue [x] [y] [id] [hue]
                        // Similar to the tilepic command, but with an additional hue parameter.
                        gump.AddControl(new StaticPic(gump, gumpParams), currentGUMPPage);
                        break;
                    case "noclose":
                        // NoClose 
                        // Prevents that the gump can be closed by right clicking.
                        gump.IsUncloseableWithRMB = true;
                        break;
                    case "nodispose":
                        // NoDispose 
                        //Prevents that the gump can be closed by hitting Esc.
                        gump.IsUncloseableWithEsc = true;
                        break;
                    case "nomove":
                        // NoMove
                        // Locks the gump in his position. 
                        gump.BlockMovement = true;
                        break;
                    case "group":
                        // Group [Number]
                        // Links radio buttons to a group. Add gump before radiobuttons to do so. See also endgroup.
                        currentRadioGroup++;
                        break;
                    case "endgroup":
                        // EndGroup
                        //  Links radio buttons to a group. Add gump after radiobuttons to do so. See also group. 
                        currentRadioGroup++;
                        break;
                    case "radio":
                        // Radio [x] [y] [released-id] [pressed-id] [status] [return-value]
                        // Same as Checkbox, but only one Radiobutton can be pressed at the same time, and they are linked via the 'Group' command.
                        gump.AddControl(new RadioButton(gump, currentRadioGroup, gumpParams, gumpLines), currentGUMPPage);
                        break;
                    case "checkbox":
                        // CheckBox [x] [y] [released-id] [pressed-id] [status] [return-value]
                        // Adds a CheckBox to the gump. Multiple CheckBoxes can be pressed at the same time.
                        // Check the [return-value] if you want to know which CheckBoxes were selected.
                        gump.AddControl(new CheckBox(gump, gumpParams, gumpLines), currentGUMPPage);
                        break;
                    case "xmfhtmlgump":
                        // XmfHtmlGump [x] [y] [width] [height] [cliloc-nr] [background] [scrollbar]
                        // Similar to the htmlgump command, but in place of the [text-id] a CliLoc entry is used.
                        gump.AddControl(new HtmlGumpling(gump, int.Parse(gumpParams[1]), int.Parse(gumpParams[2]), int.Parse(gumpParams[3]), int.Parse(gumpParams[4]),
                            int.Parse(gumpParams[6]), int.Parse(gumpParams[7]),
                            "<span color=#000>" + provider.GetString(int.Parse(gumpParams[5]))), currentGUMPPage);
                        break;
                    case "xmfhtmlgumpcolor":
                        // XmfHtmlGumpColor [x] [y] [width] [height] [cliloc-nr] [background] [scrollbar] [color]
                        // Similar to the xmfhtmlgump command, but additionally a [color] can be specified.
                        gump.AddControl(new HtmlGumpling(gump, int.Parse(gumpParams[1]), int.Parse(gumpParams[2]), int.Parse(gumpParams[3]), int.Parse(gumpParams[4]),
                            int.Parse(gumpParams[6]), int.Parse(gumpParams[7]),
                            string.Format("<span color=#{0}>{1}", Utility.GetColorFromInt(int.Parse(gumpParams[8])), provider.GetString(int.Parse(gumpParams[5])))), currentGUMPPage);
                        (gump.LastControl as HtmlGumpling).Hue = 0;
                        break;
                    case "xmfhtmltok":
                        // XmfHtmlTok [x] [y] [width] [height] [background] [scrollbar] [color] [cliloc-nr] @[arguments]@
                        // Similar to xmfhtmlgumpcolor command, but the parameter order is different and an additionally
                        // [argument] entry enclosed with @'s can be used. With gump you can specify texts that will be
                        // added to the CliLoc entry. 
                        string messageWithArgs = provider.GetString(1070788);
                        int argReplaceBegin = messageWithArgs.IndexOf("~1");
                        if (argReplaceBegin != -1)
                        {
                            int argReplaceEnd = messageWithArgs.IndexOf("~", argReplaceBegin + 2);
                            if (argReplaceEnd != -1)
                            {
                                if (gumpParams.Length == 10 && gumpParams[9].Length >= 2)
                                {
                                    messageWithArgs = string.Format("{0}{1}{2}",
                                        messageWithArgs.Substring(0, argReplaceBegin),
                                        gumpParams[9].Substring(1, gumpParams[9].Length - 2),
                                        (argReplaceEnd > messageWithArgs.Length - 1) ? messageWithArgs.Substring(argReplaceEnd) : string.Empty);
                                }
                            }
                        }
                        gump.AddControl(new HtmlGumpling(gump, 
                            int.Parse(gumpParams[1]), int.Parse(gumpParams[2]), int.Parse(gumpParams[3]), int.Parse(gumpParams[4]),
                            int.Parse(gumpParams[5]), int.Parse(gumpParams[6]),
                            string.Format("<font color=#{0}>{1}", Utility.GetColorFromUshort(ushort.Parse(gumpParams[7])), messageWithArgs)), currentGUMPPage);
                        (gump.LastControl as HtmlGumpling).Hue = 0;
                        Tracer.Warn(string.Format("GUMP: Unhandled {0}.", gumpParams[0]));
                        break;
                    case "tooltip":
                        // Tooltip [cliloc-number]
                        // Adds to the previous layoutarray entry a Tooltip with the in [cliloc-number] defined CliLoc entry.
                        string cliloc = provider.GetString(int.Parse(gumpPieces[1]));
                        if (gump.LastControl != null)
                            gump.LastControl.SetTooltip(cliloc);
                        else
                            Tracer.Warn(string.Format("GUMP: No control for gump tooltip: {0}.", gumpParams[1]));
                        break;
                    case "noresize":

                        break;
                    default:
                        Tracer.Critical("GUMP: Unknown piece '" + gumpParams[0] + "'.");
                        break;
                }
            }
        }
    }
}
