using Manager;
using Scenes.DontDestroyOnLoad;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class PlaySpeed : MonoBehaviour
{
    public TMP_InputField inputField;
    public Button ok;
    private void Start()
    {
        ok.onClick.AddListener(() =>
        {
            if (!float.TryParse(inputField.text, out float playSpeed)) return;
            double currentTime = ProgressManager.Instance.CurrentTime;
            ProgressManager.Instance.SetPlaySpeed(playSpeed);
            ProgressManager.Instance.SetTime(currentTime);

            GlobalData.Refresh<IRefresh>((interfaceMethod) => interfaceMethod.Refresh());
        });
    }
}
