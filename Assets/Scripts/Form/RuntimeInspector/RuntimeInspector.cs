using Data.Enumerate;
using Form.LabelWindow;

namespace Form.RuntimeInspector
{
    public class RuntimeInspector : LabelWindowContent
    {
        public RuntimeInspectorNamespace.RuntimeInspector runtimeInspector;

        private void Start()
        {
            if (labelWindow.associateLabelWindow != null)
            {
                foreach (LabelItem item in labelWindow.associateLabelWindow.labels)
                {
                    if (item.labelWindowContent.labelWindowContentType == LabelWindowContentType.RuntimeHierarchy)
                    {
                        RuntimeHierarchy.RuntimeHierarchy runtimeHierarchy =
                            (RuntimeHierarchy.RuntimeHierarchy)item.labelWindowContent;
                        runtimeInspector.ConnectedHierarchy = runtimeHierarchy.runtimeHierarchy;
                        runtimeHierarchy.runtimeHierarchy.ConnectedInspector = runtimeInspector;
                        return;
                    }
                }
            }

            foreach (LabelItem item in labelWindow.labels)
            {
                if (item.labelWindowContent.labelWindowContentType == LabelWindowContentType.RuntimeHierarchy)
                {
                    RuntimeHierarchy.RuntimeHierarchy runtimeHierarchy =
                        (RuntimeHierarchy.RuntimeHierarchy)item.labelWindowContent;
                    runtimeInspector.ConnectedHierarchy = runtimeHierarchy.runtimeHierarchy;
                    runtimeHierarchy.runtimeHierarchy.ConnectedInspector = runtimeInspector;
                    return;
                }
            }
        }
    }
}