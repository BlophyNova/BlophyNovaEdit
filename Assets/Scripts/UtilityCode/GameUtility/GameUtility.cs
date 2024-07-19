using System;
using System.Collections.Generic;
using Manager;
using UnityEngine;
using UtilityCode.Extension;
using Event = Data.ChartData.Event;
namespace UtilityCode.GameUtility
{
    public class GameUtility
    {
        /// <summary>
        /// 这个就是在一个事件当中，根据时间获取值的一个方法
        /// </summary>
        /// <param name="event">传递一个事件给我</param>
        /// <param name="time">传递一个时间进去</param>
        /// <returns>返回的是这个时间（就是第二个参数）点上的值</returns>
        public static float GetValueWithEvent(Event @event, float time)
        {
            float percentage = CalculatedPercentage(@event, time);
            float res = percentage * (@event.endValue - @event.startValue) + @event.startValue;//用百分比*总时间再加上开始时间就是当前这个时间所代表的值了
            return res == 0 ? -.0001f : res;//返回这个值
        }

        private static float CalculatedPercentage(Event @event, float time)
        {
            float eventTimeDelta = @event.endTime - @event.startTime;//计算时间结束时间和开始时间的时间差
            if (eventTimeDelta <= 0) return 1;
            float currentTime = time - @event.startTime;//计算自从事件开始以来经历了多长时间
            float percentage = @event.curve.Evaluate(currentTime / eventTimeDelta);//用当前所经过的时间/时间差得到时间维度的百分比，然后用eva函数通过时间评估值
            return percentage;
        }
        /// <summary>
        /// 计算速度曲线
        /// </summary>
        /// <param name="speeds">给我一个Speed事件列表</param>
        /// <returns>返回一个处理好的AnimationCurve</returns>
        public static List<Keyframe> CalculatedSpeedCurve(Event[] speeds)
        {
            List<Keyframe> keys = new();//声明一个Keys列表
            Vector2 keySeedSpeed = Vector2.zero;//Key种子，用来记录上一次循环结束时的Time和Value信息
            foreach (var item in speeds)
            {
                if (item.startValue == item.endValue)//如果开始值和结束值相等
                {
                    item.curve.ClearMiddle();//清除中间的所有点
                }
            }
            for (int i = 0; i < speeds.Length; i++)//循环遍历所有事件
            {
                float tant = (speeds[i].endValue - speeds[i].startValue)
                    / (speeds[i].endTime - speeds[i].startTime);//这个计算的是：因为ST和ET与SV和EV形成的图形并不是正方形导致的斜率和百分比导致的误差，所以用Y/X计算出变化后的斜率

                DisposeKey(speeds, keys, keySeedSpeed, i, tant);//处理Key


                keySeedSpeed.x = keys[^1].time;//将这次处理后的最后一个Time赋值
                keySeedSpeed.y = keys[^1].value;//将这次处理后的最后一个Value赋值
            }
            return keys;//将获得到的Key列表全部赋值,然后返回出去
        }
        /// <summary>
        /// 根据速度图计算位移图
        /// </summary>
        /// <param name="canvasSpeed">速度图</param>
        /// <param name="keyframes">速度图的点</param>
        /// <returns>返回一个位移图</returns>
        public static AnimationCurve CalculatedOffsetCurve(AnimationCurve canvasSpeed, List<Keyframe> keyframes)
        {
            /*速度图的value等于位移图的斜率*/
            List<Keyframe> resultKeyframes = new()//声明一个列表，第一个点默认从0，0开始
            {
                new() {weightedMode=WeightedMode.Both, time = 0, value = 0, outWeight = keyframes[0].outWeight, outTangent = keyframes[0].value }
            };

            //下面是第一层for循环，，因为上边自动添加了第一个点，所以这里直接跳过第一个点，避免了数组越界的bug
            for (int i = 1; i < keyframes.Count; i++)
            {
                float result = 0;//计算这个点和上一个点的面积
                result = CalculateArea(canvasSpeed, keyframes, i, result);

                Keyframe keyframe = new()//声明一个key
                {
                    weightedMode = WeightedMode.Both,//key的模式为Both
                    value = result + resultKeyframes[^1].value,//Key的Value直接等于面积结果加上次计算点的value
                    time = keyframes[i].time,//时间数据直接赋值
                };
                keyframe = CalculateKeyframeProperty(keyframes, keyframe);
                AddKey2KeyList(resultKeyframes, keyframe, true);//使用严格搜索，如果这个时间有key就踢掉之前的key重加一次
            }
            return new() { keys = resultKeyframes.ToArray(), preWrapMode = WrapMode.ClampForever, postWrapMode = WrapMode.ClampForever };//把处理好的Key放到AnimationCurve里返回出去
        }

