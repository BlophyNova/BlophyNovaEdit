using System;
using System.Reflection;
using System.Collections.Generic;

using UnityEditor;
using UnityEngine;

namespace Smart
{
    using GL = GUILayout;

    using static EditorGUILayout;

    public partial class Inspector
    {
        public enum ViewType
        {
            Comfortable = 0,
            Small = 1,
            ComponentsOnly = 2,
            ContentsOnly = 3,
            Page = 4
        }

        public class InspectorSettings
        {
            public void Draw()
            {
                if(showSettings.Value)
                {
                    BeginVertical("Helpbox");
                    ViewType vt = (ViewType)viewTypeValue.Value;
                    viewTypeValue.Value = (int)(ViewType)EnumPopup("View", vt);
                    showButtons.Value = Toggle("Show Buttons", showButtons.Value);
                    showInfo.Value = Toggle("Show Info", showInfo.Value);
                    matchWord.Value = Toggle("Search Match Word", matchWord.Value);
                    SlotSizeField();
                    MaxSlotField();
                    if (GL.Button("Close", "miniButton"))
                    {
                        showSettings.Value = false;
                    }
                    EndVertical();
                }

                if(showSearchBar.Value)
                {
                    BeginHorizontal();
                    filter = GL.TextField(filter, "SearchTextField");
                    if (GL.Button(GUIContent.none, "SearchCancelButton"))
                    { filter = ""; EditorGUI.FocusTextInControl(null); }
                    EndHorizontal();
                }
            }

            void SlotSizeField()
            {
                EditorGUI.BeginChangeCheck();
                slotHeight.Value = IntField("Slot Size", (int)slotHeight.Value);
                if (EditorGUI.EndChangeCheck())
                {
                    slotHeight.Value = slotHeight.Value < 4 ? 4 : slotHeight.Value;
                }
            }

            void MaxSlotField()
            {
                EditorGUI.BeginChangeCheck();
                maxSlots.Value = IntField("Max Slots Per Row", maxSlots.Value);
                if (EditorGUI.EndChangeCheck())
                {
                    maxSlots.Value = maxSlots.Value < 1 ? 1 : maxSlots.Value;
                    window.OnSelectionChange();
                }
            }
        }
    }
}
