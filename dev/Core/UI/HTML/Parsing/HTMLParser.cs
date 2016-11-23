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
using System.IO;
using System.Text;

namespace UltimaXNA.Core.UI.HTML.Parsing
{
	/// <summary>
	/// Allows to parse HTML by splitting it into small token (HTMLchunks) such as tags, text, comments etc.
	/// 
	/// Do NOT create multiple instances of this class - REUSE single instance
	/// Do NOT call same instance from multiple threads - it is NOT thread safe
	/// </summary>
	public class HTMLparser : IDisposable
	{
		/// <summary>
		/// If false (default) then HTML entities (like "&nbsp;") will not be decoded, otherwise they will
		/// be decoded: this should be set if you deal with unicode data that takes advantage of entities
		/// and in cases when you need to deal with final string representation
		/// </summary>
		public bool bDecodeEntities
		{
			set
			{
				m_bDecodeEntities = oE.bDecodeEntities=value;
			}
			get
			{
				return oE.bDecodeEntities;
			}
		}

		bool m_bDecodeEntities=false;

		/// <summary>
		/// If false (default) then mini entity set (&nbsp;) will be decoded, but not all of them
		/// </summary>
		public bool bDecodeMiniEntities
		{
			set
			{
				oE.bMiniEntities=value;
			}
			get
			{
				return oE.bMiniEntities;
			}
		}

		/// <summary>
		/// If true (default) then heuristics engine will be used to match tags and attributes quicker, it is
		/// possible to add new tags to it, <see cref="oHE"/>
		/// </summary>
		public bool bEnableHeuristics
		{
			get
			{
				return oTP.bEnableHeuristics;	
			}
			set
			{
				oTP.bEnableHeuristics=value;
			}
		}

		/// <summary>
		/// If true then exception will be thrown in case of inability to set encoding taken
		/// from HTML - this is possible if encoding was incorrect or not supported, this would lead
		/// to abort in processing. Default behavior is to use Default encoding that should keep symbols as
		/// is - most likely garbage looking things if encoding was not supported.
		/// </summary>
		public bool bThrowExceptionOnEncodingSetFailure=false;

		/// <summary>
		/// If true (default: false) then parsed tag chunks will contain raw HTML, otherwise only comments will have it set
		/// <p>
		/// Performance hint: keep it as false, you can always get to original HTML as each chunk contains
		/// offset from which parsing started and finished, thus allowing to set exact HTML that was parsed
		/// </p>
		/// </summary>
		/// <exclude/>
		public bool bKeepRawHTML=false;

		/// <summary>
		/// If true (default) then HTML for comments tags themselves AND between them will be set to oHTML variable, otherwise it will be empty
		/// but you can always set it later 
		/// </summary>
		public bool bAutoKeepComments=true;

		/// <summary>
		/// If true (default: false) then HTML for script tags themselves AND between them will be set to oHTML variable, otherwise it will be empty
		/// but you can always set it later
		/// </summary>
		public bool bAutoKeepScripts=true;

		/// <summary>
		/// If true (and either bAutoKeepComments or bAutoKeepScripts is true), then oHTML will be set
		/// to data BETWEEN tags excluding those tags themselves, as otherwise FULL HTML will be set, ie:
		/// '<!-- comments -->' but if this is set to true then only ' comments ' will be returned
		/// </summary>
		public bool bAutoExtractBetweenTagsOnly=true;

		/// <summary>
		/// Long winded name... by default if tag is closed BUT it has got parameters then we will consider it
		/// open tag, this is not right for proper XML parsing
		/// </summary>
		public bool bAutoMarkClosedTagsWithParamsAsOpen=true;

		/// <summary>
		/// If true (default), then all whitespace before TAG starts will be compressed to single space char (32 or 0x20)
		/// this makes parser run a bit faster, if you need exact whitespace before tags then change this flag to FALSE
		/// </summary>
		public bool bCompressWhiteSpaceBeforeTag=true;

