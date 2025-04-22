using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace Form.NotePropertyEdit.ValueEdit
{
    public partial class EditEvent
    {
        private float CenterXYSnapTo16_9(float value, bool isX)
        {
            if (isX)
            {
                return 1600f * value - 800;
            }
            else
            {
                return 900f * value - 450;
            }
        }
        private float MoveXYSnapTo16_9(float value, bool isX)
        {
            return value * 100f;
        }
        private float ScaleXYSnapTo16_9(float value, bool isX)
        {
            return value * 200f;
        }
        private float AlphaSnapTo0_255(float value, bool isX)
        {
            return value * 255f;
        }
        float Value16_9ToCenterXY(float value, bool isX)
        {
            if (isX)
            {
                return (value + 800f) / 1600f;
            }
            else
            {
                return (value + 450f) / 900f;
            }
        }
        float Value16_9ToMoveXY(float value, bool isX)
        {
            return value / 100f;
        }
        float Value16_9ToScaleXY(float value, bool isX)
        {
            return value / 200f;
        }
        float Value0_255ToAlpha(float value, bool isX)
        {
            return value / 255f;
        }

    }
}