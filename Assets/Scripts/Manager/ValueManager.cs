using UnityEngine;
using UnityEngine.Serialization;
using UtilityCode.Singleton;

namespace Manager
{
    public class ValueManager : MonoBehaviourSingleton<ValueManager> //这里存放一些数值相关的东西
    {
        [Header("下面这些值都是人为根据需求拟定")]
        [Tooltip("计算面积的精细度")]
        public float calculatedAreaRange;

        [Tooltip("保留多少位，和上边的有强关联，如0.1就保留1位，0.01就2位，以此类推")]
        public int reservedBits;

        [Tooltip("方框的精细程度")] public float boxFineness; //方框线的精细度

        [Tooltip("每帧划多少算移动中")] public float flickRange;

        [Tooltip("最多处理多少个手指")] public int maxSpeckleCount;

        [Tooltip("手指按下后我要保存多少秒的位置")] public float fingerSavePosition;

        [Tooltip("音符渲染小层，每一大层有多少小层,方框默认占用一层，所以音符赋值的之后长度应当为总长度-1，比如3层，框用一层，音符就只能用两层，代表层级的数组长度就是2而不是3")]
        public int noteRendererOrder;

        [Tooltip("因为编辑器检测屏幕刷新了始终是0，所以这里手动设置编辑器目标FPS")]
        public int editorTargetFPS;

        [FormerlySerializedAs("FPS")]
        [Tooltip("Runtime目标FPS")]
        public int fps;

        [Tooltip("Good判定为Perfect的百分之多少，这里输入0-1之间的数据表示百分比")]
        public float goodJudgePercent;

        [Tooltip("判定线上边的空间判定范围，世界坐标为主")] public float onlineJudgeRange;
        [Tooltip("判定线下边的空间判定范围，世界坐标为主")] public float offlineJudgeRange;
        [Tooltip("音符右边的空间判定范围，世界坐标为主")] public float noteRightJudgeRange;
        [Tooltip("音符左边的空间判定范围，世界坐标为主")] public float noteLeftJudgeRange;

        [FormerlySerializedAs("fullFlick_noteRightJudgeRange")]
        [Tooltip("大滑键音符左边的空间判定范围，世界坐标为主")]
        public float fullFlickNoteRightJudgeRange;

        [FormerlySerializedAs("fullFlick_noteLeftJudgeRange")]
        [Tooltip("大滑键音符左边的空间判定范围，世界坐标为主")]
        public float fullFlickNoteLeftJudgeRange;

        [Tooltip("当前系统的目标帧率")] public int currentTargetFPS;

        [Tooltip("perfect判定的打击特效的颜色")] public Color perfectJudge;
        [Tooltip("good判定的打击特效的颜色")] public Color goodJudge;
        [Tooltip("bad判定的打击特效的颜色")] public Color badJudge;

        [Tooltip("other判定的打击特效的颜色（错误判定，不应该出现，方便Debug）")]
        public Color otherJudge;

        [Tooltip("打击特效的大小")] public float hitEffectScale;

        [Tooltip("Hold音符判定手指离开多长时间重新放回去是为不Miss")]
        public float holdLeaveScreenTime;

        [FormerlySerializedAs("holdHitEffectCDTime")]
        [Tooltip("Hold音符的特效播放间隔多长时间")]
        public float holdHitEffectCdTime; //就是Hold打击特效播放完一次后CD是多少，播放完一次后多少秒内不能继续播放

        [Tooltip("Tap音符权重")] public int tapWeight;
        [Tooltip("Hold音符权重")] public int holdWeight;
        [Tooltip("Drag音符权重")] public int dragWeight;
        [Tooltip("Flick音符权重")] public int flickWeight;
        [Tooltip("FullFlick音符权重")] public int fullFlickWeight;
        [Tooltip("Point音符权重")] public int pointWeight;

        protected override void OnAwake()
        {
            Application.targetFrameRate = fps;
            currentTargetFPS = Application.isEditor switch
            {
                true => editorTargetFPS,
                false => Screen.currentResolution.refreshRate
            };
        }
    }
}