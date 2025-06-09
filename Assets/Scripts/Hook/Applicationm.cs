using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.Networking;
using Object = UnityEngine.Object;

namespace Hook
{
    public class Applicationm : MonoBehaviour
    {
        public static string dataPath => Application.dataPath;
        public static bool isEditor => Application.isEditor;
        public static string streamingAssetsPath
        {
            get
            {
                if (Application.platform != RuntimePlatform.Android)
                {
                    return Application.streamingAssetsPath;
                }

                
                return new Uri($"/storage/emulated/0/Android/data/{Application.identifier}/files/StreamingAssets").LocalPath;
            }
        }

        public static async UniTask Init(string rawPath)
        {
            if (Directory.Exists(rawPath)) return;

            Directory.CreateDirectory(rawPath); 
            await InitStreamingAssetsFileAsync();
        }
        static async UniTask InitStreamingAssetsFileAsync()
        {
            List<string> streamingAssetFiles = (await Resources.LoadAsync("StreamingAssetsManifest") as TextAsset)?.text.Split("\n").ToList();
            streamingAssetFiles!.RemoveAt(streamingAssetFiles.Count - 1);
            foreach (string streamingAssetFile in streamingAssetFiles!)
            {
                //string filePath = new Uri(Path.Combine(Application.streamingAssetsPath, streamingAssetFile)).LocalPath;
                string filePath = Path.Combine(Application.streamingAssetsPath, streamingAssetFile);
                Debug.Log(filePath);
                byte[] binData = (await UnityWebRequest.Get(filePath).SendWebRequest()).downloadHandler.data;
                string path = new Uri($"{streamingAssetsPath}/{streamingAssetFile}").LocalPath;
                Debug.Log(path);
                Directory.CreateDirectory(Path.GetDirectoryName(path)!);
                File.WriteAllBytes(path,binData);
            }
        }
    }
}