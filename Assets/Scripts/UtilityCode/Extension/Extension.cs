using System;
using UnityEngine;
namespace UtilityCode.Extension
{
    public static class AnimationCurveExtension
    {
        /// <summary>
        /// 清除中间的Key
        /// </summary>
        /// <param name="curve"></param>
        public static void ClearMiddle(this AnimationCurve curve)
        {
            int length = curve.length;//记录一下length
            for (int i = 1; i < length - 1; i++)//如果i小于length-1（除去了第1个和最后一个）
            {
                curve.RemoveKey(1);//循环清掉第2个key
            }
        }
        /// <summary>
        /// 清除所有Key
        /// </summary>
        /// <param name="curve">需要清除Key的AnimationCurve</param>
        public static void ClearAll(this AnimationCurve curve)
        {
            int length = curve.length;//记录一下length
            for (int i = 0; i < length; i++)//遍历所有Key
                curve.RemoveKey(0);//移除
        }
    }
    /// <summary>
    /// 自行拓展的数学类
    /// </summary>
    public static class Mathm
    {
        /// <summary>
        /// 取正负
        /// </summary>
        /// <param name="number">需要取的数值</param>
        /// <returns>返回正负</returns>
        public static int Sign(float number)
        {
            int result = Math.Sign(number);//取正负
            result = result switch { 0 => 1, _ => result };//如果是0按照正来算，其余结果直接返回
            return result;//返回
        }
    }
}