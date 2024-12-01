using Scenes.PublicScripts;

namespace Form.LabelWindow
{
    public class CloseLabel : PublicButton
    {
        public LabelItem labelItem;
        public LabelWindow labelWindow;

        private void Start()
        {
            thisButton.onClick.AddListener(() =>
            {
                labelWindow.labels.Remove(labelItem);
                if (labelItem.labelWindowContent.isActiveAndEnabled)
                {
                    if (labelWindow.labels.Count > 0)
                    {
                        labelWindow.labels[0].labelWindowContent.gameObject.SetActive(true);
                    }
                }

                Destroy(labelItem.labelWindowContent.gameObject);
                Destroy(labelItem.gameObject);
            });
        }
    }
}