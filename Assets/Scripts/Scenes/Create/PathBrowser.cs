using Scenes.PublicScripts;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using TMPro.EditorUtilities;
using UnityEngine;
using static SimpleFileBrowser.FileBrowser;
public class PathBrowser : PublicButton
{
    public TMP_InputField inputField;

    void Start()
    {
        thisButton.onClick.AddListener(() => 
        {
            ShowLoadDialog(onSuccess: (string[] paths) =>
            {
                inputField.text = paths[0];
            }, onCancel: () => { }, pickMode: PickMode.Files);
        });
    }
}
