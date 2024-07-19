using Data.Enumerate;
using Manager;
using UnityEngine;
namespace Controller
{
    public class HoldController : NoteController
    {
        public Transform holdBody;//音符身体
        public Transform holdHead;//音符头

        public float remainTime;//停留时间

        public bool reJudge;//如果已经可以重新播放特效了，有没有重新播放，默认为没有
        public bool isMissed;//Miss掉了
        public float reJudgeTime;//距离上一次打击特效播放已经过去多久了
        public float checkTime = -.1f;//手指离开了多长时间

        private NoteJudge noteJudge;
        private bool isEarly;
        /// <summary>
        /// 初始化
        /// </summary>
        public override void Init()
        {
            AnimationCurve localOffset = decideLineController.canvasLocalOffset;//拿到位移图的索引
            holdBody.transform.localScale = //设置缩放
                Vector2.up * (localOffset.Evaluate(thisNote.EndTime) - localOffset.Evaluate(thisNote.hitTime)) + Vector2.right;
            isMissed = false;   //重置状态
            reJudge = false;    //重置状态
            checkTime = -.1f;   //重置状态
            reJudgeTime = 0;    //重置状态
            noteJudge = NoteJudge.Miss;//重置状态
            isEarly = true;//重置状态
            base.Init();
        }
        public override void Judge(double currentTime, TouchPhase touchPhase)
        {
            switch (touchPhase)//如果触摸阶段
            {
                case TouchPhase.Began://是开始阶段
                    TouchPhaseBegan();
                    break;
                default://剩下的
                    checkTime = Time.time;//更新时间
                    reJudgeTime += Time.deltaTime;//累加重新判定时间
                    if (reJudgeTime >= ValueManager.Instance.holdHitEffectCdTime)//如果已经到了这顶的时间，那就
                    {
                        reJudgeTime = 0;//重置为0
                        reJudge = true;//设置状态为可以重新播放打击特效了
                    }
                    break;
            }
        }
        public override void Judge()
        {
            switch (isJudged)
            {
                case true:
                    Judge(thisNote.hitTime, TouchPhase.Moved);
                    break;
                case false:
                    Judge(thisNote.hitTime, TouchPhase.Began);
                    break;
            }


        }
        /// <summary>
        /// 开始阶段
        /// </summary>
        private void TouchPhaseBegan()
        {
            switch (isJudged)
            {
                case true://如果isJudge为True，说明是抬起来再按回去的，直接播放打击特效
                    PlayEffectWithoutAddScore(NoteJudge.Perfect, ValueManager.Instance.perfectJudge, true);
                    break;
                case false://如果没有判定过并且触摸阶段是开始触摸
                    isJudged = true;//设置状态为判定过了
                    checkTime = Time.time;//设置时间
                    CompletedJudge();
                    JudgeLevel(out noteJudge, out isEarly);//获得到判定等级
                    PlayEffectWithoutAddScore(noteJudge, GetColorWithNoteJudge(noteJudge), isEarly);
                    break;
            }
        }
        /// <summary>
        /// 判定等级
        /// </summary>
        /// <param name="noteJudge">判定等级</param>
        /// <param name="isEarly">是否过早</param>
        protected override void JudgeLevel(out NoteJudge noteJudge, out bool isEarly)
        {
            base.JudgeLevel(out noteJudge, out isEarly);
            noteJudge = noteJudge switch
            {
                // NoteJudge.Bad => NoteJudge.Early,//过滤bad为Good判定
                _ => noteJudge
            };
        }
        /// <summary>
        /// Hold音符Miss掉了
        /// </summary>
        private void HoldMiss()
        {
            ChangeColor(new Color(1, 1, 1, .3f));//Miss掉了就设置透明度为30%
        }
        public override void ReturnPool()
        {
            PlayEffect(noteJudge, GetColorWithNoteJudge(noteJudge), isEarly);
        }
        public override void NoteHoldArise()
        {
            //这里放“现在是通过遮罩做的，我想的是，未来可不可以去掉遮罩，做成上下自动检测拥有停留时间”中的内容
            //***************************************************************************************

            if (checkTime > 0 && isJudged)//chechTime大于0说明手指有判定了，并且已经判定过了
            {
                if (Time.time - checkTime > ValueManager.Instance.holdLeaveScreenTime && !isMissed)//如果当前时间距离手指在我这里判定的最后一帧已经超过预先设置的时间了并且没有Miss
                {
                    isMissed = true;//这个条件下肯定已经Miss了，设置状态
                    HoldMiss();//调用Miss函数
                }
                else if (Time.time - checkTime <= ValueManager.Instance.holdHitEffectCdTime && !isMissed && reJudge)//如果当前时间距离手指在我这里判定的最后一帧没有超过预先设置的时间并且没有Miss和可以进行重判了
                {
                    //checkTime = Time.time;
                    //没有Miss
                    //打击特效
                    PlayEffectWithoutAddScore(NoteJudge.Perfect, ValueManager.Instance.perfectJudge, true);
                    reJudge = false;//重判一次完成后就设置状态
                }
            }
            if( !(ProgressManager.Instance.CurrentTime >= thisNote.hitTime + JudgeManager.Bad) || isJudged || isMissed )//如果当前时间已经超过了打击时间+bad时间（也就是一开始就没有按下手指）并且没有判定过并且没有Miss
                return;
            isMissed = true;//这个条件下肯定已经Miss了，设置状态
            HoldMiss();//调用Miss函数
        }

        public override void PassHitTime(double currentTime)
        {
            AnimationCurve localOffset = decideLineController.canvasLocalOffset;//拿到位移图的索引
            transform.localPosition = new Vector2(transform.localPosition.x, -noteCanvas.localPosition.y);//将位置保留到判定线的位置
            holdBody.transform.localScale = new Vector2(1, localOffset.Evaluate(thisNote.EndTime) - localOffset.Evaluate((float)currentTime));//设置缩放
        }
    }
}
