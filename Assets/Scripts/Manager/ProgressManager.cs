using System.Diagnostics;
using UnityEngine;
using UtilityCode.Singleton;
namespace Manager
{
    internal class ProgressManager : MonoBehaviourSingleton<ProgressManager>
    {
        private readonly Stopwatch musicPlayerTime = new Stopwatch();//计时器，谱面时间和音乐时间的
        private double dspStartPlayMusic;//开始时间
        private double dspLastPlayMusic;//上一次暂停后的时间
        private double offset;//偏移
        private double skipTime;//时间跳转
        public double CurrentTime => musicPlayerTime.ElapsedMilliseconds / 1000d + skipTime;//当前时间


        /// <summary>
        /// 开始播放
        /// </summary>
        /// <param name="offset">偏移</param>
        public void StartPlay(double offset = 0)
        {
            this.offset = offset;//偏移
            dspStartPlayMusic = AudioSettings.dspTime + this.offset;//获取到开始播放的时间
            dspLastPlayMusic = dspStartPlayMusic;//同步LastPlayMusic
            AssetManager.Instance.musicPlayer.time = 0;
            AssetManager.Instance.musicPlayer.PlayScheduled(dspStartPlayMusic);//在绝对的时间线上播放
            musicPlayerTime.Start();//开始计时
        }
        /// <summary>
        /// 暂停播放
        /// </summary>
        public void PausePlay()
        {
            dspLastPlayMusic = AudioSettings.dspTime + offset;//更新暂停时候的时间
            StopMusic();//暂停播放音乐
        }
        /// <summary>
        /// 继续播放音乐
        /// </summary>
        public void ContinuePlay()
        {
            AssetManager.Instance.musicPlayer.UnPause();//播放器解除暂停状态
            musicPlayerTime.Start();//音乐播放器的时间开始播放
        }

        /// <summary>
        /// 暂停时间
        /// </summary>
        private void StopMusic()
        {
            musicPlayerTime.Stop();
            AssetManager.Instance.musicPlayer.Pause();
        }
        /// <summary>
        /// 跳转时间
        /// </summary>
        /// <param name="time">跳转到哪里</param>
        public void SetTime(double time)
        {
            double timeDelta = time - CurrentTime;
            AssetManager.Instance.musicPlayer.time += (float)timeDelta;
            skipTime += timeDelta;
        }
        /// <summary>
        /// 在当前时间的基础上加或者减时间
        /// </summary>
        /// <param name="time">加多少或者减多少时间</param>
        public void OffsetTime(double time)
        {
            AssetManager.Instance.musicPlayer.time += (float)time;
            skipTime += time;
        }
        /// <summary>
        /// 重置时间
        /// </summary>
        public void ResetTime()
        {
            skipTime = 0;
            musicPlayerTime.Reset();
        }
        /// <summary>
        /// 让时间重新开始计算
        /// </summary>
        public void RestartTime()
        {
            skipTime = 0;
            musicPlayerTime.Restart();
        }
    }
}
