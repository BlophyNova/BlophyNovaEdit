using System.Collections;
using Scenes.PublicScripts;
using TMPro;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;
using static SimpleFileBrowser.FileBrowser;

namespace Scenes.Select
{
    public class PathBrowser : PublicButton
    {
        public TMP_InputField inputField;
        public Image image;

        private void Start()
        {
            thisButton.onClick.AddListener(() =>
            {
                ShowLoadDialog(paths =>
                {
                    inputField.text = paths[0];
                    if (image == null)
                    {
                        return;
                    }

                    StartCoroutine(GetIllustration(paths[0]));
                    image.color = Color.white;
                    image.type = Image.Type.Simple;
                    image.preserveAspect = true;
                }, () => { }, PickMode.Files
#if PLATFORM_ANDROID
                ,initialPath:"/storage/emulated/0"
#endif
                    );
            });
        }

        private IEnumerator GetIllustration(string path)
        {
            UnityWebRequest unityWebRequest = UnityWebRequestTexture.GetTexture($"file://{path}");
            yield return unityWebRequest.SendWebRequest();
            Texture2D cph = DownloadHandlerTexture.GetContent(unityWebRequest);
            image.sprite = Sprite.Create(cph, new Rect(0, 0, cph.width, cph.height), new Vector2(0.5f, 0.5f));
        }
    }
}