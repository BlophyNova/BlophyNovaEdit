namespace UtilityCode.ValueConvert
{
    public class ValueConvert
    {
        public static float CenterXYSnapTo16_9(float value, bool isX)
        {
            if (isX)
            {
                return 1600f * value - 800;
            }

            return 900f * value - 450;
        }

        public static float MoveXYSnapTo16_9(float value, bool isX)
        {
            return value * 100f;
        }

        public static float ScaleXYSnapTo16_9(float value, bool isX)
        {
            return value * 200f;
        }

        public static float AlphaSnapTo0_255(float value, bool isX)
        {
            return value * 255f;
        }

        public static float Value16_9ToCenterXY(float value, bool isX)
        {
            if (isX)
            {
                return (value + 800f) / 1600f;
            }

            return (value + 450f) / 900f;
        }

        public static float Value16_9ToMoveXY(float value, bool isX)
        {
            return value / 100f;
        }

        public static float Value16_9ToScaleXY(float value, bool isX)
        {
            return value / 200f;
        }

        public static float Value0_255ToAlpha(float value, bool isX)
        {
            return value / 255f;
        }
    }
}