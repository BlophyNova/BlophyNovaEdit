using System.Collections;
using System.Collections.Generic;
using Manager;
using Scenes.DontDestroyOnLoad;
using UnityEngine;
using UtilityCode.Singleton;
namespace Controller
{
    public class GameController : MonoBehaviourSingleton<GameController>/*,IRefresh*/
    {
        public bool isLoading;
        public List<BoxController> boxes;
        private IEnumerator Start()
        {
            isLoading = false;
            InstNewBox();
            yield return new WaitForSeconds(3);//等8秒
            StateManager.Instance.IsStart = true;//设置状态IsStart为True

        }

        private void InstNewBox()
        {
            foreach (BoxController item in boxes)
            {
                Destroy(item.gameObject);
            }
            boxes.Clear();
            for (int i = 0; i < AssetManager.Instance.chartData.boxes.Count; i++)
            {
                BoxController newItem = Instantiate(AssetManager.Instance.boxController, AssetManager.Instance.box)
                    .SetSortSeed(i * ValueManager.Instance.noteRendererOrder)//这里的3是每一层分为三小层，第一层是方框渲染层，第二和三层是音符渲染层，有些音符占用两个渲染层，例如Hold，FullFlick
                    .Init(AssetManager.Instance.chartData.boxes[i]);
                boxes.Add(newItem);
            }
        }

        private void Update()
        {
            if( !(GlobalData.Instance.chartData.globalData.musicLength - ProgressManager.Instance.CurrentTime <= .1f) || isLoading )
                return;
            isLoading = true;

        }

        public void RefreshChartPreview()=>InstNewBox();
    }
}
