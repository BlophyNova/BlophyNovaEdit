using Data.ChartEdit;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
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
        bpmValue.onValueChanged.AddListener((v) => 
        {
            myBPM.currentBPM = int.Parse(bpmValue.text);
            AssemblySystem.Exe(AssemblySystem.FindAllInterfaceByTypes<IRefresh>(), (interfaceMethod)=>interfaceMethod.Refresh());
        });
    }
}
