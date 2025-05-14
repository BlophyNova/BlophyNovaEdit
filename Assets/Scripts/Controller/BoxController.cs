using Data.ChartData;
using JetBrains.Annotations;
using Manager;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Serialization;
using UtilityCode.Algorithm;
using UtilityCode.GameUtility;
using UtilityCode.ObjectPool;
using Event = Data.ChartData.Event;

namespace Controller
{
    public class BoxController : MonoBehaviour
    {
        public Transform squarePosition; //方框的位置
        public Camera mainCamera;

        public TMP_Text boxIDText;
        public DecideLineController[] decideLineControllers; //所有的判定线控制器
        public SpriteRenderer[] spriteRenderers; //所有的渲染组件
        public ShowPointInGameView[] showPointInGameView;
        public ObjectPoolQueue<RippleController> ripples;

        public Box box; //谱面，单独这个box的谱面

        public int currentBoxID;
        public int sortSeed; //层级顺序种子
        public SpriteMask spriteMask; //遮罩

        public float currentScaleX;
        public float currentScaleY;
        public float currentAlpha; //默认值
        public float currentCenterX; //默认值
        public float currentCenterY; //默认值
        public float currentLineAlpha; //默认值
        public float currentMoveX; //默认值
        public float currentMoveY; //默认值
        public float currentRotate; //默认值
        public float boxFineness;

        [FormerlySerializedAs("last_currentScaleX")]
        public float lastCurrentScaleX;

        [FormerlySerializedAs("last_currentScaleY")]
        public float lastCurrentScaleY;

        [FormerlySerializedAs("last_currentAlpha")]
        public float lastCurrentAlpha;

        [FormerlySerializedAs("last_currentCenterX")]
        public float lastCurrentCenterX;

        [FormerlySerializedAs("last_currentCenterY")]
        public float lastCurrentCenterY;

        [FormerlySerializedAs("last_currentLineAlpha")]
        public float lastCurrentLineAlpha;

        [FormerlySerializedAs("last_currentMoveX")]
        public float lastCurrentMoveX;

        [FormerlySerializedAs("last_currentMoveY")]
        public float lastCurrentMoveY;

        [FormerlySerializedAs("last_currentRotate")]
        public float lastCurrentRotate;

        [FormerlySerializedAs("last_boxFineness")]
        public float lastBoxFineness;

        [FormerlySerializedAs("index_currentScaleX")]
        public int indexCurrentScaleX;

        [FormerlySerializedAs("index_currentScaleY")]
        public int indexCurrentScaleY;

        [FormerlySerializedAs("index_currentAlpha")]
        public int indexCurrentAlpha;

        [FormerlySerializedAs("index_currentCenterX")]
        public int indexCurrentCenterX;

        [FormerlySerializedAs("index_currentCenterY")]
        public int indexCurrentCenterY;

        [FormerlySerializedAs("index_currentLineAlpha")]
        public int indexCurrentLineAlpha;

        [FormerlySerializedAs("index_currentMoveX")]
        public int indexCurrentMoveX;

        [FormerlySerializedAs("index_currentMoveY")]
        public int indexCurrentMoveY;

        [FormerlySerializedAs("index_currentRotate")]
        public int indexCurrentRotate;

        [FormerlySerializedAs("raw_center")] public Vector2 rawCenter;

        public Vector2 center;
        public Color alpha;
        public Color lineAlpha;
        public Vector2 move;
        public Vector2 scale;
        public Quaternion rotation;
        public Vector2 horizontalFineness;
        public Vector2 verticalFineness;

