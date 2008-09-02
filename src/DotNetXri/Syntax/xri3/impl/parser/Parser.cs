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
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Text;
using DotNetXri.Syntax.Xri3.Impl.Parser;

namespace DotNetXri.Syntax.Xri3.Impl.Parser
{
	public class Parser
	{
		static public void Main(string[] args)
		{
			Properties arguments = new Properties();
			string error = "";
			bool ok = args.Length > 0;

			if (ok)
			{
				arguments.setProperty("Trace", "Off");
				arguments.setProperty("Rule", "xri");

				for (int i = 0; i < args.Length; i++)
				{
					if (args[i].Equals("-trace"))
						arguments.setProperty("Trace", "On");
					else if (args[i].Equals("-visitor"))
						arguments.setProperty("Visitor", args[++i]);
					else if (args[i].Equals("-file"))
						arguments.setProperty("File", args[++i]);
					else if (args[i].Equals("-string"))
						arguments.setProperty("String", args[++i]);
					else if (args[i].Equals("-rule"))
						arguments.setProperty("Rule", args[++i]);
					else
					{
						error = "unknown argument: " + args[i];
						ok = false;
					}
				}
			}

			if (arguments.getProperty("File") == null &&
				arguments.getProperty("String") == null)
			{
				error = "insufficient arguments";
				ok = false;
			}

			if (!ok)
			{
				Logger.Info("error: " + error);
				Logger.Info("usage: Parser [-rule rulename] [-trace] <-file file | -string string> [-visitor visitor]");
			}
			else
			{
				try
				{
					Parser parser = new Parser();
					Rule rule = null;

					parser.traceOff();
					if (arguments.getProperty("Trace").equals("On"))
						parser.traceOn();

					if (arguments.getProperty("File") != null)
						rule = parser.parse(arguments.getProperty("Rule"), File.OpenRead(arguments.getProperty("File")));
					else if (arguments.getProperty("String") != null)
						rule = parser.parse(arguments.getProperty("Rule"), arguments.getProperty("String"));

					if (arguments.getProperty("Visitor") != null)
					{
						Visitor visitor =
						  (Visitor)Activator.CreateInstance(Type.GetType(arguments.getProperty("Visitor")));
						visitor.visit(rule);
					}
				}
				catch (ArgumentException e)
				{
					Logger.Info("argument error: " + e.Message);
				}
				catch (IOException e)
				{
					Logger.Info("io error: " + e.Message);
				}
				catch (ParserException e)
				{
					Logger.Info("parser error: " + e.Message);
				}
				catch (TypeLoadException e)
				{
					Logger.Info("visitor error: class not found - " + e.Message);
				}
				catch (MethodAccessException e)
				{
					Logger.Info("visitor error: illegal access - " + e.Message);
				}
				catch (TargetInvocationException e)
				{
					Logger.Info("visitor error: instantiation failure - " + e.Message);
				}
			}
		}

		/* ---------------------------------------------------------------------------
		 * public parsers
		 * ---------------------------------------------------------------------------
		 */

		public Rule parse(string rulename, string value)
		{
			if (rulename == null)
				throw new ArgumentNullException("null rulename");
			if (value == null)
				throw new ArgumentNullException("null string");

			return decode(rulename, value);
		}

		public Rule parse(string rulename, Stream inValue)
		{
			if (rulename == null)
				throw new ArgumentNullException("null rulename");
			if (inValue == null)
				throw new ArgumentNullException("null input stream");

			int ch = 0;
			StringBuilder outValue = new StringBuilder();

			TextReader reader = new StreamReader(inValue);

			while ((ch = reader.Read()) != -1)
				outValue.Append((char)ch);

			return decode(rulename, outValue.ToString());
		}

		/* ---------------------------------------------------------------------------
		 * private data
		 * ---------------------------------------------------------------------------
		 */

		private string text;
		private int index = 0;

		private bool trace = false;
		private int level = 0;
		private int error = -1;
		private Stack<string> callStack = new Stack<string>();
		private Stack<string> errorStack = new Stack<string>();

		private const string newline = Environment.NewLine;

		/* ---------------------------------------------------------------------------
		 * private trace
		 * ---------------------------------------------------------------------------
		 */

		private void traceOn()
		{
			trace = true;
		}
		private void traceOff()
		{
			trace = false;
		}

		private void push(string function)
		{
			callStack.Push(function);
			if (trace)
			{
				Logger.Info("-> " + ++level + ": " + function + "()");
				Logger.Info(index + ": " + text.Substring(index, index + 10 > text.Length ? text.Length - index : 10).replaceAll("[^\\p{Print}]", " "));
			}
		}

		private void push(string function, string regex)
		{
			callStack.Push(function);
			if (trace)
			{
				Logger.Info("-> " + ++level + ": " + function + "(" + regex + ")");
				Logger.Info(index + ": " + text.Substring(index, index + 10 > text.Length ? text.Length - index : 10).replaceAll("[^\\p{Print}]", " "));
			}
		}

		private void push(string function, string spelling, string regex)
		{
			callStack.Push(function);
			if (trace)
			{
				Logger.Info("-> " + ++level + ": " + function + "(" + spelling + ", " + regex + ")");
				Logger.Info(index + ": " + text.Substring(index, index + 10 > text.Length ? text.Length - index : 10).replaceAll("[^\\p{Print}]", " "));
			}
		}

		private void pop(String function, bool result, int length)
		{
			callStack.Pop();
			if (trace)
			{
				Logger.Info("<- " + level-- + ": " + function + "(" + (result ? "true," : "false,") + length + ")");
			}
			if (!result)
			{
				if (index > error)
				{
					error = index;
					errorStack = new Stack<string>();
					errorStack.addAll(callStack);
				}
			}
			else
			{
				if (index > error)
					error = -1;
			}
		}

		/* ---------------------------------------------------------------------------
		 * private decoders
		 * ---------------------------------------------------------------------------
		 */

		private Rule decode(string rulename, string text)
		{
			this.text = text;

			Rule rule = null;
			if (true == false)
				;
			else if (rulename.Equals("xri", StringComparison.InvariantCultureIgnoreCase))
				rule = decode_xri();
			else if (rulename.Equals("xri-scheme", StringComparison.InvariantCultureIgnoreCase))
				rule = decode_xri_scheme();
			else if (rulename.Equals("xri-noscheme", StringComparison.InvariantCultureIgnoreCase))
				rule = decode_xri_noscheme();
			else if (rulename.Equals("xri-reference", StringComparison.InvariantCultureIgnoreCase))
				rule = decode_xri_reference();
			else if (rulename.Equals("relative-xri-ref", StringComparison.InvariantCultureIgnoreCase))
				rule = decode_relative_xri_ref();
			else if (rulename.Equals("relative-xri-part", StringComparison.InvariantCultureIgnoreCase))
				rule = decode_relative_xri_part();
			else if (rulename.Equals("xri-hier-part", StringComparison.InvariantCultureIgnoreCase))
				rule = decode_xri_hier_part();
			else if (rulename.Equals("xri-authority", StringComparison.InvariantCultureIgnoreCase))
				rule = decode_xri_authority();
			else if (rulename.Equals("subseg", StringComparison.InvariantCultureIgnoreCase))
				rule = decode_subseg();
			else if (rulename.Equals("global-subseg", StringComparison.InvariantCultureIgnoreCase))
				rule = decode_global_subseg();
			else if (rulename.Equals("local-subseg", StringComparison.InvariantCultureIgnoreCase))
				rule = decode_local_subseg();
			else if (rulename.Equals("gcs-char", StringComparison.InvariantCultureIgnoreCase))
				rule = decode_gcs_char();
			else if (rulename.Equals("lcs-char", StringComparison.InvariantCultureIgnoreCase))
				rule = decode_lcs_char();
			else if (rulename.Equals("rel-subseg", StringComparison.InvariantCultureIgnoreCase))
				rule = decode_rel_subseg();
			else if (rulename.Equals("rel-subseg-nc", StringComparison.InvariantCultureIgnoreCase))
				rule = decode_rel_subseg_nc();
			else if (rulename.Equals("literal", StringComparison.InvariantCultureIgnoreCase))
				rule = decode_literal();
			else if (rulename.Equals("literal-nc", StringComparison.InvariantCultureIgnoreCase))
				rule = decode_literal_nc();
			else if (rulename.Equals("xref", StringComparison.InvariantCultureIgnoreCase))
				rule = decode_xref();
			else if (rulename.Equals("xref-empty", StringComparison.InvariantCultureIgnoreCase))
				rule = decode_xref_empty();
			else if (rulename.Equals("xref-xri-reference", StringComparison.InvariantCultureIgnoreCase))
				rule = decode_xref_xri_reference();
			else if (rulename.Equals("xref-IRI", StringComparison.InvariantCultureIgnoreCase))
				rule = decode_xref_IRI();
			else if (rulename.Equals("xref-value", StringComparison.InvariantCultureIgnoreCase))
				rule = decode_xref_value();
			else if (rulename.Equals("xri-path", StringComparison.InvariantCultureIgnoreCase))
				rule = decode_xri_path();
			else if (rulename.Equals("xri-path-abempty", StringComparison.InvariantCultureIgnoreCase))
				rule = decode_xri_path_abempty();
			else if (rulename.Equals("xri-path-abs", StringComparison.InvariantCultureIgnoreCase))
				rule = decode_xri_path_abs();
			else if (rulename.Equals("xri-path-noscheme", StringComparison.InvariantCultureIgnoreCase))
				rule = decode_xri_path_noscheme();
			else if (rulename.Equals("xri-segment", StringComparison.InvariantCultureIgnoreCase))
				rule = decode_xri_segment();
			else if (rulename.Equals("xri-segment-nz", StringComparison.InvariantCultureIgnoreCase))
				rule = decode_xri_segment_nz();
			else if (rulename.Equals("xri-segment-nc", StringComparison.InvariantCultureIgnoreCase))
				rule = decode_xri_segment_nc();
			else if (rulename.Equals("xri-pchar", StringComparison.InvariantCultureIgnoreCase))
				rule = decode_xri_pchar();
			else if (rulename.Equals("xri-pchar-nc", StringComparison.InvariantCultureIgnoreCase))
				rule = decode_xri_pchar_nc();
			else if (rulename.Equals("xri-reserved", StringComparison.InvariantCultureIgnoreCase))
				rule = decode_xri_reserved();
			else if (rulename.Equals("xri-gen-delims", StringComparison.InvariantCultureIgnoreCase))
				rule = decode_xri_gen_delims();
			else if (rulename.Equals("xri-sub-delims", StringComparison.InvariantCultureIgnoreCase))
				rule = decode_xri_sub_delims();
			else if (rulename.Equals("IRI", StringComparison.InvariantCultureIgnoreCase))
				rule = decode_IRI();
			else if (rulename.Equals("scheme", StringComparison.InvariantCultureIgnoreCase))
				rule = decode_scheme();
			else if (rulename.Equals("ihier-part", StringComparison.InvariantCultureIgnoreCase))
				rule = decode_ihier_part();
			else if (rulename.Equals("iauthority", StringComparison.InvariantCultureIgnoreCase))
				rule = decode_iauthority();
			else if (rulename.Equals("iuserinfo", StringComparison.InvariantCultureIgnoreCase))
				rule = decode_iuserinfo();
			else if (rulename.Equals("ihost", StringComparison.InvariantCultureIgnoreCase))
				rule = decode_ihost();
			else if (rulename.Equals("IP-literal", StringComparison.InvariantCultureIgnoreCase))
				rule = decode_IP_literal();
			else if (rulename.Equals("IPvFuture", StringComparison.InvariantCultureIgnoreCase))
				rule = decode_IPvFuture();
			else if (rulename.Equals("IPv6address", StringComparison.InvariantCultureIgnoreCase))
				rule = decode_IPv6address();
			else if (rulename.Equals("ls32", StringComparison.InvariantCultureIgnoreCase))
				rule = decode_ls32();
			else if (rulename.Equals("h16", StringComparison.InvariantCultureIgnoreCase))
				rule = decode_h16();
			else if (rulename.Equals("IPv4address", StringComparison.InvariantCultureIgnoreCase))
				rule = decode_IPv4address();
			else if (rulename.Equals("dec-octet", StringComparison.InvariantCultureIgnoreCase))
				rule = decode_dec_octet();
			else if (rulename.Equals("ireg-name", StringComparison.InvariantCultureIgnoreCase))
				rule = decode_ireg_name();
			else if (rulename.Equals("port", StringComparison.InvariantCultureIgnoreCase))
				rule = decode_port();
			else if (rulename.Equals("ipath-abempty", StringComparison.InvariantCultureIgnoreCase))
				rule = decode_ipath_abempty();
			else if (rulename.Equals("ipath-abs", StringComparison.InvariantCultureIgnoreCase))
				rule = decode_ipath_abs();
			else if (rulename.Equals("ipath-rootless", StringComparison.InvariantCultureIgnoreCase))
				rule = decode_ipath_rootless();
			else if (rulename.Equals("ipath-empty", StringComparison.InvariantCultureIgnoreCase))
				rule = decode_ipath_empty();
			else if (rulename.Equals("isegment", StringComparison.InvariantCultureIgnoreCase))
				rule = decode_isegment();
			else if (rulename.Equals("isegment-nz", StringComparison.InvariantCultureIgnoreCase))
				rule = decode_isegment_nz();
			else if (rulename.Equals("iquery", StringComparison.InvariantCultureIgnoreCase))
				rule = decode_iquery();
			else if (rulename.Equals("iprivate", StringComparison.InvariantCultureIgnoreCase))
				rule = decode_iprivate();
			else if (rulename.Equals("ifragment", StringComparison.InvariantCultureIgnoreCase))
				rule = decode_ifragment();
			else if (rulename.Equals("ipchar", StringComparison.InvariantCultureIgnoreCase))
				rule = decode_ipchar();
			else if (rulename.Equals("iunreserved", StringComparison.InvariantCultureIgnoreCase))
				rule = decode_iunreserved();
			else if (rulename.Equals("pct-encoded", StringComparison.InvariantCultureIgnoreCase))
				rule = decode_pct_encoded();
			else if (rulename.Equals("ucschar", StringComparison.InvariantCultureIgnoreCase))
				rule = decode_ucschar();
			else if (rulename.Equals("reserved", StringComparison.InvariantCultureIgnoreCase))
				rule = decode_reserved();
			else if (rulename.Equals("gen-delims", StringComparison.InvariantCultureIgnoreCase))
				rule = decode_gen_delims();
			else if (rulename.Equals("sub-delims", StringComparison.InvariantCultureIgnoreCase))
				rule = decode_sub_delims();
			else if (rulename.Equals("unreserved", StringComparison.InvariantCultureIgnoreCase))
				rule = decode_unreserved();
			else if (rulename.Equals("ALPHA", StringComparison.InvariantCultureIgnoreCase))
				rule = decode_ALPHA();
			else if (rulename.Equals("BIT", StringComparison.InvariantCultureIgnoreCase))
				rule = decode_BIT();
			else if (rulename.Equals("CHAR", StringComparison.InvariantCultureIgnoreCase))
				rule = decode_CHAR();
			else if (rulename.Equals("CR", StringComparison.InvariantCultureIgnoreCase))
				rule = decode_CR();
			else if (rulename.Equals("CRLF", StringComparison.InvariantCultureIgnoreCase))
				rule = decode_CRLF();
			else if (rulename.Equals("CTL", StringComparison.InvariantCultureIgnoreCase))
				rule = decode_CTL();
			else if (rulename.Equals("DIGIT", StringComparison.InvariantCultureIgnoreCase))
				rule = decode_DIGIT();
			else if (rulename.Equals("DQUOTE", StringComparison.InvariantCultureIgnoreCase))
				rule = decode_DQUOTE();
			else if (rulename.Equals("HEXDIG", StringComparison.InvariantCultureIgnoreCase))
				rule = decode_HEXDIG();
			else if (rulename.Equals("HTAB", StringComparison.InvariantCultureIgnoreCase))
				rule = decode_HTAB();
			else if (rulename.Equals("LF", StringComparison.InvariantCultureIgnoreCase))
				rule = decode_LF();
			else if (rulename.Equals("LWSP", StringComparison.InvariantCultureIgnoreCase))
				rule = decode_LWSP();
			else if (rulename.Equals("OCTET", StringComparison.InvariantCultureIgnoreCase))
				rule = decode_OCTET();
			else if (rulename.Equals("SP", StringComparison.InvariantCultureIgnoreCase))
				rule = decode_SP();
			else if (rulename.Equals("VCHAR", StringComparison.InvariantCultureIgnoreCase))
				rule = decode_VCHAR();
			else if (rulename.Equals("WSP", StringComparison.InvariantCultureIgnoreCase))
				rule = decode_WSP();
			else
				throw new ArgumentException("unknown rule");

			if (rule == null)
			{
				string marker = "                              ";
				StringBuilder errorBuffer = new StringBuilder();
				int start = (error < 30) ? 0 : error - 30;
				int end = (text.Length < error + 30) ? text.Length : error + 30;

				errorBuffer.Append("rule \"" + (string)errorStack.Peek() + "\" failed" + newline);
				errorBuffer.Append(text.Substring(start, end - start).replaceAll("[^\\p{Print}]", " ") + newline);
				errorBuffer.Append(marker.Substring(0, error < 30 ? error : 30) + "^" + newline);
				errorBuffer.Append("rule stack:");

				for (IEnumerator<string> i = errorStack.GetEnumerator(); i.MoveNext(); )
					errorBuffer.Append(newline + "  " + i.Current);

				throw new ParserException(errorBuffer.ToString());
			}

			if (text.Length > index)
			{
				string marker = "                              ";
				StringBuilder errorBuffer = new StringBuilder();
				int start = (index < 30) ? 0 : index - 30;
				int end = (text.Length < index + 30) ? text.Length : index + 30;

				errorBuffer.Append("extra data found" + newline);
				errorBuffer.Append(text.Substring(start, end - start).replaceAll("[^\\p{Print}]", " ") + newline);
				errorBuffer.Append(marker.Substring(0, index < 30 ? index : 30) + "^" + newline);

				throw new ParserException(errorBuffer.ToString());
			}

			return rule;
		}

		private xri decode_xri()
		{
			push("xri");

			bool decoded = true;
			int s0 = index;
			var e0 = new List<Rule>();
			Rule rule;

			decoded = false;
			if (!decoded)
			{
				{
					var e1 = new List<Rule>();
					int s1 = index;
					decoded = true;
					if (decoded)
					{
						bool f1 = true;
						int c1 = 0;
						for (int i1 = 0; i1 < 1 && f1; i1++)
						{
							rule = decode_xri_scheme();
							if ((f1 = rule != null))
							{
								e1.Add(rule);
								c1++;
							}
						}
						decoded = c1 == 1;
					}
					if (decoded)
						e0.AddRange(e1);
					else
						index = s1;
				}
			}
			if (!decoded)
			{
				{
					var e1 = new List<Rule>();
					int s1 = index;
					decoded = true;
					if (decoded)
					{
						bool f1 = true;
						int c1 = 0;
						for (int i1 = 0; i1 < 1 && f1; i1++)
						{
							rule = decode_xri_noscheme();
							if ((f1 = rule != null))
							{
								e1.Add(rule);
								c1++;
							}
						}
						decoded = c1 == 1;
					}
					if (decoded)
						e0.AddRange(e1);
					else
						index = s1;
				}
			}

			rule = null;
			if (decoded)
				rule = new xri(text.Substring(s0, index - s0), e0);
			else
				index = s0;

			pop("xri", decoded, index - s0);

			return (xri)rule;
		}

		private xri_scheme decode_xri_scheme()
		{
			push("xri-scheme");

			bool decoded = true;
			int s0 = index;
			var e0 = new List<Rule>();
			Rule rule;

			decoded = false;
			if (!decoded)
			{
				{
					var e1 = new List<Rule>();
					int s1 = index;
					decoded = true;
					if (decoded)
					{
						bool f1 = true;
						int c1 = 0;
						for (int i1 = 0; i1 < 1 && f1; i1++)
						{
							rule = decode_StringValue("xri:");
							if ((f1 = rule != null))
							{
								e1.Add(rule);
								c1++;
							}
						}
						decoded = c1 == 1;
					}
					if (decoded)
					{
						bool f1 = true;
						int c1 = 0;
						for (int i1 = 0; i1 < 1 && f1; i1++)
						{
							rule = decode_xri_noscheme();
							if ((f1 = rule != null))
							{
								e1.Add(rule);
								c1++;
							}
						}
						decoded = c1 == 1;
					}
					if (decoded)
						e0.AddRange(e1);
					else
						index = s1;
				}
			}

			rule = null;
			if (decoded)
				rule = new xri_scheme(text.Substring(s0, index - s0), e0);
			else
				index = s0;

			pop("xri-scheme", decoded, index - s0);

			return (xri_scheme)rule;
		}

		private xri_noscheme decode_xri_noscheme()
		{
			push("xri-noscheme");

			bool decoded = true;
			int s0 = index;
			var e0 = new List<Rule>();
			Rule rule;

			decoded = false;
			if (!decoded)
			{
				{
					var e1 = new List<Rule>();
					int s1 = index;
					decoded = true;
					if (decoded)
					{
						bool f1 = true;
						int c1 = 0;
						for (int i1 = 0; i1 < 1 && f1; i1++)
						{
							rule = decode_xri_hier_part();
							if ((f1 = rule != null))
							{
								e1.Add(rule);
								c1++;
							}
						}
						decoded = c1 == 1;
					}
					if (decoded)
					{
						bool f1 = true;
						int c1 = 0;
						for (int i1 = 0; i1 < 1 && f1; i1++)
						{
							decoded = false;
							if (!decoded)
							{
								{
									var e2 = new List<Rule>();
									int s2 = index;
									decoded = true;
									if (decoded)
									{
										bool f2 = true;
										int c2 = 0;
										for (int i2 = 0; i2 < 1 && f2; i2++)
										{
											rule = decode_StringValue("?");
											if ((f2 = rule != null))
											{
												e2.Add(rule);
												c2++;
											}
										}
										decoded = c2 == 1;
									}
									if (decoded)
									{
										bool f2 = true;
										int c2 = 0;
										for (int i2 = 0; i2 < 1 && f2; i2++)
										{
											rule = decode_iquery();
											if ((f2 = rule != null))
											{
												e2.Add(rule);
												c2++;
											}
										}
										decoded = c2 == 1;
									}
									if (decoded)
										e1.AddRange(e2);
									else
										index = s2;
								}
							}
							if (decoded)
								c1++;
							f1 = decoded;
						}
						decoded = true;
					}
					if (decoded)
					{
						bool f1 = true;
						int c1 = 0;
						for (int i1 = 0; i1 < 1 && f1; i1++)
						{
							decoded = false;
							if (!decoded)
							{
								{
									var e2 = new List<Rule>();
									int s2 = index;
									decoded = true;
									if (decoded)
									{
										bool f2 = true;
										int c2 = 0;
										for (int i2 = 0; i2 < 1 && f2; i2++)
										{
											rule = decode_StringValue("#");
											if ((f2 = rule != null))
											{
												e2.Add(rule);
												c2++;
											}
										}
										decoded = c2 == 1;
									}
									if (decoded)
									{
										bool f2 = true;
										int c2 = 0;
										for (int i2 = 0; i2 < 1 && f2; i2++)
										{
											rule = decode_ifragment();
											if ((f2 = rule != null))
											{
												e2.Add(rule);
												c2++;
											}
										}
										decoded = c2 == 1;
									}
									if (decoded)
										e1.AddRange(e2);
									else
										index = s2;
								}
							}
							if (decoded)
								c1++;
							f1 = decoded;
						}
						decoded = true;
					}
					if (decoded)
						e0.AddRange(e1);
					else
						index = s1;
				}
			}

			rule = null;
			if (decoded)
				rule = new xri_noscheme(text.Substring(s0, index - s0), e0);
			else
				index = s0;

			pop("xri-noscheme", decoded, index - s0);

			return (xri_noscheme)rule;
		}

		private xri_reference decode_xri_reference()
		{
			push("xri-reference");

			bool decoded = true;
			int s0 = index;
			var e0 = new List<Rule>();
			Rule rule;

			decoded = false;
			if (!decoded)
			{
				{
					var e1 = new List<Rule>();
					int s1 = index;
					decoded = true;
					if (decoded)
					{
						bool f1 = true;
						int c1 = 0;
						for (int i1 = 0; i1 < 1 && f1; i1++)
						{
							rule = decode_xri();
							if ((f1 = rule != null))
							{
								e1.Add(rule);
								c1++;
							}
						}
						decoded = c1 == 1;
					}
					if (decoded)
						e0.AddRange(e1);
					else
						index = s1;
				}
			}
			if (!decoded)
			{
				{
					var e1 = new List<Rule>();
					int s1 = index;
					decoded = true;
					if (decoded)
					{
						bool f1 = true;
						int c1 = 0;
						for (int i1 = 0; i1 < 1 && f1; i1++)
						{
							rule = decode_relative_xri_ref();
							if ((f1 = rule != null))
							{
								e1.Add(rule);
								c1++;
							}
						}
						decoded = c1 == 1;
					}
					if (decoded)
						e0.AddRange(e1);
					else
						index = s1;
				}
			}

			rule = null;
			if (decoded)
				rule = new xri_reference(text.Substring(s0, index - s0), e0);
			else
				index = s0;

			pop("xri-reference", decoded, index - s0);

			return (xri_reference)rule;
		}

		private relative_xri_ref decode_relative_xri_ref()
		{
			push("relative-xri-ref");

			bool decoded = true;
			int s0 = index;
			var e0 = new List<Rule>();
			Rule rule;

			decoded = false;
			if (!decoded)
			{
				{
					var e1 = new List<Rule>();
					int s1 = index;
					decoded = true;
					if (decoded)
					{
						bool f1 = true;
						int c1 = 0;
						for (int i1 = 0; i1 < 1 && f1; i1++)
						{
							rule = decode_relative_xri_part();
							if ((f1 = rule != null))
							{
								e1.Add(rule);
								c1++;
							}
						}
						decoded = c1 == 1;
					}
					if (decoded)
					{
						bool f1 = true;
						int c1 = 0;
						for (int i1 = 0; i1 < 1 && f1; i1++)
						{
							decoded = false;
							if (!decoded)
							{
								{
									var e2 = new List<Rule>();
									int s2 = index;
									decoded = true;
									if (decoded)
									{
										bool f2 = true;
										int c2 = 0;
										for (int i2 = 0; i2 < 1 && f2; i2++)
										{
											rule = decode_StringValue("?");
											if ((f2 = rule != null))
											{
												e2.Add(rule);
												c2++;
											}
										}
										decoded = c2 == 1;
									}
									if (decoded)
									{
										bool f2 = true;
										int c2 = 0;
										for (int i2 = 0; i2 < 1 && f2; i2++)
										{
											rule = decode_iquery();
											if ((f2 = rule != null))
											{
												e2.Add(rule);
												c2++;
											}
										}
										decoded = c2 == 1;
									}
									if (decoded)
										e1.AddRange(e2);
									else
										index = s2;
								}
							}
							if (decoded)
								c1++;
							f1 = decoded;
						}
						decoded = true;
					}
					if (decoded)
					{
						bool f1 = true;
						int c1 = 0;
						for (int i1 = 0; i1 < 1 && f1; i1++)
						{
							decoded = false;
							if (!decoded)
							{
								{
									var e2 = new List<Rule>();
									int s2 = index;
									decoded = true;
									if (decoded)
									{
										bool f2 = true;
										int c2 = 0;
										for (int i2 = 0; i2 < 1 && f2; i2++)
										{
											rule = decode_StringValue("#");
											if ((f2 = rule != null))
											{
												e2.Add(rule);
												c2++;
											}
										}
										decoded = c2 == 1;
									}
									if (decoded)
									{
										bool f2 = true;
										int c2 = 0;
										for (int i2 = 0; i2 < 1 && f2; i2++)
										{
											rule = decode_ifragment();
											if ((f2 = rule != null))
											{
												e2.Add(rule);
												c2++;
											}
										}
										decoded = c2 == 1;
									}
									if (decoded)
										e1.AddRange(e2);
									else
										index = s2;
								}
							}
							if (decoded)
								c1++;
							f1 = decoded;
						}
						decoded = true;
					}
					if (decoded)
						e0.AddRange(e1);
					else
						index = s1;
				}
			}

			rule = null;
			if (decoded)
				rule = new relative_xri_ref(text.Substring(s0, index - s0), e0);
			else
				index = s0;

			pop("relative-xri-ref", decoded, index - s0);

			return (relative_xri_ref)rule;
		}

		private relative_xri_part decode_relative_xri_part()
		{
			push("relative-xri-part");

			bool decoded = true;
			int s0 = index;
			var e0 = new List<Rule>();
			Rule rule;

			decoded = false;
			if (!decoded)
			{
				{
					var e1 = new List<Rule>();
					int s1 = index;
					decoded = true;
					if (decoded)
					{
						bool f1 = true;
						int c1 = 0;
						for (int i1 = 0; i1 < 1 && f1; i1++)
						{
							rule = decode_xri_path_abs();
							if ((f1 = rule != null))
							{
								e1.Add(rule);
								c1++;
							}
						}
						decoded = c1 == 1;
					}
					if (decoded)
						e0.AddRange(e1);
					else
						index = s1;
				}
			}
			if (!decoded)
			{
				{
					var e1 = new List<Rule>();
					int s1 = index;
					decoded = true;
					if (decoded)
					{
						bool f1 = true;
						int c1 = 0;
						for (int i1 = 0; i1 < 1 && f1; i1++)
						{
							rule = decode_xri_path_noscheme();
							if ((f1 = rule != null))
							{
								e1.Add(rule);
								c1++;
							}
						}
						decoded = c1 == 1;
					}
					if (decoded)
						e0.AddRange(e1);
					else
						index = s1;
				}
			}
			if (!decoded)
			{
				{
					var e1 = new List<Rule>();
					int s1 = index;
					decoded = true;
					if (decoded)
					{
						bool f1 = true;
						int c1 = 0;
						for (int i1 = 0; i1 < 1 && f1; i1++)
						{
							rule = decode_ipath_empty();
							if ((f1 = rule != null))
							{
								e1.Add(rule);
								c1++;
							}
						}
						decoded = c1 == 1;
					}
					if (decoded)
						e0.AddRange(e1);
					else
						index = s1;
				}
			}

			rule = null;
			if (decoded)
				rule = new relative_xri_part(text.Substring(s0, index - s0), e0);
			else
				index = s0;

			pop("relative-xri-part", decoded, index - s0);

			return (relative_xri_part)rule;
		}

