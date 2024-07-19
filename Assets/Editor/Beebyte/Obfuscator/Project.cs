/*
 * Copyright (c) 2015-2020 Beebyte Limited. All rights reserved.
 */
#if !BEEBYTE_OBFUSCATOR_DISABLE
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Beebyte.Obfuscator;
using Editor.Beebyte.Obfuscator.Assembly;
using UnityEditor;
using UnityEngine;
namespace Editor.Beebyte.Obfuscator
{
	/**
	 * Handles obfuscation calls for a Unity project and controls restoration of backed up files.
	 */
	public class Project
	{
		private Options options;

		private bool monoBehaviourAssetsNeedReverting = false;
		private bool hasError;
		private bool hasObfuscated;
		private bool noCSharpScripts;
		
		public bool ShouldObfuscate()
		{
			if (options == null) options = OptionsManager.LoadOptions();
			return options.enabled && (options.obfuscateReleaseOnly == false || Debug.isDebugBuild == false);
		}

		public bool IsSuccess()
		{
			return hasObfuscated || !ShouldObfuscate();
		}

		public bool HasCSharpScripts()
		{
			return !noCSharpScripts;
		}

		public bool HasMonoBehaviourAssetsThatNeedReverting()
		{
			return monoBehaviourAssetsNeedReverting;
		}

		public void ObfuscateIfNeeded()
		{
#if UNITY_3_1 || UNITY_3_2 || UNITY_3_3 || UNITY_3_4 || UNITY_3_5 || UNITY_4_0 || UNITY_4_1 || UNITY_4_2
			if (!EditorApplication.isPlayingOrWillChangePlaymode && !_hasObfuscated && _hasError == false)
#else
			if (!EditorApplication.isPlayingOrWillChangePlaymode && !hasObfuscated && hasError == false && BuildPipeline.isBuildingPlayer)
#endif
			{
#if !UNITY_5_6_OR_NEWER
				EditorApplication.update += PipelineHook.ClearProjectViaUpdate;
#endif
				try
				{
					EditorApplication.LockReloadAssemblies();
					ObfuscateWhileLocked();
				}
				catch (Exception e)
				{
					Debug.LogError("Obfuscation Failed: " + e);
					hasError = true;
					throw new OperationCanceledException("Obfuscation failed", e);
				}
				finally
				{
					EditorApplication.UnlockReloadAssemblies();
				}
			}
		}

		private void ObfuscateWhileLocked()
		{
			if (options == null) options = OptionsManager.LoadOptions();

			if (ShouldObfuscate() == false) return;

			AssemblySelector selector = new AssemblySelector(options);

			ICollection<string> compiledDlls = selector.GetCompiledAssemblyPaths();

			if (compiledDlls.Count > 0)
			{
				EditorApplication.update += RestoreUtils.RestoreOriginalDlls;
			}
			
			IDictionary<string, string> backupMap = FileBackup.GetBackupMap(compiledDlls);
			FileBackup.Backup(backupMap);

			ICollection<string> dlls = selector.GetAssemblyPaths();

			if (dlls.Count == 0 && compiledDlls.Count == 0)
			{
				noCSharpScripts = true;
				return;
			}

#if UNITY_2017_3_OR_NEWER
			global::Beebyte.Obfuscator.Obfuscator.AppendReferenceAssemblies(AssemblyReferenceLocator.GetAssemblyReferenceDirectories().ToArray());
#endif
				
#if UNITY_2018_2_OR_NEWER
			global::Beebyte.Obfuscator.Obfuscator.ObfuscateMonoBehavioursByAssetDatabase(false);
			var obfuscateMonoBehaviourNames = options.obfuscateMonoBehaviourClassNames;
			try
			{
				if (IsXCodeProject() && options.obfuscateMonoBehaviourClassNames)
				{
					Debug.LogWarning("MonoBehaviour class names will not be obfuscated when creating Xcode projects");
					options.obfuscateMonoBehaviourClassNames = false;
				}
#endif

				global::Beebyte.Obfuscator.Obfuscator.Obfuscate(dlls, compiledDlls, options, EditorUserBuildSettings.activeBuildTarget);

#if !UNITY_2018_2_OR_NEWER
			if (_options.obfuscateMonoBehaviourClassNames)
			{
				/*
				 * RestoreAssets must be registered via the update delegate because [PostProcessBuild] is not guaranteed to be called
				 */
				EditorApplication.update += RestoreUtils.RestoreMonobehaviourSourceFiles;
				_monoBehaviourAssetsNeedReverting = true;
			}
#else
			}
			finally
			{
				options.obfuscateMonoBehaviourClassNames = obfuscateMonoBehaviourNames;
			}
#endif
			hasObfuscated = true;
		}

#if UNITY_2018_2_OR_NEWER
		private bool IsXCodeProject()
		{
			return EditorUserBuildSettings.activeBuildTarget == BuildTarget.StandaloneOSX &&
			       EditorUserBuildSettings.GetPlatformSettings("OSXUniversal", "CreateXcodeProject").Equals("true");
		}
#endif

		public void ObfuscateAssets(BuildTarget buildTarget, string pathToBuildProject)
		{
#if UNITY_2018_2_OR_NEWER
			if (IsXCodeProject()) return;
			if (options == null) options = OptionsManager.LoadOptions();
			if (options.obfuscateMonoBehaviourClassNames && File.Exists("_AssetTranslations"))
			{
				string pathToGlobalGameManagersAsset = GlobalGameManagersPath.GetPathToGlobalGameManagersAsset(buildTarget, pathToBuildProject);
				global::Beebyte.Obfuscator.Obfuscator.RenameScriptableAssets("_AssetTranslations", pathToGlobalGameManagersAsset);
			}
#endif
		}
	}
}
#endif
