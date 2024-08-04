using Manager;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using TMPro.EditorUtilities;
using UnityEngine;

public class DebugText : LabelWindowContent
{
    public TMP_Text debugText;
    private void Update()
    {
        debugText.text = $"CurrentFPS: {1 / GetSmoothDeltaTime():F2}\n";
    }
    private static readonly Queue<float> DeltaTimeSamples = new();//估计是用来存DeltaTime的样本的，配合下面的属性工作
    private const float SmoothDeltaTimePeriod = 1.5f;//因为直接用1/Time.unscaledDeltaTime会导致变化特别快，人眼无法捕捉，所以取1.5秒之内的平滑值
    public static float GetSmoothDeltaTime()//方法
    {
        float time = Time.unscaledTime;//将当前的游戏运行的时间（不受时间缩放影响的）存到浮点数的值中
        DeltaTimeSamples.Enqueue(time);//将Time推到这个队列中
        if (DeltaTimeSamples.Count > 1)//如果队列中有东西
        {
            float startTime = DeltaTimeSamples.Peek();//查看队列中第一个数据
            float delta = time - startTime;//用当前获得的时间减去开始时间得到现在距离开始时间的这段时间段
            float smoothDelta = delta / DeltaTimeSamples.Count;//将刚才获得的delta除以队列数量得到平均值
            if (delta > SmoothDeltaTimePeriod)//如果delta大于上边常量所指向的1.5秒
                DeltaTimeSamples.Dequeue();//那就弹出第一个数据
            return smoothDelta;//返回这个平均值
        }
        return Time.unscaledDeltaTime;//如果队列中没有数据，就直接返回不受时间缩放影响的这一帧与上一帧的时间增量
    }
}
