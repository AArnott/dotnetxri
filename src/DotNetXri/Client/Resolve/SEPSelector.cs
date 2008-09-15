namespace DotNetXri.Client.Resolve {
	using System.Text;
	using DotNetXri.Client.Xml;
	using System.Collections;
	using System;
	using DotNetXri.Syntax;
	using DotNetXri.Loggers;

	public class SEPSelector {

		private static ILog log = Logger.Create(typeof(Service));


		public static List select(List seps, string inType, string inMediaType, string inPath, ResolverFlags flags) {
			int n = seps.Count;
			ArrayList sepOut = new ArrayList();
			ArrayList defaultSepOut = new ArrayList(n);

			bool noDefaultType = flags.isNoDefaultT();
			bool noDefaultPath = flags.isNoDefaultP();
			bool noDefaultMediaType = flags.isNoDefaultM();

			// every SEP has each of the flags
			bool[] positiveType = new bool[n];
			bool[] positivePath = new bool[n];
			bool[] positiveMediaType = new bool[n];
			bool[] defaultType = new bool[n];
			bool[] defaultPath = new bool[n];
			bool[] defaultMediaType = new bool[n];
			bool[] presentType = new bool[n];
			bool[] presentPath = new bool[n];
			bool[] presentMediaType = new bool[n];

			log.Info("select type='" + inType + "' mtype='" + inMediaType + "' path='" + inPath + "' len(SEPs)=" + n);

			for (int i = 0; i < n; i++) {
				ArrayList sels;
				IEnumerator it;
				Service sep = (Service)seps[i];
				defaultSepOut.Add(null); // occupy the slot by setting to null

				log.Info("SEPSelector.select SEP[" + i + "] = " + sep);

				// flag to continue main loop from SEL loop
				bool sepDone = false;

				/// do Type SELs
				sepDone = false;
				sels = sep.getTypes();
				for (it = sels.GetEnumerator(); it.MoveNext(); ) {
					SEPType typeSEL = (SEPType)it.Current;
					presentType[i] = true;

					if (matchSEL(typeSEL, inType)) {
						if (typeSEL.getSelect()) {
							log.Info("SEPSelector.select SEP[" + i + "] Type is selected.");
							sepOut.Add(sep);
							sepDone = true;
							break; // next sep
						} else {
							log.Info("SEPSelector.select SEP[" + i + "] Type is a positive match.");
							positiveType[i] = true;
						}
					} else if (!noDefaultType && !positiveType[i] && isDefaultMatch(typeSEL)) {
						log.Info("SEPSelector.select SEP[" + i + "] Type is a default match.");
						defaultType[i] = true;
					}
				} // end-foreach type-sel

				if (sepDone)
					continue;

				if (!presentType[i] && !noDefaultType) {
					log.Info("SEPSelector.select SEP[" + i + "] Type is a default match (no Type element found).");
					defaultType[i] = true;
				}


				/// do Path SELs
				sepDone = false;
				sels = sep.getPaths();
				for (it = sels.GetEnumerator(); it.MoveNext(); ) {
					SEPPath pathSEL = (SEPPath)it.Current;
					presentPath[i] = true;

					if (matchSEL(pathSEL, inPath)) {
						if (pathSEL.getSelect()) {
							log.Info("SEPSelector.select SEP[" + i + "] Path is selected.");
							sepOut.Add(sep);
							sepDone = true;
							break; // next sep
						} else {
							log.Info("SEPSelector.select SEP[" + i + "] Path is a positive match.");
							positivePath[i] = true;
						}
					} else if (!noDefaultPath && !positivePath[i] && isDefaultMatch(pathSEL)) {
						log.Info("SEPSelector.select SEP[" + i + "] Path is a default match.");
						defaultPath[i] = true;
					}
				} // end-foreach path-sel

				if (sepDone)
					continue;

				if (!presentPath[i] && !noDefaultPath) {
					log.Info("SEPSelector.select SEP[" + i + "] Path is a default match (no Path element found).");
					defaultPath[i] = true;
				}



				/// do MediaType SELs
				sepDone = false;
				sels = sep.getMediaTypes();
				for (it = sels.GetEnumerator(); it.MoveNext(); ) {
					SEPMediaType mediaTypeSEL = (SEPMediaType)it.Current;
					presentMediaType[i] = true;

					if (matchSEL(mediaTypeSEL, inMediaType)) {
						if (mediaTypeSEL.getSelect()) {
							log.Info("SEPSelector.select SEP[" + i + "] MediaType is selected.");
							sepOut.Add(sep);
							sepDone = true;
							break; // next sep
						} else {
							log.Info("SEPSelector.select SEP[" + i + "] MediaType is a positive match.");
							positiveMediaType[i] = true;
						}
					} else if (!noDefaultMediaType && !positiveMediaType[i] && isDefaultMatch(mediaTypeSEL)) {
						log.Info("SEPSelector.select SEP[" + i + "] MediaType is a default match.");
						defaultMediaType[i] = true;
					}
				} // end-foreach mediatype-sel

				if (sepDone)
					continue;

				if (!presentMediaType[i] && !noDefaultMediaType) {
					log.Info("SEPSelector.select SEP[" + i + "] MediaType is a default match (no MediaType element found).");
					defaultMediaType[i] = true;
				}

				if (positiveType[i] && positivePath[i] && positiveMediaType[i]) {
					log.Info("SEPSelector.select SEP[" + i + "] is an ALL positive match.");
					sepOut.Add(sep);
					// next sep
				} else if (sepOut.Count == 0 && (
							(positiveType[i] || defaultType[i]) &&
							(positivePath[i] || defaultPath[i]) &&
							(positiveMediaType[i] || defaultMediaType[i]))
						) {
					log.Info("SEPSelector.select SEP[" + i + "] is a default match.");
					defaultSepOut[i] = sep; // instead of using add(), override the null at this index pos
				}
			} // end-foreach sep

			if (sepOut.Count == 0) {
				int[] numMatches = new int[n];

				for (int i = 0; i < n; i++) {
					object sep = defaultSepOut[i];
					if (sep == null)
						continue;

					if (positiveType[i])
						numMatches[i]++;
					if (positivePath[i])
						numMatches[i]++;
					if (positiveMediaType[i])
						numMatches[i]++;
				}


				/// add the seps with default match and has 2 positive matches
				for (int i = 0; i < n; i++) {
					if (numMatches[i] >= 2) {
						log.Info("SEPSelector.select Phase 2 - SEP[" + i + "] is selected for having 2 positive matches.");
						sepOut.Add(seps[i]);
					}
				}

				/// still empty, add those seps with default match and has 1 positive match
				if (sepOut.Count == 0) {
					for (int i = 0; i < n; i++) {
						if (numMatches[i] == 1) {
							log.Info("SEPSelector.select Phase 2 - SEP[" + i + "] is selected for having 1 positive match.");
							sepOut.Add(seps[i]);
						}
					}
				}

				/// still empty? add the default seps
				if (sepOut.Count == 0) {
					for (int i = 0; i < n; i++) {
						object sep = defaultSepOut[i];
						if (sep != null) {
							log.Info("SEPSelector.select Phase 2 - SEP[" + i + "] is selected for being a default match.");
							sepOut.Add(sep);
						}
					}
				}
			}

			return sepOut;
		}


		public static bool isDefaultMatch(SEPElement sel) {
			string m = sel.getMatch();
			return (m != null && m.Equals(SEPElement.MATCH_ATTR_DEFAULT));
		}


		/* This method is used to match the element of the service */
		private static bool matchSEL(SEPElement element, string inValue) {
			string matchAttr = element.getMatch();
			string selVal = element.getValue();

			if (matchAttr != null) {
				if (matchAttr.Equals(SEPElement.MATCH_ATTR_ANY))
					return true;
				else if (matchAttr.Equals(SEPElement.MATCH_ATTR_NON_NULL)) {
					return (inValue != null && inValue.Length > 0);
				} else if (matchAttr.Equals(SEPElement.MATCH_ATTR_NULL)) {
					return (inValue == null || inValue.Length == 0);
				}
				/*
				else if (elementMatch.Equals(SEPElement.MATCH_ATTR_NONE)) {
					return false;
				}
				*/

				// fall through
			}

			// In CD02 if "match" attribute is absent, we match content
			if (matchAttr == null || matchAttr.Equals(SEPElement.MATCH_ATTR_CONTENT)) {
				// special case: input value is null (against e.g. <Path />)
				if (inValue == null || inValue.Length == 0)
					return (selVal.Length == 0);

				if (element is SEPType)
					return matchType(selVal, inValue);
				else if (element is SEPPath)
					return matchPath(selVal, inValue);
				else if (element is SEPMediaType)
					return matchMediaType(selVal, inValue);
				else
					throw new RuntimeException("Unsupported SEL");
			}

			// MATCH_ATTR_DEFAULT is not considered here because it is checked elsewhere
			return false;
		}



		public static bool matchType(string selType, string inType) {
			return inType.Equals(selType);
		}

		public static bool matchPath(string selPath, string inPath) {
			// XXX use Unicode caseless matching
			if (inPath.Equals(selPath, StringComparison.OrdinalIgnoreCase))
				return true;

			log.Info("xrdPath = '" + selPath + "'");
			log.Info("inputPath = '" + inPath + "'");

			if (selPath.Length > 0 && selPath[0] != '/')
				selPath = '/' + selPath; // prepend leading slash

			try {
				XRIAbsolutePath xrdAbsPath = new XRIAbsolutePath(selPath);
				XRIAbsolutePath inputAbsPath = new XRIAbsolutePath(inPath);
				return xrdAbsPath.isPrefixOf(inputAbsPath);
			} catch (XRIParseException e) {
				log.Error("matchPath(selPath='" + selPath + "', inPath='" + inPath + "' - XRIParseException caught: " + e.getMessage());
				return false;
			}
		}


		public static bool matchMediaType(string selMediaType, string inMediaType) {
			MimeType candidateMimeType = MimeType.parse(inMediaType);
			MimeType critMimeType = MimeType.parse(selMediaType);
			return critMimeType.Equals(candidateMimeType);
		}





		private ArrayList seps = null;
		private string matchTypeValue = null;
		private string matchMediaTypeValue = null;
		private string matchPathValue = null;
		private bool matchedNonDefaultType = false;
		private bool matchedNonDefaultMediaType = false;
		private bool matchedNonDefaultPath = false;

		// cloned copies of seps, these are changed for, why cloned
		// because in the phase 1 of matching of classes asumes the 
		// some default values of match attribute, that are required
		// for select rules, otherwise requires to check default values 
		// of match attributes in phase2 (select) + also keep the indexes remove all later
		private ArrayList matchedPhase1List = null;

		// by the end of phase2 will have finally selected indexs (pointers to seps list)
		private ArrayList indexes = new ArrayList();

		public SEPSelector(ArrayList pSeps) {
			seps = pSeps;
		}

		public void reset() {
			seps = null;
		}

		public ArrayList getSelectedSEPs(string type, string mediaType, string path) {
			this.matchTypeValue = type;
			this.matchMediaTypeValue = mediaType;
			this.matchPathValue = path;
			this.matchedNonDefaultType = false;
			this.matchedNonDefaultMediaType = false;
			this.matchedNonDefaultPath = false;

			matchedPhase1List = new ArrayList();
			applyMatchingRules();
			removeMatchDefaults();
			applySelectionRules();
			ArrayList returnList = new ArrayList();
			for (int i = 0; i < seps.Count; i++) {
				// eliminate all the elements that must be removed using indexes list
				int inx = i;
				if (!indexes.Contains(inx)) {
					returnList.Add(seps[i]);
				}
			}
			matchedPhase1List.Clear();
			indexes.Clear();
			return returnList;
		}


		private bool applyMatchingRules() {
			try {
				for (int i = 0; i < seps.Count; i++) {

					Service service = (Service)seps[i];
					service = (Service)service.clone();
					ArrayList types = service.getTypes();
					ArrayList mtypes = service.getMediaTypes();
					ArrayList paths = service.getPaths();
					ArrayList elements = new ArrayList();

					/* rule 1 is applied after 2, 3, 4 for processing optimization 
					 * if SEP element is omited consider <element match="default" /> 
					 * according specs this would resort if matching elements found */
					if (types.Count == 0) {
						SEPType type = new SEPType(null, SEPElement.MATCH_ATTR_DEFAULT, false);
						service.addType(type);
						types = service.getTypes();
					}
					if (mtypes.Count == 0) {
						SEPMediaType mtype = new SEPMediaType(null, SEPElement.MATCH_ATTR_DEFAULT, false);
						service.addMediaType(mtype);
						mtypes = service.getMediaTypes();
					}
					if (paths.Count == 0) {
						SEPPath path = new SEPPath(null, SEPElement.MATCH_ATTR_DEFAULT, false);
						service.addPath(path);
						paths = service.getPaths();
					}

					/* Rule 2  follows the naturally the element value is null */

					/* Rule 3 if element present but match attribute omitted or null 
					 * examples: <Type /> or <Type>xxxx</Type> or 
					 * <Type match="" /> or <Type match="">yyyy</Type>
					 * Then set match="content" 
					 */
					elements.AddRange(service.getTypes());
					elements.AddRange(service.getMediaTypes());
					elements.AddRange(service.getPaths());

					// anytime we find an element that has match="none", we will 
					// skip the service entirely.
					// Should be optimized and merged into one of the loops below
					bool foundMatchNone = false;
					for (int j = 0; j < elements.Count; j++) {
						SEPElement element = (SEPElement)elements[j];
						if (element.getMatch() != null && element.getMatch().Equals(SEPElement.MATCH_ATTR_NONE)) {
							foundMatchNone = true;
						}
					}
					if (foundMatchNone) {
						// skip this service by adding a null reference so 
						// our index is not screwed... UGLY!!!
						matchedPhase1List.Add(null);
						continue;
					}

					// set the default match attribute, should be optimized by merging with
					// one of the loops below
					for (int j = 0; j < elements.Count; j++) {
						SEPElement element = (SEPElement)elements[j];
						if (element.getMatch() == null || element.getMatch().Equals("")) {
							element.setMatch(SEPElement.MATCH_ATTR_CONTENT);
						}
					}

					ArrayList output = new ArrayList();
					/* Rule 4 use table 20 (XRI specs march 2006 wd10 ) to match rules */
					for (int j = 0; j < types.Count; j++) {
						SEPElement element = (SEPElement)types[j];
						if (element.getMatch().Equals(SEPElement.MATCH_ATTR_DEFAULT)) {
							// retain match="default"
							output.Add(element);
						} else if (match(element)) {
							matchedNonDefaultType = true;
							output.Add(element);
						}
					}
					types = output;
					service.setTypes(types);

					output = new ArrayList();
					for (int j = 0; j < mtypes.Count; j++) {
						SEPElement element = (SEPElement)mtypes[j];
						if (element.getMatch().Equals(SEPElement.MATCH_ATTR_DEFAULT)) {
							output.Add(element);
						} else if (match(element)) {
							matchedNonDefaultMediaType = true;
							output.Add(element);
						}
					}
					mtypes = output;
					service.setMediaTypes(mtypes);

					output = new ArrayList();
					for (int j = 0; j < paths.Count; j++) {
						SEPElement element = (SEPElement)paths[j];
						if (element.getMatch().Equals(SEPElement.MATCH_ATTR_DEFAULT)) {
							output.Add(element);
						} else if (match(element)) {
							matchedNonDefaultPath = true;
							output.Add(element);
						}
					}
					paths = output;
					service.setPaths(paths);

					matchedPhase1List.Add(service);
				}
			} catch (CloneNotSupportedException cnse) {
				log.Error("This should not happen as we implemented Service.clone() - the only clone call we make here");
				throw new RuntimeException("Internal error: " + cnse.getMessage());
			}

			return true;
		}



		private bool removeMatchDefaults() {
			for (int i = 0; i < matchedPhase1List.Count; i++) {
				Service sep = (Service)matchedPhase1List[i];
				if (sep == null) continue; // inactive service

				// earlier we found a non-default match for Type element,
				// remove all the match="default" elements
				if (matchedNonDefaultType) {
					ArrayList newTypes = new ArrayList();
					for (int j = 0; j < sep.getTypes().Count; j++) {
						SEPElement element = (SEPElement)sep.getTypes()[j];
						if (!element.getMatch().Equals(SEPElement.MATCH_ATTR_DEFAULT)) {
							newTypes.Add(element);
						} else log.Info("removeMatchDefaults - removing Type[" + j + "] from Service[" + i + "]");
					}
					sep.setTypes(newTypes);
				}
				if (matchedNonDefaultPath) {
					ArrayList newPaths = new ArrayList();
					for (int j = 0; j < sep.getPaths().Count; j++) {
						SEPElement element = (SEPElement)sep.getPaths()[j];
						if (!element.getMatch().Equals(SEPElement.MATCH_ATTR_DEFAULT)) {
							newPaths.Add(element);
						} else log.Info("removeMatchDefaults - removing Path[" + j + "] from Service[" + i + "]");
					}
					sep.setPaths(newPaths);
				}
				if (matchedNonDefaultMediaType) {
					ArrayList newMediaTypes = new ArrayList();
					for (int j = 0; j < sep.getMediaTypes().Count; j++) {
						SEPElement element = (SEPElement)sep.getMediaTypes()[j];
						if (!element.getMatch().Equals(SEPElement.MATCH_ATTR_DEFAULT)) {
							newMediaTypes.Add(element);
						} else log.Info("removeMatchDefaults - removing MediaType[" + j + "] from Service[" + i + "]");
					}
					sep.setMediaTypes(newMediaTypes);
				}
			}
			return true;
		}


		private bool applySelectionRules() {
			for (int i = 0; i < matchedPhase1List.Count; i++) {
				Service sep = (Service)matchedPhase1List[i];
				if (sep == null) {
					log.Debug("applySelectionRules - service[" + i + "] is inactive");
					// inactive service
				} else if (!canBeSelected(sep)) {
					log.Debug("applySelectionRules - service[" + i + "] will not be selected");
					indexes.Add(i);
				} else {
					log.Debug("applySelectionRules - service[" + i + "] is good to go");
				}
			}

			return true;
		}


		/* The input service elements contain all matched elements of service
		 * We assume all the service elements met the criteria and filtered
		 * as per the matching rules applied before applying select criteria
		 */
		private bool canBeSelected(Service service) {
			ArrayList elements = new ArrayList();

			elements.AddRange(service.getTypes());
			elements.AddRange(service.getMediaTypes());
			elements.AddRange(service.getPaths());

			/*
			 * Section 8.3
			 * Rule 1 - if any of the elements has a match attribute value of 'none', do not select the service
			 *          (shouldn't this be done in the matching phase?)
			 * Rule 2 - OR operation any element of service matches then select this service
			 * Rule 3 - case when select is false
			 * ie.e AND operation => at least one matchTypeValue, mediatype & matchPathValue must match
			 */
			bool typeMatched = false;
			bool mediaTypeMatched = false;
			bool pathMatched = false;
			bool selectService = false;

			for (int i = 0; i < elements.Count; i++) {
				SEPElement element = (SEPElement)elements[i];

				if (element.getMatch().Equals(SEPElement.MATCH_ATTR_NONE))
					// rule 1 - do not select this service when any element has match="none"
					return false;

				if (element.getSelect())
					selectService = true;

				if (element is SEPType) {
					typeMatched = true;
				} else if (element is SEPMediaType) {
					mediaTypeMatched = true;
				} else if (element is SEPPath) {
					pathMatched = true;
				}
			}

			if (selectService || (typeMatched && mediaTypeMatched && pathMatched)) {
				return true;
			}

			return false;
		}

		/* This method is used to match the element of the service */
		private bool match(SEPElement element) {
			string elementMatch = element.getMatch();
			string elementValue = element.getValue();

			string matchElementValue = null;

			if (element is SEPType) {
				matchElementValue = this.matchTypeValue;
			} else if (element is SEPMediaType) {
				matchElementValue = this.matchMediaTypeValue;
			} else if (element is SEPPath) {
				matchElementValue = this.matchPathValue;
			}

			if (elementMatch.Equals(SEPElement.MATCH_ATTR_CONTENT)) {
				if (matchElementValue == null && elementValue == null) return true;
				if (matchElementValue != null && elementValue == null) return false;
				if (matchElementValue == null && elementValue != null) return false;
				return matchContent(element);
			}
			if (elementMatch.Equals(SEPElement.MATCH_ATTR_ANY)) {
				return true;
			}
			if (elementMatch.Equals(SEPElement.MATCH_ATTR_NON_NULL)) {
				if (matchElementValue != null && matchElementValue.Length != 0)
					return true;
				return false;
			}
			if (elementMatch.Equals(SEPElement.MATCH_ATTR_NULL)) {
				if (matchElementValue == null || matchElementValue.Length == 0)
					return true;
				return false;
			}
			if (elementMatch.Equals(SEPElement.MATCH_ATTR_NONE)) {
				return false;
			}

			// MATCH_ATTR_DEFAULT is not considered here because it should always stay
			// (whether internally added or already present in the original XRDS)
			// during the matching phase, and only removed if other non-default 
			// elements are matched.

			return false;
		}


		public bool matchContent(SEPElement candidate) {
			if (candidate is SEPType) {
				return matchContent((SEPType)candidate);
			} else if (candidate is SEPPath) {
				return matchContent((SEPPath)candidate);
			} else if (candidate is SEPMediaType) {
				return matchContent((SEPMediaType)candidate);
			}
			return false;
		}

		public bool matchContent(SEPType type) {
			return this.matchTypeValue.Equals(type.getValue());
		}

		public bool matchContent(SEPPath path) {
			string xrdPath = trimPath(path.getValue());
			string inputPath = trimPath(this.matchPathValue);

			// try verbatim match (caseless)
			if (inputPath.Equals(xrdPath, StringComparison.OrdinalIgnoreCase))
				return true;

			log.Info("xrdPath = '" + xrdPath + "'");
			log.Info("inputPath = '" + inputPath + "'");

			XRIAbsolutePath xrdAbsPath = new XRIAbsolutePath("/" + xrdPath);
			XRIAbsolutePath inputAbsPath = new XRIAbsolutePath("/" + inputPath);

			return xrdAbsPath.isPrefixOf(inputAbsPath);
		}


		public bool matchContent(SEPMediaType mtype) {
			MimeType candidateMimeType = MimeType.parse(mtype.getValue());
			MimeType critMimeType = MimeType.parse(this.matchMediaTypeValue);
			return critMimeType.Equals(candidateMimeType);
		}


		/**
		 * Removes trailing delimiters '/', '*' or '!'
		 * @param path
		 * @return
		 */
		private string trimPath(string path) {
			StringBuilder sb = new StringBuilder(path.Trim());

			while (sb.Length > 0) {
				char last = sb[sb.Length - 1];
				if (last == '/' || last == '*' || last == '!')
					sb.Remove(sb.Length - 1, 1);
				else
					break;
			}
			return sb.ToString();
		}
	}
}