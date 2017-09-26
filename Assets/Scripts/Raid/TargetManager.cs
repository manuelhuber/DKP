using System.Collections.Generic;
using System.Linq;
using Damage.Common;
using UnityEngine;

namespace Raid {
    public class TargetManager {
        private static readonly List<Team> Targets = new List<Team>();

        public static void ClearList() {
            Targets.Clear();
        }

        public static void AddTarget(GameObject gameObject) {
            var t = gameObject.GetComponent<Team>();
            if (t != null) Targets.Add(t);
        }

        public static void RemoveTarget(GameObject gameObject) {
            Targets.Remove(gameObject.GetComponent<Team>());
        }

        public static List<GameObject> GetEnemies(Team team) {
            return Targets.Where(other => other.TeamId != team.TeamId)
                .Select(t => t.gameObject).ToList();
        }

        public static List<GameObject> GetAllies(Team team) {
            return Targets.Where(other => other.TeamId == team.TeamId)
                .Select(t => t.gameObject).ToList();
        }

        public static List<int> GetEnemyIds(Team team) {
            // this is just hard coded for now. Not sure how many teams there ever will be?
            var allTeams = new List<int> {0, 1, 2, 3, 4, 5};
            allTeams.Remove(team.TeamId);
            return allTeams;
        }
    }
}