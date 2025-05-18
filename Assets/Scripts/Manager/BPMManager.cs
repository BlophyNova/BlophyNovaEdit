using System.Collections.Generic;
using Scenes.DontDestroyOnLoad;
using UtilityCode.Singleton;
using BPM = Data.ChartEdit.BPM;

namespace Manager
{
    public class BPMManager : MonoBehaviourSingleton<BPMManager>
    {
        public List<BPM> bpmList => GlobalData.Instance.chartEditData.bpmList;

        /// <summary>
        ///     每秒几拍
        /// </summary>
        public float CurrentBeatsPerSecond => bpmList[CurrentBPMListIndex].currentBPM / 60;

        /// <summary>
        ///     每分钟几拍
        /// </summary>
        public float thisCurrentTotalBPM => bpmList[CurrentBPMListIndex].currentBPM;

        public int CurrentBPMListIndex
        {
            get
            {
                float lastBPMStartTime = 0;
                int index = 0;
                for (; index < bpmList.Count; index++)
                {
                    if (index + 1 >= bpmList.Count)
                    {
                        break;
                    }

                    float currentBPMHoldTime = (bpmList[index + 1].ThisStartBPM - bpmList[index].ThisStartBPM) /
                                               (bpmList[index].currentBPM / 60);
                    if (currentBPMHoldTime + lastBPMStartTime <= ProgressManager.Instance.CurrentTime)
                    {
                        lastBPMStartTime += currentBPMHoldTime;
                    }
                    else
                    {
                        break;
                    }
                }

                return index;
            }
        }

        /// <summary>
        ///     根据时间获取当前的beats
        /// </summary>
        /// <param name="secondsTime"></param>
        /// <returns></returns>
        public float GetCurrentBeatsWithSecondsTime(float secondsTime)
        {
            float result =
                bpmList[GetBPMListIndexWithSecondsTime(secondsTime)].currentBPM / 60 *
                (secondsTime - GetLastBPMStartTimeWithSecondsTime(secondsTime)) +
                bpmList[GetBPMListIndexWithSecondsTime(secondsTime)].ThisStartBPM;
            return result;
        }

        //public BPM GetBPMWithSecondsTime(float SecondsTime)
        //{
        //    int BPMSecondsTime = (int)GetCurrentBeatsWithSecondsTime(SecondsTime);
        //    BPM bpm = new()
        //    {
        //        integer = BPMSecondsTime,
        //        molecule = 0,
        //        denominator = int.Parse(HorizontalLine.Instance.thisText.text) + 1
        //    };
        //    return bpm;
        //}
        /// <summary>
        ///     根据时间获取当前bpm在bpmlist中的索引
        /// </summary>
        /// <param name="secondsTime"></param>
        /// <returns></returns>
        public int GetBPMListIndexWithSecondsTime(float secondsTime)
        {
            float lastBPMStartTime = 0;
            int index = 0;
            for (; index < bpmList.Count; index++)
            {
                if (index + 1 >= bpmList.Count)
                {
                    break;
                }

                float currentBPMHoldTime = (bpmList[index + 1].ThisStartBPM - bpmList[index].ThisStartBPM) /
                                           (bpmList[index].currentBPM / 60);
                if (currentBPMHoldTime + lastBPMStartTime <= secondsTime)
                {
                    lastBPMStartTime += currentBPMHoldTime;
                }
                else
                {
                    break;
                }
            }

            return index;
        }

        /// <summary>
        ///     根据当前时间，获取上一个bpm的开始节拍
        /// </summary>
        /// <param name="secondsTime"></param>
        /// <returns></returns>
        public float GetLastBPMStartTimeWithSecondsTime(float secondsTime)
        {
            float lastBPMStartTime = 0;
            int index = 0;
            for (; index < bpmList.Count; index++)
            {
                if (index + 1 >= bpmList.Count)
                {
                    break;
                }

                float currentBPMHoldTime = (bpmList[index + 1].ThisStartBPM - bpmList[index].ThisStartBPM) /
                                           (bpmList[index].currentBPM / 60);
                if (currentBPMHoldTime + lastBPMStartTime <= secondsTime)
                {
                    lastBPMStartTime += currentBPMHoldTime;
                }
                else
                {
                    break;
                }
            }

            return lastBPMStartTime;
        }

        public BPM GetBeatsBySeconds(float seconds)
        {
            BPM bpm = new();
            seconds -= GetSecondsTimeByBeats(bpmList[^1].ThisStartBPM);
            float resBeats = bpmList[^1].currentBPM / 60 * seconds;
            bpm.integer = (int)resBeats;
            bpm.denominator = 1000;
            bpm.molecule = (int)((resBeats - (int)resBeats) * 1000);
            bpm.currentBPM = bpmList[^1].currentBPM;
            return bpm;
        }

        /// <summary>
        ///     根据beats获取当前时间
        /// </summary>
        /// <param name="beats"></param>
        /// <returns></returns>
        public float GetSecondsTimeByBeats(float beats)
        {
            float secondsTime = 0;
            int index = -1;
            for (int i = 0; i < bpmList.Count; i++)
            {
                if (bpmList[i].ThisStartBPM <= beats)
                {
                    index++;
                }
            }

            for (int i = 0; i < index; i++)
            {
                secondsTime += (bpmList[i + 1].ThisStartBPM - bpmList[i].ThisStartBPM) /
                               GetBeatsPerSecondWithBPMEvent(bpmList[i]);
            }

            secondsTime += (beats - bpmList[index].ThisStartBPM) / GetBeatsPerSecondWithBPMEvent(bpmList[index]);
            return secondsTime;
        }

        /// <summary>
        ///     获取指定bpm的每秒几拍
        /// </summary>
        /// <param name="bpm"></param>
        /// <returns></returns>
        public float GetBeatsPerSecondWithBPMEvent(BPM bpm)
        {
            return bpm.currentBPM / 60;
        }

        /// <summary>
        ///     根据beats获取这个beats在bpmlist中的索引
        /// </summary>
        /// <param name="beats"></param>
        /// <returns></returns>
        public int GetBPMListIndexWithBeats(float beats)
        {
            int BPMListIndex = 0;
            for (int i = 0; i < bpmList.Count; i++)
            {
                if (bpmList[i].ThisStartBPM < beats)
                {
                    BPMListIndex++;
                }
                else
                {
                    break;
                }
            }

            return BPMListIndex;
        }
    }
}