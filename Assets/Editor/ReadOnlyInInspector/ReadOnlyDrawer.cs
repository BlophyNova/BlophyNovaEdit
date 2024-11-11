/* ReadOnly Inspector Attribute Kit
 * 2024 xeetsh (Max Schmitt)
 * Published under Unity Asset Store Terms of Service and EULA https://unity.com/legal/as-terms
 * File version 1.0
 */

namespace xeetsh.ReadOnlyInspectorAttributeKit {
    using UnityEditor;
    using UnityEngine;

    [CustomPropertyDrawer(typeof(ReadOnlyAttribute))]
    public class ReadOnlyDrawer : PropertyDrawer {
        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label) {
            string value = string.Empty;
            Rect labelPosition = EditorGUI.PrefixLabel(position, label);

            switch (property.propertyType) {
                case SerializedPropertyType.Integer:
                    value = property.intValue.ToString();
                    break;
                case SerializedPropertyType.Float:
                    value = property.floatValue.ToString("0.000000");
                    break;
                case SerializedPropertyType.Boolean:
                    value = property.boolValue.ToString();
                    break;
                case SerializedPropertyType.String:
                    value = property.stringValue;
                    break;
                case SerializedPropertyType.Color:
                    DrawColorField(position, labelPosition, property);
                    return;
                case SerializedPropertyType.Vector2:
                    value = property.vector2Value.ToString();
                    break;
                case SerializedPropertyType.Vector3:
                    value = property.vector3Value.ToString();
                    break;
                case SerializedPropertyType.Vector4:
                    value = property.vector4Value.ToString();
                    break;
                case SerializedPropertyType.Vector2Int:
                    value = property.vector2IntValue.ToString();
                    break;
                case SerializedPropertyType.Vector3Int:
                    value = property.vector3IntValue.ToString();
                    break;
                case SerializedPropertyType.Rect:
                    value = property.rectValue.ToString();
                    break;
                case SerializedPropertyType.RectInt:
                    value = property.rectIntValue.ToString();
                    break;
                case SerializedPropertyType.Quaternion:
                    value = $"{property.quaternionValue} Euler: {property.quaternionValue.eulerAngles}";
                    break;
                case SerializedPropertyType.Bounds:
                    value = property.boundsValue.ToString();
                    break;
                case SerializedPropertyType.BoundsInt:
                    value = property.boundsIntValue.ToString();
                    break;
                case SerializedPropertyType.AnimationCurve:
                    DrawAnimationCurveField(position, labelPosition, property);
                    return;
#if UNITY_6000_0_OR_NEWER
            case SerializedPropertyType.Gradient:
                DrawGradientField(position, labelPosition, property);
                break;
#endif
                case SerializedPropertyType.LayerMask:
                    value = property.intValue.ToString();
                    break;
                case SerializedPropertyType.Enum:
                    value = $"{property.enumDisplayNames[property.enumValueIndex]} ({property.enumValueIndex})";
                    break;
                case SerializedPropertyType.ObjectReference:
                    DrawObjectReferenceField(position, property);
                    return;
                default:
                    value = "Unsupported";
                    break;
            }

            EditorGUI.LabelField(position, label.text, value);
        }

        private void DrawColorField(Rect position, Rect labelPosition, SerializedProperty property) {
            EditorGUI.LabelField(position, property.displayName);
            Color originalColor = GUI.color;
            GUI.color = property.colorValue;
            EditorGUI.LabelField(labelPosition, property.colorValue.ToString());
            GUI.color = originalColor;
        }

#if UNITY_6000_0_OR_NEWER
    private void DrawGradientField(Rect position, Rect labelPosition, SerializedProperty property) {
        EditorGUI.LabelField(position, property.displayName);
        GUI.enabled = false;
        EditorGUI.GradientField(labelPosition, property.gradientValue);
        GUI.enabled = true;
    }
#endif

        private void DrawAnimationCurveField(Rect position, Rect labelPosition, SerializedProperty property) {
            EditorGUI.LabelField(position, property.displayName);
            GUI.enabled = false;
            EditorGUI.CurveField(labelPosition, property.animationCurveValue);
            GUI.enabled = true;
        }

        private void DrawObjectReferenceField(Rect position, SerializedProperty property) {
            if (property.objectReferenceValue != null) {
                GUI.enabled = false;
                EditorGUI.ObjectField(position, property);
                GUI.enabled = true;
            } else {
                EditorGUI.LabelField(position, property.displayName, "null");
            }
        }
    }
}