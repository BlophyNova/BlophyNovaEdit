using Form.LabelWindow;
using UnityEngine.UI;

namespace Form.QuickOpen
{
    public class QuickOpen : LabelWindowContent
    {
        public Button bpmList;
        public Button notePropertyEdit;
        public Button boxList;
        public Button associateLabelWindow;

        private void Start()
        {
            bpmList.onClick.AddListener(() => { GetT<BPMList.BPMList>().labelItem.labelButton.ThisLabelGetFocus(); });
            notePropertyEdit.onClick.AddListener(() =>
            {
                GetT<NotePropertyEdit.NotePropertyEdit>().labelItem.labelButton.ThisLabelGetFocus();
            });
            boxList.onClick.AddListener(() => { GetT<BoxList.BoxList>().labelItem.labelButton.ThisLabelGetFocus(); });
            associateLabelWindow.onClick.AddListener(() =>
            {
                GetT<AssociateLabelWindow.AssociateLabelWindow>().labelItem.labelButton.ThisLabelGetFocus();
            });
        }

        private T GetT<T>() where T : LabelWindowContent
        {
            foreach (LabelItem labelItem in labelWindow.labels)
            {
                if (labelItem.labelWindowContent is T)
                {
                    return (T)labelItem.labelWindowContent;
                }
            }

            return null;
        }
    }
}