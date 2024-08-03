using Data.ChartEdit;
using Scenes.DontDestoryOnLoad;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class BPMItem : MonoBehaviour
{
    public BPM myBPM;
    public TMP_InputField bpmValue;
    public TMP_InputField startBeats;
    public Button deleteBPM;
    private void Start()
    {
        bpmValue.onEndEdit.AddListener((v) => 
        {
            myBPM.currentBPM = int.Parse(bpmValue.text);

            GlobalData.Refresh<IRefresh>((interfaceMethod) => interfaceMethod.Refresh());
        });
        startBeats.onEndEdit.AddListener((v) =>
        {
            Match match = Regex.Match(v, @"(\d+):(\d+)/(\d+)");
            if (match.Success)
            {
                myBPM.integer = int.Parse(match.Groups[1].Value);
                myBPM.molecule = int.Parse(match.Groups[2].Value);
                myBPM.denominator = int.Parse(match.Groups[3].Value);

                GlobalData.Refresh<IRefresh>((interfaceMethod) => interfaceMethod.Refresh());
            }
        });
        deleteBPM.onClick.AddListener(() => 
        {
            if (BPMManager.Instance.bpmList.Count > 1)
            {
                BPMManager.Instance.bpmList.Remove(myBPM);

                GlobalData.Refresh<IRefresh>((interfaceMethod) => interfaceMethod.Refresh());
                Destroy(gameObject);
            }
            else
            {
                Alert.EnableAlert("呜呜呜，咱保留至少一个bpm可以嘛？");
            }
        });
    }
}
