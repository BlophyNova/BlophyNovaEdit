using Data.Enumerate;
using Manager;
using Scenes.DontDestoryOnLoad;
using UnityEngine;
namespace Controller
{
    public class DragController : NoteController
    {
        public override void Judge(double currentTime, TouchPhase touchPhase)
        {
            isJudged = true;//设置状态
        }
        public override void Judge()
        {
            isJudged = true;
        }
        public override void PassHitTime(double currentTime)
        {
            base.PassHitTime(currentTime);//执行基类的方法
            if (isJudged)//如果判定成功
            {
                base.Judge(0, TouchPhase.Canceled);//执行判定，因为基类的判定没有用到TouchPhase，所以这里就随便用一个了
            }
        }
        public override void ReturnPool()
        {
            if (!isJudged)//如果没有判定过
            {
                GlobalData.Instance.score.AddScore(thisNote.noteType, NoteJudge.Miss, true);//加Miss分
                return;
            }
            CompletedJudge();
            PlayEffect(NoteJudge.Perfect, ValueManager.Instance.perfectJudge, true);
        }
    }
}