		/// <summary>
		/// If true then pure whitespace before tags will be ignored - but only IF its purely whitespace. 
		/// Enabling this feature will increase performance, however this will be at cost of correctness as 
		/// some text has essential whitespacing done just before tags.
		/// 
		/// REMOVED
		/// </summary>
		//public bool bIgnoreWhiteSpaceBeforeTags=false;

		/// <summary>
		/// Heuristics engine used by Tag Parser to quickly match known tags and attribute names, can be disabled
		/// or you can add more tags to it to fit your most likely cases, it is currently tuned for HTML
		/// </summary>
		public HTMLheuristics oHE=new HTMLheuristics();

		/// <summary>
		/// Internal -- dynamic string for text accumulation
		/// </summary>
		DynaString sText=new DynaString("");

		/// <summary>
		/// This chunk will be returned when it was parsed
		/// </summary>
		HTMLchunk oChunk=new HTMLchunk(true);

		/// <summary>
		/// Tag parser object
		/// </summary>
		TagParser oTP=new TagParser();

		/// <summary>
		/// Encoding used to convert binary data into string
		/// </summary>
		public Encoding oEnc=null;

		/// <summary>
		/// Byte array with HTML will be kept here
		/// </summary>
		byte[] bHTML;

        /// <summary>
        /// The input html string. Saved because we parse the html as bytes, but we want to output unicode chars.
        /// </summary>
        string m_OriginalHtml;

		/// <summary>
		/// Current position pointing to byte in bHTML
		/// </summary>
		/// <exclude/
		int iCurPos=0;

		/// <summary>
		/// Length of bHTML -- it appears to be faster to use it than bHTML.Length
		/// </summary>
		int iDataLength=0;

		/// <summary>
		/// Whitespace lookup table - 0 is not whitespace, otherwise it is
		/// </summary>
		static byte[] bWhiteSpace=new byte[byte.MaxValue+1];

		/// <summary>
		/// Entities manager
		/// </summary>
		HTMLentities oE=new HTMLentities();

		static HTMLparser()
		{
			// set bytes that are whitespace
			bWhiteSpace[' ']=1;
			bWhiteSpace['\t']=1;
			bWhiteSpace[13]=1;
			bWhiteSpace[10]=1;
		}

		public HTMLparser()
		{
			// init heuristics engine
			oHE.AddTag("a","href");
			oHE.AddTag("b","");
			oHE.AddTag("p","class");
			oHE.AddTag("i","");
			oHE.AddTag("s","");
			oHE.AddTag("u","");

			oHE.AddTag("td","align,valign,bgcolor,rowspan,colspan");
			oHE.AddTag("table","border,width,cellpadding");
			oHE.AddTag("span","");
			oHE.AddTag("option","");
			oHE.AddTag("select","");

			oHE.AddTag("tr","");
			oHE.AddTag("div","class,align");
			oHE.AddTag("img","src,width,height,title,alt");
			oHE.AddTag("input","");
			oHE.AddTag("br","");
			oHE.AddTag("li","");
			oHE.AddTag("ul","");
			oHE.AddTag("ol","");
			oHE.AddTag("hr","");
			oHE.AddTag("h1","");
			oHE.AddTag("h2","");
			oHE.AddTag("h3","");
			oHE.AddTag("h4","");
			oHE.AddTag("h5","");
			oHE.AddTag("h6","");
			oHE.AddTag("font","size,color");
			oHE.AddTag("meta","name,content,http-equiv");
			oHE.AddTag("base","href");
			
			// these are pretty rare
			oHE.AddTag("script","");
			oHE.AddTag("style","");
			oHE.AddTag("html","");
			oHE.AddTag("body","");
		}

		/// <summary>
		/// Sets chunk param hash mode
		/// </summary>
		/// <param name="bHashMode">If true then tag's params will be kept in Chunk's hashtable (slower), otherwise kept in arrays (sParams/sValues)</param>
		public void SetChunkHashMode(bool bHashMode)
		{
			oChunk.bHashMode=bHashMode;
		}

		bool bDisposed=false;

		public void Dispose()
		{
			Dispose(true);
			GC.SuppressFinalize(this);
		}

