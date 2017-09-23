using System.Collections.Generic;
using System.Linq;
using Damage;
using UnityEngine;

namespace Raid {
    public class TargetManager {
        private static readonly List<Team> targets = new List<Team>();

        public static void ClearList() {
            targets.Clear();
        }

        public static void AddTarget(GameObject gameObject) {
            var t = gameObject.GetComponent<Team>();
            if (t != null) targets.Add(t);
        }

        public static void RemoveTarget(GameObject gameObject) {
            targets.Remove(gameObject.GetComponent<Team>());
        }

        public static List<GameObject> GetEnemies(int teamId) {
            return targets.Where(team => team.TeamId != teamId)
                .Select(t => t.gameObject).ToList();
        }

        public static List<GameObject> GetAllies(int teamId) {
            return targets.Where(team => team.TeamId == teamId)
                .Select(t => t.gameObject).ToList();
        }
    }
}