using System.Collections.Generic;
using Data.ChartEdit;
using Data.Interface;
using Form.PropertyEdit;
using Manager;
using Scenes.DontDestroyOnLoad;
using TMPro;
using UnityEngine;
using UtilityCode.ObjectPool;

namespace Form.NoteEdit
{
    public class BasicLine : MonoBehaviour, IRefreshUI
    {
        public TMP_Text currentBeatsText; //显示节拍线的
        public RectTransform basicLine; //基准线的position
        public RectTransform noteCanvas; //承载音符的画布
        public RectTransform arisePosition; //出现位置
        public BeatLine beatLinePrefab; //节拍先的预制件
        public List<BeatLine> beatLines = new(); //节拍线游戏物体管理列表
        public BPM nextBPMWithAriseLine = new();

        //没错，第二波优化我要上对象池了
        private ObjectPoolQueue<BeatLine> beatLineObjectPoolQueue;

        public float AriseLineAndBasicLineSeconds => AriseLineAndBasicLinePositionYDelta / 100; //基准线和出现线之间是多少秒

        public float AriseLineAndBasicLinePositionYDelta =>
            Vector2.Distance(basicLine.localPosition, arisePosition.localPosition); //出现线和基准线的Y轴插值

        public float CurrentBasicLine =>
            YScale.Instance.GetPositionYWithSecondsTime((float)ProgressManager.Instance.CurrentTime);

        public float CurrentAriseLine =>
            YScale.Instance.GetPositionYWithSecondsTime((float)ProgressManager.Instance.CurrentTime) +
            AriseLineAndBasicLineSeconds;

        private void Start()
        {
            beatLineObjectPoolQueue = new ObjectPoolQueue<BeatLine>(beatLinePrefab, 0, noteCanvas.transform);
        }

        private void Update()
        {
            //Debug.Log($"AriseLineAndBasicLineSeconds:{AriseLineAndBasicLineSeconds}|AriseLineAndBasicLinePositionYDelta:{AriseLineAndBasicLinePositionYDelta}");

            float currentBeats =
                BPMManager.Instance.GetCurrentBeatsWithSecondsTime((float)ProgressManager.Instance.CurrentTime);
            currentBeatsText.text = $"{currentBeats:F2}\t";
            noteCanvas.anchoredPosition = CurrentBasicLine * Vector2.down;
            UpdateBeatLines();
        }

        public void RefreshUI()
        {
            Refresh();
        }

        /// <summary>
        ///     在bpm改变的时候应该被调用
        /// </summary>
        public void Refresh()
        {
            nextBPMWithAriseLine =
                new BPM(
                    (int)BPMManager.Instance.GetCurrentBeatsWithSecondsTime((float)ProgressManager.Instance
                        .CurrentTime), 0,
                    1);
            //BPMManager.Instance.GetCurrentBeatsWithSecondsTime((float)ProgressManager.Instance.CurrentTime)
            if (nextBPMWithAriseLine.ThisStartBPM < 0)
            {
                nextBPMWithAriseLine = BPM.Zero;
            }

            foreach (BeatLine item in beatLines)
            {
                beatLineObjectPoolQueue.ReturnObject(item);
            }

            beatLines.Clear();
            Update();
        }

        private void UpdateBeatLines()
        {
            float ariseBeats = BPMManager.Instance.GetCurrentBeatsWithSecondsTime(
                (float)(ProgressManager.Instance.CurrentTime +
                        AriseLineAndBasicLineSeconds * (1 / YScale.Instance.CurrentYScale)));
            float currentBeats =
                BPMManager.Instance.GetCurrentBeatsWithSecondsTime((float)ProgressManager.Instance.CurrentTime);
            //int ariseBeatLineIndex = Algorithm.BinarySearch(BPMManager.Instance.bpmList, m => m.ThisStartBPM < ariseBeats, true);
            while (nextBPMWithAriseLine.ThisStartBPM < ariseBeats)
            {
                BeatLine initBeatLine = beatLineObjectPoolQueue.PrepareObject()
                    .Init(nextBPMWithAriseLine.ThisStartBPM, nextBPMWithAriseLine);
                initBeatLine.transform.SetAsFirstSibling();
                beatLines.Add(initBeatLine);
                nextBPMWithAriseLine.AddOneBeat(GlobalData.Instance.chartEditData.beatSubdivision);
            }

            for (int i = 0; i < beatLines.Count; i++)
            {
                if (beatLines[i].thisBPM.ThisStartBPM < currentBeats || beatLines[i].thisBPM.ThisStartBPM > ariseBeats)
                {
                    BeatLine thisBeatLine = beatLines[i--];

                    beatLines.Remove(thisBeatLine);
                    beatLineObjectPoolQueue.ReturnObject(thisBeatLine);
                }
            }
        }
    }
}