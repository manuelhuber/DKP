using System;
using System.Collections.Generic;
using Control;
using UnityEditor;
using UnityEditor.Graphs;
using UnityEngine;
using Util;

namespace Editor {
    internal enum PropertyType {
        Float,
        Object
    }

    internal class SpellTargetProperty {
        public string Key;
        public PropertyType Type;
    }

    [CustomPropertyDrawer(typeof(SpellTargeting))]
    public class SpellTargetingDrawer : PropertyDrawer {
        private static readonly SpellTargetProperty TargetPrefab =
            new SpellTargetProperty {Key = "TargetPrefab", Type = PropertyType.Object};

        private static readonly SpellTargetProperty TargetSize =
            new SpellTargetProperty {Key = "TargetSize", Type = PropertyType.Float};

        private static readonly SpellTargetProperty RangePrefab =
            new SpellTargetProperty {Key = "RangePrefab", Type = PropertyType.Object};

        private static readonly SpellTargetProperty Range =
            new SpellTargetProperty {Key = "Range", Type = PropertyType.Float};

        private static int _count;

        private static readonly List<SpellTargetProperty> PointAttributes = new List<SpellTargetProperty> {
            TargetPrefab,
            TargetSize,
            RangePrefab,
            Range,
        };

        public override float GetPropertyHeight(SerializedProperty prop, GUIContent label) {
            var lines = prop.isExpanded ? _count + 2 : 1;
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
                case SpellTargetingType.Self:
                    break;
                case SpellTargetingType.Point:
                    PointAttributes.ForEach(att => DrawProperty(att, position, property));
                    break;
                case SpellTargetingType.TwoPoints:
                    break;
                case SpellTargetingType.Line:
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }

            EditorGUI.indentLevel--;
        }

        private static void DrawProperty(SpellTargetProperty internalProp, Rect position, SerializedProperty parent) {
            var pos = new Rect(position);
            pos.y += ++_count * EditorGUIUtility.singleLineHeight;
            pos.height = EditorGUIUtility.singleLineHeight;
            var prop = parent.FindPropertyRelative(internalProp.Key);
            switch (internalProp.Type) {
                case PropertyType.Float:
                    FloatField(pos, prop);
                    break;
                case PropertyType.Object:
                    ObjectField(pos, prop);
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
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