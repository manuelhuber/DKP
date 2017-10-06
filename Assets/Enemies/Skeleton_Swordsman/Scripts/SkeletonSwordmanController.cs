using System.Collections.Generic;
using System.Linq;
using Damage;
using Damage.Melee;
using Generic;
using UnityEngine;
using UnityEngine.AI;

namespace Enemies.Skeleton_Swordsman.Scripts {
    public class SkeletonSwordmanController : DkpMonoBehaviour {
        public float SlashAndKickTimer;
        public GameObject SlashAndKickPrefab;

        private MeleeAttack meleeAttack;
        private float nextSlashAndKick;
        private Damageable damageable;
        private List<GameObject> activeStuff = new List<GameObject>();
        private NavMeshAgent agent;

        public void PauseBehaviour(bool value) {
            meleeAttack.StopAttack = value;
            agent.enabled = !value;
        }


        private void Awake() {
            agent = GetComponent<NavMeshAgent>();

            meleeAttack = GetComponent<MeleeAttack>();
            damageable = GetComponent<Damageable>();
        }

        private void Update() {
            CheckDead();
            if (!meleeAttack.InRange || !(nextSlashAndKick < Time.time)) return;
            var a = Instantiate(SlashAndKickPrefab, gameObject.transform).GetComponent<SlashAndKick>();
            a.Caster = this;
            activeStuff.Add(a.gameObject);
            nextSlashAndKick = Time.time + SlashAndKickTimer;
        }

        private void CheckDead() {
            if (!damageable.IsDead()) return;
            foreach (var o in activeStuff.Where(a => a != null)) {
                Destroy(o);
            }
        }
    }
}