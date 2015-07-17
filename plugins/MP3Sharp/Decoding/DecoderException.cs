// /***************************************************************************
//  * DecoderException.cs
//  * Copyright (c) 2015 the authors.
//  * 
//  * All rights reserved. This program and the accompanying materials
//  * are made available under the terms of the GNU Lesser General Public License
//  * (LGPL) version 3 which accompanies this distribution, and is available at
//  * https://www.gnu.org/licenses/lgpl-3.0.en.html
//  *
//  * This library is distributed in the hope that it will be useful,
//  * but WITHOUT ANY WARRANTY; without even the implied warranty of
//  * MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the GNU
//  * Lesser General Public License for more details.
//  *
//  ***************************************************************************/

using System;
using System.Runtime.Serialization;

namespace MP3Sharp.Decoding
{
    /// <summary>
    ///     The DecoderException represents the class of
    ///     errors that can occur when decoding MPEG audio.
    /// </summary>
    [Serializable]
    internal class DecoderException : MP3SharpException
    {
        private int m_ErrorCode;

        public DecoderException(string message, Exception inner) : base(message, inner)
        {
            InitBlock();
        }

        public DecoderException(int errorcode, Exception inner) : this(GetErrorString(errorcode), inner)
        {
            InitBlock();
            m_ErrorCode = errorcode;
        }

        protected DecoderException(SerializationInfo info, StreamingContext context) : base(info, context)
        {
            m_ErrorCode = info.GetInt32("ErrorCode");
        }

        public virtual int ErrorCode
        {
            get { return m_ErrorCode; }
        }

        public override void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            if (info == null)
            {
                throw new ArgumentNullException("info");
            }

            info.AddValue("ErrorCode", m_ErrorCode);
            base.GetObjectData(info, context);
        }

        private void InitBlock()
        {
            m_ErrorCode = DecoderErrors.UNKNOWN_ERROR;
        }

        public static string GetErrorString(int errorcode)
        {
            // REVIEW: use resource file to map error codes
            // to locale-sensitive strings. 

            return "Decoder errorcode " + Convert.ToString(errorcode, 16);
        }
    }
}