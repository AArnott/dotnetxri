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
namespace DotNetXri.Client.Resolve {

	//using java.util.Iterator;
	//using java.util.ArrayList;
	//using org.openxri.xml.XRD;
	using System.Collections;


	/*
	********************************************************************************
	* Class: ResolveInfo
	********************************************************************************
	*/
	/**
 * Holds information for a XRI resolve request.
 * @author =chetan
 */
	public class ResolveInfo {
		// ordered vector of descriptors used/obtained during resolve
		private ArrayList moResolveChains = new ArrayList();

		/*
		****************************************************************************
		* Constructor()
		****************************************************************************
		*/
		/**
	 * Constructor - initializes as unresolved.
	 */
		public ResolveInfo(string sXRI) {
			ResolveChain oChain = new ResolveChain(sXRI);
			moResolveChains.Add(oChain);

		} // Constructor()

		/*
		****************************************************************************
		* getXRI()
		****************************************************************************
		*/
		/**
	 * Get the XRI attempted to be resolved
	 */
		public string getXRI() {
			return ((ResolveChain)moResolveChains[0]).getXRI();

		} // getXRI()

		/*
		****************************************************************************
		* resolvedAll()
		****************************************************************************
		*/
		/**
	 * Was the XRI resolved to its completion?
	 */
		public bool resolvedAll() {
			return getLastChain().resolvedAll();

		} // resolvedAll()

		/*
		****************************************************************************
		* addChain()
		****************************************************************************
		*/
		/**
	 * Add the ResolveChain to the ResolvedInfo
	 */
		public void addChain(ResolveChain oChain) {
			moResolveChains.Add(oChain);

		} // addChain()

		/*
		****************************************************************************
		* getFinalXRIDescriptor()
		****************************************************************************
		*/
		/**
	 * Get the last descriptor encountered during resolve.
	 *
	 * For a complete resolve, that is when resolvedAll() is true, the
	 * final descriptor belongs to the complete XRI.
	 *
	 * For an incomplete resolve, this descriptor, if non-null, gives
	 * the descriptor that was queried last for a resolve
	 * getUnresolved() gives the remaining part of XRI that is still
	 * left to be resolved.
	 */
		public XRD getFinalXRIDescriptor() {
			return getLastChain().getFinalXRIDescriptor();

		} // getFinalXRIDescriptor()

		/*
		****************************************************************************
		* getUnresolved()
		****************************************************************************
		*/
		/**
	 * Get the unresolved portion.
	 * This portion does not start with "xri".
	 * Example: Could be ".yahoo.john" for "xri:@email.com.yahoo.john".
	 * Null is returned for complete resolutions.
	 */
		public string getUnresolved() {
			return getLastChain().getUnresolved();

		} // getUnresolved()

		/*
		****************************************************************************
		* getFirstChain()
		****************************************************************************
		*/
		/**
	 * Get the first chain that was being resolved
	 */
		public ResolveChain getFirstChain() {
			return (ResolveChain)moResolveChains[0];

		} // getFirstChain()

		/*
		****************************************************************************
		* getLastChain()
		****************************************************************************
		*/
		/**
	 * Get the last chain that was being resolved
	 */
		public ResolveChain getLastChain() {
			return (ResolveChain)moResolveChains[moResolveChains.Count - 1];

		} // getLastChain()

		/*
		****************************************************************************
		* getNumChains()
		****************************************************************************
		*/
		/**
	 * Get the number of chains during resolution
	 */
		public int getNumChains() {
			return moResolveChains.Count;

		} // getNumChains()

		/*
		****************************************************************************
		* getChainIterator()
		****************************************************************************
		*/
		/**
	 * Get the number of chains during resolution
	 */
		public IEnumerator getChainIterator() {
			return moResolveChains.GetEnumerator();

		} // getChainIterator()

	} // Class: ResolveInfo
}