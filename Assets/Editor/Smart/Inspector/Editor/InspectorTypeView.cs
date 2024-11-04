using System;
using System.Reflection;
using System.Collections.Generic;

using UnityEditor;
using UnityEngine;

namespace Smart
{
    using GL = GUILayout;
    using Object = UnityEngine.Object;

    public partial class Inspector
    {
        public class InspectorTypeView : List<ButtonType>
        {
            public void Draw()
            {
                for (int i = 0; i < Count; i++)
                {
                    if (this[i].Click())
                    {
                        selection.NarrowSelection(this[i].type);
                        FREE();
                    }
                }
            }

            public void FREE()
            {
                for (int i = 0; i < Count; i++)
                {
                    this[i] = null;
                }

                Clear();
            }
        }

        //
        // BUTTON TYPE
        //

        public class ButtonType
        {
            public Object value;
            public string type;
            GUIContent content;

            public ButtonType(Object value, string type)
            {
                this.value = value;
                this.type = type;
            }

            public bool Click()
            {
                content = new GUIContent();
                //content = ObjectContent(value, value.GetType());
                content.text = type;
                content.image = AssetPreview.GetMiniTypeThumbnail(value.GetType());

                GUIStyle style = new GUIStyle("button");
                style.alignment = TextAnchor.MiddleLeft;

                return GL.Button(content, style, GL.Height(30));
            }
        }
    }
}