		private xri_hier_part decode_xri_hier_part()
		{
			push("xri-hier-part");

			bool decoded = true;
			int s0 = index;
			var e0 = new List<Rule>();
			Rule rule;

			decoded = false;
			if (!decoded)
			{
				{
					var e1 = new List<Rule>();
					int s1 = index;
					decoded = true;
					if (decoded)
					{
						bool f1 = true;
						int c1 = 0;
						for (int i1 = 0; i1 < 1 && f1; i1++)
						{
							rule = decode_xri_authority();
							if ((f1 = rule != null))
							{
								e1.Add(rule);
								c1++;
							}
						}
						decoded = c1 == 1;
					}
					if (decoded)
					{
						bool f1 = true;
						int c1 = 0;
						for (int i1 = 0; i1 < 1 && f1; i1++)
						{
							rule = decode_xri_path_abempty();
							if ((f1 = rule != null))
							{
								e1.Add(rule);
								c1++;
							}
						}
						decoded = c1 == 1;
					}
					if (decoded)
						e0.AddRange(e1);
					else
						index = s1;
				}
			}

			rule = null;
			if (decoded)
				rule = new xri_hier_part(text.Substring(s0, index - s0), e0);
			else
				index = s0;

			pop("xri-hier-part", decoded, index - s0);

			return (xri_hier_part)rule;
		}

		private xri_authority decode_xri_authority()
		{
			push("xri-authority");

			bool decoded = true;
			int s0 = index;
			var e0 = new List<Rule>();
			Rule rule;

			decoded = false;
			if (!decoded)
			{
				{
					var e1 = new List<Rule>();
					int s1 = index;
					decoded = true;
					if (decoded)
					{
						bool f1 = true;
						int c1 = 0;
						for (int i1 = 0; i1 < 1 && f1; i1++)
						{
							rule = decode_global_subseg();
							if ((f1 = rule != null))
							{
								e1.Add(rule);
								c1++;
							}
						}
						decoded = c1 == 1;
					}
					if (decoded)
					{
						bool f1 = true;
						int c1 = 0;
						while (f1)
						{
							rule = decode_subseg();
							if ((f1 = rule != null))
							{
								e1.Add(rule);
								c1++;
							}
						}
						decoded = true;
					}
					if (decoded)
						e0.AddRange(e1);
					else
						index = s1;
				}
			}

			rule = null;
			if (decoded)
				rule = new xri_authority(text.Substring(s0, index - s0), e0);
			else
				index = s0;

			pop("xri-authority", decoded, index - s0);

			return (xri_authority)rule;
		}

		private subseg decode_subseg()
		{
			push("subseg");

			bool decoded = true;
			int s0 = index;
			var e0 = new List<Rule>();
			Rule rule;

			decoded = false;
			if (!decoded)
			{
				{
					var e1 = new List<Rule>();
					int s1 = index;
					decoded = true;
					if (decoded)
					{
						bool f1 = true;
						int c1 = 0;
						for (int i1 = 0; i1 < 1 && f1; i1++)
						{
							rule = decode_global_subseg();
							if ((f1 = rule != null))
							{
								e1.Add(rule);
								c1++;
							}
						}
						decoded = c1 == 1;
					}
					if (decoded)
						e0.AddRange(e1);
					else
						index = s1;
				}
			}
			if (!decoded)
			{
				{
					var e1 = new List<Rule>();
					int s1 = index;
					decoded = true;
					if (decoded)
					{
						bool f1 = true;
						int c1 = 0;
						for (int i1 = 0; i1 < 1 && f1; i1++)
						{
							rule = decode_local_subseg();
							if ((f1 = rule != null))
							{
								e1.Add(rule);
								c1++;
							}
						}
						decoded = c1 == 1;
					}
					if (decoded)
						e0.AddRange(e1);
					else
						index = s1;
				}
			}

			rule = null;
			if (decoded)
				rule = new subseg(text.Substring(s0, index - s0), e0);
			else
				index = s0;

			pop("subseg", decoded, index - s0);

			return (subseg)rule;
		}

		private global_subseg decode_global_subseg()
		{
			push("global-subseg");

			bool decoded = true;
			int s0 = index;
			var e0 = new List<Rule>();
			Rule rule;

			decoded = false;
			if (!decoded)
			{
				{
					var e1 = new List<Rule>();
					int s1 = index;
					decoded = true;
					if (decoded)
					{
						bool f1 = true;
						int c1 = 0;
						for (int i1 = 0; i1 < 1 && f1; i1++)
						{
							rule = decode_gcs_char();
							if ((f1 = rule != null))
							{
								e1.Add(rule);
								c1++;
							}
						}
						decoded = c1 == 1;
					}
					if (decoded)
					{
						bool f1 = true;
						int c1 = 0;
						for (int i1 = 0; i1 < 1 && f1; i1++)
						{
							decoded = false;
							if (!decoded)
							{
								{
									var e2 = new List<Rule>();
									int s2 = index;
									decoded = true;
									if (decoded)
									{
										bool f2 = true;
										int c2 = 0;
										for (int i2 = 0; i2 < 1 && f2; i2++)
										{
											rule = decode_rel_subseg();
											if ((f2 = rule != null))
											{
												e2.Add(rule);
												c2++;
											}
										}
										decoded = c2 == 1;
									}
									if (decoded)
										e1.AddRange(e2);
									else
										index = s2;
								}
							}
							if (!decoded)
							{
								{
									var e2 = new List<Rule>();
									int s2 = index;
									decoded = true;
									if (decoded)
									{
										bool f2 = true;
										int c2 = 0;
										for (int i2 = 0; i2 < 1 && f2; i2++)
										{
											rule = decode_local_subseg();
											if ((f2 = rule != null))
											{
												e2.Add(rule);
												c2++;
											}
										}
										decoded = c2 == 1;
									}
									if (decoded)
										e1.AddRange(e2);
									else
										index = s2;
								}
							}
							if (decoded)
								c1++;
							f1 = decoded;
						}
						decoded = true;
					}
					if (decoded)
						e0.AddRange(e1);
					else
						index = s1;
				}
			}

			rule = null;
			if (decoded)
				rule = new global_subseg(text.Substring(s0, index - s0), e0);
			else
				index = s0;

			pop("global-subseg", decoded, index - s0);

			return (global_subseg)rule;
		}

		private local_subseg decode_local_subseg()
		{
			push("local-subseg");

			bool decoded = true;
			int s0 = index;
			var e0 = new List<Rule>();
			Rule rule;

			decoded = false;
			if (!decoded)
			{
				{
					var e1 = new List<Rule>();
					int s1 = index;
					decoded = true;
					if (decoded)
					{
						bool f1 = true;
						int c1 = 0;
						for (int i1 = 0; i1 < 1 && f1; i1++)
						{
							rule = decode_lcs_char();
							if ((f1 = rule != null))
							{
								e1.Add(rule);
								c1++;
							}
						}
						decoded = c1 == 1;
					}
					if (decoded)
					{
						bool f1 = true;
						int c1 = 0;
						for (int i1 = 0; i1 < 1 && f1; i1++)
						{
							decoded = false;
							if (!decoded)
							{
								{
									var e2 = new List<Rule>();
									int s2 = index;
									decoded = true;
									if (decoded)
									{
										bool f2 = true;
										int c2 = 0;
										for (int i2 = 0; i2 < 1 && f2; i2++)
										{
											rule = decode_rel_subseg();
											if ((f2 = rule != null))
											{
												e2.Add(rule);
												c2++;
											}
										}
										decoded = c2 == 1;
									}
									if (decoded)
										e1.AddRange(e2);
									else
										index = s2;
								}
							}
							if (decoded)
								c1++;
							f1 = decoded;
						}
						decoded = true;
					}
					if (decoded)
						e0.AddRange(e1);
					else
						index = s1;
				}
			}

			rule = null;
			if (decoded)
				rule = new local_subseg(text.Substring(s0, index - s0), e0);
			else
				index = s0;

			pop("local-subseg", decoded, index - s0);

			return (local_subseg)rule;
		}

		private gcs_char decode_gcs_char()
		{
			push("gcs-char");

			bool decoded = true;
			int s0 = index;
			var e0 = new List<Rule>();
			Rule rule;

			decoded = false;
			if (!decoded)
			{
				{
					var e1 = new List<Rule>();
					int s1 = index;
					decoded = true;
					if (decoded)
					{
						bool f1 = true;
						int c1 = 0;
						for (int i1 = 0; i1 < 1 && f1; i1++)
						{
							rule = decode_StringValue("=");
							if ((f1 = rule != null))
							{
								e1.Add(rule);
								c1++;
							}
						}
						decoded = c1 == 1;
					}
					if (decoded)
						e0.AddRange(e1);
					else
						index = s1;
				}
			}
			if (!decoded)
			{
				{
					var e1 = new List<Rule>();
					int s1 = index;
					decoded = true;
					if (decoded)
					{
						bool f1 = true;
						int c1 = 0;
						for (int i1 = 0; i1 < 1 && f1; i1++)
						{
							rule = decode_StringValue("@");
							if ((f1 = rule != null))
							{
								e1.Add(rule);
								c1++;
							}
						}
						decoded = c1 == 1;
					}
					if (decoded)
						e0.AddRange(e1);
					else
						index = s1;
				}
			}
			if (!decoded)
			{
				{
					var e1 = new List<Rule>();
					int s1 = index;
					decoded = true;
					if (decoded)
					{
						bool f1 = true;
						int c1 = 0;
						for (int i1 = 0; i1 < 1 && f1; i1++)
						{
							rule = decode_StringValue("+");
							if ((f1 = rule != null))
							{
								e1.Add(rule);
								c1++;
							}
						}
						decoded = c1 == 1;
					}
					if (decoded)
						e0.AddRange(e1);
					else
						index = s1;
				}
			}
			if (!decoded)
			{
				{
					var e1 = new List<Rule>();
					int s1 = index;
					decoded = true;
					if (decoded)
					{
						bool f1 = true;
						int c1 = 0;
						for (int i1 = 0; i1 < 1 && f1; i1++)
						{
							rule = decode_StringValue("$");
							if ((f1 = rule != null))
							{
								e1.Add(rule);
								c1++;
							}
						}
						decoded = c1 == 1;
					}
					if (decoded)
						e0.AddRange(e1);
					else
						index = s1;
				}
			}

			rule = null;
			if (decoded)
				rule = new gcs_char(text.Substring(s0, index - s0), e0);
			else
				index = s0;

			pop("gcs-char", decoded, index - s0);

			return (gcs_char)rule;
		}

		private lcs_char decode_lcs_char()
		{
			push("lcs-char");

			bool decoded = true;
			int s0 = index;
			var e0 = new List<Rule>();
			Rule rule;

			decoded = false;
			if (!decoded)
			{
				{
					var e1 = new List<Rule>();
					int s1 = index;
					decoded = true;
					if (decoded)
					{
						bool f1 = true;
						int c1 = 0;
						for (int i1 = 0; i1 < 1 && f1; i1++)
						{
							rule = decode_StringValue("*");
							if ((f1 = rule != null))
							{
								e1.Add(rule);
								c1++;
							}
						}
						decoded = c1 == 1;
					}
					if (decoded)
						e0.AddRange(e1);
					else
						index = s1;
				}
			}
			if (!decoded)
			{
				{
					var e1 = new List<Rule>();
					int s1 = index;
					decoded = true;
					if (decoded)
					{
						bool f1 = true;
						int c1 = 0;
						for (int i1 = 0; i1 < 1 && f1; i1++)
						{
							rule = decode_StringValue("!");
							if ((f1 = rule != null))
							{
								e1.Add(rule);
								c1++;
							}
						}
						decoded = c1 == 1;
					}
					if (decoded)
						e0.AddRange(e1);
					else
						index = s1;
				}
			}

			rule = null;
			if (decoded)
				rule = new lcs_char(text.Substring(s0, index - s0), e0);
			else
				index = s0;

			pop("lcs-char", decoded, index - s0);

			return (lcs_char)rule;
		}

		private rel_subseg decode_rel_subseg()
		{
			push("rel-subseg");

			bool decoded = true;
			int s0 = index;
			var e0 = new List<Rule>();
			Rule rule;

			decoded = false;
			if (!decoded)
			{
				{
					var e1 = new List<Rule>();
					int s1 = index;
					decoded = true;
					if (decoded)
					{
						bool f1 = true;
						int c1 = 0;
						for (int i1 = 0; i1 < 1 && f1; i1++)
						{
							rule = decode_literal();
							if ((f1 = rule != null))
							{
								e1.Add(rule);
								c1++;
							}
						}
						decoded = c1 == 1;
					}
					if (decoded)
						e0.AddRange(e1);
					else
						index = s1;
				}
			}
			if (!decoded)
			{
				{
					var e1 = new List<Rule>();
					int s1 = index;
					decoded = true;
					if (decoded)
					{
						bool f1 = true;
						int c1 = 0;
						for (int i1 = 0; i1 < 1 && f1; i1++)
						{
							rule = decode_xref();
							if ((f1 = rule != null))
							{
								e1.Add(rule);
								c1++;
							}
						}
						decoded = c1 == 1;
					}
					if (decoded)
						e0.AddRange(e1);
					else
						index = s1;
				}
			}

			rule = null;
			if (decoded)
				rule = new rel_subseg(text.Substring(s0, index - s0), e0);
			else
				index = s0;

			pop("rel-subseg", decoded, index - s0);

			return (rel_subseg)rule;
		}

		private rel_subseg_nc decode_rel_subseg_nc()
		{
			push("rel-subseg-nc");

			bool decoded = true;
			int s0 = index;
			var e0 = new List<Rule>();
			Rule rule;

			decoded = false;
			if (!decoded)
			{
				{
					var e1 = new List<Rule>();
					int s1 = index;
					decoded = true;
					if (decoded)
					{
						bool f1 = true;
						int c1 = 0;
						for (int i1 = 0; i1 < 1 && f1; i1++)
						{
							rule = decode_literal_nc();
							if ((f1 = rule != null))
							{
								e1.Add(rule);
								c1++;
							}
						}
						decoded = c1 == 1;
					}
					if (decoded)
						e0.AddRange(e1);
					else
						index = s1;
				}
			}
			if (!decoded)
			{
				{
					var e1 = new List<Rule>();
					int s1 = index;
					decoded = true;
					if (decoded)
					{
						bool f1 = true;
						int c1 = 0;
						for (int i1 = 0; i1 < 1 && f1; i1++)
						{
							rule = decode_xref();
							if ((f1 = rule != null))
							{
								e1.Add(rule);
								c1++;
							}
						}
						decoded = c1 == 1;
					}
					if (decoded)
						e0.AddRange(e1);
					else
						index = s1;
				}
			}

			rule = null;
			if (decoded)
				rule = new rel_subseg_nc(text.Substring(s0, index - s0), e0);
			else
				index = s0;

			pop("rel-subseg-nc", decoded, index - s0);

			return (rel_subseg_nc)rule;
		}

		private literal decode_literal()
		{
			push("literal");

			bool decoded = true;
			int s0 = index;
			var e0 = new List<Rule>();
			Rule rule;

			decoded = false;
			if (!decoded)
			{
				{
					var e1 = new List<Rule>();
					int s1 = index;
					decoded = true;
					if (decoded)
					{
						bool f1 = true;
						int c1 = 0;
						for (int i1 = 0; i1 < 1 && f1; i1++)
						{
							rule = decode_xri_pchar();
							if ((f1 = rule != null))
							{
								e1.Add(rule);
								c1++;
							}
						}
						while (f1)
						{
							rule = decode_xri_pchar();
							if ((f1 = rule != null))
							{
								e1.Add(rule);
								c1++;
							}
						}
						decoded = c1 >= 1;
					}
					if (decoded)
						e0.AddRange(e1);
					else
						index = s1;
				}
			}

			rule = null;
			if (decoded)
				rule = new literal(text.Substring(s0, index - s0), e0);
			else
				index = s0;

			pop("literal", decoded, index - s0);

			return (literal)rule;
		}

		private literal_nc decode_literal_nc()
		{
			push("literal-nc");

			bool decoded = true;
			int s0 = index;
			var e0 = new List<Rule>();
			Rule rule;

			decoded = false;
			if (!decoded)
			{
				{
					var e1 = new List<Rule>();
					int s1 = index;
					decoded = true;
					if (decoded)
					{
						bool f1 = true;
						int c1 = 0;
						for (int i1 = 0; i1 < 1 && f1; i1++)
						{
							rule = decode_xri_pchar_nc();
							if ((f1 = rule != null))
							{
								e1.Add(rule);
								c1++;
							}
						}
						while (f1)
						{
							rule = decode_xri_pchar_nc();
							if ((f1 = rule != null))
							{
								e1.Add(rule);
								c1++;
							}
						}
						decoded = c1 >= 1;
					}
					if (decoded)
						e0.AddRange(e1);
					else
						index = s1;
				}
			}

			rule = null;
			if (decoded)
				rule = new literal_nc(text.Substring(s0, index - s0), e0);
			else
				index = s0;

			pop("literal-nc", decoded, index - s0);

			return (literal_nc)rule;
		}

		private xref decode_xref()
		{
			push("xref");

			bool decoded = true;
			int s0 = index;
			var e0 = new List<Rule>();
			Rule rule;

			decoded = false;
			if (!decoded)
			{
				{
					var e1 = new List<Rule>();
					int s1 = index;
					decoded = true;
					if (decoded)
					{
						bool f1 = true;
						int c1 = 0;
						for (int i1 = 0; i1 < 1 && f1; i1++)
						{
							rule = decode_xref_empty();
							if ((f1 = rule != null))
							{
								e1.Add(rule);
								c1++;
							}
						}
						decoded = c1 == 1;
					}
					if (decoded)
						e0.AddRange(e1);
					else
						index = s1;
				}
			}
			if (!decoded)
			{
				{
					var e1 = new List<Rule>();
					int s1 = index;
					decoded = true;
					if (decoded)
					{
						bool f1 = true;
						int c1 = 0;
						for (int i1 = 0; i1 < 1 && f1; i1++)
						{
							rule = decode_xref_xri_reference();
							if ((f1 = rule != null))
							{
								e1.Add(rule);
								c1++;
							}
						}
						decoded = c1 == 1;
					}
					if (decoded)
						e0.AddRange(e1);
					else
						index = s1;
				}
			}
			if (!decoded)
			{
				{
					var e1 = new List<Rule>();
					int s1 = index;
					decoded = true;
					if (decoded)
					{
						bool f1 = true;
						int c1 = 0;
						for (int i1 = 0; i1 < 1 && f1; i1++)
						{
							rule = decode_xref_IRI();
							if ((f1 = rule != null))
							{
								e1.Add(rule);
								c1++;
							}
						}
						decoded = c1 == 1;
					}
					if (decoded)
						e0.AddRange(e1);
					else
						index = s1;
				}
			}

			rule = null;
			if (decoded)
				rule = new xref(text.Substring(s0, index - s0), e0);
			else
				index = s0;

			pop("xref", decoded, index - s0);

			return (xref)rule;
		}

		private xref_empty decode_xref_empty()
		{
			push("xref-empty");

			bool decoded = true;
			int s0 = index;
			var e0 = new List<Rule>();
			Rule rule;

			decoded = false;
			if (!decoded)
			{
				{
					var e1 = new List<Rule>();
					int s1 = index;
					decoded = true;
					if (decoded)
					{
						bool f1 = true;
						int c1 = 0;
						for (int i1 = 0; i1 < 1 && f1; i1++)
						{
							rule = decode_StringValue("()");
							if ((f1 = rule != null))
							{
								e1.Add(rule);
								c1++;
							}
						}
						decoded = c1 == 1;
					}
					if (decoded)
						e0.AddRange(e1);
					else
						index = s1;
				}
			}

			rule = null;
			if (decoded)
				rule = new xref_empty(text.Substring(s0, index - s0), e0);
			else
				index = s0;

			pop("xref-empty", decoded, index - s0);

			return (xref_empty)rule;
		}

		private xref_xri_reference decode_xref_xri_reference()
		{
			push("xref-xri-reference");

			bool decoded = true;
			int s0 = index;
			var e0 = new List<Rule>();
			Rule rule;

			decoded = false;
			if (!decoded)
			{
				{
					var e1 = new List<Rule>();
					int s1 = index;
					decoded = true;
					if (decoded)
					{
						bool f1 = true;
						int c1 = 0;
						for (int i1 = 0; i1 < 1 && f1; i1++)
						{
							rule = decode_StringValue("(");
							if ((f1 = rule != null))
							{
								e1.Add(rule);
								c1++;
							}
						}
						decoded = c1 == 1;
					}
					if (decoded)
					{
						bool f1 = true;
						int c1 = 0;
						for (int i1 = 0; i1 < 1 && f1; i1++)
						{
							rule = decode_xri_reference();
							if ((f1 = rule != null))
							{
								e1.Add(rule);
								c1++;
							}
						}
						decoded = c1 == 1;
					}
					if (decoded)
					{
						bool f1 = true;
						int c1 = 0;
						for (int i1 = 0; i1 < 1 && f1; i1++)
						{
							rule = decode_StringValue(")");
							if ((f1 = rule != null))
							{
								e1.Add(rule);
								c1++;
							}
						}
						decoded = c1 == 1;
					}
					if (decoded)
						e0.AddRange(e1);
					else
						index = s1;
				}
			}

			rule = null;
			if (decoded)
				rule = new xref_xri_reference(text.Substring(s0, index - s0), e0);
			else
				index = s0;

			pop("xref-xri-reference", decoded, index - s0);

			return (xref_xri_reference)rule;
		}

		private xref_IRI decode_xref_IRI()
		{
			push("xref-IRI");

			bool decoded = true;
			int s0 = index;
			var e0 = new List<Rule>();
			Rule rule;

			decoded = false;
			if (!decoded)
			{
				{
					var e1 = new List<Rule>();
					int s1 = index;
					decoded = true;
					if (decoded)
					{
						bool f1 = true;
						int c1 = 0;
						for (int i1 = 0; i1 < 1 && f1; i1++)
						{
							rule = decode_StringValue("(");
							if ((f1 = rule != null))
							{
								e1.Add(rule);
								c1++;
							}
						}
						decoded = c1 == 1;
					}
					if (decoded)
					{
						bool f1 = true;
						int c1 = 0;
						for (int i1 = 0; i1 < 1 && f1; i1++)
						{
							rule = decode_IRI();
							if ((f1 = rule != null))
							{
								e1.Add(rule);
								c1++;
							}
						}
						decoded = c1 == 1;
					}
					if (decoded)
					{
						bool f1 = true;
						int c1 = 0;
						for (int i1 = 0; i1 < 1 && f1; i1++)
						{
							rule = decode_StringValue(")");
							if ((f1 = rule != null))
							{
								e1.Add(rule);
								c1++;
							}
						}
						decoded = c1 == 1;
					}
					if (decoded)
						e0.AddRange(e1);
					else
						index = s1;
				}
			}

			rule = null;
			if (decoded)
				rule = new xref_IRI(text.Substring(s0, index - s0), e0);
			else
				index = s0;

			pop("xref-IRI", decoded, index - s0);

			return (xref_IRI)rule;
		}

		private xref_value decode_xref_value()
		{
			push("xref-value");

			bool decoded = true;
			int s0 = index;
			var e0 = new List<Rule>();
			Rule rule;

			decoded = false;
			if (!decoded)
			{
				{
					var e1 = new List<Rule>();
					int s1 = index;
					decoded = true;
					if (decoded)
					{
						bool f1 = true;
						int c1 = 0;
						for (int i1 = 0; i1 < 1 && f1; i1++)
						{
							rule = decode_xri_reference();
							if ((f1 = rule != null))
							{
								e1.Add(rule);
								c1++;
							}
						}
						decoded = c1 == 1;
					}
					if (decoded)
						e0.AddRange(e1);
					else
						index = s1;
				}
			}
			if (!decoded)
			{
				{
					var e1 = new List<Rule>();
					int s1 = index;
					decoded = true;
					if (decoded)
					{
						bool f1 = true;
						int c1 = 0;
						for (int i1 = 0; i1 < 1 && f1; i1++)
						{
							rule = decode_IRI();
							if ((f1 = rule != null))
							{
								e1.Add(rule);
								c1++;
							}
						}
						decoded = c1 == 1;
					}
					if (decoded)
						e0.AddRange(e1);
					else
						index = s1;
				}
			}

			rule = null;
			if (decoded)
				rule = new xref_value(text.Substring(s0, index - s0), e0);
			else
				index = s0;

			pop("xref-value", decoded, index - s0);

			return (xref_value)rule;
		}

		private xri_path decode_xri_path()
		{
			push("xri-path");

			bool decoded = true;
			int s0 = index;
			var e0 = new List<Rule>();
			Rule rule;

			decoded = false;
			if (!decoded)
			{
				{
					var e1 = new List<Rule>();
					int s1 = index;
					decoded = true;
					if (decoded)
					{
						bool f1 = true;
						int c1 = 0;
						for (int i1 = 0; i1 < 1 && f1; i1++)
						{
							rule = decode_xri_path_abempty();
							if ((f1 = rule != null))
							{
								e1.Add(rule);
								c1++;
							}
						}
						decoded = c1 == 1;
					}
					if (decoded)
						e0.AddRange(e1);
					else
						index = s1;
				}
			}
			if (!decoded)
			{
				{
					var e1 = new List<Rule>();
					int s1 = index;
					decoded = true;
					if (decoded)
					{
						bool f1 = true;
						int c1 = 0;
						for (int i1 = 0; i1 < 1 && f1; i1++)
						{
							rule = decode_xri_path_abs();
							if ((f1 = rule != null))
							{
								e1.Add(rule);
								c1++;
							}
						}
						decoded = c1 == 1;
					}
					if (decoded)
						e0.AddRange(e1);
					else
						index = s1;
				}
			}
			if (!decoded)
			{
				{
					var e1 = new List<Rule>();
					int s1 = index;
					decoded = true;
					if (decoded)
					{
						bool f1 = true;
						int c1 = 0;
						for (int i1 = 0; i1 < 1 && f1; i1++)
						{
							rule = decode_xri_path_noscheme();
							if ((f1 = rule != null))
							{
								e1.Add(rule);
								c1++;
							}
						}
						decoded = c1 == 1;
					}
					if (decoded)
						e0.AddRange(e1);
					else
						index = s1;
				}
			}
			if (!decoded)
			{
				{
					var e1 = new List<Rule>();
					int s1 = index;
					decoded = true;
					if (decoded)
					{
						bool f1 = true;
						int c1 = 0;
						for (int i1 = 0; i1 < 1 && f1; i1++)
						{
							rule = decode_ipath_empty();
							if ((f1 = rule != null))
							{
								e1.Add(rule);
								c1++;
							}
						}
						decoded = c1 == 1;
					}
					if (decoded)
						e0.AddRange(e1);
					else
						index = s1;
				}
			}

			rule = null;
			if (decoded)
				rule = new xri_path(text.Substring(s0, index - s0), e0);
			else
				index = s0;

			pop("xri-path", decoded, index - s0);

			return (xri_path)rule;
		}

		private xri_path_abempty decode_xri_path_abempty()
		{
			push("xri-path-abempty");

			bool decoded = true;
			int s0 = index;
			var e0 = new List<Rule>();
			Rule rule;

			decoded = false;
			if (!decoded)
			{
				{
					var e1 = new List<Rule>();
					int s1 = index;
					decoded = true;
					if (decoded)
					{
						bool f1 = true;
						int c1 = 0;
						while (f1)
						{
							decoded = false;
							if (!decoded)
							{
								{
									var e2 = new List<Rule>();
									int s2 = index;
									decoded = true;
									if (decoded)
									{
										bool f2 = true;
										int c2 = 0;
										for (int i2 = 0; i2 < 1 && f2; i2++)
										{
											rule = decode_StringValue("/");
											if ((f2 = rule != null))
											{
												e2.Add(rule);
												c2++;
											}
										}
										decoded = c2 == 1;
									}
									if (decoded)
									{
										bool f2 = true;
										int c2 = 0;
										for (int i2 = 0; i2 < 1 && f2; i2++)
										{
											rule = decode_xri_segment();
											if ((f2 = rule != null))
											{
												e2.Add(rule);
												c2++;
											}
										}
										decoded = c2 == 1;
									}
									if (decoded)
										e1.AddRange(e2);
									else
										index = s2;
								}
							}
							if (decoded)
								c1++;
							f1 = decoded;
						}
						decoded = true;
					}
					if (decoded)
						e0.AddRange(e1);
					else
						index = s1;
				}
			}

			rule = null;
			if (decoded)
				rule = new xri_path_abempty(text.Substring(s0, index - s0), e0);
			else
				index = s0;

			pop("xri-path-abempty", decoded, index - s0);

			return (xri_path_abempty)rule;
		}

		private xri_path_abs decode_xri_path_abs()
		{
			push("xri-path-abs");

			bool decoded = true;
			int s0 = index;
			var e0 = new List<Rule>();
			Rule rule;

			decoded = false;
			if (!decoded)
			{
				{
					var e1 = new List<Rule>();
					int s1 = index;
					decoded = true;
					if (decoded)
					{
						bool f1 = true;
						int c1 = 0;
						for (int i1 = 0; i1 < 1 && f1; i1++)
						{
							rule = decode_StringValue("/");
							if ((f1 = rule != null))
							{
								e1.Add(rule);
								c1++;
							}
						}
						decoded = c1 == 1;
					}
					if (decoded)
					{
						bool f1 = true;
						int c1 = 0;
						for (int i1 = 0; i1 < 1 && f1; i1++)
						{
							decoded = false;
							if (!decoded)
							{
								{
									var e2 = new List<Rule>();
									int s2 = index;
									decoded = true;
									if (decoded)
									{
										bool f2 = true;
										int c2 = 0;
										for (int i2 = 0; i2 < 1 && f2; i2++)
										{
											rule = decode_xri_segment_nz();
											if ((f2 = rule != null))
											{
												e2.Add(rule);
												c2++;
											}
										}
										decoded = c2 == 1;
									}
									if (decoded)
									{
										bool f2 = true;
										int c2 = 0;
										while (f2)
										{
											decoded = false;
											if (!decoded)
											{
												{
													var e3 = new List<Rule>();
													int s3 = index;
													decoded = true;
													if (decoded)
													{
														bool f3 = true;
														int c3 = 0;
														for (int i3 = 0; i3 < 1 && f3; i3++)
														{
															rule = decode_StringValue("/");
															if ((f3 = rule != null))
															{
																e3.Add(rule);
																c3++;
															}
														}
														decoded = c3 == 1;
													}
													if (decoded)
													{
														bool f3 = true;
														int c3 = 0;
														for (int i3 = 0; i3 < 1 && f3; i3++)
														{
															rule = decode_xri_segment();
															if ((f3 = rule != null))
															{
																e3.Add(rule);
																c3++;
															}
														}
														decoded = c3 == 1;
													}
													if (decoded)
														e2.AddRange(e3);
													else
														index = s3;
												}
											}
											if (decoded)
												c2++;
											f2 = decoded;
										}
										decoded = true;
									}
									if (decoded)
										e1.AddRange(e2);
									else
										index = s2;
								}
							}
							if (decoded)
								c1++;
							f1 = decoded;
						}
						decoded = true;
					}
					if (decoded)
						e0.AddRange(e1);
					else
						index = s1;
				}
			}

			rule = null;
			if (decoded)
				rule = new xri_path_abs(text.Substring(s0, index - s0), e0);
			else
				index = s0;

			pop("xri-path-abs", decoded, index - s0);

			return (xri_path_abs)rule;
		}

