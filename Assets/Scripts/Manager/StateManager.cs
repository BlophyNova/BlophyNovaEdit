using Scenes.DontDestroyOnLoad;
using UnityEngine;
using UtilityCode.Singleton;
namespace Manager
{
    public class StateManager : MonoBehaviourSingleton<StateManager>
    {
        private bool isStart;//已经开始
        private bool isEnd;//已经结束
        private bool isPause;//已经暂停
        public bool IsStart
        {
            get => isStart;
            set
            {
                if (isStart) return;//如果已经开始了就直接返回
                isStart = value;//设置状态为开始
                //AssetManager.Instance.musicPlayer.PlayScheduled(AssetManager.Instance.chartData.globalData.offset + GlobalData.Instance.offset);//播放音乐，带上延迟
                ProgressManager.Instance.StartPlay(GlobalData.Instance.chartEditData.offset);//谱面开始播放
                AssetManager.Instance.box.gameObject.SetActive(true);//激活所有方框

            }
        }
        public bool IsEnd
        {
            get => isEnd;
            set => isEnd = value;
        }
        public bool IsPause
        {
            get => isPause;
            set
            {
                isPause = value;
                switch (value)
                {
                    case true:
                        ProgressManager.Instance.PausePlay();
                        break;
                    case false:
                        ProgressManager.Instance.ContinuePlay();
                        //double currentTime = ProgressManager.Instance.CurrentTime;
                        //ProgressManager.Instance.ResetTime();
                        //ProgressManager.Instance.StartPlay();
                        //ProgressManager.Instance.Offset = GlobalData.Instance.chartEditData.offset;
                        //ProgressManager.Instance.SetTime(currentTime);
                        break;
                }
            }
        }
        public bool IsPlaying => IsStart && !IsPause && !IsEnd;//正在播放中，判定方法为：已经开始并且没有暂停没有结束
        public static void RestartTime(bool isContinuePlay)
        {
            Instance.isStart = false;
            Instance.isPause = false;
            Instance.isEnd = false;
            ProgressManager.Instance.ResetTime();

            if (isContinuePlay)
            {
                Instance.IsStart = true;
            }
        }
    }
}
