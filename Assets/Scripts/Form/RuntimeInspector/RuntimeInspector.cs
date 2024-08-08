using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RuntimeInspector : LabelWindowContent
{
    public RuntimeInspectorNamespace.RuntimeInspector runtimeInspector;
    private void Start()
    {
        if(labelWindow.associateLabelWindow != null)
        {
            foreach (LabelItem item in labelWindow.associateLabelWindow.labels)
            {
                if(item.labelWindowContent.labelWindowContentType == LabelWindowContentType.RuntimeHierarchy)
                {
                    RuntimeHierarchy runtimeHierarchy = (RuntimeHierarchy)item.labelWindowContent;
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
                RuntimeHierarchy runtimeHierarchy = (RuntimeHierarchy)item.labelWindowContent;
                runtimeInspector.ConnectedHierarchy = runtimeHierarchy.runtimeHierarchy;
                runtimeHierarchy.runtimeHierarchy.ConnectedInspector = runtimeInspector;
                return;
            }
        }
    }
}
