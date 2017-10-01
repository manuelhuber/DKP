using PCs.Scripts;
using UnityEditor;
using UnityEngine;

namespace Editor {
    [CustomEditor(typeof(CharacterBuilder), true, isFallback = true)]
    public class CharacterBuilderEditor : UnityEditor.Editor {
        private bool showAttack = true;

        public override void OnInspectorGUI() {
            var builder = target as CharacterBuilder;
            if (builder == null) return;

            DrawDefaultInspector();

            showAttack = EditorGUILayout.Foldout(showAttack, "Attack");
            if (showAttack) DrawAttackEditor(builder);
        }

        private static void DrawAttackEditor(CharacterBuilder builder) {
            builder.AttackType = (AttackType) EditorGUILayout.EnumPopup("Type", builder.AttackType);

            builder.AttackRange = EditorGUILayout.FloatField("Range", builder.AttackRange);
            builder.AttackIntervall = EditorGUILayout.FloatField("Intervall", builder.AttackIntervall);


            if (builder.AttackType == AttackType.Melee) {
                builder.AttackDamage = EditorGUILayout.IntField("Melee Damage", builder.AttackDamage);
            } else {
                builder.ProjectilePrefab = (GameObject) EditorGUILayout.ObjectField("Projectile Prefab",
                    builder.ProjectilePrefab, typeof(GameObject), true);
            }
        }
    }
}