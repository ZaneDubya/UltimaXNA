/***************************************************************************
 *   Copyright  (c) Alex Chudnovsky, Majestic-12 Ltd (UK).
 *              2005+ All rights reserved
 *   Web:       http://www.majestic12.co.uk
 *   E-mail:    alexc@majestic12.co.uk
 * 
 *   Redistribution and use in source and binary forms, with or without
 *   modification, are permitted provided that the following conditions
 *   are met:
 * 
 *   * Redistributions of source code must retain the above copyright
 *   notice, this list of conditions and the following disclaimer.
 *   * Redistributions in binary form must reproduce the above copyright
 *   notice, this list of conditions and the following disclaimer in the
 *   documentation and/or other materials provided with the distribution.
 *   * Neither the name of the Majestic-12 nor the names of its contributors
 *   may be used to endorse or promote products derived from this software
 *   without specific prior written permission.
 * 
 *   THIS SOFTWARE IS PROVIDED BY THE COPYRIGHT HOLDERS AND CONTRIBUTORS
 *   "AS IS" AND ANY EXPRESS OR IMPLIED WARRANTIES, INCLUDING, BUT NOT
 *   LIMITED TO, THE IMPLIED WARRANTIES OF MERCHANTABILITY AND FITNESS FOR
 *   A PARTICULAR PURPOSE ARE DISCLAIMED. IN NO EVENT SHALL THE COPYRIGHT
 *   OWNER OR CONTRIBUTORS BE LIABLE FOR ANY DIRECT, INDIRECT, INCIDENTAL, 
 *   SPECIAL, EXEMPLARY, OR CONSEQUENTIAL DAMAGES (INCLUDING, BUT NOT
 *   LIMITED TO, PROCUREMENT OF SUBSTITUTE GOODS OR SERVICES; LOSS OF USE,
 *   DATA, OR PROFITS; OR BUSINESS INTERRUPTION) HOWEVER CAUSED AND ON ANY
 *   THEORY OF LIABILITY, WHETHER IN CONTRACT, STRICT LIABILITY, OR TORT
 *   (INCLUDING NEGLIGENCE OR OTHERWISE) ARISING IN ANY WAY OUT OF THE USE
 *   OF THIS SOFTWARE, EVEN IF ADVISED OF THE POSSIBILITY OF SUCH DAMAGE.
 ***************************************************************************/

using System;
using System.Text;
using System.Collections;

namespace UltimaXNA.Core.UI.HTML.Parsing
{
	/// <summary>
	/// Class for fast dynamic string building - it is faster than StringBuilder
	/// </summary>
	///<exclude/>
	internal class DynaString	: IDisposable
	{
		/// <summary>
		/// Finalised text will be available in this string
		/// </summary>
		public string sText;

		/// <summary>
		/// CRITICAL: that much capacity will be allocated (once) for this object -- for performance reasons
		/// we do NOT have range checks because we make reasonably safe assumption that accumulated string will
		/// fit into the buffer. If you have very abnormal strings then you should increase buffer accordingly.
		/// </summary>
		public static int TEXT_CAPACITY=1024*256-1;

		public byte[] bBuffer;
		public int iBufPos;
		private int iLength;

		Encoding oEnc=Encoding.Default;

		private bool bDisposed=false;

		/// <summary>
		/// Constructor 
		/// </summary>
		/// <param name="sString">Initial string</param>
		internal DynaString(string sString)
		{
			sText=sString;
			iBufPos=0;
			bBuffer=new byte[TEXT_CAPACITY+1];
			iLength=sString.Length;
		}

		public void Dispose()
		{
			Dispose(true);
			GC.SuppressFinalize(this);
		}

		private void Dispose(bool bDisposing)
		{
			if(!bDisposed)
			{
				bBuffer=null;
			}

			bDisposed=true;
		}

		/// <summary>
		/// Resets object to zero length string
		/// </summary>
		public void Clear()
		{
			sText="";
			iLength=0;
			iBufPos=0;
		}

		/// <summary>
		/// Sets encoding to be used for conversion of binary data into string
		/// </summary>
		/// <param name="p_oEnc">Encoding object</param>
		public void SetEncoding(Encoding p_oEnc)
		{
			oEnc=p_oEnc;
		}

		/*
		/// <summary>
		/// Appends a "char" to the buffer
		/// </summary>
		/// <param name="cChar">Appends char (byte really)</param>
		public void Append(byte cChar)
		{
			// Length++;

			if(iBufPos>=TEXT_CAPACITY)
			{
				if(sText.Length==0)
				{
					sText=oEnc.GetString(bBuffer,0,iBufPos);
				}
				else
					//sText+=new string(bBuffer,0,iBufPos);
					sText+=oEnc.GetString(bBuffer,0,iBufPos);

				iLength+=iBufPos;

				iBufPos=0;
			}

			bBuffer[iBufPos++]=cChar;
		}
		*/
		/// <summary>
		/// Appends proper char with smart handling of Unicode chars
		/// </summary>
		/// <param name="cChar">Char to append</param>
		public void Append(char cChar)
		{
			if(cChar<=127)
				bBuffer[iBufPos++]=(byte)cChar;
			else
			{
				// unicode character - this is really bad way of doing it, but 
				// it seems to be called almost never
				byte[] bBytes=oEnc.GetBytes(cChar.ToString());

				// 16/09/07 Possible bug reported by Martin Bächtold: 
				// test case: 
				// <meta http-equiv="Content-Category" content="text/html; charset=windows-1251">
				// &#1329;&#1378;&#1400;&#1406;&#1397;&#1377;&#1398; &#1341;&#1377;&#1401;&#1377;&#1407;&#1400;&#1410;&#1408;

				// the problem is that some unicode chars might not be mapped to bytes by specified encoding
				// in the HTML itself, this means we will get single byte ? - this will look like failed conversion
				// Not good situation that we need to deal with :(
				if(bBytes.Length==1 && bBytes[0]==(char)'?')
				{
					// TODO: 

					for(int i=0; i<bBytes.Length; i++)
						bBuffer[iBufPos++]=bBytes[i];
				}
				else
				{
					for(int i=0; i<bBytes.Length; i++)
						bBuffer[iBufPos++]=bBytes[i];
				}
			}
		}
		
		/// <summary>
		/// Creates string from buffer using set encoder
		/// </summary>
		internal string SetToString()
		{
			if(iBufPos>0)
			{
				if(sText.Length==0)
				{
					sText=oEnc.GetString(bBuffer,0,iBufPos);
				}
				else
					//sText+=new string(bBuffer,0,iBufPos);
					sText+=oEnc.GetString(bBuffer,0,iBufPos);

				iLength+=iBufPos;
				iBufPos=0;
			}

			return sText;
		}

		/// <summary>
		/// Creates string from buffer using default encoder
		/// </summary>
		internal string SetToStringASCII()
		{
			if(iBufPos>0)
			{
				if(sText.Length==0)
				{
					sText=Encoding.Default.GetString(bBuffer,0,iBufPos);
				}
				else
					//sText+=new string(bBuffer,0,iBufPos);
					sText+=Encoding.Default.GetString(bBuffer,0,iBufPos);

				iLength+=iBufPos;
				iBufPos=0;
			}

			return sText;
		}

	}
}
