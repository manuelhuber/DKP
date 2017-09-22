using System.Collections.Generic;
using Control;
using Damage;
using UnityEngine;
using Util;

namespace Abilities.Scripts {
    [CreateAssetMenu(menuName = "Spells/AoEUnderSelf")]
    public class AoEUnderSelf : Ability {
        public AreaOfEffect AoEPrefab;

        public override bool OnActivation(GameObject c) {
            Vector3 hitOnTerrain;
            PositionUtil.ProjectOnTerrainFromPosition(c.transform.position, out hitOnTerrain);
            var areaOfEffect = Instantiate(AoEPrefab, hitOnTerrain, c.transform.rotation);
            areaOfEffect.AffectedTeams = new List<int> {c.GetComponent<Team>().TeamId};
            return true;
        }
    }
}