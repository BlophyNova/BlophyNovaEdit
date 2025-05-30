using System;
using UnityEngine;

namespace Data.EaseData
{
    [Serializable]
    public class EaseData
    {
        public AnimationCurve thisCurve;

        public float area;

        //public AnimationCurve speed;
        public AnimationCurve offset;
        public Ease easeType;

        public override bool Equals(object obj)
        {
            bool res = false;
            if (obj is EaseData)
            {
                EaseData my_obj = obj as EaseData;
                if (Mathf.Abs(area - my_obj.area) > .001f)
                {
                    return false;
                }

                if (my_obj.easeType != easeType)
                {
                    return true;
                }

                res = true;
            }

            return res;
        }
    }

    /// <summary>
    ///     缓动类型
    /// </summary>
    public enum Ease
    {
        Linear = 0,
        InSine = 1,
        OutSine = 2,
        InOutSine = 3,
        InQuad = 4,
        OutQuad = 5,
        InOutQuad = 6,
        InCubic = 7,
        OutCubic = 8,
        InOutCubic = 9,
        InQuart = 10,
        OutQuart = 11,
        InOutQuart = 12,
        InQuint = 13,
        OutQuint = 14,
        InOutQuint = 15,
        InExpo = 16,
        OutExpo = 17,
        InOutExpo = 18,
        InCirc = 19,
        OutCirc = 20,
        InOutCirc = 21,
        InElastic = 22,
        OutElastic = 23,
        InOutElastic = 24,
        InBack = 25,
        OutBack = 26,
        InOutBack = 27,
        InBounce = 28,
        OutBounce = 29,
        InOutBounce = 30
    }
}