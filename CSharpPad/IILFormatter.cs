﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Gobiner.CSharpPad
{
	public interface IILFormatter
	{
		string[] Format(IDictionary<Type, TypeMethodInfo> ILLookup);
	}
}