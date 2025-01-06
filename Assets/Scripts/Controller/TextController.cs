using Data.ChartData;
using Manager;
using System.Collections;
using TMPro;
using UnityEngine;
using UtilityCode.GameUtility;
using UtilityCode.ObjectPool;

namespace Controller
{
    public class TextController : MonoBehaviour
    {
        public TextMeshProUGUI thisTextMeshProUGUI;
        public Text thisText;
        public ObjectPoolQueue<TextController> textObjectPool;
        public Camera mainCamera;
        public Vector2 currentRawPosition;

        private void Update()
        {
            float currentTime = (float)ProgressManager.Instance.CurrentTime;
            currentRawPosition.Set(
                GameUtility.GetValueWithEvent(thisText.moveX, currentTime),
                GameUtility.GetValueWithEvent(thisText.moveY, currentTime));
            transform.position = (Vector2)mainCamera.ViewportToWorldPoint(currentRawPosition);
        }

        public TextController Init(Text thisText, ObjectPoolQueue<TextController> textObjectPool)
        {
            currentRawPosition = Vector2.zero;
            mainCamera = Camera.main;
            this.textObjectPool = textObjectPool;
            this.thisText = thisText;
            thisTextMeshProUGUI.text = thisText.text;
            thisTextMeshProUGUI.fontSize = thisText.size;
            StartCoroutine(ReturnObjectPool());
            return this;
        }

        private IEnumerator ReturnObjectPool()
        {
            yield return new WaitForSeconds(thisText.endTime - thisText.startTime);
            textObjectPool.ReturnObject(this);
        }
    }
}