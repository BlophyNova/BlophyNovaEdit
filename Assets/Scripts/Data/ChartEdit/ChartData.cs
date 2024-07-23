using Scenes.DontDestoryOnLoad;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace Data.ChartEdit
{
    [Serializable]
    public class ChartData
    {
        public float yScale;
        public int beatSubdivision;//节拍线细分(单位：份)
        public int verticalSubdivision;//垂直线细分(单位：份)
        public List<BPM> bpmList = new();
    }
    [Serializable]
    public class BPM
    {
        public int integer = 0;
        /// <summary>
        /// 分子
        /// </summary>
        public int molecule = 0;
        /// <summary>
        /// 分母
        /// </summary>
        public int denominator = 1;
        /// <summary>
        /// 当前BPM的开始或者说上一个BPM的结束拍
        /// </summary>
        public float ThisStartBPM => integer + molecule / (float)denominator;
        public float currentBPM;
        public BPM() { }
        public BPM(BPM bpm)
        {
            this.molecule = bpm.molecule;
            this.denominator = bpm.denominator;
            this.integer = bpm.integer;
            this.currentBPM = bpm.currentBPM;
        }
        public void AddOneBeat()
        {
            denominator = GlobalData.Instance.chartEditData.beatSubdivision;
            if (molecule < denominator - 1) molecule++;
            else if (molecule + 1 >= denominator)
            {
                molecule = 0;
                integer++;
            }
        }
    }
}