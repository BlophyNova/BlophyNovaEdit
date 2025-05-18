using System.Collections;
using System.Collections.Generic;
using Manager;
using Scenes.DontDestroyOnLoad;
using UnityEngine;
using UtilityCode.Singleton;

namespace Controller
{
    public class GameController : MonoBehaviourSingleton<GameController> /*,IRefresh*/
    {
        public bool isLoading;
        public List<BoxController> boxes;
        public int currentBoxID;

        private IEnumerator Start()
        {
            yield return new WaitUntil(() =>
                GlobalData.Instance.chartData.metaData.musicLength > 1 &&
                GlobalData.Instance.chartData.boxes.Count > 0);
            isLoading = false;
            InstNewBox();
            yield return new WaitForSeconds(1); //等1秒
            StateManager.Instance.IsStart = true; //设置状态IsStart为True
            StateManager.Instance.IsPause = true;
        }

        private void Update()
        {
            if (!(GlobalData.Instance.chartData.metaData.musicLength - ProgressManager.Instance.CurrentTime <= .1f) ||
                isLoading)
            {
                return;
            }

            isLoading = true;
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
                    .SetSortSeed(i * ValueManager.Instance
                        .noteRendererOrder) //这里的3是每一层分为三小层，第一层是方框渲染层，第二和三层是音符渲染层，有些音符占用两个渲染层，例如Hold，FullFlick
                    .SetBoxID(i)
                    .Init(AssetManager.Instance.chartData.boxes[i]);
                for (int j = 0; j < newItem.decideLineControllers.Length; j++)
                {
                    newItem.decideLineControllers[j].lineNoteController.currentBoxID = i;
                    newItem.decideLineControllers[j].lineNoteController.currentLineID = j;
                    newItem.decideLineControllers[j].currentBoxID = i;
                    newItem.decideLineControllers[j].currentLineID = j;
                }

                boxes.Add(newItem);
            }

            boxes[currentBoxID].SetShowXYPoint(currentBoxID);
        }

        public void RefreshChartPreview()
        {
            InstNewBox();
        }

        public void ChangeShowXYPoint(int currentBoxID)
        {
            this.currentBoxID = currentBoxID;
            foreach (BoxController item in boxes)
            {
                item.SetShowXYPoint(currentBoxID);
            }
        }
    }
}