using PCs.Scripts;
using UnityEditor;
using UnityEngine;

namespace XX_EditorScripts {
    [CustomEditor(typeof(CharacterBuilder), true, isFallback = true)]
    public class CharacterBuilderEditor : Editor {
        public override void OnInspectorGUI() {
            base.OnInspectorGUI();
            var builder = target as CharacterBuilder;
            if (builder == null) return;

            if (builder.AttackType == AttackType.Melee) {
                builder.AttackDamage = EditorGUILayout.IntField("Melee Damage", builder.AttackDamage);
            } else {
                builder.ProjectilePrefab = (GameObject) EditorGUILayout.ObjectField("Projectile Prefab",
                    builder.ProjectilePrefab, typeof(GameObject), true);
            }
        }
    }
}