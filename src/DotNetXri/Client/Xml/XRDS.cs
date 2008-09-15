/*
* Copyright 2005 OpenXRI Foundation
* Subsequently ported and altered by Andrew Arnott
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
using System.Collections;
using System.Xml;
namespace DotNetXri.Client.Xml {

//using java.io.Serializable;
//using java.net.UriFormatException;
//using java.text.ParseException;
//using java.util.ArrayList;

//using org.apache.xerces.dom.XmlDocument;
//using org.openxri.util.DOMUtils;
//using org.w3c.dom.XmlDocument;
//using org.w3c.dom.XmlElement;
//using org.w3c.dom.NamedNodeMap;
//using org.w3c.dom.Node;


/*
********************************************************************************
* Class: XRDS
********************************************************************************
*/ /**
* This class describes the XRDS XML element used for XRI Authority
* resolution.
* @author =chetan
*/
public class XRDS : Serializable
{
	private ArrayList moXRDs = new ArrayList();
	
	private string _ref = null;
	private string redirect = null;
	
	/*
	****************************************************************************
	* Constructor()
	****************************************************************************
	*/ /**
	* Contructs an emtpy XRDS element
	*/
	public XRDS() {} // Constructor()

	/*
	****************************************************************************
	* Constructor()
	****************************************************************************
	*/ /**
	*  This creates the obj from DOM and optionally preserves the DOM of
	* the XRD child elements.
	* @param bKeepXRIDDOMs - Whether or not the XRDS should keep
	* a copy of their DOM after construction.
	*/
	public XRDS(XmlElement oElem, bool bKeepXRIDDOMs)
		//throws UriFormatException, ParseException
	{
		fromDOM(oElem, bKeepXRIDDOMs);

	} // Constructor()

	/*
	****************************************************************************
	* add()
	****************************************************************************
	*/ /**
	* Adds an XRD element
	*/
	public void add(XRD oDescriptor)
	{
		moXRDs.Add(oDescriptor);

	} // add()

	/*
	****************************************************************************
	* add()
	****************************************************************************
	*/ /**
	* Adds a nested XRDS element
	*/
	public void add(XRDS oDescriptor)
	{
		moXRDs.Add(oDescriptor);

	} // add()

	
	public void addAll(XRDS other) {
		for (int i = 0; i < other.getNumChildren(); i++) {
			if (other.isXRDSAt(i))
				add(other.getXRDSAt(i));
			else
				add(other.getDescriptorAt(i));
		}
	}
	
	/*
	****************************************************************************
	* getNumDescriptors()
	****************************************************************************
	*/ /**
	* Returns the number of XRDs + XRDSs
	*/
	public int getNumChildren()
	{
		return (moXRDs == null) ? 0 : moXRDs.Count;

	} // getNumDescriptors()

	/**
	* Gets the number or <code>XRD</code> elements among its children
	* 
	*/
	public int getNumXRD()
	{
		int count = 0;
		for (int i = 0; i < getNumChildren(); i++) {
			if (isXRDSAt(i))
				continue;
			count++;
		}
		return count;
	}
	
	/**
	* Gets the number of <code>XRDS</code> elements among its children.
	* This indicates the number of Refs followed.
	*/
	public int getNumXRDS()
	{
		return getNumChildren() - getNumXRD();
	}
	
	/*
	****************************************************************************
	* getDescriptorAt()
	****************************************************************************
	*/ /**
	* Returns the XRD at the given index
	*/
	public XRD getDescriptorAt(int n)
	{
			if (n >= getNumChildren()){
			return null;
		}
		Object o = moXRDs[n];
		if(o is XRD) return (XRD)o;
		return null;

	} // getDescriptorAt()

	/*
	****************************************************************************
	* getDescriptorAt()
	****************************************************************************
	*/ /**
	* Returns the XRD at the given index
	*/
	public XRDS getXRDSAt(int n)
	{
		if (n >= getNumChildren()){
			return null;
		}
		Object o = moXRDs[n];
		if(o is XRDS) return (XRDS)o;
		return null;
	} // getDescriptorAt()

	public bool isXRDSAt(int n) {
		Object o = moXRDs[n];
		
		if ( o == null) return false;
		
		if (o is XRDS)
			return true;
		return false;
	}
	
	public bool isXRDAt(int n) {
		Object o = moXRDs[n];
		
		if ( o == null) return false;
		
		if (o is XRD)
			return true;
		return false;
	}
	/*
	****************************************************************************
	* reset()
	****************************************************************************
	*/ /**
	* This method resets the state of the obj.
	*/
	public void reset()
	{
		moXRDs = new ArrayList();

	} // reset()

