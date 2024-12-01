using Scenes.PublicScripts;

namespace Form.LabelWindow
{
    public class LabelButton : PublicButton
    {
        public LabelItem labelItem;

        private void Start()
        {
            thisButton.onClick.AddListener(ThisLabelGetFocus);
        }

        public void ThisLabelGetFocus()
        {
            labelItem.labelWindow.currentLabelItem.LabelLostFocus();
            labelItem.labelWindow.currentLabelItem = labelItem;
            labelItem.labelWindow.currentLabelItem.LabelGetFocus();
            //foreach (var item in labelItem.labelWindow.labels)
            //{
            //    item.labelWindowContent.gameObject.SetActive(false);
            //}
            //labelItem.labelWindowContent.gameObject.SetActive(true);
            labelItem.labelWindowContent.transform.SetAsLastSibling();
        }
    }
}