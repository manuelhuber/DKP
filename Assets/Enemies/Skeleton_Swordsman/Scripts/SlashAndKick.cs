using System.Collections.Generic;
using System.Linq;
using Damage;
using Damage.Common;
using Damage.Melee;
using Generic;
using UnityEngine;

namespace Enemies.Skeleton_Swordsman.Scripts {
    public class SlashAndKick : DkpMonoBehaviour {
        public float Duration;
        public int NumberOfHits;
        public int DmgPerHit;
        public float KnockbackForce;
        public int[] AffectedTeams;
        public SkeletonSwordmanController Caster;

        private float startTime;
        private float endTime;
        private float nextAttack;
        private float attackTimeIntervall;
        private readonly List<Damageable> targets = new List<Damageable>();
        private Material mat;
        private MeleeAttack casterMelee;

        private void Start() {
            startTime = Time.time;
            attackTimeIntervall = Duration / NumberOfHits;
            nextAttack = Time.time + attackTimeIntervall;
            endTime = startTime + Duration;
            mat = gameObject.GetComponent<Projector>().material;
            Caster.GetComponent<Animator>().SetTrigger("Slash");
            Caster.PauseBehaviour(true);
        }

        private void Update() {
            AnimateIndicator();
            if (nextAttack <= Time.time) Slash();
            if (endTime <= Time.time) Kick();
        }

        private void Slash() {
            targets.ForEach(damageable => damageable.ModifyHitpoints(DmgPerHit));
            nextAttack = Time.time + attackTimeIntervall;
        }

        private void Kick() {
            targets.ForEach(damageable => {
                var body = damageable.gameObject.GetComponent<Rigidbody>();
                if (body == null) return;
                var dir = body.gameObject.transform.position - Caster.transform.position;
                body.AddForce(dir * KnockbackForce, ForceMode.Impulse);
            });
            Destroy(gameObject);
        }

        private void OnDestroy() {
            Caster.PauseBehaviour(false);
        }

        private void AnimateIndicator() {
            if (mat == null) return;
            mat.SetFloat("_Fill", (Time.time - startTime) / Duration);
        }

        private void OnTriggerExit(Collider other) {
            targets.Remove(other.gameObject.GetComponent<Damageable>());
        }

        private void OnTriggerEnter(Collider other) {
            var team = other.gameObject.GetComponent<Team>();
            if (team != null && AffectedTeams.Contains(team.TeamId)) {
                targets.Add(other.gameObject.GetComponent<Damageable>());
            }
        }
    }
}