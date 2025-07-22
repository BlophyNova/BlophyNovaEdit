using System;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using Data.ChartData;
using Data.Enumerate;
using Manager;
using UnityEngine;
using UnityEngine.Serialization;

namespace Controller
{
    public class NoteController : MonoBehaviour
    {
        public List<RenderOrder> renderOrder; //渲染层级

        [FormerlySerializedAs("length_renderOrder")]
        public int lengthRenderOrder = -1; //一共多少层

        public Note thisNote; //负责储存这个音符的一些数据
        public DecideLineController decideLineController; //判定线引用
        public SpriteRenderer texture;
        public Sprite commonTexture;
        public Sprite multiTexture;

        public bool isOnlineNote; //是否事判定线上方的音符
        public bool isJudged; //是否已经判定过了

        public Transform noteCanvas; //音符画布的引用

        protected int LengthRenderOrder
        {
            get
            {
                if (lengthRenderOrder < 0) //如果小于0说明没有调用过
                {
                    lengthRenderOrder = renderOrder.Count; //赋值
                }

                return lengthRenderOrder; //返回
            }
        }

        protected float PointNoteCurrentOffset =>
            decideLineController.canvasLocalOffset.Evaluate((float)ProgressManager.Instance
                .CurrentTime); //根据当前速度计算出按照当前的位移就是现在的Point应该在的位置

        /// <summary>
        ///     从对象池出来召唤一次
        /// </summary>
        public virtual void Init()
        {
            MultiOrCommon();
            ChangeColor(Color.white); //初始化为白色
            isJudged = false; //重置isJudged
            //syncAlpha
            if (thisNote.syncAlpha && thisNote.isFakeNote) return;
            VisualTime();
            Offset();
        } //初始化方法

        public virtual void Hide()
        {
            if (thisNote.noteType is NoteType.FullFlickPink or NoteType.FullFlickBlue or NoteType.Hold or NoteType.Point) return;
            texture.color = Color.clear;
        }
        public virtual void Show()
        {
            if (thisNote.noteType is NoteType.FullFlickPink or NoteType.FullFlickBlue or NoteType.Hold or NoteType.Point) return;
            texture.color = Color.white;
        }

        public virtual async void Offset()
        {
            if (thisNote.visualTime > 0) return;
            if (thisNote.offset == 0)
            {
                Show();
                return;
            }
            Hide();
            float offsetCanvas = noteCanvas.localPosition.y - thisNote.offset;
            while (noteCanvas.localPosition.y >= offsetCanvas)
            {
                transform.localPosition = new(thisNote.positionX, PointNoteCurrentOffset + 2);
                await UniTask.NextFrame();
            }
            //await UniTask.WaitUntil(()=> noteCanvas.localPosition.y <= offsetCanvas);
            Vector2 before = new(thisNote.positionX, thisNote.hitFloorPosition);
            //Vector2 after = Vector2.up * (thisNote.hitFloorPosition + thisNote.offset);
            float showTime = (float)ProgressManager.Instance.CurrentTime;
            Show();
            while (true)
            {
                float currentTime = (float)ProgressManager.Instance.CurrentTime;
                if (currentTime >= thisNote.hitTime)
                {
                    break;
                }

                float percentage = (currentTime - showTime) / (thisNote.hitTime - showTime);
                transform.localPosition = before + (1 - percentage) * thisNote.offset * Vector2.up;
                await UniTask.NextFrame();
            }
        }
        public virtual async void VisualTime()
        {
            if (thisNote.visualTime <= 0)
            {
                Show();
                return;
            }
            Hide();
            await UniTask.WaitUntil(() => ProgressManager.Instance.CurrentTime > thisNote.hitTime - thisNote.visualTime);
            Show();
        }
        public virtual void SetAlpha(Color color)
        {
            if (thisNote.noteType is NoteType.FullFlickPink or NoteType.FullFlickBlue or NoteType.Hold or NoteType.Point) return;
            texture.color = color;
        }
        protected virtual void MultiOrCommon()
        {
            texture.sprite = thisNote.hasOther switch
            {
                true => multiTexture,
                false => commonTexture
            };
        }

        /// <summary>
        ///     更改颜色
        /// </summary>
        /// <param name="targetColor"></param>
        protected void ChangeColor(Color targetColor)
        {
            for (int i = 0; i < LengthRenderOrder; i++) //循环渲染层级的每一层
            {
                for (int j = 0; j < renderOrder[i].LengthSpriteRenderers; j++) //循环每一层的所有素材
                {
                    renderOrder[i].tierCount[j].color = targetColor; //改为目标颜色
                }
            }
        }

        /// <summary>
        ///     这里是被子类重写的方法，用于执行某些音符判定后特性的方法(非空)
        /// </summary>
        public virtual void Judge(double currentTime, TouchPhase touchPhase)
        {
            ReturnObjectPool(); //返回对象池
        }

