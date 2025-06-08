using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

namespace Hook
{
    public class Applicationm : MonoBehaviour
    {
        public static string streamingAssetsPath
        {
            get
            {
                if (Application.platform == RuntimePlatform.Android)
                {
                    string rawPath = new Uri($"/storage/emulated/0/Android/data/{Application.identifier}/files/StreamingAssets").LocalPath;
                    if (!Directory.Exists(rawPath))
                    {
                        Directory.CreateDirectory(rawPath);
                    }

                    return rawPath;
                }
                return Application.streamingAssetsPath;
            }
        }
    }
}