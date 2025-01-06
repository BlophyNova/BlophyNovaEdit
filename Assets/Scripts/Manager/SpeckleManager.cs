using Controller;
using Data.ChartData;
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;
using UtilityCode.Algorithm;
using UtilityCode.Singleton;

namespace Manager
{
    public class SpeckleManager : MonoBehaviourSingleton<SpeckleManager>
    {
        public Speckle[] speckles; //手指触摸列表

        public List<LineNoteController> allLineNoteControllers = new(); //所有的判定线，一个框默认五个判定线
        public List<LineNoteController> isInRangeLine = new(); //根据横轴判定，在范围内的判定线列表
        public List<NoteController> waitNote = new(); //等待确定判定的音符
        public int lineNoteControllerCount = -1; //初始化为-1

        public int LineNoteControllerCount
        {
            get
            {
                if (lineNoteControllerCount < 0) //如果小于0，说明没有执行过这里的代码
                {
                    lineNoteControllerCount = allLineNoteControllers.Count; //就获取一次
                }

                return lineNoteControllerCount; //返回获取到的数据
            }
        }

        private void Update()
        {
            UpdateTouch();
        }

        protected override void OnAwake() //唤醒
        {
            speckles = new Speckle[ValueManager.Instance.maxSpeckleCount]; //初始化手指触摸列表
            for (int i = 0; i < speckles.Length; i++) //循环每个列表
            {
                speckles[i] =
                    new Speckle(
                        new Vector2[(int)(ValueManager.Instance.currentTargetFPS *
                                          ValueManager.Instance.fingerSavePosition)], Vector2.zero); //初始化触摸
            }
        }

        /// <summary>
        ///     更新触摸
        /// </summary>
        private void UpdateTouch()
        {
            for (int i = 0; i < Input.touchCount; i++) //遍历所有手指
            {
                Touch currentTouch = Input.GetTouch(i); //获取到这一帧的当前循环到的手指
                speckles[i].SetCurrentTouch(currentTouch, i); //设置当前触摸到触摸管理列表
                isInRangeLine.Clear(); //清除“所有在手指周围的判定线”
                waitNote.Clear(); //清除等待判定的音符
                //思路：先判定判定线的横轴，在那个判定线范围区间内
                //然后时间判定，找到时间值最低的Notes
                //然后根据Notes做竖轴空间判定
                for (int j = 0; j < LineNoteControllerCount; j++) //每根手指遍历每根线,在哪些线的横轴范围内
                {
                    if (allLineNoteControllers[j].SpeckleInThisLine(speckles[i].CurrentPosition)) //如果判定线说：确实在范围内
                    {
                        isInRangeLine.Add(allLineNoteControllers[j]); //加入“所有在手指周围的判定线”
                    }
                }

                for (int j = 0; j < isInRangeLine.Count; j++) //时间判定
                {
                    FindNotes(isInRangeLine[j].ariseOnlineNotes, waitNote); //时间判定，当前时间±bad时间直接的所有音符
                    FindNotes(isInRangeLine[j].ariseOfflineNotes, waitNote); //时间判定，当前时间±bad时间直接的所有音符
                }

                for (int j = 0; j < waitNote.Count; j++) //音符竖轴判定
                {
                    switch (speckles[i].Phase) //判定阶段
                    {
                        case TouchPhase.Began: //开始
                            BeganJudge(i, j);
                            break;
                        case TouchPhase.Moved: //移动
                            MovedJudge(i, j);
                            break;
                        case TouchPhase.Stationary: //静止不动
                            StationaryJudge(i, j);
                            break;
                    }
                }
            }
        }

        /// <summary>
        ///     判定的开始阶段
        /// </summary>
        /// <param name="i">for循环索引</param>
        /// <param name="j">for循环索引</param>
        private void BeganJudge(int i, int j)
        {
            switch (waitNote[j].thisNote.noteType) //看看这个阶段遇到了啥音符
            {
                case NoteType.Flick: //如果是flick
                case NoteType.Drag: //和drag
                    break; //啥也不干
                default: //如果是别的音符
                    if (!speckles[i].isUsed && !waitNote[j].isJudged && //如果这个手指没判定过并且音符也没有被判定过并且在音符判定范围内
                        waitNote[j].IsinRange(speckles[i].CurrentPosition))
                    {
                        speckles[i].isUsed = !speckles[i].isUsed; //修改isUsed为True
                        waitNote[j].Judge(ProgressManager.Instance.CurrentTime, TouchPhase.Began); //调用音符的判定
                    }

                    break;
            }
        }

