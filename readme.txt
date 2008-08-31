This is the future home of DotNetXri.

To customize it for a library:
1. Find & Replace in Files with case sensitive search: 
	DotNetXri -> YourLibrary
2. Do a dir /s *DotNetXri* in the root of the project and rename all files/directories to *YourLibrary*.
	 dir -rec . *DotNetXri* |% { ren $_.fullname $_.name.replace("DotNetXri", "YourLibrary") }