        private static Keyframe CalculateKeyframeProperty(List<Keyframe> keyframes, Keyframe keyframe)
        {
            int indexFirstKeyframe = keyframes.FindIndex(m => m.time == keyframe.time);//同一时间下列表中第一个Keyframe的索引
            int indexLastKeyframe = keyframes.FindLastIndex(m => m.time == keyframe.time);//同一时间下列表中最后一个Keyframe的索引
            Keyframe firstKeyframe = keyframes[indexFirstKeyframe];//获取到同一个时间下第一个Keyframe
            Keyframe lastKeyframe = keyframes[indexLastKeyframe];//获取到同一个时间下最后一个Keyframe

            int signInTangentFirstKeyframe = Mathm.Sign(firstKeyframe.inTangent);//获取到入点斜率的正负性，>=0返回1，<0返回-1
            int signOutTangentLastKeyframe = Mathm.Sign(lastKeyframe.outTangent);//获取到出点斜率的正负性，>=0返回1，<0返回-1

            VerifyWhetherOrNotNegation(keyframes, indexFirstKeyframe, indexLastKeyframe, ref firstKeyframe, ref lastKeyframe, ref signInTangentFirstKeyframe, ref signOutTangentLastKeyframe);

            keyframe.inTangent = firstKeyframe.value * signInTangentFirstKeyframe;//key的入点斜率等于在这一时刻下所有点中第一个点的速度（value）值
            keyframe.inWeight = firstKeyframe.inWeight;//key的入点百分比等于在这一时刻下所有点中第一个点的百分比
            keyframe.outTangent = lastKeyframe.value * signOutTangentLastKeyframe;//key的出点斜率等于在这一时刻下所有点中最后一个点的速度（value）值
            keyframe.outWeight = lastKeyframe.outWeight;//key的出点百分比等于在这一时刻下所有点中最后一个点的百分比
            return keyframe;
        }

        private static float CalculateArea(AnimationCurve canvasSpeed, List<Keyframe> keyframes, int i, float result)
        {
            for (float j = keyframes[i - 1].time;//j等于上一个点的Time
                j < keyframes[i].time;//让j小于这个点的Time，就是让j处于这个点和上一点之间
                j += ValueManager.Instance.calculatedAreaRange)//每次处理步长
            {
                float currentDeltaTime = i - 1 + j;
                float currentValue = canvasSpeed.Evaluate(currentDeltaTime);//用i-1定位上一个点的索引，用j定位当前的处于那个区间
                float lastDeltaTime = i - 1 + j - ValueManager.Instance.calculatedAreaRange;
                float lastTimeValue = canvasSpeed.Evaluate(lastDeltaTime);//用i-1定位上一个点的索引，用j定位当前处于那个区间，再减去面积计算步长，配合上一行代码得到梯形的上底加下底

                result += (currentValue + lastTimeValue) * ValueManager.Instance.calculatedAreaRange / 2;//面积累加，加的内容是：（上底加下底）乘高除2
                result = (float)Math.Round(result, ValueManager.Instance.reservedBits);//为了较小误差，保留小数点后ValueManager.Instance.reservedBits位数，四舍五入
                j = (float)Math.Round(j, ValueManager.Instance.reservedBits);//为了较小误差，保留小数点后三位数，四舍五入
            }

            return result;
        }

