using System.Collections.Generic;
using Controller;
using Data.ChartData;
using UnityEngine;
using UtilityCode.ObjectPool;
using UtilityCode.Singleton;

namespace Manager
{
    public class TextManager : MonoBehaviourSingleton<TextManager>
    {
        public RectTransform textCanvas;
        public List<Text> texts;
        public int lastIndex; //上次召唤到Note[]列表的什么位置了，从上次的位置继续
        public ObjectPoolQueue<TextController> textObjectPool;

        private void Update()
        {
            float currentTime = (float)ProgressManager.Instance.CurrentTime;

            for (int i = lastIndex; i < texts.Count; i++)
            {
                if (!(texts[i].startTime < currentTime))
                {
                    break; // 如果当前文本还没有开始时间，则跳出循环
                }

                textObjectPool.PrepareObject().Init(texts[i], textObjectPool);
                lastIndex++;
            }
        }

        public void Init(List<Text> texts)
        {
            this.texts = texts;
            textObjectPool = new ObjectPoolQueue<TextController>(AssetManager.Instance.text, 0, textCanvas);
            if (texts != null && texts.Count != 0)
            {
                return;
            }

            Destroy(textCanvas.gameObject);
            Destroy(gameObject);
        }
    }
}