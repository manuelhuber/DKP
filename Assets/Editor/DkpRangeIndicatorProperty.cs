using System;
using System.Collections.Generic;
using Control;
using UnityEditor;
using UnityEngine;
using Util;

namespace Editor {
    [CustomPropertyDrawer(typeof(DkpRangeIndicator))]
    public class DkpRangeIndicatorProperty : PropertyDrawer {
        private const string IndicatorPrefab = "IndicatorPrefab";
        private const string Range = "Range";
        private const string IndicatorSize = "IndicatorSize";
        private static int _count;

        private static readonly List<string> PointAttributes = new List<string> {
            Range,
            IndicatorPrefab,
            IndicatorSize
        };

        public override float GetPropertyHeight(SerializedProperty prop, GUIContent label) {
            // use the default property height, which takes into account the expanded state
            var lines = prop.isExpanded ? _count + 1 : 1;
            return EditorGUIUtility.singleLineHeight * lines;
        }

        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label) {
            var parent = property.serializedObject.targetObject as Ability;
            if (parent == null) return;

            var foldoutPos = new Rect(position) {height = EditorGUIUtility.singleLineHeight};
            property.isExpanded = EditorGUI.Foldout(foldoutPos, property.isExpanded, label);
            if (!property.isExpanded) return;

            EditorGUI.indentLevel++;

            _count = 0;
            switch (parent.IndicatorType) {
                case RangeIndicatorType.Self:
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
        }

        private static void DrawProperty(string propertyName, Rect position, SerializedProperty parent) {
            var pos = new Rect(position);
            pos.y += ++_count * EditorGUIUtility.singleLineHeight;
            pos.height = EditorGUIUtility.singleLineHeight;
            var prop = parent.FindPropertyRelative(propertyName);
            switch (propertyName) {
                case Range:
                case IndicatorSize:
                    FloatField(pos, prop);
                    break;
                case IndicatorPrefab:
                    ObjectField(pos, prop);
                    break;
            }
        }


        private static void FloatField(Rect pos, SerializedProperty prop) {
            prop.floatValue = EditorGUI.FloatField(pos, new GUIContent(prop.displayName), prop.floatValue);
        }

        private static void ObjectField(Rect pos, SerializedProperty prop) {
            EditorGUI.ObjectField(pos, prop);
        }
    }
}