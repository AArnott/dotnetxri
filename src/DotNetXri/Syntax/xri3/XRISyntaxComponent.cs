package org.openxri.xri3;

import java.io.Serializable;

public interface XRISyntaxComponent extends Serializable, Cloneable, Comparable {

	public String toURINormalForm();
	public String toIRINormalForm();
}