        private void Update()
        {
            UpdateEvents();
        }
        public void SetShowXYPoint(int currentBoxID)
        {
            bool isShowText = false;
            if (currentBoxID == this.currentBoxID) isShowText = true;
            showPointInGameView[^1].lineID = $"{currentBoxID}";
            foreach (var item in showPointInGameView)
            {
                item.isShowText = isShowText;
            }
        }
        /// <summary>
        ///     设置遮罩种子
        /// </summary>
        /// <param name="sortSeed">种子开始</param>
        /// <returns>返回自身</returns>
        public BoxController SetSortSeed(int sortSeed)
        {
            this.sortSeed = sortSeed; //设置我自己的遮罩到我自己
            spriteMask.frontSortingOrder =
                sortSeed + ValueManager.Instance.noteRendererOrder - 1; //遮罩种子+一共多少层-1（这个1是我自己占用了，所以减去）
            spriteMask.backSortingOrder = sortSeed - 1; //遮罩的优先级是前包容后不包容，所以后的遮罩层级向下探一个
            foreach (SpriteRenderer t in spriteRenderers)
            {
                t.sortingOrder = sortSeed; //赋值
            }

            return this; //返回自己
        }
        /// <summary>
        ///     初始化
        /// </summary>
        /// <param name="thisBox">这个方框</param>
        /// <returns></returns>
        public BoxController Init(Box thisBox)
        {
            box = thisBox; //赋值thisBox到box
            mainCamera = Camera.main;
            int lengthDecideLineControllers = decideLineControllers.Length; //获得到当前判定线的数量
            for (int i = 0; i < lengthDecideLineControllers; i++) //遍历
            {
                decideLineControllers[i].ThisLine = box.lines[i]; //将line的源数据赋值过去
            }

            boxFineness = ValueManager.Instance.boxFineness;
            ripples = new ObjectPoolQueue<RippleController>(AssetManager.Instance.ripple, 0, squarePosition);
            return this; //返回自身
        }

        public BoxController SetBoxID(int boxID)
        {
            currentBoxID = boxID;
            boxIDText.text = $"{boxID}";
            return this;
        }

        public void PlayRipple()
        {
            StartCoroutine(Play());
        }

        private IEnumerator Play()
        {
            RippleController ripple = ripples.PrepareObject().Init(currentScaleX, currentScaleY);
            yield return new WaitForSeconds(1.1f); //打击特效时长是0.5秒，0.6秒是为了兼容误差
            ripples.ReturnObject(ripple);
        }

        private void UpdateEvents()
        {
            float currentTime = (float)ProgressManager.Instance.CurrentTime;

            UpdateCenterAndRotation();
            UpdateAlpha();
            UpdateLineAlpha();
            UpdateMove();
            UpdateScale();
            CalculateAllEventCurrentValue(ref currentTime);
        }

        /// <summary>
        ///     根据谱面数据更新当前所有事件
        /// </summary>
        /// <param name="currentTime">当前时间</param>
        private void CalculateAllEventCurrentValue(ref float currentTime)
        {
            lastCurrentMoveX = currentMoveX;
            lastCurrentMoveY = currentMoveY;
            lastCurrentCenterX = currentCenterX;
            lastCurrentCenterY = currentCenterY;
            lastCurrentScaleX = currentScaleX;
            lastCurrentScaleY = currentScaleY;
            lastCurrentRotate = currentRotate;
            lastCurrentAlpha = currentAlpha;
            lastCurrentLineAlpha = currentLineAlpha;
            currentMoveX = CalculateCurrentValue(box.boxEvents.moveX, ref currentTime, ref currentMoveX);
            currentMoveY = CalculateCurrentValue(box.boxEvents.moveY, ref currentTime, ref currentMoveY);
            currentCenterX = CalculateCurrentValue(box.boxEvents.centerX, ref currentTime, ref currentCenterX);
            currentCenterY = CalculateCurrentValue(box.boxEvents.centerY, ref currentTime, ref currentCenterY);
            currentRotate = CalculateCurrentValue(box.boxEvents.rotate, ref currentTime, ref currentRotate);
            currentAlpha = CalculateCurrentValue(box.boxEvents.alpha, ref currentTime, ref currentAlpha);
            currentLineAlpha = CalculateCurrentValue(box.boxEvents.lineAlpha, ref currentTime, ref currentLineAlpha);
            currentScaleX = CalculateCurrentValue(box.boxEvents.scaleX, ref currentTime, ref currentScaleX);
            currentScaleY = CalculateCurrentValue(box.boxEvents.scaleY, ref currentTime, ref currentScaleY);
        }

