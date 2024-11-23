using Scenes.PublicScripts;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;
using static SimpleFileBrowser.FileBrowser;
public class PathBrowser : PublicButton
{
    public TMP_InputField inputField;
    public Image image;
    void Start()
    {
        thisButton.onClick.AddListener(() =>
        {
            ShowLoadDialog(onSuccess: (string[] paths) =>
            {
                inputField.text = paths[0];
                if (image == null) return;
                StartCoroutine(GetIllustration(paths[0]));
                image.color = Color.white;
                image.type = Image.Type.Simple;
                image.preserveAspect = true;
            }, onCancel: () => { }, pickMode: PickMode.Files);
        });
    }
    IEnumerator GetIllustration(string path)
    {
        UnityWebRequest unityWebRequest = UnityWebRequestTexture.GetTexture($"file://{path}");
        yield return unityWebRequest.SendWebRequest();
        Texture2D cph = DownloadHandlerTexture.GetContent(unityWebRequest);
        image.sprite = Sprite.Create(cph, new Rect(0, 0, cph.width, cph.height), new Vector2(0.5f, 0.5f));
    }
}