		private void Dispose(bool bDisposing)
		{
			if(!bDisposed)
			{
				bDisposed=true;

				if(oChunk!=null)
				{
					oChunk.Dispose();
					oChunk=null;
				}

				if(sText!=null)
				{
					sText.Dispose();
					sText=null;
				}

				bHTML=null;

				if(oE!=null)
				{
					oE.Dispose();
					oE=null;
				}

				if(oTP!=null)
				{
					oTP.Dispose();
					oTP=null;
				}

			}

		}

		/// <summary>
		/// Sets oHTML variable in a chunk to the raw HTML that was parsed for that chunk.
		/// </summary>
		/// <param name="oChunk">Chunk returned by ParseNext function, it must belong to the same HTMLparser that
		/// was initiated with the same HTML data that this chunk belongs to</param>
		public void SetRawHTML(HTMLchunk oChunk)
		{
            // note: this really should have been byte array assigned rather than string
            // it would be more correct originality-wise
			oChunk.oHTML=oEnc.GetString(bHTML,oChunk.iChunkOffset,oChunk.iChunkLength);
		}

		/// <summary>
		/// Closes object and releases all allocated resources
		/// </summary>
		public void Close()
		{
			Dispose();
		}

		/// <summary>
		/// Sets encoding 
		/// </summary>
		/// <param name="p_oEnc">Encoding object</param>
		public void SetEncoding(Encoding p_oEnc)
		{
			oChunk.SetEncoding(p_oEnc);
			sText.SetEncoding(p_oEnc);
			
			if(p_oEnc!=null)
				oEnc=p_oEnc;
			/*
			if(oCharset.IsSupported(sCharset))
			{
				oCharset.SetCharset(sCharset);
				//bCharConv=true;
			}
			//else Console.WriteLine("Charset '{0}' is not supported!",sCharset);
			*/
		}

		/// <summary>
		/// Sets current encoding in format used in HTTP headers and HTML META tags
		/// </summary>
		/// <param name="sCharSet">Charset as </param>
		/// <returns>True if encoding was set, false otherwise (in which case Default encoding will be used)</returns>
		public bool SetEncoding(string p_sCharSet)
		{
			string sCharSet=p_sCharSet;

			try
			{
				if(sCharSet.IndexOf(";")!=-1)
					sCharSet=GetCharSet(sCharSet);

				sCharSet=sCharSet.Replace("?","");

				oEnc=Encoding.GetEncoding(sCharSet);

				// FIXIT: check here that the encoding set was not some fallback to default
				// due to lack of support on target machine

				//Utils.WriteLine("Setting encoding: "+sCharSet);
				SetEncoding(oEnc);
			}
			catch(Exception oEx)
			{
				if(bThrowExceptionOnEncodingSetFailure)
					throw new Exception("Failed to set encoding '"+sCharSet+"', original encoding string: '"+p_sCharSet+"'. Original exception message: "+oEx.Message);

				// encoding was not supported - we will fall back to default one
				oEnc=Encoding.Default;
				SetEncoding(oEnc);
		
				return false;
			}

			return true;
		}

		/// <summary>
		/// Retrieves charset information from format used in HTTP headers and META descriptions
		/// </summary>
		/// <param name="sData">Data to find charset info from</param>
		/// <returns>Charset</returns>
		static string GetCharSet(string sData)
		{
			try
			{
				if(sData==null)
					return "";

				int iIdx=sData.ToLower().IndexOf("charset=");

				if(iIdx!=-1)
					return sData.Substring(iIdx+8,sData.Length-iIdx-8).ToLower().Trim();
				
			}
			catch //(Exception oEx)
			{
				// FIXIT: not ideal because it wont be visible in WinForms builds...
				//Console.WriteLine(oEx.ToString());
			}

			return "";
		}

		/// <summary>
		/// Inits mini-entities mode: only "nbsp" will be converted into space, all other entities 
		/// will be left as is
		/// </summary>
		public void InitMiniEntities()
		{
			oE.InitMiniEntities();
		}