		private xri_path_noscheme decode_xri_path_noscheme()
		{
			push("xri-path-noscheme");

			bool decoded = true;
			int s0 = index;
			var e0 = new List<Rule>();
			Rule rule;

			decoded = false;
			if (!decoded)
			{
				{
					var e1 = new List<Rule>();
					int s1 = index;
					decoded = true;
					if (decoded)
					{
						bool f1 = true;
						int c1 = 0;
						for (int i1 = 0; i1 < 1 && f1; i1++)
						{
							rule = decode_xri_segment_nc();
							if ((f1 = rule != null))
							{
								e1.Add(rule);
								c1++;
							}
						}
						decoded = c1 == 1;
					}
					if (decoded)
					{
						bool f1 = true;
						int c1 = 0;
						while (f1)
						{
							decoded = false;
							if (!decoded)
							{
								{
									var e2 = new List<Rule>();
									int s2 = index;
									decoded = true;
									if (decoded)
									{
										bool f2 = true;
										int c2 = 0;
										for (int i2 = 0; i2 < 1 && f2; i2++)
										{
											rule = decode_StringValue("/");
											if ((f2 = rule != null))
											{
												e2.Add(rule);
												c2++;
											}
										}
										decoded = c2 == 1;
									}
									if (decoded)
									{
										bool f2 = true;
										int c2 = 0;
										for (int i2 = 0; i2 < 1 && f2; i2++)
										{
											rule = decode_xri_segment();
											if ((f2 = rule != null))
											{
												e2.Add(rule);
												c2++;
											}
										}
										decoded = c2 == 1;
									}
									if (decoded)
										e1.AddRange(e2);
									else
										index = s2;
								}
							}
							if (decoded)
								c1++;
							f1 = decoded;
						}
						decoded = true;
					}
					if (decoded)
						e0.AddRange(e1);
					else
						index = s1;
				}
			}

			rule = null;
			if (decoded)
				rule = new xri_path_noscheme(text.Substring(s0, index - s0), e0);
			else
				index = s0;

			pop("xri-path-noscheme", decoded, index - s0);

			return (xri_path_noscheme)rule;
		}

		private xri_segment decode_xri_segment()
		{
			push("xri-segment");

			bool decoded = true;
			int s0 = index;
			var e0 = new List<Rule>();
			Rule rule;

			decoded = false;
			if (!decoded)
			{
				{
					var e1 = new List<Rule>();
					int s1 = index;
					decoded = true;
					if (decoded)
					{
						bool f1 = true;
						int c1 = 0;
						for (int i1 = 0; i1 < 1 && f1; i1++)
						{
							decoded = false;
							if (!decoded)
							{
								{
									var e2 = new List<Rule>();
									int s2 = index;
									decoded = true;
									if (decoded)
									{
										bool f2 = true;
										int c2 = 0;
										for (int i2 = 0; i2 < 1 && f2; i2++)
										{
											rule = decode_rel_subseg();
											if ((f2 = rule != null))
											{
												e2.Add(rule);
												c2++;
											}
										}
										decoded = c2 == 1;
									}
									if (decoded)
										e1.AddRange(e2);
									else
										index = s2;
								}
							}
							if (decoded)
								c1++;
							f1 = decoded;
						}
						decoded = true;
					}
					if (decoded)
					{
						bool f1 = true;
						int c1 = 0;
						while (f1)
						{
							rule = decode_subseg();
							if ((f1 = rule != null))
							{
								e1.Add(rule);
								c1++;
							}
						}
						decoded = true;
					}
					if (decoded)
						e0.AddRange(e1);
					else
						index = s1;
				}
			}

			rule = null;
			if (decoded)
				rule = new xri_segment(text.Substring(s0, index - s0), e0);
			else
				index = s0;

			pop("xri-segment", decoded, index - s0);

			return (xri_segment)rule;
		}

		private xri_segment_nz decode_xri_segment_nz()
		{
			push("xri-segment-nz");

			bool decoded = true;
			int s0 = index;
			var e0 = new List<Rule>();
			Rule rule;

			decoded = false;
			if (!decoded)
			{
				{
					var e1 = new List<Rule>();
					int s1 = index;
					decoded = true;
					if (decoded)
					{
						bool f1 = true;
						int c1 = 0;
						for (int i1 = 0; i1 < 1 && f1; i1++)
						{
							decoded = false;
							if (!decoded)
							{
								{
									var e2 = new List<Rule>();
									int s2 = index;
									decoded = true;
									if (decoded)
									{
										bool f2 = true;
										int c2 = 0;
										for (int i2 = 0; i2 < 1 && f2; i2++)
										{
											rule = decode_rel_subseg();
											if ((f2 = rule != null))
											{
												e2.Add(rule);
												c2++;
											}
										}
										decoded = c2 == 1;
									}
									if (decoded)
										e1.AddRange(e2);
									else
										index = s2;
								}
							}
							if (!decoded)
							{
								{
									var e2 = new List<Rule>();
									int s2 = index;
									decoded = true;
									if (decoded)
									{
										bool f2 = true;
										int c2 = 0;
										for (int i2 = 0; i2 < 1 && f2; i2++)
										{
											rule = decode_subseg();
											if ((f2 = rule != null))
											{
												e2.Add(rule);
												c2++;
											}
										}
										decoded = c2 == 1;
									}
									if (decoded)
										e1.AddRange(e2);
									else
										index = s2;
								}
							}
							if (decoded)
								c1++;
							f1 = decoded;
						}
						decoded = c1 == 1;
					}
					if (decoded)
					{
						bool f1 = true;
						int c1 = 0;
						while (f1)
						{
							rule = decode_subseg();
							if ((f1 = rule != null))
							{
								e1.Add(rule);
								c1++;
							}
						}
						decoded = true;
					}
					if (decoded)
						e0.AddRange(e1);
					else
						index = s1;
				}
			}

			rule = null;
			if (decoded)
				rule = new xri_segment_nz(text.Substring(s0, index - s0), e0);
			else
				index = s0;

			pop("xri-segment-nz", decoded, index - s0);

			return (xri_segment_nz)rule;
		}

		private xri_segment_nc decode_xri_segment_nc()
		{
			push("xri-segment-nc");

			bool decoded = true;
			int s0 = index;
			var e0 = new List<Rule>();
			Rule rule;

			decoded = false;
			if (!decoded)
			{
				{
					var e1 = new List<Rule>();
					int s1 = index;
					decoded = true;
					if (decoded)
					{
						bool f1 = true;
						int c1 = 0;
						for (int i1 = 0; i1 < 1 && f1; i1++)
						{
							decoded = false;
							if (!decoded)
							{
								{
									var e2 = new List<Rule>();
									int s2 = index;
									decoded = true;
									if (decoded)
									{
										bool f2 = true;
										int c2 = 0;
										for (int i2 = 0; i2 < 1 && f2; i2++)
										{
											rule = decode_rel_subseg_nc();
											if ((f2 = rule != null))
											{
												e2.Add(rule);
												c2++;
											}
										}
										decoded = c2 == 1;
									}
									if (decoded)
										e1.AddRange(e2);
									else
										index = s2;
								}
							}
							if (!decoded)
							{
								{
									var e2 = new List<Rule>();
									int s2 = index;
									decoded = true;
									if (decoded)
									{
										bool f2 = true;
										int c2 = 0;
										for (int i2 = 0; i2 < 1 && f2; i2++)
										{
											rule = decode_subseg();
											if ((f2 = rule != null))
											{
												e2.Add(rule);
												c2++;
											}
										}
										decoded = c2 == 1;
									}
									if (decoded)
										e1.AddRange(e2);
									else
										index = s2;
								}
							}
							if (decoded)
								c1++;
							f1 = decoded;
						}
						decoded = c1 == 1;
					}
					if (decoded)
					{
						bool f1 = true;
						int c1 = 0;
						while (f1)
						{
							rule = decode_subseg();
							if ((f1 = rule != null))
							{
								e1.Add(rule);
								c1++;
							}
						}
						decoded = true;
					}
					if (decoded)
						e0.AddRange(e1);
					else
						index = s1;
				}
			}

			rule = null;
			if (decoded)
				rule = new xri_segment_nc(text.Substring(s0, index - s0), e0);
			else
				index = s0;

			pop("xri-segment-nc", decoded, index - s0);

			return (xri_segment_nc)rule;
		}

		private xri_pchar decode_xri_pchar()
		{
			push("xri-pchar");

			bool decoded = true;
			int s0 = index;
			var e0 = new List<Rule>();
			Rule rule;

			decoded = false;
			if (!decoded)
			{
				{
					var e1 = new List<Rule>();
					int s1 = index;
					decoded = true;
					if (decoded)
					{
						bool f1 = true;
						int c1 = 0;
						for (int i1 = 0; i1 < 1 && f1; i1++)
						{
							rule = decode_iunreserved();
							if ((f1 = rule != null))
							{
								e1.Add(rule);
								c1++;
							}
						}
						decoded = c1 == 1;
					}
					if (decoded)
						e0.AddRange(e1);
					else
						index = s1;
				}
			}
			if (!decoded)
			{
				{
					var e1 = new List<Rule>();
					int s1 = index;
					decoded = true;
					if (decoded)
					{
						bool f1 = true;
						int c1 = 0;
						for (int i1 = 0; i1 < 1 && f1; i1++)
						{
							rule = decode_pct_encoded();
							if ((f1 = rule != null))
							{
								e1.Add(rule);
								c1++;
							}
						}
						decoded = c1 == 1;
					}
					if (decoded)
						e0.AddRange(e1);
					else
						index = s1;
				}
			}
			if (!decoded)
			{
				{
					var e1 = new List<Rule>();
					int s1 = index;
					decoded = true;
					if (decoded)
					{
						bool f1 = true;
						int c1 = 0;
						for (int i1 = 0; i1 < 1 && f1; i1++)
						{
							rule = decode_xri_sub_delims();
							if ((f1 = rule != null))
							{
								e1.Add(rule);
								c1++;
							}
						}
						decoded = c1 == 1;
					}
					if (decoded)
						e0.AddRange(e1);
					else
						index = s1;
				}
			}
			if (!decoded)
			{
				{
					var e1 = new List<Rule>();
					int s1 = index;
					decoded = true;
					if (decoded)
					{
						bool f1 = true;
						int c1 = 0;
						for (int i1 = 0; i1 < 1 && f1; i1++)
						{
							rule = decode_StringValue(":");
							if ((f1 = rule != null))
							{
								e1.Add(rule);
								c1++;
							}
						}
						decoded = c1 == 1;
					}
					if (decoded)
						e0.AddRange(e1);
					else
						index = s1;
				}
			}

			rule = null;
			if (decoded)
				rule = new xri_pchar(text.Substring(s0, index - s0), e0);
			else
				index = s0;

			pop("xri-pchar", decoded, index - s0);

			return (xri_pchar)rule;
		}

		private xri_pchar_nc decode_xri_pchar_nc()
		{
			push("xri-pchar-nc");

			bool decoded = true;
			int s0 = index;
			var e0 = new List<Rule>();
			Rule rule;

			decoded = false;
			if (!decoded)
			{
				{
					var e1 = new List<Rule>();
					int s1 = index;
					decoded = true;
					if (decoded)
					{
						bool f1 = true;
						int c1 = 0;
						for (int i1 = 0; i1 < 1 && f1; i1++)
						{
							rule = decode_iunreserved();
							if ((f1 = rule != null))
							{
								e1.Add(rule);
								c1++;
							}
						}
						decoded = c1 == 1;
					}
					if (decoded)
						e0.AddRange(e1);
					else
						index = s1;
				}
			}
			if (!decoded)
			{
				{
					var e1 = new List<Rule>();
					int s1 = index;
					decoded = true;
					if (decoded)
					{
						bool f1 = true;
						int c1 = 0;
						for (int i1 = 0; i1 < 1 && f1; i1++)
						{
							rule = decode_pct_encoded();
							if ((f1 = rule != null))
							{
								e1.Add(rule);
								c1++;
							}
						}
						decoded = c1 == 1;
					}
					if (decoded)
						e0.AddRange(e1);
					else
						index = s1;
				}
			}
			if (!decoded)
			{
				{
					var e1 = new List<Rule>();
					int s1 = index;
					decoded = true;
					if (decoded)
					{
						bool f1 = true;
						int c1 = 0;
						for (int i1 = 0; i1 < 1 && f1; i1++)
						{
							rule = decode_xri_sub_delims();
							if ((f1 = rule != null))
							{
								e1.Add(rule);
								c1++;
							}
						}
						decoded = c1 == 1;
					}
					if (decoded)
						e0.AddRange(e1);
					else
						index = s1;
				}
			}

			rule = null;
			if (decoded)
				rule = new xri_pchar_nc(text.Substring(s0, index - s0), e0);
			else
				index = s0;

			pop("xri-pchar-nc", decoded, index - s0);

			return (xri_pchar_nc)rule;
		}

		private xri_reserved decode_xri_reserved()
		{
			push("xri-reserved");

			bool decoded = true;
			int s0 = index;
			var e0 = new List<Rule>();
			Rule rule;

			decoded = false;
			if (!decoded)
			{
				{
					var e1 = new List<Rule>();
					int s1 = index;
					decoded = true;
					if (decoded)
					{
						bool f1 = true;
						int c1 = 0;
						for (int i1 = 0; i1 < 1 && f1; i1++)
						{
							rule = decode_xri_gen_delims();
							if ((f1 = rule != null))
							{
								e1.Add(rule);
								c1++;
							}
						}
						decoded = c1 == 1;
					}
					if (decoded)
						e0.AddRange(e1);
					else
						index = s1;
				}
			}
			if (!decoded)
			{
				{
					var e1 = new List<Rule>();
					int s1 = index;
					decoded = true;
					if (decoded)
					{
						bool f1 = true;
						int c1 = 0;
						for (int i1 = 0; i1 < 1 && f1; i1++)
						{
							rule = decode_xri_sub_delims();
							if ((f1 = rule != null))
							{
								e1.Add(rule);
								c1++;
							}
						}
						decoded = c1 == 1;
					}
					if (decoded)
						e0.AddRange(e1);
					else
						index = s1;
				}
			}

			rule = null;
			if (decoded)
				rule = new xri_reserved(text.Substring(s0, index - s0), e0);
			else
				index = s0;

			pop("xri-reserved", decoded, index - s0);

			return (xri_reserved)rule;
		}

		private xri_gen_delims decode_xri_gen_delims()
		{
			push("xri-gen-delims");

			bool decoded = true;
			int s0 = index;
			var e0 = new List<Rule>();
			Rule rule;

			decoded = false;
			if (!decoded)
			{
				{
					var e1 = new List<Rule>();
					int s1 = index;
					decoded = true;
					if (decoded)
					{
						bool f1 = true;
						int c1 = 0;
						for (int i1 = 0; i1 < 1 && f1; i1++)
						{
							rule = decode_StringValue(":");
							if ((f1 = rule != null))
							{
								e1.Add(rule);
								c1++;
							}
						}
						decoded = c1 == 1;
					}
					if (decoded)
						e0.AddRange(e1);
					else
						index = s1;
				}
			}
			if (!decoded)
			{
				{
					var e1 = new List<Rule>();
					int s1 = index;
					decoded = true;
					if (decoded)
					{
						bool f1 = true;
						int c1 = 0;
						for (int i1 = 0; i1 < 1 && f1; i1++)
						{
							rule = decode_StringValue("/");
							if ((f1 = rule != null))
							{
								e1.Add(rule);
								c1++;
							}
						}
						decoded = c1 == 1;
					}
					if (decoded)
						e0.AddRange(e1);
					else
						index = s1;
				}
			}
			if (!decoded)
			{
				{
					var e1 = new List<Rule>();
					int s1 = index;
					decoded = true;
					if (decoded)
					{
						bool f1 = true;
						int c1 = 0;
						for (int i1 = 0; i1 < 1 && f1; i1++)
						{
							rule = decode_StringValue("?");
							if ((f1 = rule != null))
							{
								e1.Add(rule);
								c1++;
							}
						}
						decoded = c1 == 1;
					}
					if (decoded)
						e0.AddRange(e1);
					else
						index = s1;
				}
			}
			if (!decoded)
			{
				{
					var e1 = new List<Rule>();
					int s1 = index;
					decoded = true;
					if (decoded)
					{
						bool f1 = true;
						int c1 = 0;
						for (int i1 = 0; i1 < 1 && f1; i1++)
						{
							rule = decode_StringValue("#");
							if ((f1 = rule != null))
							{
								e1.Add(rule);
								c1++;
							}
						}
						decoded = c1 == 1;
					}
					if (decoded)
						e0.AddRange(e1);
					else
						index = s1;
				}
			}
			if (!decoded)
			{
				{
					var e1 = new List<Rule>();
					int s1 = index;
					decoded = true;
					if (decoded)
					{
						bool f1 = true;
						int c1 = 0;
						for (int i1 = 0; i1 < 1 && f1; i1++)
						{
							rule = decode_StringValue("[");
							if ((f1 = rule != null))
							{
								e1.Add(rule);
								c1++;
							}
						}
						decoded = c1 == 1;
					}
					if (decoded)
						e0.AddRange(e1);
					else
						index = s1;
				}
			}
			if (!decoded)
			{
				{
					var e1 = new List<Rule>();
					int s1 = index;
					decoded = true;
					if (decoded)
					{
						bool f1 = true;
						int c1 = 0;
						for (int i1 = 0; i1 < 1 && f1; i1++)
						{
							rule = decode_StringValue("]");
							if ((f1 = rule != null))
							{
								e1.Add(rule);
								c1++;
							}
						}
						decoded = c1 == 1;
					}
					if (decoded)
						e0.AddRange(e1);
					else
						index = s1;
				}
			}
			if (!decoded)
			{
				{
					var e1 = new List<Rule>();
					int s1 = index;
					decoded = true;
					if (decoded)
					{
						bool f1 = true;
						int c1 = 0;
						for (int i1 = 0; i1 < 1 && f1; i1++)
						{
							rule = decode_StringValue("(");
							if ((f1 = rule != null))
							{
								e1.Add(rule);
								c1++;
							}
						}
						decoded = c1 == 1;
					}
					if (decoded)
						e0.AddRange(e1);
					else
						index = s1;
				}
			}
			if (!decoded)
			{
				{
					var e1 = new List<Rule>();
					int s1 = index;
					decoded = true;
					if (decoded)
					{
						bool f1 = true;
						int c1 = 0;
						for (int i1 = 0; i1 < 1 && f1; i1++)
						{
							rule = decode_StringValue(")");
							if ((f1 = rule != null))
							{
								e1.Add(rule);
								c1++;
							}
						}
						decoded = c1 == 1;
					}
					if (decoded)
						e0.AddRange(e1);
					else
						index = s1;
				}
			}
			if (!decoded)
			{
				{
					var e1 = new List<Rule>();
					int s1 = index;
					decoded = true;
					if (decoded)
					{
						bool f1 = true;
						int c1 = 0;
						for (int i1 = 0; i1 < 1 && f1; i1++)
						{
							rule = decode_gcs_char();
							if ((f1 = rule != null))
							{
								e1.Add(rule);
								c1++;
							}
						}
						decoded = c1 == 1;
					}
					if (decoded)
						e0.AddRange(e1);
					else
						index = s1;
				}
			}
			if (!decoded)
			{
				{
					var e1 = new List<Rule>();
					int s1 = index;
					decoded = true;
					if (decoded)
					{
						bool f1 = true;
						int c1 = 0;
						for (int i1 = 0; i1 < 1 && f1; i1++)
						{
							rule = decode_lcs_char();
							if ((f1 = rule != null))
							{
								e1.Add(rule);
								c1++;
							}
						}
						decoded = c1 == 1;
					}
					if (decoded)
						e0.AddRange(e1);
					else
						index = s1;
				}
			}

			rule = null;
			if (decoded)
				rule = new xri_gen_delims(text.Substring(s0, index - s0), e0);
			else
				index = s0;

			pop("xri-gen-delims", decoded, index - s0);

			return (xri_gen_delims)rule;
		}

		private xri_sub_delims decode_xri_sub_delims()
		{
			push("xri-sub-delims");

			bool decoded = true;
			int s0 = index;
			var e0 = new List<Rule>();
			Rule rule;

			decoded = false;
			if (!decoded)
			{
				{
					var e1 = new List<Rule>();
					int s1 = index;
					decoded = true;
					if (decoded)
					{
						bool f1 = true;
						int c1 = 0;
						for (int i1 = 0; i1 < 1 && f1; i1++)
						{
							rule = decode_StringValue("&");
							if ((f1 = rule != null))
							{
								e1.Add(rule);
								c1++;
							}
						}
						decoded = c1 == 1;
					}
					if (decoded)
						e0.AddRange(e1);
					else
						index = s1;
				}
			}
			if (!decoded)
			{
				{
					var e1 = new List<Rule>();
					int s1 = index;
					decoded = true;
					if (decoded)
					{
						bool f1 = true;
						int c1 = 0;
						for (int i1 = 0; i1 < 1 && f1; i1++)
						{
							rule = decode_StringValue(";");
							if ((f1 = rule != null))
							{
								e1.Add(rule);
								c1++;
							}
						}
						decoded = c1 == 1;
					}
					if (decoded)
						e0.AddRange(e1);
					else
						index = s1;
				}
			}
			if (!decoded)
			{
				{
					var e1 = new List<Rule>();
					int s1 = index;
					decoded = true;
					if (decoded)
					{
						bool f1 = true;
						int c1 = 0;
						for (int i1 = 0; i1 < 1 && f1; i1++)
						{
							rule = decode_StringValue(",");
							if ((f1 = rule != null))
							{
								e1.Add(rule);
								c1++;
							}
						}
						decoded = c1 == 1;
					}
					if (decoded)
						e0.AddRange(e1);
					else
						index = s1;
				}
			}
			if (!decoded)
			{
				{
					var e1 = new List<Rule>();
					int s1 = index;
					decoded = true;
					if (decoded)
					{
						bool f1 = true;
						int c1 = 0;
						for (int i1 = 0; i1 < 1 && f1; i1++)
						{
							rule = decode_StringValue("'");
							if ((f1 = rule != null))
							{
								e1.Add(rule);
								c1++;
							}
						}
						decoded = c1 == 1;
					}
					if (decoded)
						e0.AddRange(e1);
					else
						index = s1;
				}
			}

			rule = null;
			if (decoded)
				rule = new xri_sub_delims(text.Substring(s0, index - s0), e0);
			else
				index = s0;

			pop("xri-sub-delims", decoded, index - s0);

			return (xri_sub_delims)rule;
		}

		private IRI decode_IRI()
		{
			push("IRI");

			bool decoded = true;
			int s0 = index;
			var e0 = new List<Rule>();
			Rule rule;

			decoded = false;
			if (!decoded)
			{
				{
					var e1 = new List<Rule>();
					int s1 = index;
					decoded = true;
					if (decoded)
					{
						bool f1 = true;
						int c1 = 0;
						for (int i1 = 0; i1 < 1 && f1; i1++)
						{
							rule = decode_scheme();
							if ((f1 = rule != null))
							{
								e1.Add(rule);
								c1++;
							}
						}
						decoded = c1 == 1;
					}
					if (decoded)
					{
						bool f1 = true;
						int c1 = 0;
						for (int i1 = 0; i1 < 1 && f1; i1++)
						{
							rule = decode_StringValue(":");
							if ((f1 = rule != null))
							{
								e1.Add(rule);
								c1++;
							}
						}
						decoded = c1 == 1;
					}
					if (decoded)
					{
						bool f1 = true;
						int c1 = 0;
						for (int i1 = 0; i1 < 1 && f1; i1++)
						{
							rule = decode_ihier_part();
							if ((f1 = rule != null))
							{
								e1.Add(rule);
								c1++;
							}
						}
						decoded = c1 == 1;
					}
					if (decoded)
					{
						bool f1 = true;
						int c1 = 0;
						for (int i1 = 0; i1 < 1 && f1; i1++)
						{
							decoded = false;
							if (!decoded)
							{
								{
									var e2 = new List<Rule>();
									int s2 = index;
									decoded = true;
									if (decoded)
									{
										bool f2 = true;
										int c2 = 0;
										for (int i2 = 0; i2 < 1 && f2; i2++)
										{
											rule = decode_StringValue("?");
											if ((f2 = rule != null))
											{
												e2.Add(rule);
												c2++;
											}
										}
										decoded = c2 == 1;
									}
									if (decoded)
									{
										bool f2 = true;
										int c2 = 0;
										for (int i2 = 0; i2 < 1 && f2; i2++)
										{
											rule = decode_iquery();
											if ((f2 = rule != null))
											{
												e2.Add(rule);
												c2++;
											}
										}
										decoded = c2 == 1;
									}
									if (decoded)
										e1.AddRange(e2);
									else
										index = s2;
								}
							}
							if (decoded)
								c1++;
							f1 = decoded;
						}
						decoded = true;
					}
					if (decoded)
					{
						bool f1 = true;
						int c1 = 0;
						for (int i1 = 0; i1 < 1 && f1; i1++)
						{
							decoded = false;
							if (!decoded)
							{
								{
									var e2 = new List<Rule>();
									int s2 = index;
									decoded = true;
									if (decoded)
									{
										bool f2 = true;
										int c2 = 0;
										for (int i2 = 0; i2 < 1 && f2; i2++)
										{
											rule = decode_StringValue("#");
											if ((f2 = rule != null))
											{
												e2.Add(rule);
												c2++;
											}
										}
										decoded = c2 == 1;
									}
									if (decoded)
									{
										bool f2 = true;
										int c2 = 0;
										for (int i2 = 0; i2 < 1 && f2; i2++)
										{
											rule = decode_ifragment();
											if ((f2 = rule != null))
											{
												e2.Add(rule);
												c2++;
											}
										}
										decoded = c2 == 1;
									}
									if (decoded)
										e1.AddRange(e2);
									else
										index = s2;
								}
							}
							if (decoded)
								c1++;
							f1 = decoded;
						}
						decoded = true;
					}
					if (decoded)
						e0.AddRange(e1);
					else
						index = s1;
				}
			}

			rule = null;
			if (decoded)
				rule = new IRI(text.Substring(s0, index - s0), e0);
			else
				index = s0;

			pop("IRI", decoded, index - s0);

			return (IRI)rule;
		}

		private scheme decode_scheme()
		{
			push("scheme");

			bool decoded = true;
			int s0 = index;
			var e0 = new List<Rule>();
			Rule rule;

			decoded = false;
			if (!decoded)
			{
				{
					var e1 = new List<Rule>();
					int s1 = index;
					decoded = true;
					if (decoded)
					{
						bool f1 = true;
						int c1 = 0;
						for (int i1 = 0; i1 < 1 && f1; i1++)
						{
							rule = decode_ALPHA();
							if ((f1 = rule != null))
							{
								e1.Add(rule);
								c1++;
							}
						}
						decoded = c1 == 1;
					}
					if (decoded)
					{
						bool f1 = true;
						int c1 = 0;
						while (f1)
						{
							decoded = false;
							if (!decoded)
							{
								{
									var e2 = new List<Rule>();
									int s2 = index;
									decoded = true;
									if (decoded)
									{
										bool f2 = true;
										int c2 = 0;
										for (int i2 = 0; i2 < 1 && f2; i2++)
										{
											rule = decode_ALPHA();
											if ((f2 = rule != null))
											{
												e2.Add(rule);
												c2++;
											}
										}
										decoded = c2 == 1;
									}
									if (decoded)
										e1.AddRange(e2);
									else
										index = s2;
								}
							}
							if (!decoded)
							{
								{
									var e2 = new List<Rule>();
									int s2 = index;
									decoded = true;
									if (decoded)
									{
										bool f2 = true;
										int c2 = 0;
										for (int i2 = 0; i2 < 1 && f2; i2++)
										{
											rule = decode_DIGIT();
											if ((f2 = rule != null))
											{
												e2.Add(rule);
												c2++;
											}
										}
										decoded = c2 == 1;
									}
									if (decoded)
										e1.AddRange(e2);
									else
										index = s2;
								}
							}
							if (!decoded)
							{
								{
									var e2 = new List<Rule>();
									int s2 = index;
									decoded = true;
									if (decoded)
									{
										bool f2 = true;
										int c2 = 0;
										for (int i2 = 0; i2 < 1 && f2; i2++)
										{
											rule = decode_StringValue("+");
											if ((f2 = rule != null))
											{
												e2.Add(rule);
												c2++;
											}
										}
										decoded = c2 == 1;
									}
									if (decoded)
										e1.AddRange(e2);
									else
										index = s2;
								}
							}
							if (!decoded)
							{
								{
									var e2 = new List<Rule>();
									int s2 = index;
									decoded = true;
									if (decoded)
									{
										bool f2 = true;
										int c2 = 0;
										for (int i2 = 0; i2 < 1 && f2; i2++)
										{
											rule = decode_StringValue("-");
											if ((f2 = rule != null))
											{
												e2.Add(rule);
												c2++;
											}
										}
										decoded = c2 == 1;
									}
									if (decoded)
										e1.AddRange(e2);
									else
										index = s2;
								}
							}
							if (!decoded)
							{
								{
									var e2 = new List<Rule>();
									int s2 = index;
									decoded = true;
									if (decoded)
									{
										bool f2 = true;
										int c2 = 0;
										for (int i2 = 0; i2 < 1 && f2; i2++)
										{
											rule = decode_StringValue(".");
											if ((f2 = rule != null))
											{
												e2.Add(rule);
												c2++;
											}
										}
										decoded = c2 == 1;
									}
									if (decoded)
										e1.AddRange(e2);
									else
										index = s2;
								}
							}
							if (decoded)
								c1++;
							f1 = decoded;
						}
						decoded = true;
					}
					if (decoded)
						e0.AddRange(e1);
					else
						index = s1;
				}
			}

			rule = null;
			if (decoded)
				rule = new scheme(text.Substring(s0, index - s0), e0);
			else
				index = s0;

			pop("scheme", decoded, index - s0);

			return (scheme)rule;
		}

