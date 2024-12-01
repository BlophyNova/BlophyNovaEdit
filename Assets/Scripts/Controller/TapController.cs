using UnityEngine;

namespace Controller
{
    public class TapController : NoteController
    {
        /// <summary>
        ///     被成功判定后调用
        /// </summary>
        /// <param name="currentTime">当前时间</param>
        /// <param name="touchPhase">触摸阶段</param>
        public override void Judge(double currentTime, TouchPhase touchPhase)
        {
            isJudged = true; //修改属性为成功判定
            CompletedJudge();
            base.Judge(currentTime, touchPhase); //执行基类的判定方法
        }

        public override void Judge()
        {
            isJudged = true;
            CompletedJudge();
            base.Judge();
        }
    }
}