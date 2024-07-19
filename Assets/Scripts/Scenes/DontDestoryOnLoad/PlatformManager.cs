using UnityEngine;
using UtilityCode.Singleton;
namespace Scenes.DontDestoryOnLoad
{
    public class PlatformManager : MonoBehaviourSingleton<PlatformManager>
    {
        public string currentPlatformArchiveDataPath;
        public string editorArchiveDataPath;
        public string androidArchiveDataPath;
        public string iPhoneArchiveDataPath;
        private void Start()
        {
            switch (Application.platform)
            {
                case RuntimePlatform.Android:
                    currentPlatformArchiveDataPath = androidArchiveDataPath;
                    break;
                case RuntimePlatform.IPhonePlayer:
                    currentPlatformArchiveDataPath = iPhoneArchiveDataPath;
                    break;
                default:
                    if( !Application.isEditor )
                        return;
                    currentPlatformArchiveDataPath = editorArchiveDataPath;
                    break;
            }
        }
    }
}