		private ihier_part decode_ihier_part()
		{
			push("ihier-part");

			bool decoded = true;
			int s0 = index;
			var e0 = new List<Rule>();
			Rule rule;

			decoded = false;
			if (!decoded)
			{
				{
					var e1 = new List<Rule>();
					int s1 = index;
					decoded = true;
					if (decoded)
					{
						bool f1 = true;
						int c1 = 0;
						for (int i1 = 0; i1 < 1 && f1; i1++)
						{
							rule = decode_StringValue("//");
							if ((f1 = rule != null))
							{
								e1.Add(rule);
								c1++;
							}
						}
						decoded = c1 == 1;
					}
					if (decoded)
					{
						bool f1 = true;
						int c1 = 0;
						for (int i1 = 0; i1 < 1 && f1; i1++)
						{
							rule = decode_iauthority();
							if ((f1 = rule != null))
							{
								e1.Add(rule);
								c1++;
							}
						}
						decoded = c1 == 1;
					}
					if (decoded)
					{
						bool f1 = true;
						int c1 = 0;
						for (int i1 = 0; i1 < 1 && f1; i1++)
						{
							rule = decode_ipath_abempty();
							if ((f1 = rule != null))
							{
								e1.Add(rule);
								c1++;
							}
						}
						decoded = c1 == 1;
					}
					if (decoded)
						e0.AddRange(e1);
					else
						index = s1;
				}
			}
			if (!decoded)
			{
				{
					var e1 = new List<Rule>();
					int s1 = index;
					decoded = true;
					if (decoded)
					{
						bool f1 = true;
						int c1 = 0;
						for (int i1 = 0; i1 < 1 && f1; i1++)
						{
							rule = decode_ipath_abs();
							if ((f1 = rule != null))
							{
								e1.Add(rule);
								c1++;
							}
						}
						decoded = c1 == 1;
					}
					if (decoded)
						e0.AddRange(e1);
					else
						index = s1;
				}
			}
			if (!decoded)
			{
				{
					var e1 = new List<Rule>();
					int s1 = index;
					decoded = true;
					if (decoded)
					{
						bool f1 = true;
						int c1 = 0;
						for (int i1 = 0; i1 < 1 && f1; i1++)
						{
							rule = decode_ipath_rootless();
							if ((f1 = rule != null))
							{
								e1.Add(rule);
								c1++;
							}
						}
						decoded = c1 == 1;
					}
					if (decoded)
						e0.AddRange(e1);
					else
						index = s1;
				}
			}
			if (!decoded)
			{
				{
					var e1 = new List<Rule>();
					int s1 = index;
					decoded = true;
					if (decoded)
					{
						bool f1 = true;
						int c1 = 0;
						for (int i1 = 0; i1 < 1 && f1; i1++)
						{
							rule = decode_ipath_empty();
							if ((f1 = rule != null))
							{
								e1.Add(rule);
								c1++;
							}
						}
						decoded = c1 == 1;
					}
					if (decoded)
						e0.AddRange(e1);
					else
						index = s1;
				}
			}

			rule = null;
			if (decoded)
				rule = new ihier_part(text.Substring(s0, index - s0), e0);
			else
				index = s0;

			pop("ihier-part", decoded, index - s0);

			return (ihier_part)rule;
		}

		private iauthority decode_iauthority()
		{
			push("iauthority");

			bool decoded = true;
			int s0 = index;
			var e0 = new List<Rule>();
			Rule rule;

			decoded = false;
			if (!decoded)
			{
				{
					var e1 = new List<Rule>();
					int s1 = index;
					decoded = true;
					if (decoded)
					{
						bool f1 = true;
						int c1 = 0;
						for (int i1 = 0; i1 < 1 && f1; i1++)
						{
							decoded = false;
							if (!decoded)
							{
								{
									var e2 = new List<Rule>();
									int s2 = index;
									decoded = true;
									if (decoded)
									{
										bool f2 = true;
										int c2 = 0;
										for (int i2 = 0; i2 < 1 && f2; i2++)
										{
											rule = decode_iuserinfo();
											if ((f2 = rule != null))
											{
												e2.Add(rule);
												c2++;
											}
										}
										decoded = c2 == 1;
									}
									if (decoded)
									{
										bool f2 = true;
										int c2 = 0;
										for (int i2 = 0; i2 < 1 && f2; i2++)
										{
											rule = decode_StringValue("@");
											if ((f2 = rule != null))
											{
												e2.Add(rule);
												c2++;
											}
										}
										decoded = c2 == 1;
									}
									if (decoded)
										e1.AddRange(e2);
									else
										index = s2;
								}
							}
							if (decoded)
								c1++;
							f1 = decoded;
						}
						decoded = true;
					}
					if (decoded)
					{
						bool f1 = true;
						int c1 = 0;
						for (int i1 = 0; i1 < 1 && f1; i1++)
						{
							rule = decode_ihost();
							if ((f1 = rule != null))
							{
								e1.Add(rule);
								c1++;
							}
						}
						decoded = c1 == 1;
					}
					if (decoded)
					{
						bool f1 = true;
						int c1 = 0;
						for (int i1 = 0; i1 < 1 && f1; i1++)
						{
							decoded = false;
							if (!decoded)
							{
								{
									var e2 = new List<Rule>();
									int s2 = index;
									decoded = true;
									if (decoded)
									{
										bool f2 = true;
										int c2 = 0;
										for (int i2 = 0; i2 < 1 && f2; i2++)
										{
											rule = decode_StringValue(":");
											if ((f2 = rule != null))
											{
												e2.Add(rule);
												c2++;
											}
										}
										decoded = c2 == 1;
									}
									if (decoded)
									{
										bool f2 = true;
										int c2 = 0;
										for (int i2 = 0; i2 < 1 && f2; i2++)
										{
											rule = decode_port();
											if ((f2 = rule != null))
											{
												e2.Add(rule);
												c2++;
											}
										}
										decoded = c2 == 1;
									}
									if (decoded)
										e1.AddRange(e2);
									else
										index = s2;
								}
							}
							if (decoded)
								c1++;
							f1 = decoded;
						}
						decoded = true;
					}
					if (decoded)
						e0.AddRange(e1);
					else
						index = s1;
				}
			}

			rule = null;
			if (decoded)
				rule = new iauthority(text.Substring(s0, index - s0), e0);
			else
				index = s0;

			pop("iauthority", decoded, index - s0);

			return (iauthority)rule;
		}

		private iuserinfo decode_iuserinfo()
		{
			push("iuserinfo");

			bool decoded = true;
			int s0 = index;
			var e0 = new List<Rule>();
			Rule rule;

			decoded = false;
			if (!decoded)
			{
				{
					var e1 = new List<Rule>();
					int s1 = index;
					decoded = true;
					if (decoded)
					{
						bool f1 = true;
						int c1 = 0;
						while (f1)
						{
							decoded = false;
							if (!decoded)
							{
								{
									var e2 = new List<Rule>();
									int s2 = index;
									decoded = true;
									if (decoded)
									{
										bool f2 = true;
										int c2 = 0;
										for (int i2 = 0; i2 < 1 && f2; i2++)
										{
											rule = decode_iunreserved();
											if ((f2 = rule != null))
											{
												e2.Add(rule);
												c2++;
											}
										}
										decoded = c2 == 1;
									}
									if (decoded)
										e1.AddRange(e2);
									else
										index = s2;
								}
							}
							if (!decoded)
							{
								{
									var e2 = new List<Rule>();
									int s2 = index;
									decoded = true;
									if (decoded)
									{
										bool f2 = true;
										int c2 = 0;
										for (int i2 = 0; i2 < 1 && f2; i2++)
										{
											rule = decode_pct_encoded();
											if ((f2 = rule != null))
											{
												e2.Add(rule);
												c2++;
											}
										}
										decoded = c2 == 1;
									}
									if (decoded)
										e1.AddRange(e2);
									else
										index = s2;
								}
							}
							if (!decoded)
							{
								{
									var e2 = new List<Rule>();
									int s2 = index;
									decoded = true;
									if (decoded)
									{
										bool f2 = true;
										int c2 = 0;
										for (int i2 = 0; i2 < 1 && f2; i2++)
										{
											rule = decode_sub_delims();
											if ((f2 = rule != null))
											{
												e2.Add(rule);
												c2++;
											}
										}
										decoded = c2 == 1;
									}
									if (decoded)
										e1.AddRange(e2);
									else
										index = s2;
								}
							}
							if (!decoded)
							{
								{
									var e2 = new List<Rule>();
									int s2 = index;
									decoded = true;
									if (decoded)
									{
										bool f2 = true;
										int c2 = 0;
										for (int i2 = 0; i2 < 1 && f2; i2++)
										{
											rule = decode_StringValue(":");
											if ((f2 = rule != null))
											{
												e2.Add(rule);
												c2++;
											}
										}
										decoded = c2 == 1;
									}
									if (decoded)
										e1.AddRange(e2);
									else
										index = s2;
								}
							}
							if (decoded)
								c1++;
							f1 = decoded;
						}
						decoded = true;
					}
					if (decoded)
						e0.AddRange(e1);
					else
						index = s1;
				}
			}

			rule = null;
			if (decoded)
				rule = new iuserinfo(text.Substring(s0, index - s0), e0);
			else
				index = s0;

			pop("iuserinfo", decoded, index - s0);

			return (iuserinfo)rule;
		}

		private ihost decode_ihost()
		{
			push("ihost");

			bool decoded = true;
			int s0 = index;
			var e0 = new List<Rule>();
			Rule rule;

			decoded = false;
			if (!decoded)
			{
				{
					var e1 = new List<Rule>();
					int s1 = index;
					decoded = true;
					if (decoded)
					{
						bool f1 = true;
						int c1 = 0;
						for (int i1 = 0; i1 < 1 && f1; i1++)
						{
							rule = decode_IP_literal();
							if ((f1 = rule != null))
							{
								e1.Add(rule);
								c1++;
							}
						}
						decoded = c1 == 1;
					}
					if (decoded)
						e0.AddRange(e1);
					else
						index = s1;
				}
			}
			if (!decoded)
			{
				{
					var e1 = new List<Rule>();
					int s1 = index;
					decoded = true;
					if (decoded)
					{
						bool f1 = true;
						int c1 = 0;
						for (int i1 = 0; i1 < 1 && f1; i1++)
						{
							rule = decode_IPv4address();
							if ((f1 = rule != null))
							{
								e1.Add(rule);
								c1++;
							}
						}
						decoded = c1 == 1;
					}
					if (decoded)
						e0.AddRange(e1);
					else
						index = s1;
				}
			}
			if (!decoded)
			{
				{
					var e1 = new List<Rule>();
					int s1 = index;
					decoded = true;
					if (decoded)
					{
						bool f1 = true;
						int c1 = 0;
						for (int i1 = 0; i1 < 1 && f1; i1++)
						{
							rule = decode_ireg_name();
							if ((f1 = rule != null))
							{
								e1.Add(rule);
								c1++;
							}
						}
						decoded = c1 == 1;
					}
					if (decoded)
						e0.AddRange(e1);
					else
						index = s1;
				}
			}

			rule = null;
			if (decoded)
				rule = new ihost(text.Substring(s0, index - s0), e0);
			else
				index = s0;

			pop("ihost", decoded, index - s0);

			return (ihost)rule;
		}

		private IP_literal decode_IP_literal()
		{
			push("IP-literal");

			bool decoded = true;
			int s0 = index;
			var e0 = new List<Rule>();
			Rule rule;

			decoded = false;
			if (!decoded)
			{
				{
					var e1 = new List<Rule>();
					int s1 = index;
					decoded = true;
					if (decoded)
					{
						bool f1 = true;
						int c1 = 0;
						for (int i1 = 0; i1 < 1 && f1; i1++)
						{
							rule = decode_StringValue("[");
							if ((f1 = rule != null))
							{
								e1.Add(rule);
								c1++;
							}
						}
						decoded = c1 == 1;
					}
					if (decoded)
					{
						bool f1 = true;
						int c1 = 0;
						for (int i1 = 0; i1 < 1 && f1; i1++)
						{
							decoded = false;
							if (!decoded)
							{
								{
									var e2 = new List<Rule>();
									int s2 = index;
									decoded = true;
									if (decoded)
									{
										bool f2 = true;
										int c2 = 0;
										for (int i2 = 0; i2 < 1 && f2; i2++)
										{
											rule = decode_IPv6address();
											if ((f2 = rule != null))
											{
												e2.Add(rule);
												c2++;
											}
										}
										decoded = c2 == 1;
									}
									if (decoded)
										e1.AddRange(e2);
									else
										index = s2;
								}
							}
							if (!decoded)
							{
								{
									var e2 = new List<Rule>();
									int s2 = index;
									decoded = true;
									if (decoded)
									{
										bool f2 = true;
										int c2 = 0;
										for (int i2 = 0; i2 < 1 && f2; i2++)
										{
											rule = decode_IPvFuture();
											if ((f2 = rule != null))
											{
												e2.Add(rule);
												c2++;
											}
										}
										decoded = c2 == 1;
									}
									if (decoded)
										e1.AddRange(e2);
									else
										index = s2;
								}
							}
							if (decoded)
								c1++;
							f1 = decoded;
						}
						decoded = c1 == 1;
					}
					if (decoded)
					{
						bool f1 = true;
						int c1 = 0;
						for (int i1 = 0; i1 < 1 && f1; i1++)
						{
							rule = decode_StringValue("]");
							if ((f1 = rule != null))
							{
								e1.Add(rule);
								c1++;
							}
						}
						decoded = c1 == 1;
					}
					if (decoded)
						e0.AddRange(e1);
					else
						index = s1;
				}
			}

			rule = null;
			if (decoded)
				rule = new IP_literal(text.Substring(s0, index - s0), e0);
			else
				index = s0;

			pop("IP-literal", decoded, index - s0);

			return (IP_literal)rule;
		}

		private IPvFuture decode_IPvFuture()
		{
			push("IPvFuture");

			bool decoded = true;
			int s0 = index;
			var e0 = new List<Rule>();
			Rule rule;

			decoded = false;
			if (!decoded)
			{
				{
					var e1 = new List<Rule>();
					int s1 = index;
					decoded = true;
					if (decoded)
					{
						bool f1 = true;
						int c1 = 0;
						for (int i1 = 0; i1 < 1 && f1; i1++)
						{
							rule = decode_StringValue("v");
							if ((f1 = rule != null))
							{
								e1.Add(rule);
								c1++;
							}
						}
						decoded = c1 == 1;
					}
					if (decoded)
					{
						bool f1 = true;
						int c1 = 0;
						for (int i1 = 0; i1 < 1 && f1; i1++)
						{
							rule = decode_HEXDIG();
							if ((f1 = rule != null))
							{
								e1.Add(rule);
								c1++;
							}
						}
						while (f1)
						{
							rule = decode_HEXDIG();
							if ((f1 = rule != null))
							{
								e1.Add(rule);
								c1++;
							}
						}
						decoded = c1 >= 1;
					}
					if (decoded)
					{
						bool f1 = true;
						int c1 = 0;
						for (int i1 = 0; i1 < 1 && f1; i1++)
						{
							rule = decode_StringValue(".");
							if ((f1 = rule != null))
							{
								e1.Add(rule);
								c1++;
							}
						}
						decoded = c1 == 1;
					}
					if (decoded)
					{
						bool f1 = true;
						int c1 = 0;
						for (int i1 = 0; i1 < 1 && f1; i1++)
						{
							decoded = false;
							if (!decoded)
							{
								{
									var e2 = new List<Rule>();
									int s2 = index;
									decoded = true;
									if (decoded)
									{
										bool f2 = true;
										int c2 = 0;
										for (int i2 = 0; i2 < 1 && f2; i2++)
										{
											rule = decode_unreserved();
											if ((f2 = rule != null))
											{
												e2.Add(rule);
												c2++;
											}
										}
										decoded = c2 == 1;
									}
									if (decoded)
										e1.AddRange(e2);
									else
										index = s2;
								}
							}
							if (!decoded)
							{
								{
									var e2 = new List<Rule>();
									int s2 = index;
									decoded = true;
									if (decoded)
									{
										bool f2 = true;
										int c2 = 0;
										for (int i2 = 0; i2 < 1 && f2; i2++)
										{
											rule = decode_sub_delims();
											if ((f2 = rule != null))
											{
												e2.Add(rule);
												c2++;
											}
										}
										decoded = c2 == 1;
									}
									if (decoded)
										e1.AddRange(e2);
									else
										index = s2;
								}
							}
							if (!decoded)
							{
								{
									var e2 = new List<Rule>();
									int s2 = index;
									decoded = true;
									if (decoded)
									{
										bool f2 = true;
										int c2 = 0;
										for (int i2 = 0; i2 < 1 && f2; i2++)
										{
											rule = decode_StringValue(":");
											if ((f2 = rule != null))
											{
												e2.Add(rule);
												c2++;
											}
										}
										decoded = c2 == 1;
									}
									if (decoded)
										e1.AddRange(e2);
									else
										index = s2;
								}
							}
							if (decoded)
								c1++;
							f1 = decoded;
						}
						while (f1)
						{
							decoded = false;
							if (!decoded)
							{
								{
									var e2 = new List<Rule>();
									int s2 = index;
									decoded = true;
									if (decoded)
									{
										bool f2 = true;
										int c2 = 0;
										for (int i2 = 0; i2 < 1 && f2; i2++)
										{
											rule = decode_unreserved();
											if ((f2 = rule != null))
											{
												e2.Add(rule);
												c2++;
											}
										}
										decoded = c2 == 1;
									}
									if (decoded)
										e1.AddRange(e2);
									else
										index = s2;
								}
							}
							if (!decoded)
							{
								{
									var e2 = new List<Rule>();
									int s2 = index;
									decoded = true;
									if (decoded)
									{
										bool f2 = true;
										int c2 = 0;
										for (int i2 = 0; i2 < 1 && f2; i2++)
										{
											rule = decode_sub_delims();
											if ((f2 = rule != null))
											{
												e2.Add(rule);
												c2++;
											}
										}
										decoded = c2 == 1;
									}
									if (decoded)
										e1.AddRange(e2);
									else
										index = s2;
								}
							}
							if (!decoded)
							{
								{
									var e2 = new List<Rule>();
									int s2 = index;
									decoded = true;
									if (decoded)
									{
										bool f2 = true;
										int c2 = 0;
										for (int i2 = 0; i2 < 1 && f2; i2++)
										{
											rule = decode_StringValue(":");
											if ((f2 = rule != null))
											{
												e2.Add(rule);
												c2++;
											}
										}
										decoded = c2 == 1;
									}
									if (decoded)
										e1.AddRange(e2);
									else
										index = s2;
								}
							}
							if (decoded)
								c1++;
							f1 = decoded;
						}
						decoded = c1 >= 1;
					}
					if (decoded)
						e0.AddRange(e1);
					else
						index = s1;
				}
			}

			rule = null;
			if (decoded)
				rule = new IPvFuture(text.Substring(s0, index - s0), e0);
			else
				index = s0;

			pop("IPvFuture", decoded, index - s0);

			return (IPvFuture)rule;
		}

