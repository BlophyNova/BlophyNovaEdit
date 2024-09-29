/*
 * Copyright (c) 2016 Beebyte Limited. All rights reserved. 
 */
using System;

namespace Beebyte.Obfuscator
{
	[AttributeUsage(AttributeTargets.Method | AttributeTargets.Constructor | AttributeTargets.Class)]
	public class ObfuscateLiteralsAttribute : System.Attribute
	{
		public ObfuscateLiteralsAttribute()
		{
		}
	}
}