        /// <summary>
        ///     如果是移动判定
        /// </summary>
        /// <param name="i">for循环索引</param>
        /// <param name="j">for循环索引</param>
        private void MovedJudge(int i, int j)
        {
            switch (waitNote[j].thisNote.noteType) //看看这个阶段遇到了啥音符
            {
                case NoteType.Flick: //如果是Flick
                case NoteType.FullFlickPink: //或者FullFlick
                case NoteType.FullFlickBlue: //或者FullFlick
                case NoteType.Drag: //或者Drag
                case NoteType.Hold: //或者Hold
                    if (waitNote[j].IsinRange(speckles[i].CurrentPosition)) //看看音符是否在判定范围
                    {
                        waitNote[j].Judge(ProgressManager.Instance.CurrentTime, TouchPhase.Moved); //调用音符的判定
                    }

                    break; //其余音符不管
            }
        }

        /// <summary>
        ///     如果是静止不动的触摸阶段
        /// </summary>
        /// <param name="i">for循环索引</param>
        /// <param name="j">for循环索引</param>
        private void StationaryJudge(int i, int j)
        {
            switch (waitNote[j].thisNote.noteType) //看看这个阶段遇到了啥音符
            {
                case NoteType.Hold: //如果是Hold
                case NoteType.Drag: //或者Drag
                    if (waitNote[j].IsinRange(speckles[i].CurrentPosition)) //看看音符是否在判定范围
                    {
                        waitNote[j].Judge(ProgressManager.Instance.CurrentTime, TouchPhase.Stationary); //调用音符的判定
                    }

                    break;
            }
        }

        /// <summary>
        ///     寻找音符
        /// </summary>
        /// <param name="needFindNotes">需要寻找的音符列表</param>
        /// <param name="waitNotes">找到后放到什么地方</param>
        private void FindNotes(List<NoteController> needFindNotes, List<NoteController> waitNotes)
        {
            double currentTime = ProgressManager.Instance.CurrentTime; //拿到当前时间
            int indexStartJudgeNeedJudgeNote = Algorithm.BinarySearch(needFindNotes,
                m => m.thisNote.hitTime < currentTime - m.thisNote.HoldTime, false); //获取到当前时间-bad所得到的需要判定的音符的开始界限
            int indexEndJudgeNeedJudgeNote = Algorithm.BinarySearch(needFindNotes,
                m => m.thisNote.hitTime < currentTime + JudgeManager.Bad, false); //获取到当前时间+bad所得到的需要判定的音符的结束界限
            for (int i = indexStartJudgeNeedJudgeNote; i < indexEndJudgeNeedJudgeNote; i++) //每根线遍历每个出现的音符
            {
                int index = Algorithm.BinarySearch(waitNote,
                    m => m.thisNote.hitTime < needFindNotes[i].thisNote.hitTime, false); //寻找这个音符按照hitTime排序的话，应该插在什么位置
                waitNotes.Insert(index, needFindNotes[i]); //插入音符
                //waitNotes.Add(needFindNotes[i]);
            }
        }
    }

    [Serializable]
    public struct Speckle //翻译为斑点，亦为安卓系统的触摸小白点，多个小白点酷似斑点，故起Speckle
    {
        //一个斑点类就是一个手指的触摸
        [SerializeField] private float beganTime; //当前触摸段的开始时间
        public int thisIndex; //这个触摸是数组中的第几个索引
        public Vector2[] movePath; //移动过的路径

        [FormerlySerializedAs("index_movePath")]
        public int indexMovePath; //移动过的路径的索引

        [FormerlySerializedAs("isFull_movePath")]
        public bool isFullMovePath; //移动过的路径是否已被全部填充

        [FormerlySerializedAs("length_movePath")]
        public int lengthMovePath; //movePath的长度

        public bool isUsed; //这个触摸是否已经用过了
        public Vector2 startPoint; //开始位置
        [SerializeField] private TouchPhase phase; //当前的触摸阶段

        /// <summary>
        ///     这个类的构造函数
        /// </summary>
        /// <param name="movePath">移动路径</param>
        /// <param name="startPoint">开始点</param>
        /// <param name="phase">触摸阶段</param>
        /// <param name="beganTime">开始时间</param>
        /// <param name="thisIndex">这个触摸在数组中的索引</param>
        /// <param name="index_movePath">movePath的索引</param>
        /// <param name="isFull_movePath">如果movePath已经满了</param>
        public Speckle(Vector2[] movePath, Vector2 startPoint,
            TouchPhase phase = TouchPhase.Canceled, float beganTime = 0, int thisIndex = 0, int indexMovePath = 0,
            bool isFullMovePath = false)
        {
            this.beganTime = beganTime;
            this.thisIndex = thisIndex;
            this.indexMovePath = indexMovePath;
            this.isFullMovePath = isFullMovePath;
            this.phase = phase;
            this.movePath = movePath;
            this.startPoint = startPoint;
            lengthMovePath = movePath.Length;
            isUsed = false;
        }

        public Vector2 CurrentPosition =>
            movePath[LoopBackIndex(indexMovePath, -1, lengthMovePath)]; //当前位置，返回值为movePath【当前索引（回环索引）】（世界坐标轴）

