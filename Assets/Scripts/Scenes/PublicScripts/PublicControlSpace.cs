using System.Collections;
using UnityEngine;
using UnityEngine.UI;
namespace Scenes.PublicScripts
{
    public class PublicControlSpace : MonoBehaviour
    {
        public Scrollbar verticalBar;
        public float progressBar;
        public int elementCount;
        public float[] allElementDistance;//所有元素标准的距离（0-1之间的数据）
        protected float Single => 1f / (elementCount - 1);
        public int currentElementIndex;
        public float currentElement;
        // Update is called once per frame
        private void Start()
        {

            OnStart();
            InitAllElementDistance();
            Send();
        }

        protected void InitAllElementDistance()
        {
            allElementDistance = new float[elementCount];//所有元素标准的距离（0-1之间的数据）
            for (int i = 0; i < elementCount; i++)
            {
                allElementDistance[i] = Single * i;
            }
        }

        protected virtual void OnStart() { }
        private void Update()
        {
            ListUpdate();
            LargeImageUpdate();
        }
        protected virtual void LargeImageUpdate() { }
        protected virtual void ListUpdate() { }
        protected virtual void Send() { }
        protected IEnumerator Lerp()
        {
            while (Mathf.Abs(verticalBar.value - currentElement) > .0001f)
            {
                verticalBar.value = Mathf.Lerp(verticalBar.value, currentElement, .1f);
                yield return new WaitForEndOfFrame();
            }
        }
    }
}