		private IPv6address decode_IPv6address()
		{
			push("IPv6address");

			bool decoded = true;
			int s0 = index;
			var e0 = new List<Rule>();
			Rule rule;

			decoded = false;
			if (!decoded)
			{
				{
					var e1 = new List<Rule>();
					int s1 = index;
					decoded = true;
					if (decoded)
					{
						bool f1 = true;
						int c1 = 0;
						for (int i1 = 0; i1 < 6 && f1; i1++)
						{
							decoded = false;
							if (!decoded)
							{
								{
									var e2 = new List<Rule>();
									int s2 = index;
									decoded = true;
									if (decoded)
									{
										bool f2 = true;
										int c2 = 0;
										for (int i2 = 0; i2 < 1 && f2; i2++)
										{
											rule = decode_h16();
											if ((f2 = rule != null))
											{
												e2.Add(rule);
												c2++;
											}
										}
										decoded = c2 == 1;
									}
									if (decoded)
									{
										bool f2 = true;
										int c2 = 0;
										for (int i2 = 0; i2 < 1 && f2; i2++)
										{
											rule = decode_StringValue(":");
											if ((f2 = rule != null))
											{
												e2.Add(rule);
												c2++;
											}
										}
										decoded = c2 == 1;
									}
									if (decoded)
										e1.AddRange(e2);
									else
										index = s2;
								}
							}
							if (decoded)
								c1++;
							f1 = decoded;
						}
						decoded = c1 == 6;
					}
					if (decoded)
					{
						bool f1 = true;
						int c1 = 0;
						for (int i1 = 0; i1 < 1 && f1; i1++)
						{
							rule = decode_ls32();
							if ((f1 = rule != null))
							{
								e1.Add(rule);
								c1++;
							}
						}
						decoded = c1 == 1;
					}
					if (decoded)
						e0.AddRange(e1);
					else
						index = s1;
				}
			}
			if (!decoded)
			{
				{
					var e1 = new List<Rule>();
					int s1 = index;
					decoded = true;
					if (decoded)
					{
						bool f1 = true;
						int c1 = 0;
						for (int i1 = 0; i1 < 1 && f1; i1++)
						{
							rule = decode_StringValue("::");
							if ((f1 = rule != null))
							{
								e1.Add(rule);
								c1++;
							}
						}
						decoded = c1 == 1;
					}
					if (decoded)
					{
						bool f1 = true;
						int c1 = 0;
						for (int i1 = 0; i1 < 5 && f1; i1++)
						{
							decoded = false;
							if (!decoded)
							{
								{
									var e2 = new List<Rule>();
									int s2 = index;
									decoded = true;
									if (decoded)
									{
										bool f2 = true;
										int c2 = 0;
										for (int i2 = 0; i2 < 1 && f2; i2++)
										{
											rule = decode_h16();
											if ((f2 = rule != null))
											{
												e2.Add(rule);
												c2++;
											}
										}
										decoded = c2 == 1;
									}
									if (decoded)
									{
										bool f2 = true;
										int c2 = 0;
										for (int i2 = 0; i2 < 1 && f2; i2++)
										{
											rule = decode_StringValue(":");
											if ((f2 = rule != null))
											{
												e2.Add(rule);
												c2++;
											}
										}
										decoded = c2 == 1;
									}
									if (decoded)
										e1.AddRange(e2);
									else
										index = s2;
								}
							}
							if (decoded)
								c1++;
							f1 = decoded;
						}
						decoded = c1 == 5;
					}
					if (decoded)
					{
						bool f1 = true;
						int c1 = 0;
						for (int i1 = 0; i1 < 1 && f1; i1++)
						{
							rule = decode_ls32();
							if ((f1 = rule != null))
							{
								e1.Add(rule);
								c1++;
							}
						}
						decoded = c1 == 1;
					}
					if (decoded)
						e0.AddRange(e1);
					else
						index = s1;
				}
			}
			if (!decoded)
			{
				{
					var e1 = new List<Rule>();
					int s1 = index;
					decoded = true;
					if (decoded)
					{
						bool f1 = true;
						int c1 = 0;
						for (int i1 = 0; i1 < 1 && f1; i1++)
						{
							decoded = false;
							if (!decoded)
							{
								{
									var e2 = new List<Rule>();
									int s2 = index;
									decoded = true;
									if (decoded)
									{
										bool f2 = true;
										int c2 = 0;
										for (int i2 = 0; i2 < 1 && f2; i2++)
										{
											rule = decode_h16();
											if ((f2 = rule != null))
											{
												e2.Add(rule);
												c2++;
											}
										}
										decoded = c2 == 1;
									}
									if (decoded)
										e1.AddRange(e2);
									else
										index = s2;
								}
							}
							if (decoded)
								c1++;
							f1 = decoded;
						}
						decoded = true;
					}
					if (decoded)
					{
						bool f1 = true;
						int c1 = 0;
						for (int i1 = 0; i1 < 1 && f1; i1++)
						{
							rule = decode_StringValue("::");
							if ((f1 = rule != null))
							{
								e1.Add(rule);
								c1++;
							}
						}
						decoded = c1 == 1;
					}
					if (decoded)
					{
						bool f1 = true;
						int c1 = 0;
						for (int i1 = 0; i1 < 4 && f1; i1++)
						{
							decoded = false;
							if (!decoded)
							{
								{
									var e2 = new List<Rule>();
									int s2 = index;
									decoded = true;
									if (decoded)
									{
										bool f2 = true;
										int c2 = 0;
										for (int i2 = 0; i2 < 1 && f2; i2++)
										{
											rule = decode_h16();
											if ((f2 = rule != null))
											{
												e2.Add(rule);
												c2++;
											}
										}
										decoded = c2 == 1;
									}
									if (decoded)
									{
										bool f2 = true;
										int c2 = 0;
										for (int i2 = 0; i2 < 1 && f2; i2++)
										{
											rule = decode_StringValue(":");
											if ((f2 = rule != null))
											{
												e2.Add(rule);
												c2++;
											}
										}
										decoded = c2 == 1;
									}
									if (decoded)
										e1.AddRange(e2);
									else
										index = s2;
								}
							}
							if (decoded)
								c1++;
							f1 = decoded;
						}
						decoded = c1 == 4;
					}
					if (decoded)
					{
						bool f1 = true;
						int c1 = 0;
						for (int i1 = 0; i1 < 1 && f1; i1++)
						{
							rule = decode_ls32();
							if ((f1 = rule != null))
							{
								e1.Add(rule);
								c1++;
							}
						}
						decoded = c1 == 1;
					}
					if (decoded)
						e0.AddRange(e1);
					else
						index = s1;
				}
			}
			if (!decoded)
			{
				{
					var e1 = new List<Rule>();
					int s1 = index;
					decoded = true;
					if (decoded)
					{
						bool f1 = true;
						int c1 = 0;
						for (int i1 = 0; i1 < 1 && f1; i1++)
						{
							decoded = false;
							if (!decoded)
							{
								{
									var e2 = new List<Rule>();
									int s2 = index;
									decoded = true;
									if (decoded)
									{
										bool f2 = true;
										int c2 = 0;
										for (int i2 = 0; i2 < 1 && f2; i2++)
										{
											decoded = false;
											if (!decoded)
											{
												{
													var e3 = new List<Rule>();
													int s3 = index;
													decoded = true;
													if (decoded)
													{
														bool f3 = true;
														int c3 = 0;
														for (int i3 = 0; i3 < 1 && f3; i3++)
														{
															rule = decode_h16();
															if ((f3 = rule != null))
															{
																e3.Add(rule);
																c3++;
															}
														}
														decoded = c3 == 1;
													}
													if (decoded)
													{
														bool f3 = true;
														int c3 = 0;
														for (int i3 = 0; i3 < 1 && f3; i3++)
														{
															rule = decode_StringValue(":");
															if ((f3 = rule != null))
															{
																e3.Add(rule);
																c3++;
															}
														}
														decoded = c3 == 1;
													}
													if (decoded)
														e2.AddRange(e3);
													else
														index = s3;
												}
											}
											if (decoded)
												c2++;
											f2 = decoded;
										}
										decoded = true;
									}
									if (decoded)
									{
										bool f2 = true;
										int c2 = 0;
										for (int i2 = 0; i2 < 1 && f2; i2++)
										{
											rule = decode_h16();
											if ((f2 = rule != null))
											{
												e2.Add(rule);
												c2++;
											}
										}
										decoded = c2 == 1;
									}
									if (decoded)
										e1.AddRange(e2);
									else
										index = s2;
								}
							}
							if (decoded)
								c1++;
							f1 = decoded;
						}
						decoded = true;
					}
					if (decoded)
					{
						bool f1 = true;
						int c1 = 0;
						for (int i1 = 0; i1 < 1 && f1; i1++)
						{
							rule = decode_StringValue("::");
							if ((f1 = rule != null))
							{
								e1.Add(rule);
								c1++;
							}
						}
						decoded = c1 == 1;
					}
					if (decoded)
					{
						bool f1 = true;
						int c1 = 0;
						for (int i1 = 0; i1 < 3 && f1; i1++)
						{
							decoded = false;
							if (!decoded)
							{
								{
									var e2 = new List<Rule>();
									int s2 = index;
									decoded = true;
									if (decoded)
									{
										bool f2 = true;
										int c2 = 0;
										for (int i2 = 0; i2 < 1 && f2; i2++)
										{
											rule = decode_h16();
											if ((f2 = rule != null))
											{
												e2.Add(rule);
												c2++;
											}
										}
										decoded = c2 == 1;
									}
									if (decoded)
									{
										bool f2 = true;
										int c2 = 0;
										for (int i2 = 0; i2 < 1 && f2; i2++)
										{
											rule = decode_StringValue(":");
											if ((f2 = rule != null))
											{
												e2.Add(rule);
												c2++;
											}
										}
										decoded = c2 == 1;
									}
									if (decoded)
										e1.AddRange(e2);
									else
										index = s2;
								}
							}
							if (decoded)
								c1++;
							f1 = decoded;
						}
						decoded = c1 == 3;
					}
					if (decoded)
					{
						bool f1 = true;
						int c1 = 0;
						for (int i1 = 0; i1 < 1 && f1; i1++)
						{
							rule = decode_ls32();
							if ((f1 = rule != null))
							{
								e1.Add(rule);
								c1++;
							}
						}
						decoded = c1 == 1;
					}
					if (decoded)
						e0.AddRange(e1);
					else
						index = s1;
				}
			}
			if (!decoded)
			{
				{
					var e1 = new List<Rule>();
					int s1 = index;
					decoded = true;
					if (decoded)
					{
						bool f1 = true;
						int c1 = 0;
						for (int i1 = 0; i1 < 1 && f1; i1++)
						{
							decoded = false;
							if (!decoded)
							{
								{
									var e2 = new List<Rule>();
									int s2 = index;
									decoded = true;
									if (decoded)
									{
										bool f2 = true;
										int c2 = 0;
										for (int i2 = 0; i2 < 2 && f2; i2++)
										{
											decoded = false;
											if (!decoded)
											{
												{
													var e3 = new List<Rule>();
													int s3 = index;
													decoded = true;
													if (decoded)
													{
														bool f3 = true;
														int c3 = 0;
														for (int i3 = 0; i3 < 1 && f3; i3++)
														{
															rule = decode_h16();
															if ((f3 = rule != null))
															{
																e3.Add(rule);
																c3++;
															}
														}
														decoded = c3 == 1;
													}
													if (decoded)
													{
														bool f3 = true;
														int c3 = 0;
														for (int i3 = 0; i3 < 1 && f3; i3++)
														{
															rule = decode_StringValue(":");
															if ((f3 = rule != null))
															{
																e3.Add(rule);
																c3++;
															}
														}
														decoded = c3 == 1;
													}
													if (decoded)
														e2.AddRange(e3);
													else
														index = s3;
												}
											}
											if (decoded)
												c2++;
											f2 = decoded;
										}
										decoded = true;
									}
									if (decoded)
									{
										bool f2 = true;
										int c2 = 0;
										for (int i2 = 0; i2 < 1 && f2; i2++)
										{
											rule = decode_h16();
											if ((f2 = rule != null))
											{
												e2.Add(rule);
												c2++;
											}
										}
										decoded = c2 == 1;
									}
									if (decoded)
										e1.AddRange(e2);
									else
										index = s2;
								}
							}
							if (decoded)
								c1++;
							f1 = decoded;
						}
						decoded = true;
					}
					if (decoded)
					{
						bool f1 = true;
						int c1 = 0;
						for (int i1 = 0; i1 < 1 && f1; i1++)
						{
							rule = decode_StringValue("::");
							if ((f1 = rule != null))
							{
								e1.Add(rule);
								c1++;
							}
						}
						decoded = c1 == 1;
					}
					if (decoded)
					{
						bool f1 = true;
						int c1 = 0;
						for (int i1 = 0; i1 < 2 && f1; i1++)
						{
							decoded = false;
							if (!decoded)
							{
								{
									var e2 = new List<Rule>();
									int s2 = index;
									decoded = true;
									if (decoded)
									{
										bool f2 = true;
										int c2 = 0;
										for (int i2 = 0; i2 < 1 && f2; i2++)
										{
											rule = decode_h16();
											if ((f2 = rule != null))
											{
												e2.Add(rule);
												c2++;
											}
										}
										decoded = c2 == 1;
									}
									if (decoded)
									{
										bool f2 = true;
										int c2 = 0;
										for (int i2 = 0; i2 < 1 && f2; i2++)
										{
											rule = decode_StringValue(":");
											if ((f2 = rule != null))
											{
												e2.Add(rule);
												c2++;
											}
										}
										decoded = c2 == 1;
									}
									if (decoded)
										e1.AddRange(e2);
									else
										index = s2;
								}
							}
							if (decoded)
								c1++;
							f1 = decoded;
						}
						decoded = c1 == 2;
					}
					if (decoded)
					{
						bool f1 = true;
						int c1 = 0;
						for (int i1 = 0; i1 < 1 && f1; i1++)
						{
							rule = decode_ls32();
							if ((f1 = rule != null))
							{
								e1.Add(rule);
								c1++;
							}
						}
						decoded = c1 == 1;
					}
					if (decoded)
						e0.AddRange(e1);
					else
						index = s1;
				}
			}
			if (!decoded)
			{
				{
					var e1 = new List<Rule>();
					int s1 = index;
					decoded = true;
					if (decoded)
					{
						bool f1 = true;
						int c1 = 0;
						for (int i1 = 0; i1 < 1 && f1; i1++)
						{
							decoded = false;
							if (!decoded)
							{
								{
									var e2 = new List<Rule>();
									int s2 = index;
									decoded = true;
									if (decoded)
									{
										bool f2 = true;
										int c2 = 0;
										for (int i2 = 0; i2 < 3 && f2; i2++)
										{
											decoded = false;
											if (!decoded)
											{
												{
													var e3 = new List<Rule>();
													int s3 = index;
													decoded = true;
													if (decoded)
													{
														bool f3 = true;
														int c3 = 0;
														for (int i3 = 0; i3 < 1 && f3; i3++)
														{
															rule = decode_h16();
															if ((f3 = rule != null))
															{
																e3.Add(rule);
																c3++;
															}
														}
														decoded = c3 == 1;
													}
													if (decoded)
													{
														bool f3 = true;
														int c3 = 0;
														for (int i3 = 0; i3 < 1 && f3; i3++)
														{
															rule = decode_StringValue(":");
															if ((f3 = rule != null))
															{
																e3.Add(rule);
																c3++;
															}
														}
														decoded = c3 == 1;
													}
													if (decoded)
														e2.AddRange(e3);
													else
														index = s3;
												}
											}
											if (decoded)
												c2++;
											f2 = decoded;
										}
										decoded = true;
									}
									if (decoded)
									{
										bool f2 = true;
										int c2 = 0;
										for (int i2 = 0; i2 < 1 && f2; i2++)
										{
											rule = decode_h16();
											if ((f2 = rule != null))
											{
												e2.Add(rule);
												c2++;
											}
										}
										decoded = c2 == 1;
									}
									if (decoded)
										e1.AddRange(e2);
									else
										index = s2;
								}
							}
							if (decoded)
								c1++;
							f1 = decoded;
						}
						decoded = true;
					}
					if (decoded)
					{
						bool f1 = true;
						int c1 = 0;
						for (int i1 = 0; i1 < 1 && f1; i1++)
						{
							rule = decode_StringValue("::");
							if ((f1 = rule != null))
							{
								e1.Add(rule);
								c1++;
							}
						}
						decoded = c1 == 1;
					}
					if (decoded)
					{
						bool f1 = true;
						int c1 = 0;
						for (int i1 = 0; i1 < 1 && f1; i1++)
						{
							rule = decode_h16();
							if ((f1 = rule != null))
							{
								e1.Add(rule);
								c1++;
							}
						}
						decoded = c1 == 1;
					}
					if (decoded)
					{
						bool f1 = true;
						int c1 = 0;
						for (int i1 = 0; i1 < 1 && f1; i1++)
						{
							rule = decode_StringValue(":");
							if ((f1 = rule != null))
							{
								e1.Add(rule);
								c1++;
							}
						}
						decoded = c1 == 1;
					}
					if (decoded)
					{
						bool f1 = true;
						int c1 = 0;
						for (int i1 = 0; i1 < 1 && f1; i1++)
						{
							rule = decode_ls32();
							if ((f1 = rule != null))
							{
								e1.Add(rule);
								c1++;
							}
						}
						decoded = c1 == 1;
					}
					if (decoded)
						e0.AddRange(e1);
					else
						index = s1;
				}
			}
			if (!decoded)
			{
				{
					var e1 = new List<Rule>();
					int s1 = index;
					decoded = true;
					if (decoded)
					{
						bool f1 = true;
						int c1 = 0;
						for (int i1 = 0; i1 < 1 && f1; i1++)
						{
							decoded = false;
							if (!decoded)
							{
								{
									var e2 = new List<Rule>();
									int s2 = index;
									decoded = true;
									if (decoded)
									{
										bool f2 = true;
										int c2 = 0;
										for (int i2 = 0; i2 < 4 && f2; i2++)
										{
											decoded = false;
											if (!decoded)
											{
												{
													var e3 = new List<Rule>();
													int s3 = index;
													decoded = true;
													if (decoded)
													{
														bool f3 = true;
														int c3 = 0;
														for (int i3 = 0; i3 < 1 && f3; i3++)
														{
															rule = decode_h16();
															if ((f3 = rule != null))
															{
																e3.Add(rule);
																c3++;
															}
														}
														decoded = c3 == 1;
													}
													if (decoded)
													{
														bool f3 = true;
														int c3 = 0;
														for (int i3 = 0; i3 < 1 && f3; i3++)
														{
															rule = decode_StringValue(":");
															if ((f3 = rule != null))
															{
																e3.Add(rule);
																c3++;
															}
														}
														decoded = c3 == 1;
													}
													if (decoded)
														e2.AddRange(e3);
													else
														index = s3;
												}
											}
											if (decoded)
												c2++;
											f2 = decoded;
										}
										decoded = true;
									}
									if (decoded)
									{
										bool f2 = true;
										int c2 = 0;
										for (int i2 = 0; i2 < 1 && f2; i2++)
										{
											rule = decode_h16();
											if ((f2 = rule != null))
											{
												e2.Add(rule);
												c2++;
											}
										}
										decoded = c2 == 1;
									}
									if (decoded)
										e1.AddRange(e2);
									else
										index = s2;
								}
							}
							if (decoded)
								c1++;
							f1 = decoded;
						}
						decoded = true;
					}
					if (decoded)
					{
						bool f1 = true;
						int c1 = 0;
						for (int i1 = 0; i1 < 1 && f1; i1++)
						{
							rule = decode_StringValue("::");
							if ((f1 = rule != null))
							{
								e1.Add(rule);
								c1++;
							}
						}
						decoded = c1 == 1;
					}
					if (decoded)
					{
						bool f1 = true;
						int c1 = 0;
						for (int i1 = 0; i1 < 1 && f1; i1++)
						{
							rule = decode_ls32();
							if ((f1 = rule != null))
							{
								e1.Add(rule);
								c1++;
							}
						}
						decoded = c1 == 1;
					}
					if (decoded)
						e0.AddRange(e1);
					else
						index = s1;
				}
			}
			if (!decoded)
			{
				{
					var e1 = new List<Rule>();
					int s1 = index;
					decoded = true;
					if (decoded)
					{
						bool f1 = true;
						int c1 = 0;
						for (int i1 = 0; i1 < 1 && f1; i1++)
						{
							decoded = false;
							if (!decoded)
							{
								{
									var e2 = new List<Rule>();
									int s2 = index;
									decoded = true;
									if (decoded)
									{
										bool f2 = true;
										int c2 = 0;
										for (int i2 = 0; i2 < 5 && f2; i2++)
										{
											decoded = false;
											if (!decoded)
											{
												{
													var e3 = new List<Rule>();
													int s3 = index;
													decoded = true;
													if (decoded)
													{
														bool f3 = true;
														int c3 = 0;
														for (int i3 = 0; i3 < 1 && f3; i3++)
														{
															rule = decode_h16();
															if ((f3 = rule != null))
															{
																e3.Add(rule);
																c3++;
															}
														}
														decoded = c3 == 1;
													}
													if (decoded)
													{
														bool f3 = true;
														int c3 = 0;
														for (int i3 = 0; i3 < 1 && f3; i3++)
														{
															rule = decode_StringValue(":");
															if ((f3 = rule != null))
															{
																e3.Add(rule);
																c3++;
															}
														}
														decoded = c3 == 1;
													}
													if (decoded)
														e2.AddRange(e3);
													else
														index = s3;
												}
											}
											if (decoded)
												c2++;
											f2 = decoded;
										}
										decoded = true;
									}
									if (decoded)
									{
										bool f2 = true;
										int c2 = 0;
										for (int i2 = 0; i2 < 1 && f2; i2++)
										{
											rule = decode_h16();
											if ((f2 = rule != null))
											{
												e2.Add(rule);
												c2++;
											}
										}
										decoded = c2 == 1;
									}
									if (decoded)
										e1.AddRange(e2);
									else
										index = s2;
								}
							}
							if (decoded)
								c1++;
							f1 = decoded;
						}
						decoded = true;
					}
					if (decoded)
					{
						bool f1 = true;
						int c1 = 0;
						for (int i1 = 0; i1 < 1 && f1; i1++)
						{
							rule = decode_StringValue("::");
							if ((f1 = rule != null))
							{
								e1.Add(rule);
								c1++;
							}
						}
						decoded = c1 == 1;
					}
					if (decoded)
					{
						bool f1 = true;
						int c1 = 0;
						for (int i1 = 0; i1 < 1 && f1; i1++)
						{
							rule = decode_h16();
							if ((f1 = rule != null))
							{
								e1.Add(rule);
								c1++;
							}
						}
						decoded = c1 == 1;
					}
					if (decoded)
						e0.AddRange(e1);
					else
						index = s1;
				}
			}
			if (!decoded)
			{
				{
					var e1 = new List<Rule>();
					int s1 = index;
					decoded = true;
					if (decoded)
					{
						bool f1 = true;
						int c1 = 0;
						for (int i1 = 0; i1 < 1 && f1; i1++)
						{
							decoded = false;
							if (!decoded)
							{
								{
									var e2 = new List<Rule>();
									int s2 = index;
									decoded = true;
									if (decoded)
									{
										bool f2 = true;
										int c2 = 0;
										for (int i2 = 0; i2 < 6 && f2; i2++)
										{
											decoded = false;
											if (!decoded)
											{
												{
													var e3 = new List<Rule>();
													int s3 = index;
													decoded = true;
													if (decoded)
													{
														bool f3 = true;
														int c3 = 0;
														for (int i3 = 0; i3 < 1 && f3; i3++)
														{
															rule = decode_h16();
															if ((f3 = rule != null))
															{
																e3.Add(rule);
																c3++;
															}
														}
														decoded = c3 == 1;
													}
													if (decoded)
													{
														bool f3 = true;
														int c3 = 0;
														for (int i3 = 0; i3 < 1 && f3; i3++)
														{
															rule = decode_StringValue(":");
															if ((f3 = rule != null))
															{
																e3.Add(rule);
																c3++;
															}
														}
														decoded = c3 == 1;
													}
													if (decoded)
														e2.AddRange(e3);
													else
														index = s3;
												}
											}
											if (decoded)
												c2++;
											f2 = decoded;
										}
										decoded = true;
									}
									if (decoded)
									{
										bool f2 = true;
										int c2 = 0;
										for (int i2 = 0; i2 < 1 && f2; i2++)
										{
											rule = decode_h16();
											if ((f2 = rule != null))
											{
												e2.Add(rule);
												c2++;
											}
										}
										decoded = c2 == 1;
									}
									if (decoded)
										e1.AddRange(e2);
									else
										index = s2;
								}
							}
							if (decoded)
								c1++;
							f1 = decoded;
						}
						decoded = true;
					}
					if (decoded)
					{
						bool f1 = true;
						int c1 = 0;
						for (int i1 = 0; i1 < 1 && f1; i1++)
						{
							rule = decode_StringValue("::");
							if ((f1 = rule != null))
							{
								e1.Add(rule);
								c1++;
							}
						}
						decoded = c1 == 1;
					}
					if (decoded)
						e0.AddRange(e1);
					else
						index = s1;
				}
			}

			rule = null;
			if (decoded)
				rule = new IPv6address(text.Substring(s0, index - s0), e0);
			else
				index = s0;

			pop("IPv6address", decoded, index - s0);

			return (IPv6address)rule;
		}

		private ls32 decode_ls32()
		{
			push("ls32");

			bool decoded = true;
			int s0 = index;
			var e0 = new List<Rule>();
			Rule rule;

			decoded = false;
			if (!decoded)
			{
				{
					var e1 = new List<Rule>();
					int s1 = index;
					decoded = true;
					if (decoded)
					{
						bool f1 = true;
						int c1 = 0;
						for (int i1 = 0; i1 < 1 && f1; i1++)
						{
							decoded = false;
							if (!decoded)
							{
								{
									var e2 = new List<Rule>();
									int s2 = index;
									decoded = true;
									if (decoded)
									{
										bool f2 = true;
										int c2 = 0;
										for (int i2 = 0; i2 < 1 && f2; i2++)
										{
											rule = decode_h16();
											if ((f2 = rule != null))
											{
												e2.Add(rule);
												c2++;
											}
										}
										decoded = c2 == 1;
									}
									if (decoded)
									{
										bool f2 = true;
										int c2 = 0;
										for (int i2 = 0; i2 < 1 && f2; i2++)
										{
											rule = decode_StringValue(":");
											if ((f2 = rule != null))
											{
												e2.Add(rule);
												c2++;
											}
										}
										decoded = c2 == 1;
									}
									if (decoded)
									{
										bool f2 = true;
										int c2 = 0;
										for (int i2 = 0; i2 < 1 && f2; i2++)
										{
											rule = decode_h16();
											if ((f2 = rule != null))
											{
												e2.Add(rule);
												c2++;
											}
										}
										decoded = c2 == 1;
									}
									if (decoded)
										e1.AddRange(e2);
									else
										index = s2;
								}
							}
							if (decoded)
								c1++;
							f1 = decoded;
						}
						decoded = c1 == 1;
					}
					if (decoded)
						e0.AddRange(e1);
					else
						index = s1;
				}
			}
			if (!decoded)
			{
				{
					var e1 = new List<Rule>();
					int s1 = index;
					decoded = true;
					if (decoded)
					{
						bool f1 = true;
						int c1 = 0;
						for (int i1 = 0; i1 < 1 && f1; i1++)
						{
							rule = decode_IPv4address();
							if ((f1 = rule != null))
							{
								e1.Add(rule);
								c1++;
							}
						}
						decoded = c1 == 1;
					}
					if (decoded)
						e0.AddRange(e1);
					else
						index = s1;
				}
			}

			rule = null;
			if (decoded)
				rule = new ls32(text.Substring(s0, index - s0), e0);
			else
				index = s0;

			pop("ls32", decoded, index - s0);

			return (ls32)rule;
		}

		private h16 decode_h16()
		{
			push("h16");

			bool decoded = true;
			int s0 = index;
			var e0 = new List<Rule>();
			Rule rule;

			decoded = false;
			if (!decoded)
			{
				{
					var e1 = new List<Rule>();
					int s1 = index;
					decoded = true;
					if (decoded)
					{
						bool f1 = true;
						int c1 = 0;
						for (int i1 = 0; i1 < 1 && f1; i1++)
						{
							rule = decode_HEXDIG();
							if ((f1 = rule != null))
							{
								e1.Add(rule);
								c1++;
							}
						}
						for (int i1 = 1; i1 < 4 && f1; i1++)
						{
							rule = decode_HEXDIG();
							if ((f1 = rule != null))
							{
								e1.Add(rule);
								c1++;
							}
						}
						decoded = c1 >= 1;
					}
					if (decoded)
						e0.AddRange(e1);
					else
						index = s1;
				}
			}

			rule = null;
			if (decoded)
				rule = new h16(text.Substring(s0, index - s0), e0);
			else
				index = s0;

			pop("h16", decoded, index - s0);

			return (h16)rule;
		}

		private IPv4address decode_IPv4address()
		{
			push("IPv4address");

			bool decoded = true;
			int s0 = index;
			var e0 = new List<Rule>();
			Rule rule;

			decoded = false;
			if (!decoded)
			{
				{
					var e1 = new List<Rule>();
					int s1 = index;
					decoded = true;
					if (decoded)
					{
						bool f1 = true;
						int c1 = 0;
						for (int i1 = 0; i1 < 1 && f1; i1++)
						{
							rule = decode_dec_octet();
							if ((f1 = rule != null))
							{
								e1.Add(rule);
								c1++;
							}
						}
						decoded = c1 == 1;
					}
					if (decoded)
					{
						bool f1 = true;
						int c1 = 0;
						for (int i1 = 0; i1 < 1 && f1; i1++)
						{
							rule = decode_StringValue(".");
							if ((f1 = rule != null))
							{
								e1.Add(rule);
								c1++;
							}
						}
						decoded = c1 == 1;
					}
					if (decoded)
					{
						bool f1 = true;
						int c1 = 0;
						for (int i1 = 0; i1 < 1 && f1; i1++)
						{
							rule = decode_dec_octet();
							if ((f1 = rule != null))
							{
								e1.Add(rule);
								c1++;
							}
						}
						decoded = c1 == 1;
					}
					if (decoded)
					{
						bool f1 = true;
						int c1 = 0;
						for (int i1 = 0; i1 < 1 && f1; i1++)
						{
							rule = decode_StringValue(".");
							if ((f1 = rule != null))
							{
								e1.Add(rule);
								c1++;
							}
						}
						decoded = c1 == 1;
					}
					if (decoded)
					{
						bool f1 = true;
						int c1 = 0;
						for (int i1 = 0; i1 < 1 && f1; i1++)
						{
							rule = decode_dec_octet();
							if ((f1 = rule != null))
							{
								e1.Add(rule);
								c1++;
							}
						}
						decoded = c1 == 1;
					}
					if (decoded)
					{
						bool f1 = true;
						int c1 = 0;
						for (int i1 = 0; i1 < 1 && f1; i1++)
						{
							rule = decode_StringValue(".");
							if ((f1 = rule != null))
							{
								e1.Add(rule);
								c1++;
							}
						}
						decoded = c1 == 1;
					}
					if (decoded)
					{
						bool f1 = true;
						int c1 = 0;
						for (int i1 = 0; i1 < 1 && f1; i1++)
						{
							rule = decode_dec_octet();
							if ((f1 = rule != null))
							{
								e1.Add(rule);
								c1++;
							}
						}
						decoded = c1 == 1;
					}
					if (decoded)
						e0.AddRange(e1);
					else
						index = s1;
				}
			}

			rule = null;
			if (decoded)
				rule = new IPv4address(text.Substring(s0, index - s0), e0);
			else
				index = s0;

			pop("IPv4address", decoded, index - s0);

			return (IPv4address)rule;
		}

		private dec_octet decode_dec_octet()
		{
			push("dec-octet");

			bool decoded = true;
			int s0 = index;
			var e0 = new List<Rule>();
			Rule rule;

			decoded = false;
			if (!decoded)
			{
				{
					var e1 = new List<Rule>();
					int s1 = index;
					decoded = true;
					if (decoded)
					{
						bool f1 = true;
						int c1 = 0;
						for (int i1 = 0; i1 < 1 && f1; i1++)
						{
							rule = decode_DIGIT();
							if ((f1 = rule != null))
							{
								e1.Add(rule);
								c1++;
							}
						}
						decoded = c1 == 1;
					}
					if (decoded)
						e0.AddRange(e1);
					else
						index = s1;
				}
			}
			if (!decoded)
			{
				{
					var e1 = new List<Rule>();
					int s1 = index;
					decoded = true;
					if (decoded)
					{
						bool f1 = true;
						int c1 = 0;
						for (int i1 = 0; i1 < 1 && f1; i1++)
						{
							rule = decode_NumericValue("%x31-39", "[\\x31-\\x39]", 1);
							if ((f1 = rule != null))
							{
								e1.Add(rule);
								c1++;
							}
						}
						decoded = c1 == 1;
					}
					if (decoded)
					{
						bool f1 = true;
						int c1 = 0;
						for (int i1 = 0; i1 < 1 && f1; i1++)
						{
							rule = decode_DIGIT();
							if ((f1 = rule != null))
							{
								e1.Add(rule);
								c1++;
							}
						}
						decoded = c1 == 1;
					}
					if (decoded)
						e0.AddRange(e1);
					else
						index = s1;
				}
			}
			if (!decoded)
			{
				{
					var e1 = new List<Rule>();
					int s1 = index;
					decoded = true;
					if (decoded)
					{
						bool f1 = true;
						int c1 = 0;
						for (int i1 = 0; i1 < 1 && f1; i1++)
						{
							rule = decode_StringValue("1");
							if ((f1 = rule != null))
							{
								e1.Add(rule);
								c1++;
							}
						}
						decoded = c1 == 1;
					}
					if (decoded)
					{
						bool f1 = true;
						int c1 = 0;
						for (int i1 = 0; i1 < 2 && f1; i1++)
						{
							rule = decode_DIGIT();
							if ((f1 = rule != null))
							{
								e1.Add(rule);
								c1++;
							}
						}
						decoded = c1 == 2;
					}
					if (decoded)
						e0.AddRange(e1);
					else
						index = s1;
				}
			}
			if (!decoded)
			{
				{
					var e1 = new List<Rule>();
					int s1 = index;
					decoded = true;
					if (decoded)
					{
						bool f1 = true;
						int c1 = 0;
						for (int i1 = 0; i1 < 1 && f1; i1++)
						{
							rule = decode_StringValue("2");
							if ((f1 = rule != null))
							{
								e1.Add(rule);
								c1++;
							}
						}
						decoded = c1 == 1;
					}
					if (decoded)
					{
						bool f1 = true;
						int c1 = 0;
						for (int i1 = 0; i1 < 1 && f1; i1++)
						{
							rule = decode_NumericValue("%x30-34", "[\\x30-\\x34]", 1);
							if ((f1 = rule != null))
							{
								e1.Add(rule);
								c1++;
							}
						}
						decoded = c1 == 1;
					}
					if (decoded)
					{
						bool f1 = true;
						int c1 = 0;
						for (int i1 = 0; i1 < 1 && f1; i1++)
						{
							rule = decode_DIGIT();
							if ((f1 = rule != null))
							{
								e1.Add(rule);
								c1++;
							}
						}
						decoded = c1 == 1;
					}
					if (decoded)
						e0.AddRange(e1);
					else
						index = s1;
				}
			}
			if (!decoded)
			{
				{
					var e1 = new List<Rule>();
					int s1 = index;
					decoded = true;
					if (decoded)
					{
						bool f1 = true;
						int c1 = 0;
						for (int i1 = 0; i1 < 1 && f1; i1++)
						{
							rule = decode_StringValue("25");
							if ((f1 = rule != null))
							{
								e1.Add(rule);
								c1++;
							}
						}
						decoded = c1 == 1;
					}
					if (decoded)
					{
						bool f1 = true;
						int c1 = 0;
						for (int i1 = 0; i1 < 1 && f1; i1++)
						{
							rule = decode_NumericValue("%x30-35", "[\\x30-\\x35]", 1);
							if ((f1 = rule != null))
							{
								e1.Add(rule);
								c1++;
							}
						}
						decoded = c1 == 1;
					}
					if (decoded)
						e0.AddRange(e1);
					else
						index = s1;
				}
			}

			rule = null;
			if (decoded)
				rule = new dec_octet(text.Substring(s0, index - s0), e0);
			else
				index = s0;

			pop("dec-octet", decoded, index - s0);

			return (dec_octet)rule;
		}

		private ireg_name decode_ireg_name()
		{
			push("ireg-name");

			bool decoded = true;
			int s0 = index;
			var e0 = new List<Rule>();
			Rule rule;

			decoded = false;
			if (!decoded)
			{
				{
					var e1 = new List<Rule>();
					int s1 = index;
					decoded = true;
					if (decoded)
					{
						bool f1 = true;
						int c1 = 0;
						while (f1)
						{
							decoded = false;
							if (!decoded)
							{
								{
									var e2 = new List<Rule>();
									int s2 = index;
									decoded = true;
									if (decoded)
									{
										bool f2 = true;
										int c2 = 0;
										for (int i2 = 0; i2 < 1 && f2; i2++)
										{
											rule = decode_iunreserved();
											if ((f2 = rule != null))
											{
												e2.Add(rule);
												c2++;
											}
										}
										decoded = c2 == 1;
									}
									if (decoded)
										e1.AddRange(e2);
									else
										index = s2;
								}
							}
							if (!decoded)
							{
								{
									var e2 = new List<Rule>();
									int s2 = index;
									decoded = true;
									if (decoded)
									{
										bool f2 = true;
										int c2 = 0;
										for (int i2 = 0; i2 < 1 && f2; i2++)
										{
											rule = decode_pct_encoded();
											if ((f2 = rule != null))
											{
												e2.Add(rule);
												c2++;
											}
										}
										decoded = c2 == 1;
									}
									if (decoded)
										e1.AddRange(e2);
									else
										index = s2;
								}
							}
							if (!decoded)
							{
								{
									var e2 = new List<Rule>();
									int s2 = index;
									decoded = true;
									if (decoded)
									{
										bool f2 = true;
										int c2 = 0;
										for (int i2 = 0; i2 < 1 && f2; i2++)
										{
											rule = decode_sub_delims();
											if ((f2 = rule != null))
											{
												e2.Add(rule);
												c2++;
											}
										}
										decoded = c2 == 1;
									}
									if (decoded)
										e1.AddRange(e2);
									else
										index = s2;
								}
							}
							if (decoded)
								c1++;
							f1 = decoded;
						}
						decoded = true;
					}
					if (decoded)
						e0.AddRange(e1);
					else
						index = s1;
				}
			}

			rule = null;
			if (decoded)
				rule = new ireg_name(text.Substring(s0, index - s0), e0);
			else
				index = s0;

			pop("ireg-name", decoded, index - s0);

			return (ireg_name)rule;
		}

		private port decode_port()
		{
			push("port");

			bool decoded = true;
			int s0 = index;
			var e0 = new List<Rule>();
			Rule rule;

			decoded = false;
			if (!decoded)
			{
				{
					var e1 = new List<Rule>();
					int s1 = index;
					decoded = true;
					if (decoded)
					{
						bool f1 = true;
						int c1 = 0;
						while (f1)
						{
							rule = decode_DIGIT();
							if ((f1 = rule != null))
							{
								e1.Add(rule);
								c1++;
							}
						}
						decoded = true;
					}
					if (decoded)
						e0.AddRange(e1);
					else
						index = s1;
				}
			}

			rule = null;
			if (decoded)
				rule = new port(text.Substring(s0, index - s0), e0);
			else
				index = s0;

			pop("port", decoded, index - s0);

			return (port)rule;
		}

		private ipath_abempty decode_ipath_abempty()
		{
			push("ipath-abempty");

			bool decoded = true;
			int s0 = index;
			var e0 = new List<Rule>();
			Rule rule;

			decoded = false;
			if (!decoded)
			{
				{
					var e1 = new List<Rule>();
					int s1 = index;
					decoded = true;
					if (decoded)
					{
						bool f1 = true;
						int c1 = 0;
						while (f1)
						{
							decoded = false;
							if (!decoded)
							{
								{
									var e2 = new List<Rule>();
									int s2 = index;
									decoded = true;
									if (decoded)
									{
										bool f2 = true;
										int c2 = 0;
										for (int i2 = 0; i2 < 1 && f2; i2++)
										{
											rule = decode_StringValue("/");
											if ((f2 = rule != null))
											{
												e2.Add(rule);
												c2++;
											}
										}
										decoded = c2 == 1;
									}
									if (decoded)
									{
										bool f2 = true;
										int c2 = 0;
										for (int i2 = 0; i2 < 1 && f2; i2++)
										{
											rule = decode_isegment();
											if ((f2 = rule != null))
											{
												e2.Add(rule);
												c2++;
											}
										}
										decoded = c2 == 1;
									}
									if (decoded)
										e1.AddRange(e2);
									else
										index = s2;
								}
							}
							if (decoded)
								c1++;
							f1 = decoded;
						}
						decoded = true;
					}
					if (decoded)
						e0.AddRange(e1);
					else
						index = s1;
				}
			}

			rule = null;
			if (decoded)
				rule = new ipath_abempty(text.Substring(s0, index - s0), e0);
			else
				index = s0;

			pop("ipath-abempty", decoded, index - s0);

			return (ipath_abempty)rule;
		}

