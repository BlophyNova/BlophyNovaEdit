using System;
using System.IO;
using System.Text;
using Hook;
using Newtonsoft.Json;
using Scenes.DontDestroyOnLoad;
using UnityEngine;

namespace Data.GeneralSettings
{
    [Serializable]
    public class GeneralData
    {
        /*
         * 自动保存间隔（单位s）
         * 垂直同步
         * 帧数设置
         * 音乐大小
         * 音效大小
         * 鼠标滚轮速度
         * 新建方框时默认隐藏新方框
         * 更改谱面元数据
         */
        [JsonProperty]float autosaveInterval;

        [JsonIgnore]public float AutosaveInterval
        {
            get { return autosaveInterval; }
            set
            {
                autosaveInterval = value;
                SaveConfig();
            }
        }

        [JsonProperty]bool verticalSync;

        [JsonIgnore]public bool VerticalSync
        {
            get { return verticalSync; }
            set
            {
                verticalSync = value; 
                SaveConfig();
            }
        }

        [JsonProperty]bool newBoxAlpha;

        [JsonIgnore]public bool NewBoxAlpha
        {
            get { return newBoxAlpha; }
            set
            {
                newBoxAlpha = value; 
                SaveConfig();
            }
        }

        [JsonProperty]int fps;

        [JsonIgnore]public int Fps
        {
            get { return fps; }
            set
            {
                fps = value; 
                SaveConfig();
            }
        }

        [JsonProperty]float musicVolume;

        [JsonIgnore]public float MusicVolume
        {
            get { return musicVolume; }
            set
            {
                musicVolume = value; 
                SaveConfig();
            }
        }

        [JsonProperty]float soundVolume;

        [JsonIgnore]public float SoundVolume
        {
            get { return soundVolume; }
            set
            {
                soundVolume = value; 
                SaveConfig();
            }
        }

        [JsonProperty]float mouseWheelSpeed;

        [JsonIgnore]public float MouseWheelSpeed
        {
            get { return mouseWheelSpeed; }
            set
            {
                mouseWheelSpeed = value; 
                SaveConfig();
            }
        }

        void SaveConfig()
        {
            File.WriteAllText(new Uri($"{Applicationm.streamingAssetsPath}/Config/GeneralData.json").LocalPath,JsonConvert.SerializeObject(GlobalData.Instance.generalData),Encoding.UTF8);
        }

        public void Init()
        {
            
        }
    }
}