		public string ChangeToEntities(string sLine)
		{
			return ChangeToEntities(sLine,false);
		}

		/// <summary>
		/// Parses line and changes known entiry characters into proper HTML entiries
		/// </summary>
		/// <param name="sLine">Line of text</param>
		/// <returns>Line of text with proper HTML entities</returns>
		public string ChangeToEntities(string sLine,bool bChangeDangerousCharsOnly)
		{
			// PHP does not handle that well, fsckers
			//bChangeAllNonASCII=false;

			try
			{
				// scan string first and if 
				for(int i=0; i<sLine.Length; i++)
				{				
					int cChar=(int)sLine[i];

					// yeah I know its lame but its 3:30am and I had v.long debugging session :-/
					switch(cChar)
					{
						case 0:
						case 39:
						case 145:
						case 146:
						case 147:
						case 148:
							return oE.ChangeToEntities(sLine,i,bChangeDangerousCharsOnly);

						default:

							if(cChar<32) // || (bChangeAllNonASCII && cChar>=127))
								goto case 148;

							break;
					};

					if(cChar<HTMLentities.sEntityReverseLookup.Length && HTMLentities.sEntityReverseLookup[cChar]!=null)
						return oE.ChangeToEntities(sLine,i,bChangeDangerousCharsOnly);
				}

			}
			catch(Exception oEx)
			{
				Console.WriteLine("Entity exception: "+oEx.ToString());
			}

			// nothing need to be changed
			return sLine;
		}


		/// <summary>
		/// Constructs parser object using provided HTML as source for parsing
		/// </summary>
		/// <param name="p_oHTML"></param>
		public HTMLparser(string p_oHTML)
		{
			Init(p_oHTML);
		}

		/// <summary>
		/// Initialises parses with HTML to be parsed from provided string
		/// </summary>
		/// <param name="p_oHTML">String with HTML in it</param>
		public void Init(string p_oHTML)
		{
            // set default encoding
            if (oEnc == null)
                oEnc = Encoding.Default;

            m_OriginalHtml = p_oHTML;
            Init(Encoding.Default.GetBytes(p_oHTML));
		}

		/// <summary>
		/// Initialises parses with HTML to be parsed from provided data buffer: this is best in terms of
		/// correctness of parsing of various encodings that can be used in HTML
		/// </summary>
		/// <param name="p_bHTML">Data buffer with HTML in it</param>
		public void Init(byte[] p_bHTML)
		{
			Init(p_bHTML,p_bHTML.Length);
		}

		/// <summary>
		/// Inits parsing
		/// </summary>
		/// <param name="p_bHTML">Data buffer</param>
		/// <param name="p_iHtmlLength">Length of data (buffer itself can be longer) - start offset assumed to be 0</param>
		public void Init(byte[] p_bHTML,int p_iHtmlLength)
		{
			// set default encoding
            if (oEnc == null)
			    oEnc=Encoding.Default;

			CleanUp();
		
			bHTML=p_bHTML;

			// check whether we have got data that is actually in Unicode format
			// normally this would mean we have got plenty of zeroes
			// this and related code was contributed by Martin B�chtold from TTN Tele.Translator.Network
			if(bHTML.Length > 2)
			{
				if(bHTML[0]==255 && bHTML[1]==254)
				{
					bHTML=Encoding.Default.GetBytes(Encoding.Unicode.GetString(bHTML,2,bHTML.Length-2));
				}
			}

			iDataLength=p_iHtmlLength;

			oTP.Init(this,oChunk,sText,bHTML,iDataLength,oE,oHE);
		}

		/// <summary>
		/// Cleans up parser in preparation for next parsing
		/// </summary>
		public void CleanUp()
		{
			/*
			if(oEntities==null)
			{
				oEntities=InitEntities(ref iMinEntityLen,ref iMaxEntityLen,out sEntityReverseLookup);
				bMiniEntities=false;
			}
			*/

			oTP.CleanUp();

			bHTML=null;

			iCurPos=0;
			iDataLength=0;
		}

