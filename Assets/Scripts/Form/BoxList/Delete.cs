using Scenes.DontDestoryOnLoad;
using Scenes.PublicScripts;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace Form.BoxList
{
    public class Delete : PublicButton
    {
        public BoxListItem boxListItem;
        private void Start()
        {
            thisButton.onClick.AddListener(() => 
            {
                boxListItem.boxList.boxListItems.Remove(boxListItem);
                GlobalData.Instance.chartEditData.boxes.Remove(boxListItem.thisBox);
                Destroy(boxListItem.gameObject); 
                GlobalData.Refresh<IRefresh>((interfaceMethod) => interfaceMethod.Refresh());
            });
        }
    }
}
