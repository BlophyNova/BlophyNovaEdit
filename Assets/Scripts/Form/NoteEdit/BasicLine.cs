using Data.ChartEdit;
using Manager;
using Scenes.DontDestroyOnLoad;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UtilityCode.Algorithm;

public class BasicLine : MonoBehaviour, IRefresh
{
    public TMP_Text currentBeatsText;//显示节拍线的
    public RectTransform basicLine;//基准线的position
    public RectTransform noteCanvas;//承载音符的画布
    public RectTransform arisePosition;//出现位置
    public BeatLine beatLinePrefab;//节拍先的预制件
    public List<BeatLine> beatLines = new();//节拍线游戏物体管理列表
    public BPM nextBPMWithAriseLine = new();
    public float AriseLineAndBasicLineSeconds => AriseLineAndBasicLinePositionYDelta / 100;//基准线和出现线之间是多少秒
    public float AriseLineAndBasicLinePositionYDelta => Vector2.Distance(basicLine.localPosition, arisePosition.localPosition);//出现线和基准线的Y轴插值
    public float CurrentBasicLine => YScale.Instance.GetPositionYWithSecondsTime((float)ProgressManager.Instance.CurrentTime);
    public float CurrentAriseLine => YScale.Instance.GetPositionYWithSecondsTime((float)ProgressManager.Instance.CurrentTime) + AriseLineAndBasicLineSeconds;
    private void Update()
    {
        //Debug.Log($"AriseLineAndBasicLineSeconds:{AriseLineAndBasicLineSeconds}|AriseLineAndBasicLinePositionYDelta:{AriseLineAndBasicLinePositionYDelta}");

        float currentBeats = BPMManager.Instance.GetCurrentBeatsWithSecondsTime((float)ProgressManager.Instance.CurrentTime);
        currentBeatsText.text = $"{currentBeats:F2}\t";
        noteCanvas.anchoredPosition = CurrentBasicLine * Vector2.down;
        UpdateBeatLines();
    }
    /// <summary>
    /// 在bpm改变的时候应该被调用
    /// </summary>
    public void Refresh()
    {
        nextBPMWithAriseLine = new(BPMManager.Instance.GetBeatsBySeconds((float)ProgressManager.Instance.CurrentTime).integer, 0, 1);
        foreach (var item in beatLines)
        {
            Destroy(item.gameObject);
        }
        beatLines.Clear();
    }
    private void UpdateBeatLines()
    {
        float ariseBeats = BPMManager.Instance.GetCurrentBeatsWithSecondsTime((float)(ProgressManager.Instance.CurrentTime + AriseLineAndBasicLineSeconds * (1 / YScale.Instance.CurrentYScale)));
        //int ariseBeatLineIndex = Algorithm.BinarySearch(BPMManager.Instance.bpmList, m => m.ThisStartBPM < ariseBeats, true);
        while (nextBPMWithAriseLine.ThisStartBPM < ariseBeats)
        {
            BeatLine initBeatLine = Instantiate(beatLinePrefab, noteCanvas.transform).Init(nextBPMWithAriseLine.ThisStartBPM, nextBPMWithAriseLine);
            initBeatLine.transform.SetAsFirstSibling();
            beatLines.Add(initBeatLine);
            nextBPMWithAriseLine.AddOneBeat();
        }
        float currentBeats = BPMManager.Instance.GetCurrentBeatsWithSecondsTime((float)ProgressManager.Instance.CurrentTime);
        for (int i = 0; i < beatLines.Count; i++)
        {
            if (beatLines[i].thisBPM.ThisStartBPM < currentBeats || beatLines[i].thisBPM.ThisStartBPM > ariseBeats)
            {
                BeatLine thisBeatLine = beatLines[i--];

                beatLines.Remove(thisBeatLine);
                Destroy(thisBeatLine.gameObject);
            }
        }
    }
}