		/// <summary>
		/// Resets current parsed data to start
		/// </summary>
		public void Reset()
		{
			iCurPos=0;
		}

		/// <summary>
		/// Font sizes as described by W3C: http://www.w3.org/TR/REC-CSS2/fonts.html#propdef-font-size
		/// </summary>
		/// <exclude/>
		public enum FontSize
		{
			Small_xx	= 0,
			Small_x		= 1,
			Small		= 2,
			Medium		= 3,
			Large		= 4,
			Large_x		= 5,
			Large_xx	= 6,

			Unknown		= 7,
		}

		/// <summary>
		/// Checks if first font is bigger than the second
		/// </summary>
		/// <param name="oFont1">Font #1</param>
		/// <param name="oFont2">Font #2</param>
		/// <returns>True if Font #1 bigger than the second, false otherwise</returns>
		public static bool IsBiggerFont(FontSize oFont1,FontSize oFont2)
		{
			return (int)oFont1>(int)oFont2;
		}

		/// <summary>
		/// Checks if first font is equal or bigger than the second
		/// </summary>
		/// <param name="oFont1">Font #1</param>
		/// <param name="oFont2">Font #2</param>
		/// <returns>True if Font #1 equal or bigger than the second, false otherwise</returns>
		public static bool IsEqualOrBiggerFont(FontSize oFont1,FontSize oFont2)
		{
			return (int)oFont1>=(int)oFont2;
		}

		/// <summary>
		/// Parses font's tag size param 
		/// </summary>
		/// <param name="sSize">String value of the size param</param>
		/// <param name="iBaseFontSize">Optional base font size, use -1 if its not present</param>
		/// <param name="iCurSize"></param>
		/// <returns>Relative size of the font size or Unknown if it was not determined</returns>
		public static FontSize ParseFontSize(string sSize,FontSize oCurSize)
		{
			// TODO: read more http://www.w3.org/TR/REC-CSS2/fonts.html#propdef-font-size
			sSize=sSize.Trim();

			if(sSize.Length==0)
				return FontSize.Unknown;

			// check if its relative or absolute value
			int iSign=0;

			string sDigits="";

			int iDigits=0;

			for(int i=0; i<sSize.Length; i++)
			{
				char cChar=sSize[i];

				if(char.IsWhiteSpace(cChar))
					continue;

				switch(cChar)
				{
					case '+':

						if(iSign==0)
							iSign=1;
						else
							return FontSize.Unknown;

						break;

					case '-':

						if(iSign==0)
							iSign=-1;
						else
							return FontSize.Unknown;

						break;

					default:

						if(char.IsDigit(cChar))
						{
							iDigits++;

							if(sDigits.Length==0)
								sDigits=cChar.ToString();
							else
								sDigits+=cChar;
						}

						break;
				};
				
			}
			
			if(sDigits.Length==0 || iDigits==0)
				return FontSize.Unknown;

			int iFontSize=0;

			if(sDigits.Length>3)
				return FontSize.Unknown;

			int iSize=0;
			
			try
			{
				iSize=int.Parse(sDigits);
			}
			catch
			{
				return FontSize.Unknown;
			}

			if(iSign==0)
			{
				// absolute set
				iFontSize=iSize;
			}
			else	// relative change 
			{
				if(iSign<0)
					iFontSize=(int)oCurSize-iSize;
				else
					iFontSize=(int)oCurSize+iSize;
			}
			
			// sanity check
			if(iFontSize<0)
				iFontSize=0;
			else
				if(iFontSize>6)
					iFontSize=6;

			return (FontSize)iFontSize;
		}

		/// <summary>
		/// Returns next tag or null if end of document, text will be ignored completely
		/// </summary>
		/// <returns>Tag chunk or null</returns>
		public HTMLchunk ParseNextTag()
		{
			
			// skip till first < char
			while(iCurPos<iDataLength)
			{
				// check if we have got tag start
				if(bHTML[iCurPos++]==(byte)'<')
				{
					oChunk.iChunkOffset=iCurPos-1;

					return GetNextTag();
				}
			}

			return null;
		}