	/*
	****************************************************************************
	* fromDOM()
	****************************************************************************
	*/ /**
	* This populates the obj from DOM and optionally preserves the DOM of
	* the XRD child elements.
	* @param bKeepXRIDDOMs - Whether or not the XRDS should keep
	* a copy of their DOM after construction.
	*/
	public void fromDOM(XmlElement oElem, bool bKeepXRIDDOMs)
		//throws ParseException, UriFormatException
	{
		reset();
		if (oElem != null){
			NamedNodeMap attribs = oElem.getAttributes();
			XmlNode attribNode = attribs.getNamedItem(Tags.ATTR_REF);
			if (attribNode != null)
				_ref = attribNode.getNodeValue();

			attribNode = attribs.getNamedItem(Tags.ATTR_REDIRECT);
			if (attribNode != null)
				redirect = attribNode.getNodeValue();
		}
		XmlNode oChild = oElem.FirstChild;

		for (; oChild != null; oChild = oChild.NextSibling)
		{
			if (oChild.LocalName.Equals(Tags.TAG_XRD) )
			{
					moXRDs.add(
						new XRD((XmlElement) oChild, bKeepXRIDDOMs));
			}else if (oChild.LocalName.Equals(Tags.TAG_XRDS) ){
				moXRDs.add(
							new XRDS((XmlElement) oChild, bKeepXRIDDOMs));
			}
		}

	} // fromDOM()

	/*
	****************************************************************************
	* toDOM()
	****************************************************************************
	*/ /**
	*  This method will make DOM using the specified document.  If any DOM state
	* has been stored with the obj, it will not be used in this method.
	* This method generates a reference-free copy of new DOM.
	* @param oDoc - The document to use for generating DOM
	*/
	public XmlElement toDOM(XmlDocument oDoc)
	{
		// for this particular toDOM implementation, oDoc must not be null
		if (oDoc == null)
		{
			return null;
		}

		XmlElement oElem = oDoc.createElementNS(Tags.NS_XRDS, Tags.TAG_XRDS);

		if (_ref != null){
			oElem.SetAttribute(Tags.ATTR_REF, _ref);
		}
		
		if (redirect != null){
			oElem.SetAttribute(Tags.ATTR_REDIRECT, redirect);
		}
		
		for (int i = 0; i < getNumChildren(); i++)
		{
			
			XmlElement oLocal = null;
			if (isXRDSAt(i)) 
				oLocal = getXRDSAt(i).toDOM(oDoc) ;     
			else if(isXRDAt(i))
				oLocal = getDescriptorAt(i).toDOM(oDoc);
			if (oLocal != null)
				oElem.AppendChild(oLocal);
		}

		return oElem;

	} // toDOM()

	/*
	****************************************************************************
	* clearDOM()
	****************************************************************************
	*/ /**
	* Clears any DOM that has been stored with this obj
	*/
	public void clearDOM()
	{
		for (int i = 0; i < getNumChildren(); i++)
		{
			getDescriptorAt(i).clearDOM();
		}

	} // clearDOM()

	/*
	****************************************************************************
	* serializeDescriptorDOM()
	****************************************************************************
	*/ /**
	*  Serializes the obj based on calling serializeDOM(false, true) on each
	* of the child XRDS
	*/
	public string serializeDescriptorDOM()
	{
		return ToString();

	} // serializeDescriptorDOM()

	/* 
	****************************************************************************
	* ToString()
	****************************************************************************
	*/ /**
	* Returns formatted obj.  Do not use if signature needs to be preserved.
	*/
	public override string ToString()
	{
		return dump();
		

	} // ToString()

	/**
	* Returns obj as a formatted XML string.
	* @param sTab - The characters to prepend before each new line
	*/
	public string dump()
	{
		XmlDocument doc = new XmlDocument();
		XmlElement elm = this.toDOM(doc);
		doc.AppendChild(elm);
		return DOMUtils.ToString(doc);

	}

	public XRD getFinalXRD()
	{
		int last = getNumChildren() - 1;
		if (last < 0)
			return null;
		
		if (isXRDSAt(last))
			return getXRDSAt(last).getFinalXRD(); // recursive call
		else
			return getDescriptorAt(last);
	}

	public XRDS getFinalXRDS()
	{
		int last = getNumChildren() - 1;
		if (last < 0)
			return null;
		
		if (isXRDSAt(last))
			return getXRDSAt(last).getFinalXRDS(); // recursive call
		else
			return this;
	}

	public void replaceFinalXRD(XRDS children)
	{
		XRDS finalXRDS = getFinalXRDS();
		finalXRDS.removeChildAt(finalXRDS.getNumChildren() - 1);
		finalXRDS.addAll(children);
	}
	
	protected void removeChildAt(int i)
	{
		if (i < moXRDs.Count)
			moXRDs.remove(i);
	}
	
	/**
	* @return Returns the _ref.
	*/
	public string getRef() {
		return _ref;
	}

	/**
	* @param _ref The _ref to set.
	*/
	public void setRef(string _ref) {
		this._ref = _ref;
	}
	
	/**
	* @return Returns the redirect attribute.
	*/
	public string getRedirect() {
		return redirect;
	}

	/**
	* @param _ref The redirect attribute value to set.
	*/
	public void setRedirect(string redirect) {
		this.redirect = redirect;
	}
	
}
}