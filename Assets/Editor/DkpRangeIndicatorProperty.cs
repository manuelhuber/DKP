using System;
using System.Collections.Generic;
using Control;
using UnityEditor;
using UnityEngine;

namespace Editor {
    [CustomPropertyDrawer(typeof(DkpRangeIndicator))]
    public class DkpRangeIndicatorProperty : PropertyDrawer {
        private const string IndicatorPrefab = "IndicatorPrefab";
        private const string Range = "Range";
        private static int _count;

        private static readonly List<string> PointAttributes = new List<string> {
            IndicatorPrefab,
            Range
        };

        public override float GetPropertyHeight(SerializedProperty prop, GUIContent label) {
            // use the default property height, which takes into account the expanded state
            var lines = prop.isExpanded ? _count + 1 : 1;
            return EditorGUIUtility.singleLineHeight * lines;
        }

        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label) {
            var parent = property.serializedObject.targetObject as Ability;
            if (parent == null) return;

            property.isExpanded = EditorGUI.Foldout(position, property.isExpanded, label);
            if (!property.isExpanded) return;

            EditorGUI.BeginProperty(position, label, property);
            EditorGUI.indentLevel++;

            _count = 0;
            switch (parent.IndicatorType) {
                case RangeIndicatorType.Self:
                    Debug.Log("Self");
                    break;
                case RangeIndicatorType.Point:
                    PointAttributes.ForEach(att => DrawProperty(att, position, property));
                    break;
                case RangeIndicatorType.TwoPoints:
                    break;
                case RangeIndicatorType.Line:
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }

            EditorGUI.indentLevel--;
            EditorGUI.EndProperty();
        }

        private static void DrawProperty(string propertyName, Rect position, SerializedProperty property) {
            var pos = new Rect(position);
            pos.y += ++_count * EditorGUIUtility.singleLineHeight;
            pos.height = EditorGUIUtility.singleLineHeight;
            EditorGUI.PropertyField(pos, property.FindPropertyRelative(propertyName));
        }
    }
}