        /// <summary>
        /// 验证两个正负性是否需要再次取反
        /// </summary>
        /// <param name="keyframes">Keyframe列表</param>
        /// <param name="index_firstKeyframe">同一时间下列表中第一个Keyframe的索引</param>
        /// <param name="index_lastKeyframe">同一时间下列表中最后一个Keyframe的索引</param>
        /// <param name="firstKeyframe">同一个时间下第一个Keyframe</param>
        /// <param name="lastKeyframe">同一个时间下最后一个Keyframe</param>
        /// <param name="sign_inTangent_firstKeyframe">入点斜率的正负性</param>
        /// <param name="sign_outTangent_lastKeyframe">出点斜率的正负性</param>
        private static void VerifyWhetherOrNotNegation(List<Keyframe> keyframes, int indexFirstKeyframe, int indexLastKeyframe, ref Keyframe firstKeyframe, ref Keyframe lastKeyframe, ref int signInTangentFirstKeyframe, ref int signOutTangentLastKeyframe)
        {
            if (indexLastKeyframe + 1 >= keyframes.Count)//如果已经是最后一个，直接跳过下一个点的正负性检测
            {
                goto SkipLastKeyframe;
            }
            //检测下一个点是否需要取反，原理：如果Value>0并且斜率小于0并且不是同一时间上的Key，需要再次取反
            if (lastKeyframe.value > keyframes[indexLastKeyframe + 1].value
                && lastKeyframe.time != keyframes[indexLastKeyframe + 1].time
                && lastKeyframe.value > 0)
            {
                NegationValue(ref signOutTangentLastKeyframe);//取反操作
            }
        SkipLastKeyframe:
            //检测上一个点是否需要取反，原理：如果Value>0并且斜率小于0并且不是同一时间上的Key，需要再次取反
            if (firstKeyframe.value < keyframes[indexFirstKeyframe - 1].value
                && firstKeyframe.time != keyframes[indexFirstKeyframe - 1].time
                && firstKeyframe.value < 0)
            {
                NegationValue(ref signInTangentFirstKeyframe); //取反操作
            }
        }
        /// <summary>
        /// 对数值取反
        /// </summary>
        /// <param name="number">需要取反的数值</param>
        private static void NegationValue(ref int number) => number = -number;

        /// <summary>
        /// 处理Key
        /// </summary>
        /// <param name="speeds">Speed事件列表</param>
        /// <param name="keys">存处理后的key的列表</param>
        /// <param name="keySeed">上次处理完后最后一个Key的Time和Value值</param>
        /// <param name="i">这次处于第几循环</param>
        /// <param name="tant">斜率</param>
        private static void DisposeKey(Event[] speeds, List<Keyframe> keys, Vector2 keySeed, int i, float tant)
        {
            for (int j = 0; j < speeds[i].curve.length; j++)//循环遍历所有的Key
            {
                Keyframe keyframe = InstKeyframe(speeds, keySeed, i, tant, j);//生成一个Key
                if (i != 0 && j == 0)//如果不是第一个Speed事件并且是第一个AnimationCurve的Key
                {
                    keyframe.inTangent = keys[^1].inTangent;//将上次处理后的最后一个key的入点斜率拿到
                    keyframe.inWeight = keys[^1].inWeight;//将上次处理后的最后一个key的入点百分比拿到
                }
                if (keys.Count != 0 && keyframe.time == keys[^1].time && keyframe.value == keys[^1].value)
                    AddKey2KeyList(keys, keyframe, true);//将处理好的Key，加入Key的列表中
                else
                    AddKey2KeyList(keys, keyframe, false);//将处理好的Key，加入Key的列表中
            }
        }
        /// <summary>
        /// 召唤一个Key
        /// </summary>
        /// <param name="speeds">速度列表</param>
        /// <param name="keySeed">上次处理完Key的Time和Value值</param>
        /// <param name="i">这次是处于第几循环</param>
        /// <param name="tant">斜率</param>
        /// <param name="index">这个是处于这个AnimationCurve中的第几个Key</param>
        /// <returns>返回一个处理好的Key</returns>
        private static Keyframe InstKeyframe(Event[] speeds, Vector2 keySeed, int i, float tant, int index)
        {
            Keyframe keyframe = speeds[i].curve.keys[index];//把Key拿出来
            keyframe.weightedMode = WeightedMode.Both;//设置一下模式
            keyframe.time = (speeds[i].endTime - speeds[i].startTime) * keyframe.time + keySeed.x;//（当前事件的结束时间-开始时间）*当前Key的时间+上次事件处理完后的最后一个Key的时间
            keyframe.value = (speeds[i].endValue - speeds[i].startValue) * keyframe.value + speeds[i].startValue;//（当前事件的结束值-开始值）*当前Key的值+上次事件处理完后的最后一个Key的值

            keyframe.outTangent *= tant;//出点的斜率适应一下变化
            keyframe.inTangent *= tant;//入店的斜率适应一下变化（就是消除因为非正方形导致的误差）
            return keyframe;//返回Key
        }
        /// <summary>
        /// 将Key加入到Key列表
        /// </summary>
        /// <param name="keys">需要添加的Key列表</param>
        /// <param name="keyframe">需要添加的Key</param>
        private static void AddKey2KeyList(List<Keyframe> keys, Keyframe keyframe, bool isMove)
        {
            if (keys.Count != 0 && isMove)
            {
                int index = Algorithm.Algorithm.BinaryStrictSearch(keys.ToArray(), keyframe.time);//使用二分严格搜索找这个时间是否存在Key
                if (index >= 0) keys.RemoveAt(index);//如果存在就移除
            }
            keys.Add(keyframe);//移除后添加
        }
    }
}
