package org.openxri.resolve;
import java.util.*;

import org.openxri.XRIAbsolutePath;
import org.openxri.XRIParseException;
import org.openxri.xml.*;

public class SEPSelector {

    private static org.apache.commons.logging.Log log =
    	org.apache.commons.logging.LogFactory.getLog(Service.class.getName());

    
	public static List select(List seps, String inType, String inMediaType, String inPath, ResolverFlags flags)
	{
		int n = seps.size();
		ArrayList sepOut = new ArrayList();
		ArrayList defaultSepOut = new ArrayList(n);
		
		final boolean noDefaultType = flags.isNoDefaultT();
		final boolean noDefaultPath = flags.isNoDefaultP();
		final boolean noDefaultMediaType = flags.isNoDefaultM();
		
		// every SEP has each of the flags
		boolean positiveType[]      = new boolean[n];
		boolean positivePath[]      = new boolean[n];
		boolean positiveMediaType[] = new boolean[n];
		boolean defaultType[]       = new boolean[n];
		boolean defaultPath[]       = new boolean[n];
		boolean defaultMediaType[]  = new boolean[n];
		boolean presentType[]       = new boolean[n];
		boolean presentPath[]       = new boolean[n];
		boolean presentMediaType[]  = new boolean[n];
		
		log.trace("select type='" + inType + "' mtype='" + inMediaType + "' path='" + inPath + "' len(SEPs)=" + n);
		
		for (int i = 0; i < n; i++) {
			List sels;
			Iterator it;
			Service sep = (Service)seps.get(i);
			defaultSepOut.add(null); // occupy the slot by setting to null

			log.trace("SEPSelector.select SEP[" + i + "] = " + sep);

			// flag to continue main loop from SEL loop
			boolean sepDone = false;
			
			/// do Type SELs
			sepDone = false;
			sels = sep.getTypes();
			for (it = sels.iterator(); it.hasNext(); ) {
				SEPType typeSEL = (SEPType)it.next();
				presentType[i] = true;
				
				if (matchSEL(typeSEL, inType)) {
					if (typeSEL.getSelect()) {
						log.trace("SEPSelector.select SEP[" + i + "] Type is selected.");
						sepOut.add(sep);
						sepDone = true;
						break; // next sep
					}
					else {
						log.trace("SEPSelector.select SEP[" + i + "] Type is a positive match.");
						positiveType[i] = true;
					}
				}
				else if (!noDefaultType && !positiveType[i] && isDefaultMatch(typeSEL)) {
					log.trace("SEPSelector.select SEP[" + i + "] Type is a default match.");
					defaultType[i] = true;
				}
			} // end-foreach type-sel
			
			if (sepDone)
				continue;
			
			if (!presentType[i] && !noDefaultType) {
				log.trace("SEPSelector.select SEP[" + i + "] Type is a default match (no Type element found).");
				defaultType[i] = true;
			}

			
			/// do Path SELs
			sepDone = false;
			sels = sep.getPaths();
			for (it = sels.iterator(); it.hasNext(); ) {
				SEPPath pathSEL = (SEPPath)it.next();
				presentPath[i] = true;
				
				if (matchSEL(pathSEL, inPath)) {
					if (pathSEL.getSelect()) {
						log.trace("SEPSelector.select SEP[" + i + "] Path is selected.");
						sepOut.add(sep);
						sepDone = true;
						break; // next sep
					}
					else {
						log.trace("SEPSelector.select SEP[" + i + "] Path is a positive match.");
						positivePath[i] = true;
					}
				}
				else if (!noDefaultPath && !positivePath[i] && isDefaultMatch(pathSEL)) {
					log.trace("SEPSelector.select SEP[" + i + "] Path is a default match.");
					defaultPath[i] = true;
				}
			} // end-foreach path-sel
			
			if (sepDone)
				continue;
			
			if (!presentPath[i] && !noDefaultPath) {
				log.trace("SEPSelector.select SEP[" + i + "] Path is a default match (no Path element found).");
				defaultPath[i] = true;
			}
			

			
			/// do MediaType SELs
			sepDone = false;
			sels = sep.getMediaTypes();
			for (it = sels.iterator(); it.hasNext(); ) {
				SEPMediaType mediaTypeSEL = (SEPMediaType)it.next();
				presentMediaType[i] = true;
				
				if (matchSEL(mediaTypeSEL, inMediaType)) {
					if (mediaTypeSEL.getSelect()) {
						log.trace("SEPSelector.select SEP[" + i + "] MediaType is selected.");
						sepOut.add(sep);
						sepDone = true;
						break; // next sep
					}
					else {
						log.trace("SEPSelector.select SEP[" + i + "] MediaType is a positive match.");
						positiveMediaType[i] = true;
					}
				}
				else if (!noDefaultMediaType && !positiveMediaType[i] && isDefaultMatch(mediaTypeSEL)) {
					log.trace("SEPSelector.select SEP[" + i + "] MediaType is a default match.");
					defaultMediaType[i] = true;
				}
			} // end-foreach mediatype-sel
			
			if (sepDone)
				continue;
			
			if (!presentMediaType[i] && !noDefaultMediaType) {
				log.trace("SEPSelector.select SEP[" + i + "] MediaType is a default match (no MediaType element found).");
				defaultMediaType[i] = true;
			}

			if (positiveType[i] && positivePath[i] && positiveMediaType[i]) {
				log.trace("SEPSelector.select SEP[" + i + "] is an ALL positive match.");
				sepOut.add(sep);
				// next sep
			}
			else if (sepOut.isEmpty() && (
						(positiveType[i] || defaultType[i]) &&
						(positivePath[i] || defaultPath[i]) &&
						(positiveMediaType[i] || defaultMediaType[i]))
					)
			{
				log.trace("SEPSelector.select SEP[" + i + "] is a default match.");
				defaultSepOut.set(i, sep); // instead of using add(), override the null at this index pos
			}
		} // end-foreach sep

		if (sepOut.isEmpty()) {
			int numMatches[] = new int[n];
			
			for (int i = 0; i < n; i++) {
				Object sep = defaultSepOut.get(i);
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
					log.trace("SEPSelector.select Phase 2 - SEP[" + i + "] is selected for having 2 positive matches.");
					sepOut.add(seps.get(i));
				}
			}
			
			/// still empty, add those seps with default match and has 1 positive match
			if (sepOut.isEmpty()) {
				for (int i = 0; i < n; i++) {
					if (numMatches[i] == 1) {
						log.trace("SEPSelector.select Phase 2 - SEP[" + i + "] is selected for having 1 positive match.");
						sepOut.add(seps.get(i));
					}
				}
			}
			
			/// still empty? add the default seps
			if (sepOut.isEmpty()) {
				for (int i = 0; i < n; i++) {
					Object sep = defaultSepOut.get(i);
					if (sep != null) {
						log.trace("SEPSelector.select Phase 2 - SEP[" + i + "] is selected for being a default match.");
						sepOut.add(sep);
					}
				}
			}
		}
		
