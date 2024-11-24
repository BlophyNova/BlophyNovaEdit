using Manager;
using UnityEngine;
using UnityEngine.Serialization;
namespace Controller
{
    public class PointController : NoteController
    {
        public float ariseTime;//出现时间
        public Vector2[] origins;//起点
        public Vector2[] destinations;//终点
        private const int JourneyLength = 4;//路程长度，就是说，有多少个需要处理的起点终点，在这里有四个角(左上左下右上右下)，所以答案就是4
        [FormerlySerializedAs("move_edgeCorner")]
        public Transform[] moveEdgeCorner;//需要移动的边缘的角（就是需要向里缩的）
        public SpriteRenderer[] allHorizontal;//水平的渲染器
        public SpriteRenderer[] allVertical;//垂直的渲染器

        public override void Judge(double currentTime, TouchPhase touchPhase)
        {
            isJudged = true;//修改属性为成功判定
            CompletedJudge();
            base.Judge(currentTime, TouchPhase.Canceled);//执行基类的判定方法
        }
        public override void Judge()
        {
            isJudged = true;
            CompletedJudge();
            base.Judge();
        }
        public override void NoteHoldArise()
        {
            transform.localPosition = Vector2.up * PointNoteCurrentOffset;//纠正自己的位置，不会因为画布移动而移动
            SetTextureLocalScale();//缩放所有的横竖贴图
            float percent = ((float)ProgressManager.Instance.CurrentTime - ariseTime) / (thisNote.hitTime - ariseTime);//计算现在是出现时间到打击时间为百分之多少的百分比
            if (percent > 1)//如果大于1
            {
                return;//返回
            }
            for (int i = 0; i < JourneyLength; i++)//循环--遍历所有角，计算位置
            {
                //move_edgeCorner[i].localPosition = (destinations[i] - origins[i]) * (1 - percent) + origins[i];//这是从中间向周围扩散
                moveEdgeCorner[i].localPosition = (destinations[i] - origins[i]) * percent + origins[i];//（终点-起点）*百分比+起点
            }
        }
        /// <summary>
        /// 设置贴图缩放
        /// </summary>
        private void SetTextureLocalScale()
        {
            foreach (SpriteRenderer item in allHorizontal)//水平
            {
                item.transform.localScale = new Vector2(.5f + (ValueManager.Instance.boxFineness / decideLineController.box.currentScaleX), decideLineController.box.spriteRenderers[0].transform.localScale.y);//计算XY并赋值
            }
            foreach (SpriteRenderer item in allVertical)//垂直
            {
                item.transform.localScale = new Vector2(.45f - (ValueManager.Instance.boxFineness / decideLineController.box.currentScaleY), decideLineController.box.spriteRenderers[2].transform.localScale.y);//计算XY并赋值
            }
        }

        public override void Init()
        {
            for (int i = 0; i < LengthRenderOrder; i++)//循环渲染层级的每一层
            {
                for (int j = 0; j < renderOrder[i].LengthSpriteRenderers; j++)//循环每一层的所有素材
                {
                    renderOrder[i].tierCount[j].color = Color.white;//改为白色
                }
            }
            isJudged = false;//是否判定过设为假
            ariseTime = (float)ProgressManager.Instance.CurrentTime;//赋值出现时间
        }
    }
}
