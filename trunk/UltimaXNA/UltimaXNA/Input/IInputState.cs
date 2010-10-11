/********************************************************
 * 
 *  $Id: IInputService.cs 71 2009-12-11 19:16:07Z jeff $
 *  
 *  $Author: jeff $
 *  $Date: 2009-12-11 11:16:07 -0800 (Fri, 11 Dec 2009) $
 *  $Revision: 71 $
 *  
 *  $LastChangedBy: jeff $
 *  $LastChangedDate: 2009-12-11 11:16:07 -0800 (Fri, 11 Dec 2009) $
 *  $LastChangedRevision: 71 $
 *  
 *  This code is (C) Copyright 2009 Jeff Boulanger. All rights reserved.
 *  Used with permission.
 *  
 ********************************************************/

using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;

namespace UltimaXNA.Input
{
    public interface IInputService
    {
        event EventHandler<KeyEventArgs> KeyDown;
        event EventHandler<KeyPressEventArgs> KeyPress;
        event EventHandler<KeyEventArgs> KeyUp;
        event EventHandler<MouseEventArgs> MouseDown;
        event EventHandler<MouseEventArgs> MouseMove;
        event EventHandler<MouseEventArgs> MouseUp;
        event EventHandler<MouseEventArgs> MouseWheel;
        // InputBinding AddBinding(string name, bool shift, bool control, bool alt, Keys key, EventHandler beginHandler, EventHandler endHandler);
        // InputBinding AddBinding(string name, bool shift, bool control, bool alt, Keys key, EventHandler handler);
        // InputBinding AddBinding(string name, MouseButtons buttons, EventHandler beginHandler, EventHandler endHandler);
        // InputBinding AddBinding(string name, MouseButtons buttons, EventHandler handler);
    }
}
