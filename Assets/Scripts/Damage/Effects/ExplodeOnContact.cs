using UnityEngine;

namespace Damage.Effects {
    public class ExplodeOnContact : Effect {
        public float Radius;
        public int Damage;

        private bool exploding;

        private void OnTriggerEnter(Collider other) {
            var team = other.gameObject.GetComponent<Team>();
            if (team == null || !AffectedTeams.Contains(team.TeamId)) return;
            other.gameObject.GetComponent<Damageable>().ModifyHitpoints(Damage);
            if (exploding) return;
            gameObject.GetComponent<SphereCollider>().radius = Radius;
            Destroy(gameObject, 0.1f);
            exploding = true;
        }
    }
}