        /// <summary>
        ///     计算当前数值
        /// </summary>
        /// <param name="events"></param>
        /// <returns></returns>
        private static float CalculateCurrentValue(List<Event> events, ref float currentTime, ref float defaultValue)
        {
            if (events.Count <= 0 || currentTime < events[0].startTime)
            {
                return defaultValue;
            }

            int eventIndex = Algorithm.BinarySearch(events, IsCurrentEvent, true, ref currentTime); //找到当前时间下，应该是哪个事件

            if (currentTime > events[eventIndex].endTime)
            {
                if (events[eventIndex].endValue == 0)
                {
                    return -.0001f;
                }
                else
                {
                    return events[eventIndex].endValue;
                }
            }

            return GameUtility.GetValueWithEvent(events[eventIndex], currentTime); //拿到事件后根据时间Get到当前值

            static bool IsCurrentEvent(Event m, ref float currentTime)
            {
                return currentTime >= m.startTime;
            }
        }

        private void UpdateCenterAndRotation()
        {
            if (Mathf.Approximately(lastCurrentCenterX, currentCenterX) &&
                Mathf.Approximately(lastCurrentCenterY, currentCenterY) &&
                Mathf.Approximately(lastCurrentRotate, currentRotate))
            {
                return;
            }

            rawCenter.x = currentCenterX;
            rawCenter.y = currentCenterY;
            center = mainCamera.ViewportToWorldPoint(rawCenter);
            rotation = Quaternion.Euler(Vector3.forward * currentRotate);
            transform.SetPositionAndRotation(center, rotation);
        }

        private void UpdateAlpha()
        {
            if (Mathf.Approximately(lastCurrentAlpha, currentAlpha))
            {
                return;
            }

            alpha.a = currentAlpha;
            spriteRenderers[0].color =
                spriteRenderers[1].color =
                    spriteRenderers[2].color =
                        spriteRenderers[3].color = alpha; //1234根线赋值，这里的0，0，0就是黑色的线

            //for (int i = 0; i < 4; i++)
            //{
            //    spriteRenderers[i].color = new(spriteRenderers[i].color.r, spriteRenderers[i].color.g, spriteRenderers[i].color.b, currentAlpha);
            //}
            
        }

        private void UpdateLineAlpha()
        {
            if (Mathf.Approximately(lastCurrentLineAlpha, currentLineAlpha))
            {
                return;
            }

            lineAlpha.a = currentLineAlpha;
            spriteRenderers[4].color = lineAlpha;
        }

        private void UpdateMove()
        {
            if (Mathf.Approximately(lastCurrentMoveX, currentMoveX) &&
                Mathf.Approximately(lastCurrentMoveY, currentMoveY))
            {
                return;
            }

            move.x = currentMoveX;
            move.y = currentMoveY;
            squarePosition.localPosition = move;
        }

        private void UpdateScale()
        {
            if (Mathf.Approximately(lastCurrentScaleX, currentScaleX) &&
                Mathf.Approximately(lastCurrentScaleY, currentScaleY))
            {
                return;
            }

            scale.x = currentScaleX;
            scale.y = currentScaleY;
            UpdateFineness();
            squarePosition.localScale = scale;
        }

        private void UpdateFineness()
        {
            horizontalFineness.x = 2 - boxFineness / currentScaleX;
            horizontalFineness.y = boxFineness / currentScaleY;
            //缩放图片，保持视觉上相等
            spriteRenderers[0].transform.localScale = //第125根线都是水平的
                spriteRenderers[1].transform.localScale =
                    spriteRenderers[4].transform.localScale = horizontalFineness;

            verticalFineness.x = 2 + boxFineness / currentScaleY;
            verticalFineness.y = boxFineness / currentScaleX;
            spriteRenderers[2].transform.localScale = //第34都是垂直的
                spriteRenderers[3].transform.localScale = verticalFineness;
            //这里的2是初始大小*2得到的结果，初始大小就是Prefabs里的
        }
    }
}