		private ipath_abs decode_ipath_abs()
		{
			push("ipath-abs");

			bool decoded = true;
			int s0 = index;
			var e0 = new List<Rule>();
			Rule rule;

			decoded = false;
			if (!decoded)
			{
				{
					var e1 = new List<Rule>();
					int s1 = index;
					decoded = true;
					if (decoded)
					{
						bool f1 = true;
						int c1 = 0;
						for (int i1 = 0; i1 < 1 && f1; i1++)
						{
							rule = decode_StringValue("/");
							if ((f1 = rule != null))
							{
								e1.Add(rule);
								c1++;
							}
						}
						decoded = c1 == 1;
					}
					if (decoded)
					{
						bool f1 = true;
						int c1 = 0;
						for (int i1 = 0; i1 < 1 && f1; i1++)
						{
							decoded = false;
							if (!decoded)
							{
								{
									var e2 = new List<Rule>();
									int s2 = index;
									decoded = true;
									if (decoded)
									{
										bool f2 = true;
										int c2 = 0;
										for (int i2 = 0; i2 < 1 && f2; i2++)
										{
											rule = decode_isegment_nz();
											if ((f2 = rule != null))
											{
												e2.Add(rule);
												c2++;
											}
										}
										decoded = c2 == 1;
									}
									if (decoded)
									{
										bool f2 = true;
										int c2 = 0;
										while (f2)
										{
											decoded = false;
											if (!decoded)
											{
												{
													var e3 = new List<Rule>();
													int s3 = index;
													decoded = true;
													if (decoded)
													{
														bool f3 = true;
														int c3 = 0;
														for (int i3 = 0; i3 < 1 && f3; i3++)
														{
															rule = decode_StringValue("/");
															if ((f3 = rule != null))
															{
																e3.Add(rule);
																c3++;
															}
														}
														decoded = c3 == 1;
													}
													if (decoded)
													{
														bool f3 = true;
														int c3 = 0;
														for (int i3 = 0; i3 < 1 && f3; i3++)
														{
															rule = decode_isegment();
															if ((f3 = rule != null))
															{
																e3.Add(rule);
																c3++;
															}
														}
														decoded = c3 == 1;
													}
													if (decoded)
														e2.AddRange(e3);
													else
														index = s3;
												}
											}
											if (decoded)
												c2++;
											f2 = decoded;
										}
										decoded = true;
									}
									if (decoded)
										e1.AddRange(e2);
									else
										index = s2;
								}
							}
							if (decoded)
								c1++;
							f1 = decoded;
						}
						decoded = true;
					}
					if (decoded)
						e0.AddRange(e1);
					else
						index = s1;
				}
			}

			rule = null;
			if (decoded)
				rule = new ipath_abs(text.Substring(s0, index - s0), e0);
			else
				index = s0;

			pop("ipath-abs", decoded, index - s0);

			return (ipath_abs)rule;
		}

		private ipath_rootless decode_ipath_rootless()
		{
			push("ipath-rootless");

			bool decoded = true;
			int s0 = index;
			var e0 = new List<Rule>();
			Rule rule;

			decoded = false;
			if (!decoded)
			{
				{
					var e1 = new List<Rule>();
					int s1 = index;
					decoded = true;
					if (decoded)
					{
						bool f1 = true;
						int c1 = 0;
						for (int i1 = 0; i1 < 1 && f1; i1++)
						{
							rule = decode_isegment_nz();
							if ((f1 = rule != null))
							{
								e1.Add(rule);
								c1++;
							}
						}
						decoded = c1 == 1;
					}
					if (decoded)
					{
						bool f1 = true;
						int c1 = 0;
						while (f1)
						{
							decoded = false;
							if (!decoded)
							{
								{
									var e2 = new List<Rule>();
									int s2 = index;
									decoded = true;
									if (decoded)
									{
										bool f2 = true;
										int c2 = 0;
										for (int i2 = 0; i2 < 1 && f2; i2++)
										{
											rule = decode_StringValue("/");
											if ((f2 = rule != null))
											{
												e2.Add(rule);
												c2++;
											}
										}
										decoded = c2 == 1;
									}
									if (decoded)
									{
										bool f2 = true;
										int c2 = 0;
										for (int i2 = 0; i2 < 1 && f2; i2++)
										{
											rule = decode_isegment();
											if ((f2 = rule != null))
											{
												e2.Add(rule);
												c2++;
											}
										}
										decoded = c2 == 1;
									}
									if (decoded)
										e1.AddRange(e2);
									else
										index = s2;
								}
							}
							if (decoded)
								c1++;
							f1 = decoded;
						}
						decoded = true;
					}
					if (decoded)
						e0.AddRange(e1);
					else
						index = s1;
				}
			}

			rule = null;
			if (decoded)
				rule = new ipath_rootless(text.Substring(s0, index - s0), e0);
			else
				index = s0;

			pop("ipath-rootless", decoded, index - s0);

			return (ipath_rootless)rule;
		}

		private ipath_empty decode_ipath_empty()
		{
			push("ipath-empty");

			bool decoded = true;
			int s0 = index;
			var e0 = new List<Rule>();
			Rule rule;

			decoded = false;
			if (!decoded)
			{
				{
					var e1 = new List<Rule>();
					int s1 = index;
					decoded = true;
					if (decoded)
					{
						bool f1 = true;
						int c1 = 0;
						for (int i1 = 0; i1 < 1 && f1; i1++)
						{
							rule = decode_StringValue("");
							if ((f1 = rule != null))
							{
								e1.Add(rule);
								c1++;
							}
						}
						decoded = c1 == 1;
					}
					if (decoded)
						e0.AddRange(e1);
					else
						index = s1;
				}
			}

			rule = null;
			if (decoded)
				rule = new ipath_empty(text.Substring(s0, index - s0), e0);
			else
				index = s0;

			pop("ipath-empty", decoded, index - s0);

			return (ipath_empty)rule;
		}

		private isegment decode_isegment()
		{
			push("isegment");

			bool decoded = true;
			int s0 = index;
			var e0 = new List<Rule>();
			Rule rule;

			decoded = false;
			if (!decoded)
			{
				{
					var e1 = new List<Rule>();
					int s1 = index;
					decoded = true;
					if (decoded)
					{
						bool f1 = true;
						int c1 = 0;
						while (f1)
						{
							rule = decode_ipchar();
							if ((f1 = rule != null))
							{
								e1.Add(rule);
								c1++;
							}
						}
						decoded = true;
					}
					if (decoded)
						e0.AddRange(e1);
					else
						index = s1;
				}
			}

			rule = null;
			if (decoded)
				rule = new isegment(text.Substring(s0, index - s0), e0);
			else
				index = s0;

			pop("isegment", decoded, index - s0);

			return (isegment)rule;
		}

		private isegment_nz decode_isegment_nz()
		{
			push("isegment-nz");

			bool decoded = true;
			int s0 = index;
			var e0 = new List<Rule>();
			Rule rule;

			decoded = false;
			if (!decoded)
			{
				{
					var e1 = new List<Rule>();
					int s1 = index;
					decoded = true;
					if (decoded)
					{
						bool f1 = true;
						int c1 = 0;
						for (int i1 = 0; i1 < 1 && f1; i1++)
						{
							rule = decode_ipchar();
							if ((f1 = rule != null))
							{
								e1.Add(rule);
								c1++;
							}
						}
						while (f1)
						{
							rule = decode_ipchar();
							if ((f1 = rule != null))
							{
								e1.Add(rule);
								c1++;
							}
						}
						decoded = c1 >= 1;
					}
					if (decoded)
						e0.AddRange(e1);
					else
						index = s1;
				}
			}

			rule = null;
			if (decoded)
				rule = new isegment_nz(text.Substring(s0, index - s0), e0);
			else
				index = s0;

			pop("isegment-nz", decoded, index - s0);

			return (isegment_nz)rule;
		}

		private iquery decode_iquery()
		{
			push("iquery");

			bool decoded = true;
			int s0 = index;
			var e0 = new List<Rule>();
			Rule rule;

			decoded = false;
			if (!decoded)
			{
				{
					var e1 = new List<Rule>();
					int s1 = index;
					decoded = true;
					if (decoded)
					{
						bool f1 = true;
						int c1 = 0;
						while (f1)
						{
							decoded = false;
							if (!decoded)
							{
								{
									var e2 = new List<Rule>();
									int s2 = index;
									decoded = true;
									if (decoded)
									{
										bool f2 = true;
										int c2 = 0;
										for (int i2 = 0; i2 < 1 && f2; i2++)
										{
											rule = decode_ipchar();
											if ((f2 = rule != null))
											{
												e2.Add(rule);
												c2++;
											}
										}
										decoded = c2 == 1;
									}
									if (decoded)
										e1.AddRange(e2);
									else
										index = s2;
								}
							}
							if (!decoded)
							{
								{
									var e2 = new List<Rule>();
									int s2 = index;
									decoded = true;
									if (decoded)
									{
										bool f2 = true;
										int c2 = 0;
										for (int i2 = 0; i2 < 1 && f2; i2++)
										{
											rule = decode_iprivate();
											if ((f2 = rule != null))
											{
												e2.Add(rule);
												c2++;
											}
										}
										decoded = c2 == 1;
									}
									if (decoded)
										e1.AddRange(e2);
									else
										index = s2;
								}
							}
							if (!decoded)
							{
								{
									var e2 = new List<Rule>();
									int s2 = index;
									decoded = true;
									if (decoded)
									{
										bool f2 = true;
										int c2 = 0;
										for (int i2 = 0; i2 < 1 && f2; i2++)
										{
											rule = decode_StringValue("/");
											if ((f2 = rule != null))
											{
												e2.Add(rule);
												c2++;
											}
										}
										decoded = c2 == 1;
									}
									if (decoded)
										e1.AddRange(e2);
									else
										index = s2;
								}
							}
							if (!decoded)
							{
								{
									var e2 = new List<Rule>();
									int s2 = index;
									decoded = true;
									if (decoded)
									{
										bool f2 = true;
										int c2 = 0;
										for (int i2 = 0; i2 < 1 && f2; i2++)
										{
											rule = decode_StringValue("?");
											if ((f2 = rule != null))
											{
												e2.Add(rule);
												c2++;
											}
										}
										decoded = c2 == 1;
									}
									if (decoded)
										e1.AddRange(e2);
									else
										index = s2;
								}
							}
							if (decoded)
								c1++;
							f1 = decoded;
						}
						decoded = true;
					}
					if (decoded)
						e0.AddRange(e1);
					else
						index = s1;
				}
			}

			rule = null;
			if (decoded)
				rule = new iquery(text.Substring(s0, index - s0), e0);
			else
				index = s0;

			pop("iquery", decoded, index - s0);

			return (iquery)rule;
		}

		private iprivate decode_iprivate()
		{
			push("iprivate");

			bool decoded = true;
			int s0 = index;
			var e0 = new List<Rule>();
			Rule rule;

			decoded = false;
			if (!decoded)
			{
				{
					var e1 = new List<Rule>();
					int s1 = index;
					decoded = true;
					if (decoded)
					{
						bool f1 = true;
						int c1 = 0;
						for (int i1 = 0; i1 < 1 && f1; i1++)
						{
							rule = decode_NumericValue("%xE000-F8FF", "[\\uE000-\\uF8FF]", 1);
							if ((f1 = rule != null))
							{
								e1.Add(rule);
								c1++;
							}
						}
						decoded = c1 == 1;
					}
					if (decoded)
						e0.AddRange(e1);
					else
						index = s1;
				}
			}

			rule = null;
			if (decoded)
				rule = new iprivate(text.Substring(s0, index - s0), e0);
			else
				index = s0;

			pop("iprivate", decoded, index - s0);

			return (iprivate)rule;
		}

		private ifragment decode_ifragment()
		{
			push("ifragment");

			bool decoded = true;
			int s0 = index;
			var e0 = new List<Rule>();
			Rule rule;

			decoded = false;
			if (!decoded)
			{
				{
					var e1 = new List<Rule>();
					int s1 = index;
					decoded = true;
					if (decoded)
					{
						bool f1 = true;
						int c1 = 0;
						while (f1)
						{
							decoded = false;
							if (!decoded)
							{
								{
									var e2 = new List<Rule>();
									int s2 = index;
									decoded = true;
									if (decoded)
									{
										bool f2 = true;
										int c2 = 0;
										for (int i2 = 0; i2 < 1 && f2; i2++)
										{
											rule = decode_ipchar();
											if ((f2 = rule != null))
											{
												e2.Add(rule);
												c2++;
											}
										}
										decoded = c2 == 1;
									}
									if (decoded)
										e1.AddRange(e2);
									else
										index = s2;
								}
							}
							if (!decoded)
							{
								{
									var e2 = new List<Rule>();
									int s2 = index;
									decoded = true;
									if (decoded)
									{
										bool f2 = true;
										int c2 = 0;
										for (int i2 = 0; i2 < 1 && f2; i2++)
										{
											rule = decode_StringValue("/");
											if ((f2 = rule != null))
											{
												e2.Add(rule);
												c2++;
											}
										}
										decoded = c2 == 1;
									}
									if (decoded)
										e1.AddRange(e2);
									else
										index = s2;
								}
							}
							if (!decoded)
							{
								{
									var e2 = new List<Rule>();
									int s2 = index;
									decoded = true;
									if (decoded)
									{
										bool f2 = true;
										int c2 = 0;
										for (int i2 = 0; i2 < 1 && f2; i2++)
										{
											rule = decode_StringValue("?");
											if ((f2 = rule != null))
											{
												e2.Add(rule);
												c2++;
											}
										}
										decoded = c2 == 1;
									}
									if (decoded)
										e1.AddRange(e2);
									else
										index = s2;
								}
							}
							if (decoded)
								c1++;
							f1 = decoded;
						}
						decoded = true;
					}
					if (decoded)
						e0.AddRange(e1);
					else
						index = s1;
				}
			}

			rule = null;
			if (decoded)
				rule = new ifragment(text.Substring(s0, index - s0), e0);
			else
				index = s0;

			pop("ifragment", decoded, index - s0);

			return (ifragment)rule;
		}

		private ipchar decode_ipchar()
		{
			push("ipchar");

			bool decoded = true;
			int s0 = index;
			var e0 = new List<Rule>();
			Rule rule;

			decoded = false;
			if (!decoded)
			{
				{
					var e1 = new List<Rule>();
					int s1 = index;
					decoded = true;
					if (decoded)
					{
						bool f1 = true;
						int c1 = 0;
						for (int i1 = 0; i1 < 1 && f1; i1++)
						{
							rule = decode_iunreserved();
							if ((f1 = rule != null))
							{
								e1.Add(rule);
								c1++;
							}
						}
						decoded = c1 == 1;
					}
					if (decoded)
						e0.AddRange(e1);
					else
						index = s1;
				}
			}
			if (!decoded)
			{
				{
					var e1 = new List<Rule>();
					int s1 = index;
					decoded = true;
					if (decoded)
					{
						bool f1 = true;
						int c1 = 0;
						for (int i1 = 0; i1 < 1 && f1; i1++)
						{
							rule = decode_pct_encoded();
							if ((f1 = rule != null))
							{
								e1.Add(rule);
								c1++;
							}
						}
						decoded = c1 == 1;
					}
					if (decoded)
						e0.AddRange(e1);
					else
						index = s1;
				}
			}
			if (!decoded)
			{
				{
					var e1 = new List<Rule>();
					int s1 = index;
					decoded = true;
					if (decoded)
					{
						bool f1 = true;
						int c1 = 0;
						for (int i1 = 0; i1 < 1 && f1; i1++)
						{
							rule = decode_sub_delims();
							if ((f1 = rule != null))
							{
								e1.Add(rule);
								c1++;
							}
						}
						decoded = c1 == 1;
					}
					if (decoded)
						e0.AddRange(e1);
					else
						index = s1;
				}
			}
			if (!decoded)
			{
				{
					var e1 = new List<Rule>();
					int s1 = index;
					decoded = true;
					if (decoded)
					{
						bool f1 = true;
						int c1 = 0;
						for (int i1 = 0; i1 < 1 && f1; i1++)
						{
							rule = decode_StringValue(":");
							if ((f1 = rule != null))
							{
								e1.Add(rule);
								c1++;
							}
						}
						decoded = c1 == 1;
					}
					if (decoded)
						e0.AddRange(e1);
					else
						index = s1;
				}
			}
			if (!decoded)
			{
				{
					var e1 = new List<Rule>();
					int s1 = index;
					decoded = true;
					if (decoded)
					{
						bool f1 = true;
						int c1 = 0;
						for (int i1 = 0; i1 < 1 && f1; i1++)
						{
							rule = decode_StringValue("@");
							if ((f1 = rule != null))
							{
								e1.Add(rule);
								c1++;
							}
						}
						decoded = c1 == 1;
					}
					if (decoded)
						e0.AddRange(e1);
					else
						index = s1;
				}
			}

			rule = null;
			if (decoded)
				rule = new ipchar(text.Substring(s0, index - s0), e0);
			else
				index = s0;

			pop("ipchar", decoded, index - s0);

			return (ipchar)rule;
		}

		private iunreserved decode_iunreserved()
		{
			push("iunreserved");

			bool decoded = true;
			int s0 = index;
			var e0 = new List<Rule>();
			Rule rule;

			decoded = false;
			if (!decoded)
			{
				{
					var e1 = new List<Rule>();
					int s1 = index;
					decoded = true;
					if (decoded)
					{
						bool f1 = true;
						int c1 = 0;
						for (int i1 = 0; i1 < 1 && f1; i1++)
						{
							rule = decode_ALPHA();
							if ((f1 = rule != null))
							{
								e1.Add(rule);
								c1++;
							}
						}
						decoded = c1 == 1;
					}
					if (decoded)
						e0.AddRange(e1);
					else
						index = s1;
				}
			}
			if (!decoded)
			{
				{
					var e1 = new List<Rule>();
					int s1 = index;
					decoded = true;
					if (decoded)
					{
						bool f1 = true;
						int c1 = 0;
						for (int i1 = 0; i1 < 1 && f1; i1++)
						{
							rule = decode_DIGIT();
							if ((f1 = rule != null))
							{
								e1.Add(rule);
								c1++;
							}
						}
						decoded = c1 == 1;
					}
					if (decoded)
						e0.AddRange(e1);
					else
						index = s1;
				}
			}
			if (!decoded)
			{
				{
					var e1 = new List<Rule>();
					int s1 = index;
					decoded = true;
					if (decoded)
					{
						bool f1 = true;
						int c1 = 0;
						for (int i1 = 0; i1 < 1 && f1; i1++)
						{
							rule = decode_StringValue("-");
							if ((f1 = rule != null))
							{
								e1.Add(rule);
								c1++;
							}
						}
						decoded = c1 == 1;
					}
					if (decoded)
						e0.AddRange(e1);
					else
						index = s1;
				}
			}
			if (!decoded)
			{
				{
					var e1 = new List<Rule>();
					int s1 = index;
					decoded = true;
					if (decoded)
					{
						bool f1 = true;
						int c1 = 0;
						for (int i1 = 0; i1 < 1 && f1; i1++)
						{
							rule = decode_StringValue(".");
							if ((f1 = rule != null))
							{
								e1.Add(rule);
								c1++;
							}
						}
						decoded = c1 == 1;
					}
					if (decoded)
						e0.AddRange(e1);
					else
						index = s1;
				}
			}
			if (!decoded)
			{
				{
					var e1 = new List<Rule>();
					int s1 = index;
					decoded = true;
					if (decoded)
					{
						bool f1 = true;
						int c1 = 0;
						for (int i1 = 0; i1 < 1 && f1; i1++)
						{
							rule = decode_StringValue("_");
							if ((f1 = rule != null))
							{
								e1.Add(rule);
								c1++;
							}
						}
						decoded = c1 == 1;
					}
					if (decoded)
						e0.AddRange(e1);
					else
						index = s1;
				}
			}
			if (!decoded)
			{
				{
					var e1 = new List<Rule>();
					int s1 = index;
					decoded = true;
					if (decoded)
					{
						bool f1 = true;
						int c1 = 0;
						for (int i1 = 0; i1 < 1 && f1; i1++)
						{
							rule = decode_StringValue("~");
							if ((f1 = rule != null))
							{
								e1.Add(rule);
								c1++;
							}
						}
						decoded = c1 == 1;
					}
					if (decoded)
						e0.AddRange(e1);
					else
						index = s1;
				}
			}
			if (!decoded)
			{
				{
					var e1 = new List<Rule>();
					int s1 = index;
					decoded = true;
					if (decoded)
					{
						bool f1 = true;
						int c1 = 0;
						for (int i1 = 0; i1 < 1 && f1; i1++)
						{
							rule = decode_ucschar();
							if ((f1 = rule != null))
							{
								e1.Add(rule);
								c1++;
							}
						}
						decoded = c1 == 1;
					}
					if (decoded)
						e0.AddRange(e1);
					else
						index = s1;
				}
			}

			rule = null;
			if (decoded)
				rule = new iunreserved(text.Substring(s0, index - s0), e0);
			else
				index = s0;

			pop("iunreserved", decoded, index - s0);

			return (iunreserved)rule;
		}

		private pct_encoded decode_pct_encoded()
		{
			push("pct-encoded");

			bool decoded = true;
			int s0 = index;
			var e0 = new List<Rule>();
			Rule rule;

			decoded = false;
			if (!decoded)
			{
				{
					var e1 = new List<Rule>();
					int s1 = index;
					decoded = true;
					if (decoded)
					{
						bool f1 = true;
						int c1 = 0;
						for (int i1 = 0; i1 < 1 && f1; i1++)
						{
							rule = decode_StringValue("%");
							if ((f1 = rule != null))
							{
								e1.Add(rule);
								c1++;
							}
						}
						decoded = c1 == 1;
					}
					if (decoded)
					{
						bool f1 = true;
						int c1 = 0;
						for (int i1 = 0; i1 < 1 && f1; i1++)
						{
							rule = decode_HEXDIG();
							if ((f1 = rule != null))
							{
								e1.Add(rule);
								c1++;
							}
						}
						decoded = c1 == 1;
					}
					if (decoded)
					{
						bool f1 = true;
						int c1 = 0;
						for (int i1 = 0; i1 < 1 && f1; i1++)
						{
							rule = decode_HEXDIG();
							if ((f1 = rule != null))
							{
								e1.Add(rule);
								c1++;
							}
						}
						decoded = c1 == 1;
					}
					if (decoded)
						e0.AddRange(e1);
					else
						index = s1;
				}
			}

			rule = null;
			if (decoded)
				rule = new pct_encoded(text.Substring(s0, index - s0), e0);
			else
				index = s0;

			pop("pct-encoded", decoded, index - s0);

			return (pct_encoded)rule;
		}

		private ucschar decode_ucschar()
		{
			push("ucschar");

			bool decoded = true;
			int s0 = index;
			var e0 = new List<Rule>();
			Rule rule;

			decoded = false;
			if (!decoded)
			{
				{
					var e1 = new List<Rule>();
					int s1 = index;
					decoded = true;
					if (decoded)
					{
						bool f1 = true;
						int c1 = 0;
						for (int i1 = 0; i1 < 1 && f1; i1++)
						{
							rule = decode_NumericValue("%xA0-D7FF", "[\\xA0-\\uD7FF]", 1);
							if ((f1 = rule != null))
							{
								e1.Add(rule);
								c1++;
							}
						}
						decoded = c1 == 1;
					}
					if (decoded)
						e0.AddRange(e1);
					else
						index = s1;
				}
			}
			if (!decoded)
			{
				{
					var e1 = new List<Rule>();
					int s1 = index;
					decoded = true;
					if (decoded)
					{
						bool f1 = true;
						int c1 = 0;
						for (int i1 = 0; i1 < 1 && f1; i1++)
						{
							rule = decode_NumericValue("%xF900-FDCF", "[\\uF900-\\uFDCF]", 1);
							if ((f1 = rule != null))
							{
								e1.Add(rule);
								c1++;
							}
						}
						decoded = c1 == 1;
					}
					if (decoded)
						e0.AddRange(e1);
					else
						index = s1;
				}
			}
			if (!decoded)
			{
				{
					var e1 = new List<Rule>();
					int s1 = index;
					decoded = true;
					if (decoded)
					{
						bool f1 = true;
						int c1 = 0;
						for (int i1 = 0; i1 < 1 && f1; i1++)
						{
							rule = decode_NumericValue("%xFDF0-FFEF", "[\\uFDF0-\\uFFEF]", 1);
							if ((f1 = rule != null))
							{
								e1.Add(rule);
								c1++;
							}
						}
						decoded = c1 == 1;
					}
					if (decoded)
						e0.AddRange(e1);
					else
						index = s1;
				}
			}

			rule = null;
			if (decoded)
				rule = new ucschar(text.Substring(s0, index - s0), e0);
			else
				index = s0;

			pop("ucschar", decoded, index - s0);

			return (ucschar)rule;
		}

		private reserved decode_reserved()
		{
			push("reserved");

			bool decoded = true;
			int s0 = index;
			var e0 = new List<Rule>();
			Rule rule;

			decoded = false;
			if (!decoded)
			{
				{
					var e1 = new List<Rule>();
					int s1 = index;
					decoded = true;
					if (decoded)
					{
						bool f1 = true;
						int c1 = 0;
						for (int i1 = 0; i1 < 1 && f1; i1++)
						{
							rule = decode_gen_delims();
							if ((f1 = rule != null))
							{
								e1.Add(rule);
								c1++;
							}
						}
						decoded = c1 == 1;
					}
					if (decoded)
						e0.AddRange(e1);
					else
						index = s1;
				}
			}
			if (!decoded)
			{
				{
					var e1 = new List<Rule>();
					int s1 = index;
					decoded = true;
					if (decoded)
					{
						bool f1 = true;
						int c1 = 0;
						for (int i1 = 0; i1 < 1 && f1; i1++)
						{
							rule = decode_sub_delims();
							if ((f1 = rule != null))
							{
								e1.Add(rule);
								c1++;
							}
						}
						decoded = c1 == 1;
					}
					if (decoded)
						e0.AddRange(e1);
					else
						index = s1;
				}
			}

			rule = null;
			if (decoded)
				rule = new reserved(text.Substring(s0, index - s0), e0);
			else
				index = s0;

			pop("reserved", decoded, index - s0);

			return (reserved)rule;
		}

		private gen_delims decode_gen_delims()
		{
			push("gen-delims");

			bool decoded = true;
			int s0 = index;
			var e0 = new List<Rule>();
			Rule rule;

			decoded = false;
			if (!decoded)
			{
				{
					var e1 = new List<Rule>();
					int s1 = index;
					decoded = true;
					if (decoded)
					{
						bool f1 = true;
						int c1 = 0;
						for (int i1 = 0; i1 < 1 && f1; i1++)
						{
							rule = decode_StringValue(":");
							if ((f1 = rule != null))
							{
								e1.Add(rule);
								c1++;
							}
						}
						decoded = c1 == 1;
					}
					if (decoded)
						e0.AddRange(e1);
					else
						index = s1;
				}
			}
			if (!decoded)
			{
				{
					var e1 = new List<Rule>();
					int s1 = index;
					decoded = true;
					if (decoded)
					{
						bool f1 = true;
						int c1 = 0;
						for (int i1 = 0; i1 < 1 && f1; i1++)
						{
							rule = decode_StringValue("/");
							if ((f1 = rule != null))
							{
								e1.Add(rule);
								c1++;
							}
						}
						decoded = c1 == 1;
					}
					if (decoded)
						e0.AddRange(e1);
					else
						index = s1;
				}
			}
			if (!decoded)
			{
				{
					var e1 = new List<Rule>();
					int s1 = index;
					decoded = true;
					if (decoded)
					{
						bool f1 = true;
						int c1 = 0;
						for (int i1 = 0; i1 < 1 && f1; i1++)
						{
							rule = decode_StringValue("?");
							if ((f1 = rule != null))
							{
								e1.Add(rule);
								c1++;
							}
						}
						decoded = c1 == 1;
					}
					if (decoded)
						e0.AddRange(e1);
					else
						index = s1;
				}
			}
			if (!decoded)
			{
				{
					var e1 = new List<Rule>();
					int s1 = index;
					decoded = true;
					if (decoded)
					{
						bool f1 = true;
						int c1 = 0;
						for (int i1 = 0; i1 < 1 && f1; i1++)
						{
							rule = decode_StringValue("#");
							if ((f1 = rule != null))
							{
								e1.Add(rule);
								c1++;
							}
						}
						decoded = c1 == 1;
					}
					if (decoded)
						e0.AddRange(e1);
					else
						index = s1;
				}
			}
			if (!decoded)
			{
				{
					var e1 = new List<Rule>();
					int s1 = index;
					decoded = true;
					if (decoded)
					{
						bool f1 = true;
						int c1 = 0;
						for (int i1 = 0; i1 < 1 && f1; i1++)
						{
							rule = decode_StringValue("[");
							if ((f1 = rule != null))
							{
								e1.Add(rule);
								c1++;
							}
						}
						decoded = c1 == 1;
					}
					if (decoded)
						e0.AddRange(e1);
					else
						index = s1;
				}
			}
			if (!decoded)
			{
				{
					var e1 = new List<Rule>();
					int s1 = index;
					decoded = true;
					if (decoded)
					{
						bool f1 = true;
						int c1 = 0;
						for (int i1 = 0; i1 < 1 && f1; i1++)
						{
							rule = decode_StringValue("]");
							if ((f1 = rule != null))
							{
								e1.Add(rule);
								c1++;
							}
						}
						decoded = c1 == 1;
					}
					if (decoded)
						e0.AddRange(e1);
					else
						index = s1;
				}
			}
			if (!decoded)
			{
				{
					var e1 = new List<Rule>();
					int s1 = index;
					decoded = true;
					if (decoded)
					{
						bool f1 = true;
						int c1 = 0;
						for (int i1 = 0; i1 < 1 && f1; i1++)
						{
							rule = decode_StringValue("@");
							if ((f1 = rule != null))
							{
								e1.Add(rule);
								c1++;
							}
						}
						decoded = c1 == 1;
					}
					if (decoded)
						e0.AddRange(e1);
					else
						index = s1;
				}
			}

			rule = null;
			if (decoded)
				rule = new gen_delims(text.Substring(s0, index - s0), e0);
			else
				index = s0;

			pop("gen-delims", decoded, index - s0);

			return (gen_delims)rule;
		}

