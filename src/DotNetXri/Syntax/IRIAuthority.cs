/*
 * Copyright 2005 OpenXRI Foundation
 * Subsequently ported and altered by Andrew Arnott and Troels Thomsen
 *
 * Licensed under the Apache License, Version 2.0 (the "License");
 * you may not use this file except in compliance with the License.
 * You may obtain a copy of the License at
 *
 *     http://www.apache.org/licenses/LICENSE-2.0
 *
 * Unless required by applicable law or agreed to in writing, software
 * distributed under the License is distributed on an "AS IS" BASIS,
 * WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
 * See the License for the specific language governing permissions and
 * limitations under the License.
 */

using System;
using System.Net;

namespace DotNetXri.Syntax
{
	/// <summary>
	/// This class provides a strong typing for a IRI Authority.  Any
	/// obj of this class that appears outside of the package is a valid
	/// IRI Authority.  It currently only accepts IRI Authorities that serve as IP
	/// Addresses or appear to be valid host names
	/// </summary>
	public class IRIAuthority : AuthorityPath
	{
		private Uri moURI = null;

		public IRIAuthority(String sPath)
			: base(sPath)
		{
			parse();
		}

		internal IRIAuthority()
		{ }

		/// <summary>
		/// The user info portion of the IRI Authority
		/// </summary>
		public string IUserInfo
		{
			get
			{
				return moURI.UserInfo;
			}
		}

		/// <summary>
		/// The host portion of the IRI Authority
		/// </summary>
		public string IHost
		{
			get
			{
				return moURI.Host;
			}
		}

		/// <summary>
		/// The port portion of the IRI Authority
		/// </summary>
		public int Port
		{
			get
			{
				return moURI.Port;
			}
		}

		/// <summary>
		/// Scans the Stream for a valid IRI-Authority
		/// </summary>
		/// <param name="oStream"></param>
		/// <returns></returns>
		bool doScan(ParseStream oStream)
		{
			bool bVal = false;
			int n = scanChars(oStream.getData());
			string sData = oStream.getData().Substring(0, n);
			try
			{
				moURI = new Uri("http", sData, null, null, null);
				String sHost = moURI.Host;
				if ((sHost != null) && (sHost.Length > 0))
				{
					char cFirst = sHost[0];
					bool bCheckIP = char.IsDigit(cFirst) ||
						(cFirst == '[');
					bVal = bCheckIP ? verifyIP(sHost) : verifyDNS(sHost);
				}
			}
			catch (UriFormatException e)
			{ }

			// consume and return true if valid
			if (bVal)
			{
				oStream.consume(n);
				return true;
			}

			return false;
		}

		private bool verifyDNS(string sHost)
		{
			// TODO Auto-generated method stub
			return true;
		}

		private bool verifyIP(string sIP)
		{
			try
			{
				IPAddress oAddr =  IPAddress.Parse(sIP);
				return oAddr != null;
			}
			catch (FormatException)
			{ }
			return false;
		}

		private int scanChars(string s)
		{
			for (int i = 0; i < s.Length; i++)
			{
				char c = s[i];

				// not exactly spec compliant, but does the right thing
				switch (c)
				{
					case '/':
					case ')':
					case '#':
					case '?':
						break;
					default:
						continue;
				}

				return i;
			}

			return s.Length;
		}

		/// <summary>
		/// Serializes the IRIAuthority into IRI normal from
		/// </summary>
		/// <returns>The IRI normal form of the IRIAuthority</returns>
		public string toIRINormalForm()
		{
			return IRIUtils.XRItoIRI(ToString(), false);
		}
		
		/// <summary>
		/// Serializes the IRIAuthority into URI normal from
		/// </summary>
		/// <returns>The URI normal form of the IRIAuthority</returns>
		public string toURINormalForm()
		{
			return IRIUtils.IRItoURI(toIRINormalForm());
		}
	}
}