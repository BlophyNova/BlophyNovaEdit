using System;
using System.IO;
using System.Text;
using Hook;
using UnityEngine;

namespace AssetsSync
{
    public class StreamingAssetsSync : MonoBehaviour
    {
        private void Start()
        {
            if (!Applicationm.isEditor)
            {
                return;
            }
#if UNITY_EDITOR
            string path = new Uri($"{Applicationm.dataPath}/Resources/StreamingAssetsManifest.txt").LocalPath;
            string streamingAssetsPath = Applicationm.streamingAssetsPath;
            StringBuilder files = new();

            SearchFiles(streamingAssetsPath);

            File.WriteAllText(path, files.ToString(), Encoding.UTF8);


            void SearchFiles(string folder)
            {
                foreach (string file in Directory.GetFiles(folder))
                {
                    if (!file.EndsWith(".meta") && !file.EndsWith(".DS_Store"))
                    {
                        //.DS_Store能不能死一死啊,UI上看不见不代表代码看不见
                        files.AppendLine(file.Replace(streamingAssetsPath + Path.DirectorySeparatorChar, ""));
                    }
                }

                foreach (string dir in Directory.GetDirectories(folder))
                {
                    SearchFiles(dir);
                }
            }
#endif
        }
    }
}