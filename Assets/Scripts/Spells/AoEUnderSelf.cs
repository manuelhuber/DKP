using System.Collections.Generic;
using Control;
using Damage;
using UnityEngine;
using Util;

namespace Spells {
    [CreateAssetMenu(menuName = "Spells/AoEUnderSelf")]
    public class AoEUnderSelf : Ability {
        public AreaOfEffect AoEPrefab;

        public override bool OnActivation(GameObject caster) {
            Vector3 hitOnTerrain;
            PositionUtil.ProjectOnTerrainFromPosition(caster.transform.position, out hitOnTerrain);
            var areaOfEffect = Instantiate(AoEPrefab, hitOnTerrain, caster.transform.rotation);
            areaOfEffect.AffectedTeams = new List<int> {caster.GetComponent<Team>().TeamId};
            return true;
        }
    }
}