		private sub_delims decode_sub_delims()
		{
			push("sub-delims");

			bool decoded = true;
			int s0 = index;
			var e0 = new List<Rule>();
			Rule rule;

			decoded = false;
			if (!decoded)
			{
				{
					var e1 = new List<Rule>();
					int s1 = index;
					decoded = true;
					if (decoded)
					{
						bool f1 = true;
						int c1 = 0;
						for (int i1 = 0; i1 < 1 && f1; i1++)
						{
							rule = decode_StringValue("!");
							if ((f1 = rule != null))
							{
								e1.Add(rule);
								c1++;
							}
						}
						decoded = c1 == 1;
					}
					if (decoded)
						e0.AddRange(e1);
					else
						index = s1;
				}
			}
			if (!decoded)
			{
				{
					var e1 = new List<Rule>();
					int s1 = index;
					decoded = true;
					if (decoded)
					{
						bool f1 = true;
						int c1 = 0;
						for (int i1 = 0; i1 < 1 && f1; i1++)
						{
							rule = decode_StringValue("$");
							if ((f1 = rule != null))
							{
								e1.Add(rule);
								c1++;
							}
						}
						decoded = c1 == 1;
					}
					if (decoded)
						e0.AddRange(e1);
					else
						index = s1;
				}
			}
			if (!decoded)
			{
				{
					var e1 = new List<Rule>();
					int s1 = index;
					decoded = true;
					if (decoded)
					{
						bool f1 = true;
						int c1 = 0;
						for (int i1 = 0; i1 < 1 && f1; i1++)
						{
							rule = decode_StringValue("&");
							if ((f1 = rule != null))
							{
								e1.Add(rule);
								c1++;
							}
						}
						decoded = c1 == 1;
					}
					if (decoded)
						e0.AddRange(e1);
					else
						index = s1;
				}
			}
			if (!decoded)
			{
				{
					var e1 = new List<Rule>();
					int s1 = index;
					decoded = true;
					if (decoded)
					{
						bool f1 = true;
						int c1 = 0;
						for (int i1 = 0; i1 < 1 && f1; i1++)
						{
							rule = decode_StringValue("'");
							if ((f1 = rule != null))
							{
								e1.Add(rule);
								c1++;
							}
						}
						decoded = c1 == 1;
					}
					if (decoded)
						e0.AddRange(e1);
					else
						index = s1;
				}
			}
			if (!decoded)
			{
				{
					var e1 = new List<Rule>();
					int s1 = index;
					decoded = true;
					if (decoded)
					{
						bool f1 = true;
						int c1 = 0;
						for (int i1 = 0; i1 < 1 && f1; i1++)
						{
							rule = decode_StringValue("*");
							if ((f1 = rule != null))
							{
								e1.Add(rule);
								c1++;
							}
						}
						decoded = c1 == 1;
					}
					if (decoded)
						e0.AddRange(e1);
					else
						index = s1;
				}
			}
			if (!decoded)
			{
				{
					var e1 = new List<Rule>();
					int s1 = index;
					decoded = true;
					if (decoded)
					{
						bool f1 = true;
						int c1 = 0;
						for (int i1 = 0; i1 < 1 && f1; i1++)
						{
							rule = decode_StringValue("+");
							if ((f1 = rule != null))
							{
								e1.Add(rule);
								c1++;
							}
						}
						decoded = c1 == 1;
					}
					if (decoded)
						e0.AddRange(e1);
					else
						index = s1;
				}
			}
			if (!decoded)
			{
				{
					var e1 = new List<Rule>();
					int s1 = index;
					decoded = true;
					if (decoded)
					{
						bool f1 = true;
						int c1 = 0;
						for (int i1 = 0; i1 < 1 && f1; i1++)
						{
							rule = decode_StringValue(",");
							if ((f1 = rule != null))
							{
								e1.Add(rule);
								c1++;
							}
						}
						decoded = c1 == 1;
					}
					if (decoded)
						e0.AddRange(e1);
					else
						index = s1;
				}
			}
			if (!decoded)
			{
				{
					var e1 = new List<Rule>();
					int s1 = index;
					decoded = true;
					if (decoded)
					{
						bool f1 = true;
						int c1 = 0;
						for (int i1 = 0; i1 < 1 && f1; i1++)
						{
							rule = decode_StringValue(";");
							if ((f1 = rule != null))
							{
								e1.Add(rule);
								c1++;
							}
						}
						decoded = c1 == 1;
					}
					if (decoded)
						e0.AddRange(e1);
					else
						index = s1;
				}
			}
			if (!decoded)
			{
				{
					var e1 = new List<Rule>();
					int s1 = index;
					decoded = true;
					if (decoded)
					{
						bool f1 = true;
						int c1 = 0;
						for (int i1 = 0; i1 < 1 && f1; i1++)
						{
							rule = decode_StringValue("=");
							if ((f1 = rule != null))
							{
								e1.Add(rule);
								c1++;
							}
						}
						decoded = c1 == 1;
					}
					if (decoded)
						e0.AddRange(e1);
					else
						index = s1;
				}
			}

			rule = null;
			if (decoded)
				rule = new sub_delims(text.Substring(s0, index - s0), e0);
			else
				index = s0;

			pop("sub-delims", decoded, index - s0);

			return (sub_delims)rule;
		}

		private unreserved decode_unreserved()
		{
			push("unreserved");

			bool decoded = true;
			int s0 = index;
			var e0 = new List<Rule>();
			Rule rule;

			decoded = false;
			if (!decoded)
			{
				{
					var e1 = new List<Rule>();
					int s1 = index;
					decoded = true;
					if (decoded)
					{
						bool f1 = true;
						int c1 = 0;
						for (int i1 = 0; i1 < 1 && f1; i1++)
						{
							rule = decode_ALPHA();
							if ((f1 = rule != null))
							{
								e1.Add(rule);
								c1++;
							}
						}
						decoded = c1 == 1;
					}
					if (decoded)
						e0.AddRange(e1);
					else
						index = s1;
				}
			}
			if (!decoded)
			{
				{
					var e1 = new List<Rule>();
					int s1 = index;
					decoded = true;
					if (decoded)
					{
						bool f1 = true;
						int c1 = 0;
						for (int i1 = 0; i1 < 1 && f1; i1++)
						{
							rule = decode_DIGIT();
							if ((f1 = rule != null))
							{
								e1.Add(rule);
								c1++;
							}
						}
						decoded = c1 == 1;
					}
					if (decoded)
						e0.AddRange(e1);
					else
						index = s1;
				}
			}
			if (!decoded)
			{
				{
					var e1 = new List<Rule>();
					int s1 = index;
					decoded = true;
					if (decoded)
					{
						bool f1 = true;
						int c1 = 0;
						for (int i1 = 0; i1 < 1 && f1; i1++)
						{
							rule = decode_StringValue("-");
							if ((f1 = rule != null))
							{
								e1.Add(rule);
								c1++;
							}
						}
						decoded = c1 == 1;
					}
					if (decoded)
						e0.AddRange(e1);
					else
						index = s1;
				}
			}
			if (!decoded)
			{
				{
					var e1 = new List<Rule>();
					int s1 = index;
					decoded = true;
					if (decoded)
					{
						bool f1 = true;
						int c1 = 0;
						for (int i1 = 0; i1 < 1 && f1; i1++)
						{
							rule = decode_StringValue(".");
							if ((f1 = rule != null))
							{
								e1.Add(rule);
								c1++;
							}
						}
						decoded = c1 == 1;
					}
					if (decoded)
						e0.AddRange(e1);
					else
						index = s1;
				}
			}
			if (!decoded)
			{
				{
					var e1 = new List<Rule>();
					int s1 = index;
					decoded = true;
					if (decoded)
					{
						bool f1 = true;
						int c1 = 0;
						for (int i1 = 0; i1 < 1 && f1; i1++)
						{
							rule = decode_StringValue("_");
							if ((f1 = rule != null))
							{
								e1.Add(rule);
								c1++;
							}
						}
						decoded = c1 == 1;
					}
					if (decoded)
						e0.AddRange(e1);
					else
						index = s1;
				}
			}
			if (!decoded)
			{
				{
					var e1 = new List<Rule>();
					int s1 = index;
					decoded = true;
					if (decoded)
					{
						bool f1 = true;
						int c1 = 0;
						for (int i1 = 0; i1 < 1 && f1; i1++)
						{
							rule = decode_StringValue("~");
							if ((f1 = rule != null))
							{
								e1.Add(rule);
								c1++;
							}
						}
						decoded = c1 == 1;
					}
					if (decoded)
						e0.AddRange(e1);
					else
						index = s1;
				}
			}

			rule = null;
			if (decoded)
				rule = new unreserved(text.Substring(s0, index - s0), e0);
			else
				index = s0;

			pop("unreserved", decoded, index - s0);

			return (unreserved)rule;
		}

		private ALPHA decode_ALPHA()
		{
			push("ALPHA");

			bool decoded = true;
			int s0 = index;
			var e0 = new List<Rule>();
			Rule rule;

			decoded = false;
			if (!decoded)
			{
				{
					var e1 = new List<Rule>();
					int s1 = index;
					decoded = true;
					if (decoded)
					{
						bool f1 = true;
						int c1 = 0;
						for (int i1 = 0; i1 < 1 && f1; i1++)
						{
							rule = decode_NumericValue("%x41-5A", "[\\x41-\\x5A]", 1);
							if ((f1 = rule != null))
							{
								e1.Add(rule);
								c1++;
							}
						}
						decoded = c1 == 1;
					}
					if (decoded)
						e0.AddRange(e1);
					else
						index = s1;
				}
			}
			if (!decoded)
			{
				{
					var e1 = new List<Rule>();
					int s1 = index;
					decoded = true;
					if (decoded)
					{
						bool f1 = true;
						int c1 = 0;
						for (int i1 = 0; i1 < 1 && f1; i1++)
						{
							rule = decode_NumericValue("%x61-7A", "[\\x61-\\x7A]", 1);
							if ((f1 = rule != null))
							{
								e1.Add(rule);
								c1++;
							}
						}
						decoded = c1 == 1;
					}
					if (decoded)
						e0.AddRange(e1);
					else
						index = s1;
				}
			}

			rule = null;
			if (decoded)
				rule = new ALPHA(text.Substring(s0, index - s0), e0);
			else
				index = s0;

			pop("ALPHA", decoded, index - s0);

			return (ALPHA)rule;
		}

		private BIT decode_BIT()
		{
			push("BIT");

			bool decoded = true;
			int s0 = index;
			var e0 = new List<Rule>();
			Rule rule;

			decoded = false;
			if (!decoded)
			{
				{
					var e1 = new List<Rule>();
					int s1 = index;
					decoded = true;
					if (decoded)
					{
						bool f1 = true;
						int c1 = 0;
						for (int i1 = 0; i1 < 1 && f1; i1++)
						{
							rule = decode_StringValue("0");
							if ((f1 = rule != null))
							{
								e1.Add(rule);
								c1++;
							}
						}
						decoded = c1 == 1;
					}
					if (decoded)
						e0.AddRange(e1);
					else
						index = s1;
				}
			}
			if (!decoded)
			{
				{
					var e1 = new List<Rule>();
					int s1 = index;
					decoded = true;
					if (decoded)
					{
						bool f1 = true;
						int c1 = 0;
						for (int i1 = 0; i1 < 1 && f1; i1++)
						{
							rule = decode_StringValue("1");
							if ((f1 = rule != null))
							{
								e1.Add(rule);
								c1++;
							}
						}
						decoded = c1 == 1;
					}
					if (decoded)
						e0.AddRange(e1);
					else
						index = s1;
				}
			}

			rule = null;
			if (decoded)
				rule = new BIT(text.Substring(s0, index - s0), e0);
			else
				index = s0;

			pop("BIT", decoded, index - s0);

			return (BIT)rule;
		}

		private CHAR decode_CHAR()
		{
			push("CHAR");

			bool decoded = true;
			int s0 = index;
			var e0 = new List<Rule>();
			Rule rule;

			decoded = false;
			if (!decoded)
			{
				{
					var e1 = new List<Rule>();
					int s1 = index;
					decoded = true;
					if (decoded)
					{
						bool f1 = true;
						int c1 = 0;
						for (int i1 = 0; i1 < 1 && f1; i1++)
						{
							rule = decode_NumericValue("%x01-7F", "[\\x01-\\x7F]", 1);
							if ((f1 = rule != null))
							{
								e1.Add(rule);
								c1++;
							}
						}
						decoded = c1 == 1;
					}
					if (decoded)
						e0.AddRange(e1);
					else
						index = s1;
				}
			}

			rule = null;
			if (decoded)
				rule = new CHAR(text.Substring(s0, index - s0), e0);
			else
				index = s0;

			pop("CHAR", decoded, index - s0);

			return (CHAR)rule;
		}

		private CR decode_CR()
		{
			push("CR");

			bool decoded = true;
			int s0 = index;
			var e0 = new List<Rule>();
			Rule rule;

			decoded = false;
			if (!decoded)
			{
				{
					var e1 = new List<Rule>();
					int s1 = index;
					decoded = true;
					if (decoded)
					{
						bool f1 = true;
						int c1 = 0;
						for (int i1 = 0; i1 < 1 && f1; i1++)
						{
							rule = decode_NumericValue("%x0D", "[\\x0D]", 1);
							if ((f1 = rule != null))
							{
								e1.Add(rule);
								c1++;
							}
						}
						decoded = c1 == 1;
					}
					if (decoded)
						e0.AddRange(e1);
					else
						index = s1;
				}
			}

			rule = null;
			if (decoded)
				rule = new CR(text.Substring(s0, index - s0), e0);
			else
				index = s0;

			pop("CR", decoded, index - s0);

			return (CR)rule;
		}

		private CRLF decode_CRLF()
		{
			push("CRLF");

			bool decoded = true;
			int s0 = index;
			var e0 = new List<Rule>();
			Rule rule;

			decoded = false;
			if (!decoded)
			{
				{
					var e1 = new List<Rule>();
					int s1 = index;
					decoded = true;
					if (decoded)
					{
						bool f1 = true;
						int c1 = 0;
						for (int i1 = 0; i1 < 1 && f1; i1++)
						{
							rule = decode_CR();
							if ((f1 = rule != null))
							{
								e1.Add(rule);
								c1++;
							}
						}
						decoded = c1 == 1;
					}
					if (decoded)
					{
						bool f1 = true;
						int c1 = 0;
						for (int i1 = 0; i1 < 1 && f1; i1++)
						{
							rule = decode_LF();
							if ((f1 = rule != null))
							{
								e1.Add(rule);
								c1++;
							}
						}
						decoded = c1 == 1;
					}
					if (decoded)
						e0.AddRange(e1);
					else
						index = s1;
				}
			}

			rule = null;
			if (decoded)
				rule = new CRLF(text.Substring(s0, index - s0), e0);
			else
				index = s0;

			pop("CRLF", decoded, index - s0);

			return (CRLF)rule;
		}

		private CTL decode_CTL()
		{
			push("CTL");

			bool decoded = true;
			int s0 = index;
			var e0 = new List<Rule>();
			Rule rule;

			decoded = false;
			if (!decoded)
			{
				{
					var e1 = new List<Rule>();
					int s1 = index;
					decoded = true;
					if (decoded)
					{
						bool f1 = true;
						int c1 = 0;
						for (int i1 = 0; i1 < 1 && f1; i1++)
						{
							rule = decode_NumericValue("%x00-1F", "[\\x00-\\x1F]", 1);
							if ((f1 = rule != null))
							{
								e1.Add(rule);
								c1++;
							}
						}
						decoded = c1 == 1;
					}
					if (decoded)
						e0.AddRange(e1);
					else
						index = s1;
				}
			}
			if (!decoded)
			{
				{
					var e1 = new List<Rule>();
					int s1 = index;
					decoded = true;
					if (decoded)
					{
						bool f1 = true;
						int c1 = 0;
						for (int i1 = 0; i1 < 1 && f1; i1++)
						{
							rule = decode_NumericValue("%x7F", "[\\x7F]", 1);
							if ((f1 = rule != null))
							{
								e1.Add(rule);
								c1++;
							}
						}
						decoded = c1 == 1;
					}
					if (decoded)
						e0.AddRange(e1);
					else
						index = s1;
				}
			}

			rule = null;
			if (decoded)
				rule = new CTL(text.Substring(s0, index - s0), e0);
			else
				index = s0;

			pop("CTL", decoded, index - s0);

			return (CTL)rule;
		}

		private DIGIT decode_DIGIT()
		{
			push("DIGIT");

			bool decoded = true;
			int s0 = index;
			var e0 = new List<Rule>();
			Rule rule;

			decoded = false;
			if (!decoded)
			{
				{
					var e1 = new List<Rule>();
					int s1 = index;
					decoded = true;
					if (decoded)
					{
						bool f1 = true;
						int c1 = 0;
						for (int i1 = 0; i1 < 1 && f1; i1++)
						{
							rule = decode_NumericValue("%x30-39", "[\\x30-\\x39]", 1);
							if ((f1 = rule != null))
							{
								e1.Add(rule);
								c1++;
							}
						}
						decoded = c1 == 1;
					}
					if (decoded)
						e0.AddRange(e1);
					else
						index = s1;
				}
			}

			rule = null;
			if (decoded)
				rule = new DIGIT(text.Substring(s0, index - s0), e0);
			else
				index = s0;

			pop("DIGIT", decoded, index - s0);

			return (DIGIT)rule;
		}

		private DQUOTE decode_DQUOTE()
		{
			push("DQUOTE");

			bool decoded = true;
			int s0 = index;
			var e0 = new List<Rule>();
			Rule rule;

			decoded = false;
			if (!decoded)
			{
				{
					var e1 = new List<Rule>();
					int s1 = index;
					decoded = true;
					if (decoded)
					{
						bool f1 = true;
						int c1 = 0;
						for (int i1 = 0; i1 < 1 && f1; i1++)
						{
							rule = decode_NumericValue("%x22", "[\\x22]", 1);
							if ((f1 = rule != null))
							{
								e1.Add(rule);
								c1++;
							}
						}
						decoded = c1 == 1;
					}
					if (decoded)
						e0.AddRange(e1);
					else
						index = s1;
				}
			}

			rule = null;
			if (decoded)
				rule = new DQUOTE(text.Substring(s0, index - s0), e0);
			else
				index = s0;

			pop("DQUOTE", decoded, index - s0);

			return (DQUOTE)rule;
		}

		private HEXDIG decode_HEXDIG()
		{
			push("HEXDIG");

			bool decoded = true;
			int s0 = index;
			var e0 = new List<Rule>();
			Rule rule;

			decoded = false;
			if (!decoded)
			{
				{
					var e1 = new List<Rule>();
					int s1 = index;
					decoded = true;
					if (decoded)
					{
						bool f1 = true;
						int c1 = 0;
						for (int i1 = 0; i1 < 1 && f1; i1++)
						{
							rule = decode_DIGIT();
							if ((f1 = rule != null))
							{
								e1.Add(rule);
								c1++;
							}
						}
						decoded = c1 == 1;
					}
					if (decoded)
						e0.AddRange(e1);
					else
						index = s1;
				}
			}
			if (!decoded)
			{
				{
					var e1 = new List<Rule>();
					int s1 = index;
					decoded = true;
					if (decoded)
					{
						bool f1 = true;
						int c1 = 0;
						for (int i1 = 0; i1 < 1 && f1; i1++)
						{
							rule = decode_StringValue("A");
							if ((f1 = rule != null))
							{
								e1.Add(rule);
								c1++;
							}
						}
						decoded = c1 == 1;
					}
					if (decoded)
						e0.AddRange(e1);
					else
						index = s1;
				}
			}
			if (!decoded)
			{
				{
					var e1 = new List<Rule>();
					int s1 = index;
					decoded = true;
					if (decoded)
					{
						bool f1 = true;
						int c1 = 0;
						for (int i1 = 0; i1 < 1 && f1; i1++)
						{
							rule = decode_StringValue("B");
							if ((f1 = rule != null))
							{
								e1.Add(rule);
								c1++;
							}
						}
						decoded = c1 == 1;
					}
					if (decoded)
						e0.AddRange(e1);
					else
						index = s1;
				}
			}
			if (!decoded)
			{
				{
					var e1 = new List<Rule>();
					int s1 = index;
					decoded = true;
					if (decoded)
					{
						bool f1 = true;
						int c1 = 0;
						for (int i1 = 0; i1 < 1 && f1; i1++)
						{
							rule = decode_StringValue("C");
							if ((f1 = rule != null))
							{
								e1.Add(rule);
								c1++;
							}
						}
						decoded = c1 == 1;
					}
					if (decoded)
						e0.AddRange(e1);
					else
						index = s1;
				}
			}
			if (!decoded)
			{
				{
					var e1 = new List<Rule>();
					int s1 = index;
					decoded = true;
					if (decoded)
					{
						bool f1 = true;
						int c1 = 0;
						for (int i1 = 0; i1 < 1 && f1; i1++)
						{
							rule = decode_StringValue("D");
							if ((f1 = rule != null))
							{
								e1.Add(rule);
								c1++;
							}
						}
						decoded = c1 == 1;
					}
					if (decoded)
						e0.AddRange(e1);
					else
						index = s1;
				}
			}
			if (!decoded)
			{
				{
					var e1 = new List<Rule>();
					int s1 = index;
					decoded = true;
					if (decoded)
					{
						bool f1 = true;
						int c1 = 0;
						for (int i1 = 0; i1 < 1 && f1; i1++)
						{
							rule = decode_StringValue("E");
							if ((f1 = rule != null))
							{
								e1.Add(rule);
								c1++;
							}
						}
						decoded = c1 == 1;
					}
					if (decoded)
						e0.AddRange(e1);
					else
						index = s1;
				}
			}
			if (!decoded)
			{
				{
					var e1 = new List<Rule>();
					int s1 = index;
					decoded = true;
					if (decoded)
					{
						bool f1 = true;
						int c1 = 0;
						for (int i1 = 0; i1 < 1 && f1; i1++)
						{
							rule = decode_StringValue("F");
							if ((f1 = rule != null))
							{
								e1.Add(rule);
								c1++;
							}
						}
						decoded = c1 == 1;
					}
					if (decoded)
						e0.AddRange(e1);
					else
						index = s1;
				}
			}

			rule = null;
			if (decoded)
				rule = new HEXDIG(text.Substring(s0, index - s0), e0);
			else
				index = s0;

			pop("HEXDIG", decoded, index - s0);

			return (HEXDIG)rule;
		}

		private HTAB decode_HTAB()
		{
			push("HTAB");

			bool decoded = true;
			int s0 = index;
			var e0 = new List<Rule>();
			Rule rule;

			decoded = false;
			if (!decoded)
			{
				{
					var e1 = new List<Rule>();
					int s1 = index;
					decoded = true;
					if (decoded)
					{
						bool f1 = true;
						int c1 = 0;
						for (int i1 = 0; i1 < 1 && f1; i1++)
						{
							rule = decode_NumericValue("%x09", "[\\x09]", 1);
							if ((f1 = rule != null))
							{
								e1.Add(rule);
								c1++;
							}
						}
						decoded = c1 == 1;
					}
					if (decoded)
						e0.AddRange(e1);
					else
						index = s1;
				}
			}

			rule = null;
			if (decoded)
				rule = new HTAB(text.Substring(s0, index - s0), e0);
			else
				index = s0;

			pop("HTAB", decoded, index - s0);

			return (HTAB)rule;
		}

		private LF decode_LF()
		{
			push("LF");

			bool decoded = true;
			int s0 = index;
			var e0 = new List<Rule>();
			Rule rule;

			decoded = false;
			if (!decoded)
			{
				{
					var e1 = new List<Rule>();
					int s1 = index;
					decoded = true;
					if (decoded)
					{
						bool f1 = true;
						int c1 = 0;
						for (int i1 = 0; i1 < 1 && f1; i1++)
						{
							rule = decode_NumericValue("%x0A", "[\\x0A]", 1);
							if ((f1 = rule != null))
							{
								e1.Add(rule);
								c1++;
							}
						}
						decoded = c1 == 1;
					}
					if (decoded)
						e0.AddRange(e1);
					else
						index = s1;
				}
			}

			rule = null;
			if (decoded)
				rule = new LF(text.Substring(s0, index - s0), e0);
			else
				index = s0;

			pop("LF", decoded, index - s0);

			return (LF)rule;
		}

		private LWSP decode_LWSP()
		{
			push("LWSP");

			bool decoded = true;
			int s0 = index;
			var e0 = new List<Rule>();
			Rule rule;

			decoded = false;
			if (!decoded)
			{
				{
					var e1 = new List<Rule>();
					int s1 = index;
					decoded = true;
					if (decoded)
					{
						bool f1 = true;
						int c1 = 0;
						while (f1)
						{
							decoded = false;
							if (!decoded)
							{
								{
									var e2 = new List<Rule>();
									int s2 = index;
									decoded = true;
									if (decoded)
									{
										bool f2 = true;
										int c2 = 0;
										for (int i2 = 0; i2 < 1 && f2; i2++)
										{
											rule = decode_WSP();
											if ((f2 = rule != null))
											{
												e2.Add(rule);
												c2++;
											}
										}
										decoded = c2 == 1;
									}
									if (decoded)
										e1.AddRange(e2);
									else
										index = s2;
								}
							}
							if (!decoded)
							{
								{
									var e2 = new List<Rule>();
									int s2 = index;
									decoded = true;
									if (decoded)
									{
										bool f2 = true;
										int c2 = 0;
										for (int i2 = 0; i2 < 1 && f2; i2++)
										{
											rule = decode_CRLF();
											if ((f2 = rule != null))
											{
												e2.Add(rule);
												c2++;
											}
										}
										decoded = c2 == 1;
									}
									if (decoded)
									{
										bool f2 = true;
										int c2 = 0;
										for (int i2 = 0; i2 < 1 && f2; i2++)
										{
											rule = decode_WSP();
											if ((f2 = rule != null))
											{
												e2.Add(rule);
												c2++;
											}
										}
										decoded = c2 == 1;
									}
									if (decoded)
										e1.AddRange(e2);
									else
										index = s2;
								}
							}
							if (decoded)
								c1++;
							f1 = decoded;
						}
						decoded = true;
					}
					if (decoded)
						e0.AddRange(e1);
					else
						index = s1;
				}
			}

			rule = null;
			if (decoded)
				rule = new LWSP(text.Substring(s0, index - s0), e0);
			else
				index = s0;

			pop("LWSP", decoded, index - s0);

			return (LWSP)rule;
		}

		private OCTET decode_OCTET()
		{
			push("OCTET");

			bool decoded = true;
			int s0 = index;
			var e0 = new List<Rule>();
			Rule rule;

			decoded = false;
			if (!decoded)
			{
				{
					var e1 = new List<Rule>();
					int s1 = index;
					decoded = true;
					if (decoded)
					{
						bool f1 = true;
						int c1 = 0;
						for (int i1 = 0; i1 < 1 && f1; i1++)
						{
							rule = decode_NumericValue("%x00-FF", "[\\x00-\\xFF]", 1);
							if ((f1 = rule != null))
							{
								e1.Add(rule);
								c1++;
							}
						}
						decoded = c1 == 1;
					}
					if (decoded)
						e0.AddRange(e1);
					else
						index = s1;
				}
			}

			rule = null;
			if (decoded)
				rule = new OCTET(text.Substring(s0, index - s0), e0);
			else
				index = s0;

			pop("OCTET", decoded, index - s0);

			return (OCTET)rule;
		}

		private SP decode_SP()
		{
			push("SP");

			bool decoded = true;
			int s0 = index;
			var e0 = new List<Rule>();
			Rule rule;

			decoded = false;
			if (!decoded)
			{
				{
					var e1 = new List<Rule>();
					int s1 = index;
					decoded = true;
					if (decoded)
					{
						bool f1 = true;
						int c1 = 0;
						for (int i1 = 0; i1 < 1 && f1; i1++)
						{
							rule = decode_NumericValue("%x20", "[\\x20]", 1);
							if ((f1 = rule != null))
							{
								e1.Add(rule);
								c1++;
							}
						}
						decoded = c1 == 1;
					}
					if (decoded)
						e0.AddRange(e1);
					else
						index = s1;
				}
			}

			rule = null;
			if (decoded)
				rule = new SP(text.Substring(s0, index - s0), e0);
			else
				index = s0;

			pop("SP", decoded, index - s0);

			return (SP)rule;
		}

		private VCHAR decode_VCHAR()
		{
			push("VCHAR");

			bool decoded = true;
			int s0 = index;
			var e0 = new List<Rule>();
			Rule rule;

			decoded = false;
			if (!decoded)
			{
				{
					var e1 = new List<Rule>();
					int s1 = index;
					decoded = true;
					if (decoded)
					{
						bool f1 = true;
						int c1 = 0;
						for (int i1 = 0; i1 < 1 && f1; i1++)
						{
							rule = decode_NumericValue("%x21-7E", "[\\x21-\\x7E]", 1);
							if ((f1 = rule != null))
							{
								e1.Add(rule);
								c1++;
							}
						}
						decoded = c1 == 1;
					}
					if (decoded)
						e0.AddRange(e1);
					else
						index = s1;
				}
			}

			rule = null;
			if (decoded)
				rule = new VCHAR(text.Substring(s0, index - s0), e0);
			else
				index = s0;

			pop("VCHAR", decoded, index - s0);

			return (VCHAR)rule;
		}

		private WSP decode_WSP()
		{
			push("WSP");

			bool decoded = true;
			int s0 = index;
			var e0 = new List<Rule>();
			Rule rule;

			decoded = false;
			if (!decoded)
			{
				{
					var e1 = new List<Rule>();
					int s1 = index;
					decoded = true;
					if (decoded)
					{
						bool f1 = true;
						int c1 = 0;
						for (int i1 = 0; i1 < 1 && f1; i1++)
						{
							rule = decode_SP();
							if ((f1 = rule != null))
							{
								e1.Add(rule);
								c1++;
							}
						}
						decoded = c1 == 1;
					}
					if (decoded)
						e0.AddRange(e1);
					else
						index = s1;
				}
			}
			if (!decoded)
			{
				{
					var e1 = new List<Rule>();
					int s1 = index;
					decoded = true;
					if (decoded)
					{
						bool f1 = true;
						int c1 = 0;
						for (int i1 = 0; i1 < 1 && f1; i1++)
						{
							rule = decode_HTAB();
							if ((f1 = rule != null))
							{
								e1.Add(rule);
								c1++;
							}
						}
						decoded = c1 == 1;
					}
					if (decoded)
						e0.AddRange(e1);
					else
						index = s1;
				}
			}

			rule = null;
			if (decoded)
				rule = new WSP(text.Substring(s0, index - s0), e0);
			else
				index = s0;

			pop("WSP", decoded, index - s0);

			return (WSP)rule;
		}

