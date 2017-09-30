using System.Collections.Generic;
using System.Linq;
using Damage;
using Damage.Common;
using Generic;
using UnityEngine;

namespace Raid {
    public class TargetManager : SingletonBehaviour<TargetManager> {
        private readonly List<Team> targets = new List<Team>();

        public void ClearList() {
            targets.Clear();
        }

        public void AddTarget(GameObject target) {
            var t = target.GetComponent<Team>();
            if (t != null) targets.Add(t);
        }

        public void RemoveTarget(GameObject target) {
            targets.Remove(target.GetComponent<Team>());
        }

        public List<GameObject> GetEnemies(Team team) {
            return GetEnemies(team.TeamId);
        }

        public List<GameObject> GetEnemies(int teamId) {
            return targets.Where(other => other.TeamId != teamId)
                .Select(t => t.gameObject).ToList();
        }

        public List<Damageable> GetValidEnemyTargets(Team team) {
            return GetValidEnemyTargets(team.TeamId);
        }

        public List<Damageable> GetValidEnemyTargets(int teamId) {
            return GetEnemies(teamId)
                .Select((o, i) => o.GetComponent<Damageable>())
                .Where((o, i) => o != null && o.Targetable).ToList();
        }

        public List<GameObject> GetAllies(Team team) {
            return targets.Where(other => other.TeamId == team.TeamId)
                .Select(t => t.gameObject).ToList();
        }

        public List<int> GetEnemyIds(Team team) {
            // this is just hard coded for now. Not sure how many teams there ever will be?
            var allTeams = new List<int> {0, 1, 2, 3, 4, 5};
            allTeams.Remove(team.TeamId);
            return allTeams;
        }
    }
}