using Controller;
using Data.ChartData;
using Scenes.DontDestroyOnLoad;
using System.Collections.Generic;
using System.Diagnostics;
using UnityEngine;
using UtilityCode.Singleton;
using static Manager.ProgressManager;
using GlobalData = Scenes.DontDestroyOnLoad.GlobalData;
namespace Manager
{
    internal class ProgressManager : MonoBehaviourSingleton<ProgressManager>
    {
        [SerializeField] private readonly Stopwatch musicPlayerTime = new Stopwatch();//计时器，谱面时间和音乐时间的
        [SerializeField] private double dspStartPlayMusic;//开始时间
        [SerializeField] private double dspLastPlayMusic;//上一次暂停后的时间
        [SerializeField] private double offset;//偏移
        public double Offset { get => offset; set => offset = value; }
        //public double Offset => GlobalData.Instance.chartData.globalData.offset;//偏移
        [SerializeField] private double skipTime;//时间跳转
        public float playSpeed = 1;
        public double CurrentTime => musicPlayerTime.ElapsedMilliseconds * playSpeed / 1000d + skipTime;//当前时间

        public delegate void OnCurrentTimeChanged(double currentTime);
        public event OnCurrentTimeChanged onCurrentTimeChanged = currentTime => { };
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
            //isStarted = true;
            musicPlayerTime.Start();//开始计时
        }
        public void SetPlaySpeed(float playSpeed)
        {
            this.playSpeed = playSpeed;
            AssetManager.Instance.musicPlayer.pitch = playSpeed;
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
        /// 暂停时间
        /// </summary>
        private void StopMusic()
        {
            musicPlayerTime.Stop();
            AssetManager.Instance.musicPlayer.Pause();
        }
        /// <summary>
        /// 继续播放音乐
        /// </summary>
        public void ContinuePlay()
        {
            double currentTime = CurrentTime;
            if (currentTime < offset)
            {


                skipTime = currentTime;
                musicPlayerTime.Reset();
                AssetManager.Instance.musicPlayer.Stop();
                AssetManager.Instance.musicPlayer.time = 0;

                double tempOffset = offset - currentTime;


                dspStartPlayMusic = AudioSettings.dspTime + tempOffset;//获取到开始播放的时间
                dspLastPlayMusic = dspStartPlayMusic;//同步LastPlayMusic
                AssetManager.Instance.musicPlayer.time = 0;
                AssetManager.Instance.musicPlayer.PlayScheduled(dspStartPlayMusic);//在绝对的时间线上播放
                musicPlayerTime.Start();//开始计时



            }
            else
            {
                //AssetManager.Instance.musicPlayer.UnPause();//播放器解除暂停状态
                //musicPlayerTime.Start();//音乐播放器的时间开始播放


                ResetTime();
                StartPlay();
                Offset = GlobalData.Instance.chartEditData.offset;
                SetTime(currentTime);
            }
        }
        /// <summary>
        /// 跳转时间
        /// </summary>
        /// <param name="time">跳转到哪里</param>
        public void SetTime(double time)
        {
            //UnityEngine.Debug.LogError($"这里不对，时间不对");

            double timeDelta = time - CurrentTime;
            if (time < offset)
            {
                //StopMusic();
                PausePlay();
                skipTime += timeDelta;
            }
            else
            {
                //AssetManager.Instance.musicPlayer.UnPause();
                //UnityEngine.Debug.Log($"AssetManager.Instance.musicPlayer.isPlaying:{AssetManager.Instance.musicPlayer.isPlaying}");
                AssetManager.Instance.musicPlayer.time = (float)(time - offset);
                skipTime += timeDelta;
            }
            onCurrentTimeChanged(CurrentTime);
        }
        /// <summary>
        /// 在当前时间的基础上加或者减时间
        /// </summary>
        /// <param name="time">加多少或者减多少时间</param>
        public void OffsetTime(double time)
        {
            AssetManager.Instance.musicPlayer.time += (float)(time - offset);
            skipTime += time;
            onCurrentTimeChanged(CurrentTime);
            ResetAllLineNoteState();
        }
        /// <summary>
        /// 重置时间
        /// </summary>
        public void ResetTime()
        {
            skipTime = 0;
            musicPlayerTime.Reset();
            AssetManager.Instance.musicPlayer.Stop();
            AssetManager.Instance.musicPlayer.time = 0;
            onCurrentTimeChanged(CurrentTime);
        }
        ///// <summary>
        ///// 让时间重新开始计算
        ///// </summary>
        //public void RestartTime()
        //{
        //    skipTime = 0;
        //    musicPlayerTime.Restart();
        //}
        void ResetAllLineNoteState()
        {
            for (int i = 0; i < SpeckleManager.Instance.allLineNoteControllers.Count; i++)
            {
                ResetLineNoteState(ref SpeckleManager.Instance.allLineNoteControllers[i].lastOnlineIndex,
                    SpeckleManager.Instance.allLineNoteControllers[i].ariseOnlineNotes,
                    SpeckleManager.Instance.allLineNoteControllers[i].endTimeAriseOnlineNotes,
                    SpeckleManager.Instance.allLineNoteControllers[i].decideLineController,
                    SpeckleManager.Instance.allLineNoteControllers[i].decideLineController.ThisLine.onlineNotes, true);

                ResetLineNoteState(ref SpeckleManager.Instance.allLineNoteControllers[i].lastOfflineIndex,
                    SpeckleManager.Instance.allLineNoteControllers[i].ariseOfflineNotes,
                    SpeckleManager.Instance.allLineNoteControllers[i].endTimeAriseOfflineNotes,
                    SpeckleManager.Instance.allLineNoteControllers[i].decideLineController,
                    SpeckleManager.Instance.allLineNoteControllers[i].decideLineController.ThisLine.offlineNotes, false);
            }
        }
        public void ResetLineNoteState(ref int lastIndex, List<NoteController> ariseLineNotes, List<NoteController> endTime_ariseLineNotes, DecideLineController decideLine, List<Note> notes, bool isOnlineNote)
        {
            lastIndex = 0;
            for (int i = 0; i < notes.Count; i++)
            {
                if (notes[i].hitTime < CurrentTime)
                {
                    lastIndex++;
                }
                else break;
            }
            for (int i = 0; i < ariseLineNotes.Count; i++)
            {
                NoteController note = ariseLineNotes[i];
                decideLine.ReturnNote(note, note.thisNote.noteType, isOnlineNote);
            }
            ariseLineNotes.Clear();
            endTime_ariseLineNotes.Clear();
        }
    }
}