        public TouchPhase Phase //当前触摸阶段
        {
            get => phase; //直接return~
            set //设置触摸阶段
            {
                phase = value; //先赋值
                switch (phase)
                {
                    case TouchPhase.Began: //如果触摸阶段是开始
                        beganTime = (float)ProgressManager.Instance.CurrentTime; //赋值当前时间进去
                        startPoint = movePath[indexMovePath - 1]; //获取到刚才index_movePath自加之前的Vector2
                        ResetIndex(ref indexMovePath); //重置索引
                        movePath[indexMovePath++] = startPoint; //把StartPoint赋值给第0个索引的movePath并且自加
                        isFullMovePath = false; //重置状态
                        isUsed = false; //重置isUsed
                        break;
                }
            }
        }

        public float FlickDirection //滑动方向
        {
            get
            {
                //如果isFull MovePath是true，就直接用最后一个二维向量减去第一个，如果是False，就用当前的movepath列表返回出去
                float v = Vector2.SignedAngle(movePath[isFullMovePath ? ^1 : indexMovePath] - movePath[0],
                    Vector2.right);
                return v < 0 ? -v : 360 - v; //如果是负数就取赋值，如果是正数直接360-当前值返回出去
            }
        }

        public float FlickLength => (movePath[isFullMovePath ? ^1 : indexMovePath] - movePath[0]).sqrMagnitude; //滑动长度

        public void SetCurrentTouch(Touch touch, int currentIndex) //当前触摸处理的初始化方法
        {
            thisIndex = currentIndex; //先赋值当前索引
            movePath[indexMovePath++] = Camera.main.ScreenToWorldPoint(touch.position); //然后吧传进来的屏幕像素坐标转换为世界坐标放进移动路径中
            Phase = touch.phase switch //看看人家系统给我传进来什么触摸阶段
            {
                TouchPhase.Began => TouchPhase.Began, //如果是触摸开始阶段那就直接赋值
                TouchPhase.Canceled => TouchPhase.Ended, //如果是取消跟踪那就默认抬起了手指
                TouchPhase.Ended => TouchPhase.Ended, //如果是抬起了手指那就直接赋值
                _ => IsMovedOrStationary(ValueManager.Instance.flickRange) //如果是剩下的Move阶段或者静止不动的阶段那就我自己来决定，不用人家给我的
            };
            CheckIndex(ref indexMovePath, movePath.Length); //检查index_movePath是不是越界了
        }

        /// <summary>
        ///     判定是移动状态还是静止不动的状态
        /// </summary>
        /// <param name="deltaRange"></param>
        /// <returns></returns>
        private TouchPhase IsMovedOrStationary(float deltaRange)
        {
            Vector2 currentPosition = movePath[LoopBackIndex(indexMovePath, -1, lengthMovePath)]; //获取到当前的位置
            Vector2 lastPosition = movePath[LoopBackIndex(indexMovePath, -2, lengthMovePath)]; //获取到上一帧的位置
            float deltaLength = (currentPosition - lastPosition).sqrMagnitude; //计算这一帧和上一帧手指的拉开的距离
            //UIManager.Instance.DebugTextString = $"FingerIndex：{thisIndex}\n" +
            //    $"FlickLength:{deltaLength}\n" +
            //    $"TouchPhase:{phase}\n" +
            //    $"CurrentTargetFPS:{ValueManager.Instance.currentTargetFPS}";
            return deltaLength < deltaRange
                ? TouchPhase.Stationary
                : TouchPhase.Moved; //如果小于设定的值就判定为Stationary ，否侧判定为Moved
        }

        /// <summary>
        ///     索引回环，防止索引越界用的
        /// </summary>
        /// <param name="currentIndex"></param>
        /// <param name="needCalculateIndex"></param>
        /// <param name="maxIndex"></param>
        /// <returns></returns>
        private int LoopBackIndex(int currentIndex, int needCalculateIndex, int maxIndex)
        {
            int indexDelta = currentIndex + needCalculateIndex; //看看计算过后的索引结果事什么
            int result; //最后要返回的结果
            if (indexDelta < 0) //如果小于0
            {
                result = indexDelta + maxIndex; //那就indexDelta + maxIndex
            }
            else if (indexDelta >= maxIndex) //如果大于等于最大值
            {
                result = indexDelta - maxIndex; //那就indexDelta - maxIndex
            }
            else //如果不大于也不小于
            {
                result = indexDelta; //直接赋值
            }

            return result; //返回最终结果
        }

        private void CheckIndex(ref int index, int maxValue) //检查index是否大于最大值；
        {
            if (index >= maxValue) //如果index大于最大值；
            {
                ResetIndex(ref index); //重置索引
            }
        }

        private void ResetIndex(ref int index)
        {
            index = 0;
            //重置一个索引
        }
    }
}