        /// <summary>
        ///     这里是被子类重写的方法，用于Autoplay判定
        /// </summary>
        public virtual void Judge()
        {
            ReturnObjectPool(); //返回对象池
        }

        /// <summary>
        ///     吧音符返回对象池
        /// </summary>
        private void ReturnObjectPool()
        {
            switch (isOnlineNote)
            {
                //看看自己是线上的音符还是线下的音符
                case true:
                    //线上的音符的话就从两个线上排序中移除自己
                    decideLineController.lineNoteController.ariseOnlineNotes.Remove(this); //hitTime排序中移除自己
                    decideLineController.lineNoteController.endTimeAriseOnlineNotes.Remove(this); //endTime排序中移除自己
                    break;
                case false:
                    //线下的音符的话就从两个线下排序中移除自己
                    decideLineController.lineNoteController.ariseOfflineNotes.Remove(this); //hitTime排序中移除自己
                    decideLineController.lineNoteController.endTimeAriseOfflineNotes.Remove(this); //endTime排序中移除自己
                    break;
            }

            ReturnPool(); //调用返回对象池才会调用的方法
            decideLineController.ReturnNote(this, thisNote.noteType, isOnlineNote); //把自己返回对象池
        }

        /// <summary>
        ///     如果当前音符超过了打击时间并且没有销毁的这段时间，每帧调用（非空）
        /// </summary>
        /// <param name="currentTime">当前时间</param>
        public virtual void PassHitTime(double currentTime)
        {
            //UIManager.Instance.DebugTextString = $"我是{thisNote.noteType},我应该在第{thisNote.hitTime}被打击，我是PassHitTime触发的";
            float currentAlpha =
                (float)(currentTime - thisNote.hitTime) /
                thisNote.HoldTime; //当前时间-打击时间/持续时间  可以拿到当前时间相对于打击时间到Miss这段时间的百分比
            for (int i = 0; i < LengthRenderOrder; i++) //遍历每一层渲染层
            {
                for (int j = 0; j < renderOrder[i].LengthSpriteRenderers; j++) //遍历每一层中需要动手脚的素材
                {
                    Color changeBeforeColor = renderOrder[i].tierCount[j].color; //记录一下修改前的Color值
                    renderOrder[i].tierCount[j].color = //rgb保持不变，当前alpha=1-currentAlpha
                        new Color(changeBeforeColor.r, changeBeforeColor.g, changeBeforeColor.b, 1 - currentAlpha);
                }
            }
        }

        /// <summary>
        ///     音符出现的时候每帧调用（空）
        /// </summary>
        public virtual void NoteHoldArise()
        {
        }

        /// <summary>
        ///     返回对象池调用一次（非空）
        /// </summary>
        public virtual void ReturnPool()
        {
            JudgeLevel(out NoteJudge noteJudge, out bool isEarly); //获得到判定等级，Perfect？Early？Bad？Miss？Early？Late？
            Color hitJudgeEffectColor = GetColorWithNoteJudge(noteJudge); //根据判定等级获得到打击特效的颜色

            PlayEffect(noteJudge, hitJudgeEffectColor, isEarly);
        }

        /// <summary>
        ///     完成判定调用的方法
        /// </summary>
        protected void CompletedJudge()
        {
            HitSoundManager.Instance.PlayHitSound(thisNote.noteType);
        }

        protected virtual void PlayRipple()
        {
            decideLineController.box.PlayRipple();
        }

        protected virtual void PlayEffect(NoteJudge noteJudge, Color hitJudgeEffectColor, bool isEarly)
        {
            PlayEffectWithoutAddScore(noteJudge, hitJudgeEffectColor, isEarly);
        }

        protected virtual void PlayEffectWithoutAddScore(NoteJudge noteJudge, Color hitJudgeEffectColor, bool isEarly)
        {
            switch (noteJudge) //判定等级枚举
            {
                case NoteJudge.Miss: //如果是Miss则不播放打击特效
                    break;
                default:
                    PlayEffectWithoutMiss(hitJudgeEffectColor);
                    break;
            }
        }

        private void PlayEffectWithoutMiss(Color hitJudgeEffectColor)
        {
            if (thisNote.effect.HasFlag(NoteEffect.Ripple))
            {
                PlayRipple();
            }

            if (thisNote.effect.HasFlag(NoteEffect.CommonEffect))
            {
                PlayHitEffectWithJudgeLevel(hitJudgeEffectColor); //根据判定等级播放打击特效
            }
        }

