using Manager;
using Scenes.DontDestroyOnLoad;
using System.Collections.Generic;
using UnityEngine;
using UtilityCode.Algorithm;
using UtilityCode.Singleton;

namespace Controller
{
    public class AutoplayController : MonoBehaviourSingleton<AutoplayController>
    {
        public SpeckleManager speckleManager;
        public float currentH;

        private void Start()
        {
            switch (GlobalData.Instance.isAutoplay)
            {
                case true:
                    speckleManager.enabled = false;
                    break;
                case false:
                    enabled = false;
                    break;
            }
        }

        private void Update()
        {
            foreach (LineNoteController lineNoteController in speckleManager.allLineNoteControllers)
            {
                FindPassHitTimeNotes(lineNoteController.ariseOnlineNotes);
                FindPassHitTimeNotes(lineNoteController.ariseOfflineNotes);
            }

            if (currentH >= 1)
            {
                currentH = 0;
            }

            currentH += Time.deltaTime;
            ValueManager.Instance.perfectJudge = Color.HSVToRGB(currentH, 1, 1);
        }

        /// <summary>
        ///     音符过了打击时间但是没有Miss掉的这个期间每一帧调用
        /// </summary>
        /// <param name="ariseNotes">需要调用的列表</param>
        private void FindPassHitTimeNotes(List<NoteController> ariseNotes)
        {
            if (ariseNotes.Count <= 0)
            {
                return;
            }

            int endIndex = Algorithm.BinarySearch(ariseNotes,
                m => m.thisNote.hitTime < ProgressManager.Instance.CurrentTime + .00001,
                false); //寻找音符过了打击时间但是没有Miss掉的音符
            for (int i = endIndex - 1; i >= 0; i--)
            {
                ariseNotes[i].Judge();
            }
        }
    }
}