		/// <summary>
		/// Parses next chunk and returns it with 
		/// </summary>
		/// <returns>HTMLchunk or null if end of data reached</returns>
		public HTMLchunk ParseNext()
		{
			if(iCurPos>=iDataLength)
				return null;

			oChunk.Clear();
			oChunk.iChunkOffset=iCurPos;

			byte cChar=bHTML[iCurPos++];

			// most likely what we have here is a normal char, 
			if(cChar==(byte)'<')
			{
				// tag parsing route - we know for sure that we have not had some text chars before 
				// that point to worry about
				return GetNextTag();
			}
			else
			{
				// check if it's whitespace - typically happens after tag end and before new tag starts
				// so it makes sense make it special case
				if(bCompressWhiteSpaceBeforeTag && cChar<=32 && bWhiteSpace[cChar]==1)
				{
					// ok first char is empty space, this can often lead to new tag
					// thus causing us to create essentially empty strings where as we could have
					// returned fixed single space string when it is necessary

					while(iCurPos<iDataLength)
					{
						cChar=bHTML[iCurPos++];			

						if(cChar<=32 && bWhiteSpace[cChar]==1)
							continue;

						// ok we got tag, but all we had before it was spaces, most likely end of lines
						// so we will return compact representation of that text data
						if(cChar==(byte)'<')
						{
							iCurPos--;

							oChunk.oType=HTMLchunkType.Text;
							oChunk.oHTML=" ";
							
							return oChunk;
						}

						break;
					}
				   
				}

				// ok normal text, we just scan it until tag or end of text
				// statistically this loop will have plenty of iterations
				// thus it makes sense to deal with pointers, we only do that if
				// we have got plenty of bytes to scan left
				int iQuadBytes=((iDataLength-iCurPos)>>2)-1;

				if(!oE.bDecodeEntities && !oE.bMiniEntities)
				{
					while(iCurPos<iDataLength)
					{
						// ok we got tag, but all we had before it was spaces, most likely end of lines
						// so we will return compact representation of that text data
						if(bHTML[iCurPos++]==(byte)'<')
						{
							iCurPos--;
							break;
						}
					}

				}
				else
				{
					// TODO: might help skipping data in quads but we need to perfect bitmap operations for that:
					// stop when at least one & or < is detected in quad
					/*
					fixed(byte* bpData=&bHTML[iCurPos])
					{
						uint* uiData=(uint*)bpData;

						for(int i=0; i<iQuadBytes; i++)
						{
							// use bitmask operation to quickly check if any of the 4 bytes
							// has got < in them - should be FAIRLY unlikely thus allowing us to skip
							// few bytes in one go
							if((~(*uiData &  0x3C3C3C3C)) )
							{
								iCurPos+=4;
								uiData++;
								continue;
							}

							break;
						}

					}
					 */

					// we might have entity here, which is first char of the text:
					if(cChar==(byte)'&')
					{
						int iLastCurPos=iCurPos-1;

						char cEntityChar=oE.CheckForEntity(bHTML,ref iCurPos,iDataLength);

						// restore current symbol
						if(cEntityChar!=0)
						{
							// ok, we have got entity on our hand, it means that we can't just copy
							// data from start of the buffer to end of text thereby avoiding having to
							// accumulate same data elsewhere
							sText.Clear();

							oChunk.bEntities=true;

							if(cEntityChar==(byte)'<')
								oChunk.bLtEntity=true;

							sText.Append(cEntityChar);

							return ParseTextWithEntities();
						}
					}

					while(iCurPos<iDataLength)
					{
						cChar=bHTML[iCurPos++];

						// ok we got tag, but all we had before it was spaces, most likely end of lines
						// so we will return compact representation of that text data
						if(cChar==(byte)'<')
						{
							iCurPos--;
							break;
						}

						// check if we got entity
						if(cChar==(byte)'&')
						{
							int iLastCurPos=iCurPos-1;

							char cEntityChar=oE.CheckForEntity(bHTML,ref iCurPos,iDataLength);

							// restore current symbol
							if(cEntityChar!=0)
							{
								// ok, we have got entity on our hand, it means that we can't just copy
								// data from start of the buffer to end of text thereby avoiding having to
								// accumulate same data elsewhere
								sText.Clear();

								int iLen=iLastCurPos-oChunk.iChunkOffset;

								if(iLen>0)
								{
									Array.Copy(bHTML,oChunk.iChunkOffset,sText.bBuffer,0,iLen);
									sText.iBufPos=iLen;
								}

								oChunk.bEntities=true;

								if(cEntityChar==(byte)'<')
									oChunk.bLtEntity=true;

								sText.Append(cEntityChar);

								return ParseTextWithEntities();
							}
						}
					}

				}

				oChunk.iChunkLength=iCurPos-oChunk.iChunkOffset;

				if(oChunk.iChunkLength==0)
					return null;

				oChunk.oType=HTMLchunkType.Text;
                // oChunk.oHTML = oEnc.GetString(bHTML, oChunk.iChunkOffset, oChunk.iChunkLength);
                oChunk.oHTML = m_OriginalHtml.Substring(oChunk.iChunkOffset, oChunk.iChunkLength);
				return oChunk;
			}

		}


