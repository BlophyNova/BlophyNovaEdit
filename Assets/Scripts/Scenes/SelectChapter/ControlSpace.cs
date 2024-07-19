using Scenes.DontDestoryOnLoad;
using Scenes.PublicScripts;
using UnityEngine;
namespace Scenes.SelectChapter
{
    public class ControlSpace : PublicControlSpace
    {

        protected override void Send()
        {
            GlobalData.Instance.currentChapterIndex = currentElementIndex;
            GlobalData.Instance.currentChapter = GlobalData.Instance.chapters[currentElementIndex].chapterName;
        }
        protected override void ListUpdate()
        {
            if (Input.touchCount > 0 &&
                Input.GetTouch(0).phase == TouchPhase.Ended)
            {
                float[] allElementDistanceWithFinger = new float[elementCount];//手指抬手的时候，所有元素距离当前值的距离（取绝对值）
                for (int i = 0; i < elementCount; i++)
                {
                    allElementDistanceWithFinger[i] =
                        Mathf.Abs(verticalBar.value - Single * i);
                }

                int minValue = 0;
                for (int i = 1; i < allElementDistanceWithFinger.Length; i++)
                {
                    minValue = allElementDistanceWithFinger[i] < allElementDistanceWithFinger[i - 1] ? i : minValue;//判断哪个元素距离当前值最小
                }
                currentElement = allElementDistance[minValue];//复制索引值
                //GlobalData.Instance.SelectChapter_CurrentChapter = minValue;
                currentElementIndex = elementCount - 1 - minValue;
                Send();
                StartCoroutine(Lerp());
            }
        }

    }
}
