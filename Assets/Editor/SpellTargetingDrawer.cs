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
        Object,
        Text
    }

    internal class SpellTargetProperty {
        public string Key;
        public PropertyType Type;
    }

    [CustomPropertyDrawer(typeof(SpellTargeting))]
    public class SpellTargetingDrawer : PropertyDrawer {
        private static float padding = 2;

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

        private static readonly List<SpellTargetProperty> SelfAttributes = new List<SpellTargetProperty> {
            RangePrefab,
            Range,
        };

        public override float GetPropertyHeight(SerializedProperty prop, GUIContent label) {
            return (EditorGUIUtility.singleLineHeight + padding) * _count;
        }

        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label) {
            var parent = property.serializedObject.targetObject as Ability;
            if (parent == null) return;

            _count = 0;
            switch (parent.IndicatorType) {
                case SpellTargetingType.Self:
                    SelfAttributes.ForEach(att => DrawProperty(att, position, property));
                    break;
                case SpellTargetingType.Point:
                    PointAttributes.ForEach(att => DrawProperty(att, position, property));
                    break;
                case SpellTargetingType.TwoPoints:
                    break;
                case SpellTargetingType.Line:
                    break;
                case SpellTargetingType.None:
                    DrawProperty(
                        new SpellTargetProperty {Key = "This ability has no targeting", Type = PropertyType.Text},
                        position, property);
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        private static void DrawProperty(SpellTargetProperty internalProp, Rect position, SerializedProperty parent) {
            var pos = new Rect(position);
            pos.y += _count++ * (EditorGUIUtility.singleLineHeight + padding);
            pos.height = EditorGUIUtility.singleLineHeight;
            var prop = parent.FindPropertyRelative(internalProp.Key);
            switch (internalProp.Type) {
                case PropertyType.Float:
                    FloatField(pos, prop);
                    break;
                case PropertyType.Object:
                    ObjectField(pos, prop);
                    break;
                case PropertyType.Text:
                    GUI.Label(pos, internalProp.Key);
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