		public class StringValue : Rule
		{
			public StringValue(string spelling, IList<Rule> rules)
				: base(spelling, rules)
			{ }

			public object visit(Visitor visitor)
			{
				return visitor.visit_StringValue(this);
			}
		}

		private StringValue decode_StringValue(String regex)
		{
			push("*StringValue", regex);

			bool decoded = true;
			int start = index;

			StringValue stringValue = null;
			try
			{
				string value = text.Substring(index, regex.Length);
				if ((decoded = value.Equals(regex, StringComparison.InvariantCultureIgnoreCase)))
				{
					index += regex.Length;
					stringValue = new StringValue(value, null);
				}
			}
			catch (IndexOutOfRangeException e)
			{
				decoded = false;
			}

			pop("*StringValue", decoded, index - start);

			return stringValue;
		}

		public class NumericValue : Rule
		{
			public NumericValue(string spelling, IList<Rule> rules)
				: base(spelling, rules)
			{ }

			public object visit(Visitor visitor)
			{
				return visitor.visit_NumericValue(this);
			}
		}

		private NumericValue decode_NumericValue(string spelling, String regex, int length)
		{
			push("*NumericValue", spelling, regex);

			bool decoded = true;
			int start = index;

			NumericValue numericValue = null;
			try
			{
				String value = text.Substring(index, length);
				if ((decoded = Pattern.matches(regex, value)))
				{
					index += length;
					numericValue = new NumericValue(value, null);
				}
			}
			catch (IndexOutOfRangeException e)
			{
				decoded = false;
			}

			pop("*NumericValue", decoded, index - start);

			return numericValue;
		}

		/* ---------------------------------------------------------------------------
		 * public rule classes
		 * ---------------------------------------------------------------------------
		 */

		internal sealed class xri : Rule
		{
			internal xri(string spelling, IList<Rule> rules)
				: base(spelling, rules)
			{ }

			public xri(xri rule)
				: base(rule.spelling, rule.rules)
			{ }

			public object visit(Visitor visitor)
			{
				return visitor.visit_xri(this);
			}
		}

		internal sealed class xri_scheme : Rule
		{
			internal xri_scheme(string spelling, IList<Rule> rules)
				: base(spelling, rules)
			{ }

			public xri_scheme(xri_scheme rule)
				: base(rule.spelling, rule.rules)
			{ }

			public object visit(Visitor visitor)
			{
				return visitor.visit_xri_scheme(this);
			}
		}

		internal sealed class xri_noscheme : Rule
		{
			internal xri_noscheme(string spelling, IList<Rule> rules)
				: base(spelling, rules)
			{ }

			public xri_noscheme(xri_noscheme rule)
				: base(rule.spelling, rule.rules)
			{ }

			public object visit(Visitor visitor)
			{
				return visitor.visit_xri_noscheme(this);
			}
		}

		internal sealed class xri_reference : Rule
		{
			internal xri_reference(string spelling, IList<Rule> rules)
				: base(spelling, rules)
			{ }

			public xri_reference(xri_reference rule)
				: base(rule.spelling, rule.rules)
			{ }

			public object visit(Visitor visitor)
			{
				return visitor.visit_xri_reference(this);
			}
		}

		internal sealed class relative_xri_ref : Rule
		{
			internal relative_xri_ref(string spelling, IList<Rule> rules)
				: base(spelling, rules)
			{ }

			public relative_xri_ref(relative_xri_ref rule)
				: base(rule.spelling, rule.rules)
			{ }

			public object visit(Visitor visitor)
			{
				return visitor.visit_relative_xri_ref(this);
			}
		}

		internal sealed class relative_xri_part : Rule
		{
			internal relative_xri_part(string spelling, IList<Rule> rules)
				: base(spelling, rules)
			{ }

			public relative_xri_part(relative_xri_part rule)
				: base(rule.spelling, rule.rules)
			{ }

			public object visit(Visitor visitor)
			{
				return visitor.visit_relative_xri_part(this);
			}
		}

		internal sealed class xri_hier_part : Rule
		{
			internal xri_hier_part(string spelling, IList<Rule> rules)
				: base(spelling, rules)
			{ }

			public xri_hier_part(xri_hier_part rule)
				: base(rule.spelling, rule.rules)
			{ }

			public object visit(Visitor visitor)
			{
				return visitor.visit_xri_hier_part(this);
			}
		}

		internal sealed class xri_authority : Rule
		{
			internal xri_authority(string spelling, IList<Rule> rules)
				: base(spelling, rules)
			{ }

			public xri_authority(xri_authority rule)
				: base(rule.spelling, rule.rules)
			{ }

			public object visit(Visitor visitor)
			{
				return visitor.visit_xri_authority(this);
			}
		}

		internal sealed class subseg : Rule
		{
			internal subseg(string spelling, IList<Rule> rules)
				: base(spelling, rules)
			{ }

			public subseg(subseg rule)
				: base(rule.spelling, rule.rules)
			{ }

			public object visit(Visitor visitor)
			{
				return visitor.visit_subseg(this);
			}
		}

		internal sealed class global_subseg : Rule
		{
			internal global_subseg(string spelling, IList<Rule> rules)
				: base(spelling, rules)
			{ }

			public global_subseg(global_subseg rule)
				: base(rule.spelling, rule.rules)
			{ }

			public object visit(Visitor visitor)
			{
				return visitor.visit_global_subseg(this);
			}
		}

		internal sealed class local_subseg : Rule
		{
			internal local_subseg(string spelling, IList<Rule> rules)
				: base(spelling, rules)
			{ }

			public local_subseg(local_subseg rule)
				: base(rule.spelling, rule.rules)
			{ }

			public object visit(Visitor visitor)
			{
				return visitor.visit_local_subseg(this);
			}
		}

		internal sealed class gcs_char : Rule
		{
			internal gcs_char(string spelling, IList<Rule> rules)
				: base(spelling, rules)
			{ }

			public gcs_char(gcs_char rule)
				: base(rule.spelling, rule.rules)
			{ }

			public object visit(Visitor visitor)
			{
				return visitor.visit_gcs_char(this);
			}
		}

		internal sealed class lcs_char : Rule
		{
			internal lcs_char(string spelling, IList<Rule> rules)
				: base(spelling, rules)
			{ }

			public lcs_char(lcs_char rule)
				: base(rule.spelling, rule.rules)
			{ }

			public object visit(Visitor visitor)
			{
				return visitor.visit_lcs_char(this);
			}
		}

		internal sealed class rel_subseg : Rule
		{
			internal rel_subseg(string spelling, IList<Rule> rules)
				: base(spelling, rules)
			{ }

			public rel_subseg(rel_subseg rule)
				: base(rule.spelling, rule.rules)
			{ }

			public object visit(Visitor visitor)
			{
				return visitor.visit_rel_subseg(this);
			}
		}

		internal sealed class rel_subseg_nc : Rule
		{
			internal rel_subseg_nc(string spelling, IList<Rule> rules)
				: base(spelling, rules)
			{ }

			public rel_subseg_nc(rel_subseg_nc rule)
				: base(rule.spelling, rule.rules)
			{ }

			public object visit(Visitor visitor)
			{
				return visitor.visit_rel_subseg_nc(this);
			}
		}

		internal sealed class literal : Rule
		{
			internal literal(string spelling, IList<Rule> rules)
				: base(spelling, rules)
			{ }

			public literal(literal rule)
				: base(rule.spelling, rule.rules)
			{ }

			public object visit(Visitor visitor)
			{
				return visitor.visit_literal(this);
			}
		}

		internal sealed class literal_nc : Rule
		{
			internal literal_nc(string spelling, IList<Rule> rules)
				: base(spelling, rules)
			{ }

			public literal_nc(literal_nc rule)
				: base(rule.spelling, rule.rules)
			{ }

			public object visit(Visitor visitor)
			{
				return visitor.visit_literal_nc(this);
			}
		}

		internal sealed class xref : Rule
		{
			internal xref(string spelling, IList<Rule> rules)
				: base(spelling, rules)
			{ }

			public xref(xref rule)
				: base(rule.spelling, rule.rules)
			{ }

			public object visit(Visitor visitor)
			{
				return visitor.visit_xref(this);
			}
		}

		internal sealed class xref_empty : Rule
		{
			internal xref_empty(string spelling, IList<Rule> rules)
				: base(spelling, rules)
			{ }

			public xref_empty(xref_empty rule)
				: base(rule.spelling, rule.rules)
			{ }

			public object visit(Visitor visitor)
			{
				return visitor.visit_xref_empty(this);
			}
		}

		internal sealed class xref_xri_reference : Rule
		{
			internal xref_xri_reference(string spelling, IList<Rule> rules)
				: base(spelling, rules)
			{ }

			public xref_xri_reference(xref_xri_reference rule)
				: base(rule.spelling, rule.rules)
			{ }

			public object visit(Visitor visitor)
			{
				return visitor.visit_xref_xri_reference(this);
			}
		}

		internal sealed class xref_IRI : Rule
		{
			internal xref_IRI(string spelling, IList<Rule> rules)
				: base(spelling, rules)
			{ }

			public xref_IRI(xref_IRI rule)
				: base(rule.spelling, rule.rules)
			{ }

			public object visit(Visitor visitor)
			{
				return visitor.visit_xref_IRI(this);
			}
		}

		internal sealed class xref_value : Rule
		{
			internal xref_value(string spelling, IList<Rule> rules)
				: base(spelling, rules)
			{ }

			public xref_value(xref_value rule)
				: base(rule.spelling, rule.rules)
			{ }

			public object visit(Visitor visitor)
			{
				return visitor.visit_xref_value(this);
			}
		}

		internal sealed class xri_path : Rule
		{
			internal xri_path(string spelling, IList<Rule> rules)
				: base(spelling, rules)
			{ }

			public xri_path(xri_path rule)
				: base(rule.spelling, rule.rules)
			{ }

			public object visit(Visitor visitor)
			{
				return visitor.visit_xri_path(this);
			}
		}

		internal sealed class xri_path_abempty : Rule
		{
			internal xri_path_abempty(string spelling, IList<Rule> rules)
				: base(spelling, rules)
			{ }

			public xri_path_abempty(xri_path_abempty rule)
				: base(rule.spelling, rule.rules)
			{ }

			public object visit(Visitor visitor)
			{
				return visitor.visit_xri_path_abempty(this);
			}
		}

		internal sealed class xri_path_abs : Rule
		{
			internal xri_path_abs(string spelling, IList<Rule> rules)
				: base(spelling, rules)
			{ }

			public xri_path_abs(xri_path_abs rule)
				: base(rule.spelling, rule.rules)
			{ }

			public object visit(Visitor visitor)
			{
				return visitor.visit_xri_path_abs(this);
			}
		}

		internal sealed class xri_path_noscheme : Rule
		{
			internal xri_path_noscheme(string spelling, IList<Rule> rules)
				: base(spelling, rules)
			{ }

			public xri_path_noscheme(xri_path_noscheme rule)
				: base(rule.spelling, rule.rules)
			{ }

			public object visit(Visitor visitor)
			{
				return visitor.visit_xri_path_noscheme(this);
			}
		}

		internal sealed class xri_segment : Rule
		{
			internal xri_segment(string spelling, IList<Rule> rules)
				: base(spelling, rules)
			{ }

			public xri_segment(xri_segment rule)
				: base(rule.spelling, rule.rules)
			{ }

			public object visit(Visitor visitor)
			{
				return visitor.visit_xri_segment(this);
			}
		}

		internal sealed class xri_segment_nz : Rule
		{
			internal xri_segment_nz(string spelling, IList<Rule> rules)
				: base(spelling, rules)
			{ }

			public xri_segment_nz(xri_segment_nz rule)
				: base(rule.spelling, rule.rules)
			{ }

			public object visit(Visitor visitor)
			{
				return visitor.visit_xri_segment_nz(this);
			}
		}

		internal sealed class xri_segment_nc : Rule
		{
			internal xri_segment_nc(string spelling, IList<Rule> rules)
				: base(spelling, rules)
			{ }

			public xri_segment_nc(xri_segment_nc rule)
				: base(rule.spelling, rule.rules)
			{ }

			public object visit(Visitor visitor)
			{
				return visitor.visit_xri_segment_nc(this);
			}
		}

		internal sealed class xri_pchar : Rule
		{
			internal xri_pchar(string spelling, IList<Rule> rules)
				: base(spelling, rules)
			{ }

			public xri_pchar(xri_pchar rule)
				: base(rule.spelling, rule.rules)
			{ }

			public object visit(Visitor visitor)
			{
				return visitor.visit_xri_pchar(this);
			}
		}

		internal sealed class xri_pchar_nc : Rule
		{
			internal xri_pchar_nc(string spelling, IList<Rule> rules)
				: base(spelling, rules)
			{ }

			public xri_pchar_nc(xri_pchar_nc rule)
				: base(rule.spelling, rule.rules)
			{ }

			public object visit(Visitor visitor)
			{
				return visitor.visit_xri_pchar_nc(this);
			}
		}

		internal sealed class xri_reserved : Rule
		{
			internal xri_reserved(string spelling, IList<Rule> rules)
				: base(spelling, rules)
			{ }

			public xri_reserved(xri_reserved rule)
				: base(rule.spelling, rule.rules)
			{ }

			public object visit(Visitor visitor)
			{
				return visitor.visit_xri_reserved(this);
			}
		}

		internal sealed class xri_gen_delims : Rule
		{
			internal xri_gen_delims(string spelling, IList<Rule> rules)
				: base(spelling, rules)
			{ }

			public xri_gen_delims(xri_gen_delims rule)
				: base(rule.spelling, rule.rules)
			{ }

			public object visit(Visitor visitor)
			{
				return visitor.visit_xri_gen_delims(this);
			}
		}

		internal sealed class xri_sub_delims : Rule
		{
			internal xri_sub_delims(string spelling, IList<Rule> rules)
				: base(spelling, rules)
			{ }

			public xri_sub_delims(xri_sub_delims rule)
				: base(rule.spelling, rule.rules)
			{ }

			public object visit(Visitor visitor)
			{
				return visitor.visit_xri_sub_delims(this);
			}
		}

		internal sealed class IRI : Rule
		{
			internal IRI(string spelling, IList<Rule> rules)
				: base(spelling, rules)
			{ }

			public IRI(IRI rule)
				: base(rule.spelling, rule.rules)
			{ }

			public object visit(Visitor visitor)
			{
				return visitor.visit_IRI(this);
			}
		}

		internal sealed class scheme : Rule
		{
			internal scheme(string spelling, IList<Rule> rules)
				: base(spelling, rules)
			{ }

			public scheme(scheme rule)
				: base(rule.spelling, rule.rules)
			{ }

			public object visit(Visitor visitor)
			{
				return visitor.visit_scheme(this);
			}
		}

		internal sealed class ihier_part : Rule
		{
			internal ihier_part(string spelling, IList<Rule> rules)
				: base(spelling, rules)
			{ }

			public ihier_part(ihier_part rule)
				: base(rule.spelling, rule.rules)
			{ }

			public object visit(Visitor visitor)
			{
				return visitor.visit_ihier_part(this);
			}
		}

		internal sealed class iauthority : Rule
		{
			internal iauthority(string spelling, IList<Rule> rules)
				: base(spelling, rules)
			{ }

			public iauthority(iauthority rule)
				: base(rule.spelling, rule.rules)
			{ }

			public object visit(Visitor visitor)
			{
				return visitor.visit_iauthority(this);
			}
		}

		internal sealed class iuserinfo : Rule
		{
			internal iuserinfo(string spelling, IList<Rule> rules)
				: base(spelling, rules)
			{ }

			public iuserinfo(iuserinfo rule)
				: base(rule.spelling, rule.rules)
			{ }

			public object visit(Visitor visitor)
			{
				return visitor.visit_iuserinfo(this);
			}
		}

		internal sealed class ihost : Rule
		{
			internal ihost(string spelling, IList<Rule> rules)
				: base(spelling, rules)
			{ }

			public ihost(ihost rule)
				: base(rule.spelling, rule.rules)
			{ }

			public object visit(Visitor visitor)
			{
				return visitor.visit_ihost(this);
			}
		}

		internal sealed class IP_literal : Rule
		{
			internal IP_literal(string spelling, IList<Rule> rules)
				: base(spelling, rules)
			{ }

			public IP_literal(IP_literal rule)
				: base(rule.spelling, rule.rules)
			{ }

			public object visit(Visitor visitor)
			{
				return visitor.visit_IP_literal(this);
			}
		}

		internal sealed class IPvFuture : Rule
		{
			internal IPvFuture(string spelling, IList<Rule> rules)
				: base(spelling, rules)
			{ }

			public IPvFuture(IPvFuture rule)
				: base(rule.spelling, rule.rules)
			{ }

			public object visit(Visitor visitor)
			{
				return visitor.visit_IPvFuture(this);
			}
		}

		internal sealed class IPv6address : Rule
		{
			internal IPv6address(string spelling, IList<Rule> rules)
				: base(spelling, rules)
			{ }

			public IPv6address(IPv6address rule)
				: base(rule.spelling, rule.rules)
			{ }

			public object visit(Visitor visitor)
			{
				return visitor.visit_IPv6address(this);
			}
		}

		internal sealed class ls32 : Rule
		{
			internal ls32(string spelling, IList<Rule> rules)
				: base(spelling, rules)
			{ }

			public ls32(ls32 rule)
				: base(rule.spelling, rule.rules)
			{ }

			public object visit(Visitor visitor)
			{
				return visitor.visit_ls32(this);
			}
		}

		internal sealed class h16 : Rule
		{
			internal h16(string spelling, IList<Rule> rules)
				: base(spelling, rules)
			{ }

			public h16(h16 rule)
				: base(rule.spelling, rule.rules)
			{ }

			public object visit(Visitor visitor)
			{
				return visitor.visit_h16(this);
			}
		}

		internal sealed class IPv4address : Rule
		{
			internal IPv4address(string spelling, IList<Rule> rules)
				: base(spelling, rules)
			{ }

			public IPv4address(IPv4address rule)
				: base(rule.spelling, rule.rules)
			{ }

			public object visit(Visitor visitor)
			{
				return visitor.visit_IPv4address(this);
			}
		}

		internal sealed class dec_octet : Rule
		{
			internal dec_octet(string spelling, IList<Rule> rules)
				: base(spelling, rules)
			{ }

			public dec_octet(dec_octet rule)
				: base(rule.spelling, rule.rules)
			{ }

			public object visit(Visitor visitor)
			{
				return visitor.visit_dec_octet(this);
			}
		}

		internal sealed class ireg_name : Rule
		{
			internal ireg_name(string spelling, IList<Rule> rules)
				: base(spelling, rules)
			{ }

			public ireg_name(ireg_name rule)
				: base(rule.spelling, rule.rules)
			{ }

			public object visit(Visitor visitor)
			{
				return visitor.visit_ireg_name(this);
			}
		}

		internal sealed class port : Rule
		{
			internal port(string spelling, IList<Rule> rules)
				: base(spelling, rules)
			{ }

			public port(port rule)
				: base(rule.spelling, rule.rules)
			{ }

			public object visit(Visitor visitor)
			{
				return visitor.visit_port(this);
			}
		}

		internal sealed class ipath_abempty : Rule
		{
			internal ipath_abempty(string spelling, IList<Rule> rules)
				: base(spelling, rules)
			{ }

			public ipath_abempty(ipath_abempty rule)
				: base(rule.spelling, rule.rules)
			{ }

			public object visit(Visitor visitor)
			{
				return visitor.visit_ipath_abempty(this);
			}
		}

		internal sealed class ipath_abs : Rule
		{
			internal ipath_abs(string spelling, IList<Rule> rules)
				: base(spelling, rules)
			{ }

			public ipath_abs(ipath_abs rule)
				: base(rule.spelling, rule.rules)
			{ }

			public object visit(Visitor visitor)
			{
				return visitor.visit_ipath_abs(this);
			}
		}

		internal sealed class ipath_rootless : Rule
		{
			internal ipath_rootless(string spelling, IList<Rule> rules)
				: base(spelling, rules)
			{ }

			public ipath_rootless(ipath_rootless rule)
				: base(rule.spelling, rule.rules)
			{ }

			public object visit(Visitor visitor)
			{
				return visitor.visit_ipath_rootless(this);
			}
		}

		internal sealed class ipath_empty : Rule
		{
			internal ipath_empty(string spelling, IList<Rule> rules)
				: base(spelling, rules)
			{ }

			public ipath_empty(ipath_empty rule)
				: base(rule.spelling, rule.rules)
			{ }

			public object visit(Visitor visitor)
			{
				return visitor.visit_ipath_empty(this);
			}
		}

		internal sealed class isegment : Rule
		{
			internal isegment(string spelling, IList<Rule> rules)
				: base(spelling, rules)
			{ }

			public isegment(isegment rule)
				: base(rule.spelling, rule.rules)
			{ }

			public object visit(Visitor visitor)
			{
				return visitor.visit_isegment(this);
			}
		}

		internal sealed class isegment_nz : Rule
		{
			internal isegment_nz(string spelling, IList<Rule> rules)
				: base(spelling, rules)
			{ }

			public isegment_nz(isegment_nz rule)
				: base(rule.spelling, rule.rules)
			{ }

			public object visit(Visitor visitor)
			{
				return visitor.visit_isegment_nz(this);
			}
		}

		internal sealed class iquery : Rule
		{
			internal iquery(string spelling, IList<Rule> rules)
				: base(spelling, rules)
			{ }

			public iquery(iquery rule)
				: base(rule.spelling, rule.rules)
			{ }

			public object visit(Visitor visitor)
			{
				return visitor.visit_iquery(this);
			}
		}

		internal sealed class iprivate : Rule
		{
			internal iprivate(string spelling, IList<Rule> rules)
				: base(spelling, rules)
			{ }

			public iprivate(iprivate rule)
				: base(rule.spelling, rule.rules)
			{ }

			public object visit(Visitor visitor)
			{
				return visitor.visit_iprivate(this);
			}
		}

		internal sealed class ifragment : Rule
		{
			internal ifragment(string spelling, IList<Rule> rules)
				: base(spelling, rules)
			{ }

			public ifragment(ifragment rule)
				: base(rule.spelling, rule.rules)
			{ }

			public object visit(Visitor visitor)
			{
				return visitor.visit_ifragment(this);
			}
		}

		internal sealed class ipchar : Rule
		{
			internal ipchar(string spelling, IList<Rule> rules)
				: base(spelling, rules)
			{ }

			public ipchar(ipchar rule)
				: base(rule.spelling, rule.rules)
			{ }

			public object visit(Visitor visitor)
			{
				return visitor.visit_ipchar(this);
			}
		}

		internal sealed class iunreserved : Rule
		{
			internal iunreserved(string spelling, IList<Rule> rules)
				: base(spelling, rules)
			{ }

			public iunreserved(iunreserved rule)
				: base(rule.spelling, rule.rules)
			{ }

			public object visit(Visitor visitor)
			{
				return visitor.visit_iunreserved(this);
			}
		}

		internal sealed class pct_encoded : Rule
		{
			internal pct_encoded(string spelling, IList<Rule> rules)
				: base(spelling, rules)
			{ }

			public pct_encoded(pct_encoded rule)
				: base(rule.spelling, rule.rules)
			{ }

			public object visit(Visitor visitor)
			{
				return visitor.visit_pct_encoded(this);
			}
		}

		internal sealed class ucschar : Rule
		{
			internal ucschar(string spelling, IList<Rule> rules)
				: base(spelling, rules)
			{ }

			public ucschar(ucschar rule)
				: base(rule.spelling, rule.rules)
			{ }

			public object visit(Visitor visitor)
			{
				return visitor.visit_ucschar(this);
			}
		}

		internal sealed class reserved : Rule
		{
			internal reserved(string spelling, IList<Rule> rules)
				: base(spelling, rules)
			{ }

			public reserved(reserved rule)
				: base(rule.spelling, rule.rules)
			{ }

			public object visit(Visitor visitor)
			{
				return visitor.visit_reserved(this);
			}
		}

		internal sealed class gen_delims : Rule
		{
			internal gen_delims(string spelling, IList<Rule> rules)
				: base(spelling, rules)
			{ }

			public gen_delims(gen_delims rule)
				: base(rule.spelling, rule.rules)
			{ }

			public object visit(Visitor visitor)
			{
				return visitor.visit_gen_delims(this);
			}
		}

		internal sealed class sub_delims : Rule
		{
			internal sub_delims(string spelling, IList<Rule> rules)
				: base(spelling, rules)
			{ }

			public sub_delims(sub_delims rule)
				: base(rule.spelling, rule.rules)
			{ }

			public object visit(Visitor visitor)
			{
				return visitor.visit_sub_delims(this);
			}
		}

		internal sealed class unreserved : Rule
		{
			internal unreserved(string spelling, IList<Rule> rules)
				: base(spelling, rules)
			{ }

			public unreserved(unreserved rule)
				: base(rule.spelling, rule.rules)
			{ }

			public object visit(Visitor visitor)
			{
				return visitor.visit_unreserved(this);
			}
		}

		internal sealed class ALPHA : Rule
		{
			internal ALPHA(string spelling, IList<Rule> rules)
				: base(spelling, rules)
			{ }

			public ALPHA(ALPHA rule)
				: base(rule.spelling, rule.rules)
			{ }

			public object visit(Visitor visitor)
			{
				return visitor.visit_ALPHA(this);
			}
		}

		internal sealed class BIT : Rule
		{
			internal BIT(string spelling, IList<Rule> rules)
				: base(spelling, rules)
			{ }

			public BIT(BIT rule)
				: base(rule.spelling, rule.rules)
			{ }

			public object visit(Visitor visitor)
			{
				return visitor.visit_BIT(this);
			}
		}

		internal sealed class CHAR : Rule
		{
			internal CHAR(string spelling, IList<Rule> rules)
				: base(spelling, rules)
			{ }

			public CHAR(CHAR rule)
				: base(rule.spelling, rule.rules)
			{ }

			public object visit(Visitor visitor)
			{
				return visitor.visit_CHAR(this);
			}
		}

		internal sealed class CR : Rule
		{
			internal CR(string spelling, IList<Rule> rules)
				: base(spelling, rules)
			{ }

			public CR(CR rule)
				: base(rule.spelling, rule.rules)
			{ }

			public object visit(Visitor visitor)
			{
				return visitor.visit_CR(this);
			}
		}

		internal sealed class CRLF : Rule
		{
			internal CRLF(string spelling, IList<Rule> rules)
				: base(spelling, rules)
			{ }

			public CRLF(CRLF rule)
				: base(rule.spelling, rule.rules)
			{ }

			public object visit(Visitor visitor)
			{
				return visitor.visit_CRLF(this);
			}
		}

		internal sealed class CTL : Rule
		{
			internal CTL(string spelling, IList<Rule> rules)
				: base(spelling, rules)
			{ }

			public CTL(CTL rule)
				: base(rule.spelling, rule.rules)
			{ }

			public object visit(Visitor visitor)
			{
				return visitor.visit_CTL(this);
			}
		}

		internal sealed class DIGIT : Rule
		{
			internal DIGIT(string spelling, IList<Rule> rules)
				: base(spelling, rules)
			{ }

			public DIGIT(DIGIT rule)
				: base(rule.spelling, rule.rules)
			{ }

			public object visit(Visitor visitor)
			{
				return visitor.visit_DIGIT(this);
			}
		}

		internal sealed class DQUOTE : Rule
		{
			internal DQUOTE(string spelling, IList<Rule> rules)
				: base(spelling, rules)
			{ }

			public DQUOTE(DQUOTE rule)
				: base(rule.spelling, rule.rules)
			{ }

			public object visit(Visitor visitor)
			{
				return visitor.visit_DQUOTE(this);
			}
		}

		internal sealed class HEXDIG : Rule
		{
			internal HEXDIG(string spelling, IList<Rule> rules)
				: base(spelling, rules)
			{ }

			public HEXDIG(HEXDIG rule)
				: base(rule.spelling, rule.rules)
			{ }

			public object visit(Visitor visitor)
			{
				return visitor.visit_HEXDIG(this);
			}
		}

		internal sealed class HTAB : Rule
		{
			internal HTAB(string spelling, IList<Rule> rules)
				: base(spelling, rules)
			{ }

			public HTAB(HTAB rule)
				: base(rule.spelling, rule.rules)
			{ }

			public object visit(Visitor visitor)
			{
				return visitor.visit_HTAB(this);
			}
		}

		internal sealed class LF : Rule
		{
			internal LF(string spelling, IList<Rule> rules)
				: base(spelling, rules)
			{ }

			public LF(LF rule)
				: base(rule.spelling, rule.rules)
			{ }

			public object visit(Visitor visitor)
			{
				return visitor.visit_LF(this);
			}
		}

		internal sealed class LWSP : Rule
		{
			internal LWSP(string spelling, IList<Rule> rules)
				: base(spelling, rules)
			{ }

			public LWSP(LWSP rule)
				: base(rule.spelling, rule.rules)
			{ }

			public object visit(Visitor visitor)
			{
				return visitor.visit_LWSP(this);
			}
		}

		internal sealed class OCTET : Rule
		{
			internal OCTET(string spelling, IList<Rule> rules)
				: base(spelling, rules)
			{ }

			public OCTET(OCTET rule)
				: base(rule.spelling, rule.rules)
			{ }

			public object visit(Visitor visitor)
			{
				return visitor.visit_OCTET(this);
			}
		}

		internal sealed class SP : Rule
		{
			internal SP(string spelling, IList<Rule> rules)
				: base(spelling, rules)
			{ }

			public SP(SP rule)
				: base(rule.spelling, rule.rules)
			{ }

			public object visit(Visitor visitor)
			{
				return visitor.visit_SP(this);
			}
		}

		internal sealed class VCHAR : Rule
		{
			internal VCHAR(string spelling, IList<Rule> rules)
				: base(spelling, rules)
			{ }

			public VCHAR(VCHAR rule)
				: base(rule.spelling, rule.rules)
			{ }

			public object visit(Visitor visitor)
			{
				return visitor.visit_VCHAR(this);
			}
		}

		internal sealed class WSP : Rule
		{
			internal WSP(string spelling, IList<Rule> rules)
				: base(spelling, rules)
			{ }

			public WSP(WSP rule)
				: base(rule.spelling, rule.rules)
			{ }

			public object visit(Visitor visitor)
			{
				return visitor.visit_WSP(this);
			}
		}
	}
}