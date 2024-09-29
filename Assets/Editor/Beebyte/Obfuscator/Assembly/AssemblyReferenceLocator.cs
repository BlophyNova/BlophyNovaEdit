#if UNITY_2017_3_OR_NEWER
/*
 * Copyright (c) 2018-2021 Beebyte Limited. All rights reserved.
 */
using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEditor.Compilation;

namespace Beebyte.Obfuscator.Assembly
{
	public class AssemblyReferenceLocator
	{
		public static IEnumerable<string> GetAssemblyReferenceDirectories()
		{
			HashSet<string> directories = new HashSet<string>();

			foreach (UnityEditor.Compilation.Assembly assembly in CompilationPipeline.GetAssemblies())
			{
				directories.UnionWith(GetAssemblyReferenceDirectories(assembly));
			}
			directories.Add("./Library/PlayerScriptAssemblies".Replace('/', Path.DirectorySeparatorChar));
			directories.UnionWith(GetUnityEditorLibDirectories());
			return directories;
		}

		private static IEnumerable<string> GetUnityEditorLibDirectories()
		{
			HashSet<string> libDirectories = new HashSet<string>();
			Stack<string> stack = new Stack<string>();
			var monoPath = EditorApplication.applicationContentsPath +
			               Path.DirectorySeparatorChar.ToString() +
			               "MonoBleedingEdge" +
			               Path.DirectorySeparatorChar.ToString() +
			               "lib" +
			               Path.DirectorySeparatorChar.ToString() +
			               "mono";
			stack.Push(monoPath);

			while (stack.Count > 0)
			{
				string dir = stack.Pop();
				if (Directory.GetFiles(dir, "*.dll").Length > 0)
				{
					libDirectories.Add(dir);
				}
				foreach (var childDirectory in Directory.GetDirectories(dir))
				{
					stack.Push(childDirectory);
				}
			}
			return libDirectories;
		}

		private static IEnumerable<string> GetAssemblyReferenceDirectories(UnityEditor.Compilation.Assembly assembly)
		{
			HashSet<string> directories = new HashSet<string>();

			if (assembly == null) return directories;

			if (assembly.compiledAssemblyReferences == null || assembly.compiledAssemblyReferences.Length <= 0)
			{
				return directories;
			}
			
			foreach (string assemblyRef in assembly.compiledAssemblyReferences)
			{
				directories.Add(Path.GetDirectoryName(assemblyRef));
			}
			return directories;
		}
	}
}
#endif
