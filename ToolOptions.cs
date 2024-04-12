using System;
using System.Collections.Generic;

namespace TestTool;

sealed class ToolOptions {

	[Option("-f", IsRequired = true, Description = "target assembly file path")]
	public string AssemblyFilePath { get; set; }
}

