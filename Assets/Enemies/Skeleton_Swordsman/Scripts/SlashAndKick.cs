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
        public GameObject Caster;
        public float[] HitOffset;

        private float startTime;
        private float endTime;
        private float nextAttack;
        private readonly List<Damageable> targets = new List<Damageable>();
        private Material mat;
        private int hitNumber;
        private MeleeAttack casterMelee;

        private void Start() {
            startTime = Time.time;
            nextAttack = Time.time + HitOffset[hitNumber++];
            endTime = startTime + Duration;
            mat = gameObject.GetComponent<Projector>().material;
            Caster.GetComponent<Animator>().SetTrigger("Slash");
            casterMelee = Caster.GetComponent<MeleeAttack>();
            casterMelee.enabled = false;
        }

        private void Update() {
            if (nextAttack <= Time.time && hitNumber < HitOffset.Length) {
                targets.ForEach(damageable => damageable.ModifyHitpoints(DmgPerHit));
                nextAttack = Time.time + HitOffset[hitNumber++];
            }
            AnimateIndicator();
            if (!(endTime <= Time.time)) return;
            targets.ForEach(damageable => {
                var body = damageable.gameObject.GetComponent<Rigidbody>();
                if (body == null) return;
                var dir = body.gameObject.transform.position - Caster.transform.position;
                body.AddForce(dir * KnockbackForce, ForceMode.Impulse);
            });
            casterMelee.enabled = true;
            Destroy(gameObject);
        }

        private void AnimateIndicator() {
            if (mat == null) return;
            mat.SetFloat("_Fill", (Time.time - startTime) / Duration);
        }

        private void OnTriggerExit(Collider other) {
            var damageable = other.gameObject.GetComponent<Damageable>();
            if (targets.Contains(damageable)) {
                targets.Remove(damageable);
            }
        }

        private void OnTriggerEnter(Collider other) {
            var team = other.gameObject.GetComponent<Team>();
            if (team != null && AffectedTeams.Contains(team.TeamId)) {
                targets.Add(other.gameObject.GetComponent<Damageable>());
            }
        }
    }
}