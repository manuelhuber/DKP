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

        public static List<GameObject> GetEnemies(int teamId) {
            return Targets.Where(team => team.TeamId != teamId)
                .Select(t => t.gameObject).ToList();
        }

        public static List<GameObject> GetAllies(int teamId) {
            return Targets.Where(team => team.TeamId == teamId)
                .Select(t => t.gameObject).ToList();
        }
    }
}