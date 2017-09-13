using UnityEngine;

namespace Damage {
    public class Team : MonoBehaviour {
        public int TeamId;

        public bool SameTeam(GameObject other) {
            return SameTeam(other.GetComponent<Team>());
        }

        public bool SameTeam(Team other) {
            return other != null && other.TeamId == TeamId;
        }
    }
}