		HTMLchunk ParseTextWithEntities()
		{
			// okay, now that we got our first entity we will need to continue
			// parsing by copying data into temporary buffer and when finished
			// convert it to string
			while(iCurPos<iDataLength)
			{
				byte cChar=bHTML[iCurPos++];

				// ok we got tag, but all we had before it was spaces, most likely end of lines
				// so we will return compact representation of that text data
				if(cChar==(byte)'<')
				{
					iCurPos--;
					break;
				}

				// check if we got entity again
				if(cChar==(byte)'&')
				{
					char cNewEntityChar=oE.CheckForEntity(bHTML,ref iCurPos,iDataLength);

					// restore current symbol
					if(cNewEntityChar!=0)
					{
						if(cNewEntityChar==(byte)'<')
							oChunk.bLtEntity=true;

						sText.Append(cNewEntityChar);

                        // we continue here since we fully parsed entity
                        continue;
					}

                    // ok we did not parse entity in which case we add & char and continue along the way
                    sText.bBuffer[sText.iBufPos++]=cChar;
                    continue;
				}

				sText.bBuffer[sText.iBufPos++]=cChar;
			}

			oChunk.iChunkLength=iCurPos-oChunk.iChunkOffset;

			oChunk.oType=HTMLchunkType.Text;
			oChunk.oHTML=sText.SetToString();

			return oChunk;
		}

		/// <summary>
		/// Internally parses tag and returns it from point when '<' was found
		/// </summary>
		/// <returns>Chunk</returns>
		HTMLchunk GetNextTag()
		{
			//iCurPos++;

			oChunk=oTP.ParseTag(ref iCurPos);

			// for backwards compatibility mark closed tags with params as open
			if(oChunk.iParams>0 && bAutoMarkClosedTagsWithParamsAsOpen && oChunk.oType==HTMLchunkType.CloseTag)
				oChunk.oType=HTMLchunkType.OpenTag;

			//                    012345
			// check for start of script
			if(oChunk.sTag.Length==6 && oChunk.sTag[0]=='s' && oChunk.sTag=="script")
			{
				if(!oChunk.bClosure)
				{
					oChunk.oType=HTMLchunkType.Script;
					oChunk=oTP.ParseScript(ref iCurPos);
					return oChunk;
				}
			}

			oChunk.iChunkLength=iCurPos-oChunk.iChunkOffset;

			if(bKeepRawHTML)
				oChunk.oHTML=oEnc.GetString(bHTML,oChunk.iChunkOffset,oChunk.iChunkLength);

			return oChunk;

		}

