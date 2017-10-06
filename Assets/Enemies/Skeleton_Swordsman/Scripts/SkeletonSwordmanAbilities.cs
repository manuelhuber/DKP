using Damage.Melee;
using Enemies.Skeleton_Swordsman.Scripts;
using Generic;
using UnityEngine;

namespace Enemies.Skeleton_Swordsman {
    public class SkeletonSwordmanAbilities : DkpMonoBehaviour {
        public float SlashAndKickTimer;
        public GameObject SlashAndKickPrefab;

        private MeleeAttack meleeAttack;
        private float nextSlashAndKick;
        private Animator animator;

        private void Awake() {
            meleeAttack = GetComponent<MeleeAttack>();
            animator = GetComponent<Animator>();
        }

        private void Update() {
            if (!meleeAttack.InRange || !(nextSlashAndKick < Time.time)) return;
            var a = Instantiate(SlashAndKickPrefab, gameObject.transform).GetComponent<SlashAndKick>();
            a.Caster = gameObject;
            nextSlashAndKick = Time.time + SlashAndKickTimer;
        }
    }
}