		return sepOut;		
	}
    
    
    public static boolean isDefaultMatch(SEPElement sel)
    {
    	String m = sel.getMatch();
    	return (m != null && m.equals(SEPElement.MATCH_ATTR_DEFAULT));
    }
    

    /* This method is used to match the element of the service */
	private static boolean matchSEL(SEPElement element, String inValue)
	{
		String matchAttr =  element.getMatch();
		String selVal = element.getValue();

		if (matchAttr != null) {		
			if (matchAttr.equals(SEPElement.MATCH_ATTR_ANY))
				return true;
			else if (matchAttr.equals(SEPElement.MATCH_ATTR_NON_NULL)) {
				return (inValue != null && inValue.length() > 0);
			}
			else if (matchAttr.equals(SEPElement.MATCH_ATTR_NULL)) {
				return (inValue == null || inValue.length() == 0);
			}
			/*
			else if (elementMatch.equals(SEPElement.MATCH_ATTR_NONE)) {
				return false;
			}
			*/
			
			// fall through
		}

		// In CD02 if "match" attribute is absent, we match content
		if (matchAttr == null || matchAttr.equals(SEPElement.MATCH_ATTR_CONTENT)) {
			// special case: input value is null (against e.g. <Path />)
			if (inValue == null || inValue.length() == 0)
				return (selVal.length() == 0);

			if (element instanceof SEPType)
				return matchType(selVal, inValue);
			else if (element instanceof SEPPath)
				return matchPath(selVal, inValue);
			else if (element instanceof SEPMediaType)
				return matchMediaType(selVal, inValue);
			else
				throw new RuntimeException("Unsupported SEL");
		}

		// MATCH_ATTR_DEFAULT is not considered here because it is checked elsewhere
		return false;
	}


	
	public static boolean matchType (String selType, String inType) {
		return inType.equals(selType);
	}
	
	public static boolean matchPath (String selPath, String inPath) {		
		// XXX use Unicode caseless matching
		if (inPath.equalsIgnoreCase(selPath))
			return true;

		log.trace("xrdPath = '" + selPath + "'");
		log.trace("inputPath = '" + inPath + "'");
		
		if (selPath.length() > 0 && selPath.charAt(0) != '/')
			selPath = '/' + selPath; // prepend leading slash

		try {
			XRIAbsolutePath xrdAbsPath = new XRIAbsolutePath(selPath);
			XRIAbsolutePath inputAbsPath = new XRIAbsolutePath(inPath);
			return xrdAbsPath.isPrefixOf(inputAbsPath);
		}
		catch (XRIParseException e) {
			log.error("matchPath(selPath='" + selPath + "', inPath='" + inPath +"' - XRIParseException caught: " + e.getMessage());
			return false;
		}
	}
	
	
	public static boolean matchMediaType (String selMediaType, String inMediaType) {
		MimeType candidateMimeType = MimeType.parse(inMediaType);
		MimeType critMimeType = MimeType.parse(selMediaType);
		return critMimeType.equals(candidateMimeType);
	}	
    
    
    
    
    
	private ArrayList seps = null;
	private String matchTypeValue = null;
	private String matchMediaTypeValue = null;
	private String matchPathValue = null;
	private boolean matchedNonDefaultType = false;
	private boolean matchedNonDefaultMediaType = false;
	private boolean matchedNonDefaultPath = false;
	
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

	public void reset()
	{
		seps = null;
	}
	
	public ArrayList getSelectedSEPs(String type, String mediaType, String path)
	{
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
		for(int i = 0; i < seps.size(); i++){
			// eliminate all the elements that must be removed using indexes list
			Integer inx = new Integer(i);
			if(! indexes.contains(inx)){
				returnList.add(seps.get(i));
			}
		}
		matchedPhase1List.clear();
		indexes.clear();
		return returnList;
	}
	
	
	private boolean applyMatchingRules(){
		try {
			for(int i=0; i < seps.size(); i++ ){
				
				Service service = (Service) seps.get(i);
				service = (Service)service.clone();
				List types = service.getTypes();
				List mtypes = service.getMediaTypes();
				List paths = service.getPaths();
				Vector elements = new Vector();
				
				/* rule 1 is applied after 2, 3, 4 for processing optimization 
				 * if SEP element is omited consider <element match="default" /> 
				 * according specs this would resort if matching elements found */
				if (types.size() == 0) {
					SEPType type = new SEPType(null,SEPElement.MATCH_ATTR_DEFAULT, Boolean.FALSE);
					service.addType(type);
					types = service.getTypes();
				}
				if (mtypes.size() == 0) {
					SEPMediaType mtype = new SEPMediaType(null,SEPElement.MATCH_ATTR_DEFAULT, Boolean.FALSE);
					service.addMediaType(mtype);
					mtypes = service.getMediaTypes();
				}
				if (paths.size() == 0) {
					SEPPath path = new SEPPath(null,SEPElement.MATCH_ATTR_DEFAULT, Boolean.FALSE);
					service.addPath(path);
					paths = service.getPaths();
				}
				
				/* Rule 2  follows the naturally the element value is null */
				
				/* Rule 3 if element present but match attribute omitted or null 
				 * examples: <Type /> or <Type>xxxx</Type> or 
				 * <Type match="" /> or <Type match="">yyyy</Type>
				 * Then set match="content" 
				 */
				elements.addAll(service.getTypes());
				elements.addAll(service.getMediaTypes());
				elements.addAll(service.getPaths());
				
				// anytime we find an element that has match="none", we will 
				// skip the service entirely.
				// Should be optimized and merged into one of the loops below
				boolean foundMatchNone = false;
				for (int j =0; j < elements.size(); j++) {
					SEPElement element = (SEPElement) elements.get(j);
					if(element.getMatch() != null && element.getMatch().equals(SEPElement.MATCH_ATTR_NONE)){
						foundMatchNone = true;
					}
				}
				if (foundMatchNone) {
					// skip this service by adding a null reference so 
					// our index is not screwed... UGLY!!!
					matchedPhase1List.add(null);
					continue;
				}

				// set the default match attribute, should be optimized by merging with
				// one of the loops below
				for(int j =0; j < elements.size(); j++){
					SEPElement element = (SEPElement) elements.get(j);
					if(element.getMatch() == null || element.getMatch().equals("")){
						element.setMatch(SEPElement.MATCH_ATTR_CONTENT);
					}
				}
				
				Vector output= new Vector();
				/* Rule 4 use table 20 (XRI specs march 2006 wd10 ) to match rules */
				for(int j =0; j < types.size(); j++){
					SEPElement element = (SEPElement) types.get(j);
					if (element.getMatch().equals(SEPElement.MATCH_ATTR_DEFAULT)) {
						// retain match="default"
						output.add(element);
					}						
					else if (match(element)) {
						matchedNonDefaultType = true;
						output.add(element);
					}
				}
				types = output;
				service.setTypes(types);
	
				output = new Vector();
				for(int j =0; j < mtypes.size(); j++){
					SEPElement element = (SEPElement) mtypes.get(j);
					if (element.getMatch().equals(SEPElement.MATCH_ATTR_DEFAULT)) {
						output.add(element);
					}
					else if (match(element)) {
						matchedNonDefaultMediaType = true;
						output.add(element);
					}
				}
				mtypes=output;
				service.setMediaTypes(mtypes);
				
				output = new Vector();
				for(int j =0; j < paths.size(); j++){
					SEPElement element = (SEPElement) paths.get(j);
					if (element.getMatch().equals(SEPElement.MATCH_ATTR_DEFAULT)) {
						output.add(element);
					}
					else if (match(element)) {
						matchedNonDefaultPath = true;
						output.add(element);
					}
				}	
				paths = output;
				service.setPaths(paths);
				
				matchedPhase1List.add(service);
			}
		}
		catch (CloneNotSupportedException cnse) {
			log.error("This should not happen as we implemented Service.clone() - the only clone call we make here");
			throw new RuntimeException("Internal error: " + cnse.getMessage());
		}
		
		return true;
	}
	
	
	
	private boolean removeMatchDefaults()
	{
		for (int i =0; i< matchedPhase1List.size(); i++){
			Service sep =(Service)matchedPhase1List.get(i);
			if (sep == null) continue; // inactive service
			
			// earlier we found a non-default match for Type element,
			// remove all the match="default" elements
			if (matchedNonDefaultType) {
				Vector newTypes = new Vector();
				for (int j =0; j < sep.getTypes().size(); j++) {
					SEPElement element = (SEPElement) sep.getTypes().get(j);
					if (!element.getMatch().equals(SEPElement.MATCH_ATTR_DEFAULT)) {
						newTypes.add(element);
					}
					else log.trace("removeMatchDefaults - removing Type[" + j + "] from Service[" + i + "]");
				}
				sep.setTypes(newTypes);
			}
			if (matchedNonDefaultPath) {
				Vector newPaths = new Vector();
				for (int j =0; j < sep.getPaths().size(); j++) {
					SEPElement element = (SEPElement) sep.getPaths().get(j);
					if (!element.getMatch().equals(SEPElement.MATCH_ATTR_DEFAULT)) {
						newPaths.add(element);
					}
					else log.trace("removeMatchDefaults - removing Path[" + j + "] from Service[" + i + "]");
				}
				sep.setPaths(newPaths);
			}
			if (matchedNonDefaultMediaType) {
				Vector newMediaTypes = new Vector();
				for (int j =0; j < sep.getMediaTypes().size(); j++) {
					SEPElement element = (SEPElement) sep.getMediaTypes().get(j);
					if (!element.getMatch().equals(SEPElement.MATCH_ATTR_DEFAULT)) {
						newMediaTypes.add(element);
					}
					else log.trace("removeMatchDefaults - removing MediaType[" + j + "] from Service[" + i + "]");
				}
				sep.setMediaTypes(newMediaTypes);
			}
		}
		return true;
	}
	
	
	private boolean applySelectionRules()
	{
		for(int i =0; i< matchedPhase1List.size(); i++){
			Service sep =(Service)matchedPhase1List.get(i);
			if (sep == null) {
				log.debug("applySelectionRules - service[" + i + "] is inactive");
				// inactive service
			}
			else if(!canBeSelected(sep)){
				log.debug("applySelectionRules - service[" + i + "] will not be selected");
				indexes.add(new Integer(i));
			}
			else {
				log.debug("applySelectionRules - service[" + i + "] is good to go");
			}
		}
		
		return true;
	}

	
	/* The input service elements contain all matched elements of service
	 * We assume all the service elements met the criteria and filtered
	 * as per the matching rules applied before applying select criteria
	 */
	private boolean canBeSelected(Service service){
	    Vector elements = new Vector();
	    
	    elements.addAll(service.getTypes());
	    elements.addAll(service.getMediaTypes());
	    elements.addAll(service.getPaths());

		/*
		 * Section 8.3
		 * Rule 1 - if any of the elements has a match attribute value of 'none', do not select the service
		 *          (shouldn't this be done in the matching phase?)
		 * Rule 2 - OR operation any element of service matches then select this service
		 * Rule 3 - case when select is false
		 * ie.e AND operation => at least one matchTypeValue, mediatype & matchPathValue must match
		 */
		boolean typeMatched = false;
		boolean mediaTypeMatched = false;
		boolean pathMatched = false;
		boolean selectService = false;
		
		for(int i =0; i < elements.size(); i++){
			SEPElement element = (SEPElement) elements.get(i);
			
			if (element.getMatch().equals(SEPElement.MATCH_ATTR_NONE))
				// rule 1 - do not select this service when any element has match="none"
				return false;
			
			if (element.getSelect())
				selectService = true;

			if(element instanceof SEPType){
				typeMatched = true;
			}
			else if(element instanceof SEPMediaType){
				mediaTypeMatched = true;
			}
			else if(element instanceof SEPPath){
				pathMatched = true;
			}
		}

		if (selectService || (typeMatched && mediaTypeMatched && pathMatched)) {
			return true;
		}

		return false;
	}

	/* This method is used to match the element of the service */
	private boolean match(SEPElement element){
		String elementMatch =  element.getMatch();
		String elementValue = element.getValue();
		
		String matchElementValue = null;
		
		if(element instanceof SEPType){
			matchElementValue = this.matchTypeValue;
		}
		else if(element instanceof SEPMediaType){
			matchElementValue = this.matchMediaTypeValue;
		}
		else if(element instanceof SEPPath){
			matchElementValue = this.matchPathValue;
		}

		if(elementMatch.equals(SEPElement.MATCH_ATTR_CONTENT)){
			if(matchElementValue == null && elementValue == null) return true;
			if(matchElementValue != null && elementValue == null )return false;
			if(matchElementValue == null && elementValue != null) return false;
			return matchContent(element);
		}
		if(elementMatch.equals(SEPElement.MATCH_ATTR_ANY)){
			return true;
		}
		if(elementMatch.equals(SEPElement.MATCH_ATTR_NON_NULL)){
			if (matchElementValue != null && matchElementValue.length() != 0)
				return true;
			return false;
		}
		if(elementMatch.equals(SEPElement.MATCH_ATTR_NULL)){
			if (matchElementValue == null || matchElementValue.length() == 0)
				return true;
			return false;
		}
		if(elementMatch.equals(SEPElement.MATCH_ATTR_NONE)){
			return false;
		}
		
		// MATCH_ATTR_DEFAULT is not considered here because it should always stay
		// (whether internally added or already present in the original XRDS)
		// during the matching phase, and only removed if other non-default 
		// elements are matched.
		
		return false;
	}

	
	public boolean matchContent (SEPElement candidate) {
		if (candidate instanceof SEPType) {
			return matchContent((SEPType)candidate);
		}
		else if (candidate instanceof SEPPath) {
			return matchContent((SEPPath)candidate);
		}
		else if (candidate instanceof SEPMediaType) {
			return matchContent((SEPMediaType)candidate);
		}
		return false;
	}
	
	public boolean matchContent (SEPType type) {
		return this.matchTypeValue.equals(type.getValue());
	}
	
	public boolean matchContent (SEPPath path) {
		String xrdPath = trimPath(path.getValue());
		String inputPath = trimPath(this.matchPathValue);
		
		// try verbatim match (caseless)
		if (inputPath.equalsIgnoreCase(xrdPath))
			return true;

		log.trace("xrdPath = '" + xrdPath + "'");
		log.trace("inputPath = '" + inputPath + "'");
		
		XRIAbsolutePath xrdAbsPath = new XRIAbsolutePath("/" + xrdPath);
		XRIAbsolutePath inputAbsPath = new XRIAbsolutePath("/" + inputPath);

		return xrdAbsPath.isPrefixOf(inputAbsPath);
	}
	
	
	public boolean matchContent (SEPMediaType mtype) {
		MimeType candidateMimeType = MimeType.parse(mtype.getValue());
		MimeType critMimeType = MimeType.parse(this.matchMediaTypeValue);
		return critMimeType.equals(candidateMimeType);
	}	


	/**
	 * Removes trailing delimiters '/', '*' or '!'
	 * @param path
	 * @return
	 */
	private String trimPath(String path) {
		StringBuffer sb = new StringBuffer(path.trim());
		
		while (sb.length() > 0) {
			char last = sb.charAt(sb.length() - 1);
			if (last == '/' || last == '*' || last == '!')
				sb.deleteCharAt(sb.length() - 1);
			else
				break;
		}
		return sb.toString();
	}
	

	
	
}