        /// <summary>
        ///     根据判定等级播放打击特效
        /// </summary>
        /// <param name="hitJudgeEffectColor">判定等级对应的颜色</param>
        protected virtual void PlayHitEffectWithJudgeLevel(Color hitJudgeEffectColor)
        {
            if (isOnlineNote)
            {
                HitEffectManager.Instance.PlayHitEffect(
                    transform.TransformPoint(Vector3.down * (decideLineController.onlineNote.transform.localPosition.y +
                                                             transform.localPosition.y)), transform.rotation,
                    hitJudgeEffectColor); //播放打击特效
            }
            else
            {
                HitEffectManager.Instance.PlayHitEffect(
                    transform.TransformPoint(Vector3.down * (decideLineController.onlineNote.transform.localPosition.y -
                                                             transform.localPosition.y)), transform.rotation,
                    hitJudgeEffectColor); //播放打击特效
            }
        }

        /// <summary>
        ///     根据音符判定等级获得到颜色
        /// </summary>
        /// <param name="noteJudge"></param>
        /// <returns></returns>
        protected static Color GetColorWithNoteJudge(NoteJudge noteJudge)
        {
            return noteJudge switch
            {
                NoteJudge.Perfect => ValueManager.Instance.perfectJudge, //如果是P、G、B就拿到对应的颜色
                NoteJudge.Early => ValueManager.Instance.goodJudge, //如果是P、G、B就拿到对应的颜色
                NoteJudge.Bad => ValueManager.Instance.badJudge, //如果是P、G、B就拿到对应的颜色
                _ => ValueManager.Instance.otherJudge //其他则Other，这个颜色的出现代表有bug
            };
        }

        /// <summary>
        ///     判定等级
        /// </summary>
        /// <param name="noteJudge">输出判定等级</param>
        /// <param name="isEarly">输出早还是晚</param>
        protected virtual void JudgeLevel(out NoteJudge noteJudge, out bool isEarly)
        {
            float currentTime = (float)ProgressManager.Instance.CurrentTime; //获取到当前时间
            noteJudge = NoteJudge.Miss; //默认Miss
            isEarly = true; //默认是早的
            if (currentTime < thisNote.hitTime + JudgeManager.Perfect &&
                currentTime > thisNote.hitTime - JudgeManager.Perfect) //如果在hitTime+perfect和hitTime-perfect之间
            {
                noteJudge = NoteJudge.Perfect; //完美判定
            }
            else if (currentTime < thisNote.hitTime && //如果在打击时间-good到打击时间之间
                     currentTime > thisNote.hitTime - JudgeManager.Good)
            {
                noteJudge = NoteJudge.Early; //Good判定，Early默认是True，所以这里不理会isEarly
            }
            else if (currentTime < thisNote.hitTime + JudgeManager.Good &&
                     currentTime > thisNote.hitTime) //如果在打击时间+good到打击时间之间
            {
                noteJudge = NoteJudge.Late; //Good判定
            }
            else if (currentTime < thisNote.hitTime && //如果在打击时间-bad到打击时间之间
                     currentTime > thisNote.hitTime - JudgeManager.Bad)
            {
                noteJudge = NoteJudge.Bad; //Bad判定
            }
            /* LateBad是个什么东西啊喂
            else if (isJudged)//如果是坏判定，晚
            {
                noteJudge = NoteJudge.Bad;//Bad判定
                isEarly = false;//LateBad设置
            }
            */
        }

        /// <summary>
        ///     判定触摸是否在音符的数轴判定范围内（非空）
        /// </summary>
        /// <param name="currentPosition">当前位置</param>
        /// <returns>是否在判定范围内</returns>
        public virtual bool IsinRange(Vector2 currentPosition)
        {
            //float onlineJudge = ValueManager.Instance.onlineJudgeRange;
            //float offlineJudge = ValueManager.Instance.offlineJudgeRange;

            float inThisLine = transform.InverseTransformPoint(currentPosition).x; //将手指的世界坐标转换为局部坐标后的x拿到

            return inThisLine <= ValueManager.Instance.noteRightJudgeRange && //如果x介于ValueManager设定的数值之间
                   inThisLine >= ValueManager.Instance.noteLeftJudgeRange;
            //UIManager.Instance.DebugTextString = $"onlineJudge:{onlineJudge}||offlineJudge:{offlineJudge}||Result:true ||inThisLine:{inThisLine}";
            //返回是
            //UIManager.Instance.DebugTextString = $"onlineJudge:{onlineJudge}||offlineJudge:{offlineJudge}||Result:false||inThisLine:{inThisLine}";
            //返回否
        }
    }

    [Serializable]
    public class RenderOrder //每一层渲染层
    {
        public List<SpriteRenderer> tierCount; //每一层渲染层下的所有需要修改的渲染组件

        [FormerlySerializedAs("length_tierCount")]
        public int lengthTierCount = -1; //长度

        public int LengthSpriteRenderers
        {
            get
            {
                if (lengthTierCount < 0) //如果小于0说明没有调用过
                {
                    lengthTierCount = tierCount.Count; //调用一次
                }

                return lengthTierCount; //返回
            }
        }
    }
}