		/// <summary>
		/// Parses WIDTH param and calculates width
		/// </summary>
		/// <param name="sWidth">WIDTH param from tag</param>
		/// <param name="iAvailWidth">Currently available width for relative calculations, if negative width will be returned as is</param>
		/// <param name="bRelative">Flag that will be set to true if width was relative</param>
		/// <returns>Width in pixels</returns>
		public static int CalculateWidth(string sWidth,int iAvailWidth,ref bool bRelative)
		{
			sWidth=sWidth.Trim();

			if(sWidth.Length==0)
				return iAvailWidth;

			try
			{
				// check if its relative %-t				
				bRelative=false;
				
				for(int i=0; i<sWidth.Length; i++)
				{
					if(sWidth[i]=='%')
						bRelative=true;

					if(!char.IsNumber(sWidth[i]))
					{
						sWidth=sWidth.Substring(0,i);
						break;
					}

				}

				int iValue=int.Parse(sWidth);

				if(bRelative && iAvailWidth>0)
					return iValue*iAvailWidth/100;
				else
					return iValue;

			}
			catch{}

			return iAvailWidth;
		}

		/// <summary>
		/// This function will decode any entities found in a string - not fast!
		/// </summary>
		/// <returns>Possibly decoded string</returns>
		public static string DecodeEntities(string sData)
		{
			return HTMLentities.DecodeEntities(sData);
		}

		/// <summary>
		/// Loads HTML from file
		/// </summary>
		/// <param name="sFileName">Full filename</param>
		public void LoadFromFile(string sFileName)
		{
			CleanUp();

			using(FileStream oFS=File.OpenRead(sFileName))
			{
				byte[] bData=new byte[oFS.Length];
	
				int iRead=oFS.Read(bData,0,bData.Length);

				if(iRead!=bData.Length)
					throw new Exception("Number of bytes read is less than expected number of bytes in a file!");

				Init(bData);
			}

			return;
		}

		/// <summary>
		/// Handles META tags that set page encoding
		/// </summary>
		/// <param name="oP">HTML parser object that is used for parsing</param>
		/// <param name="oChunk">Parsed chunk that should contain tag META</param>
		/// <param name="bEncodingSet">Your own flag that shows whether encoding was already set or not, if set
		/// once then it should not be changed - this is the logic applied by major browsers</param>
		/// <returns>True if this was META tag setting Encoding, false otherwise</returns>
		public static bool HandleMetaEncoding(HTMLparser oP,HTMLchunk oChunk,ref bool bEncodingSet)
		{
			if(oChunk.sTag.Length!=4 || oChunk.sTag[0]!='m' || oChunk.sTag!="meta")
				return false;

			// if we do not use hashmode already then we call conversion explicitly
			// this is slow, but METAs are very rare so performance penalty is low
			if(!oChunk.bHashMode)
				oChunk.ConvertParamsToHash();

			string sKey=oChunk.oParams["http-equiv"] as string;

			if(sKey!=null)
			{

				// FIXIT: even though this is happening rare I really don't like lower casing stuff
				// that most likely would not need to be - if you feel bored then rewrite this bit
				// to make it faster, it is really easy...
				switch(sKey.ToLower())
				{
					case "content-type":
					// rare case (appears to work in IE) reported to exist in some pages by Martin B�chtold
					case "content-category":

						// we might have charset here that may hint at necessity to decode page
						// check for possible encoding change

						// once encoding is set it should not be changed, but you can be damn
						// sure there are web pages out there that do that!!!
						if(!bEncodingSet)
						{
							string sData=oChunk.oParams["content"] as string;

							// it is possible we have broken META tag without Content part
							if(sData!=null)
							{

								if(oP.SetEncoding(sData))
								{
									// we may need to re-encode title

									if(!bEncodingSet)
									{
										// here you need to reencode any text that you found so far
										// most likely it will be just TITLE, the rest can be ignored anyway
										bEncodingSet=true;
									}
								}
								else
								{
									// failed to set encoding - most likely encoding string
									// was incorrect or your machine lacks codepages or something
									// else - might be good idea to put warning message here
								}
							}

						}

						return true;

					default:
						break;
				};


			}

			return false;
		}

	
	}

}

// THE END ... is just a new beginning