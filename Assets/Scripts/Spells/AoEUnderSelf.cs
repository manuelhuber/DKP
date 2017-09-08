using Control;
using Damage;
using UnityEngine;
using Util;

namespace Spells {
    public class AoEUnderSelf : Ability {
        public AreaOfEffect AoEPrefab;

        public override bool OnActivation(GameObject caster) {
            Vector3 hitOnTerrain;
            PositionUtil.ProjectOnTerrainFromPosition(caster.transform.position, out hitOnTerrain);
            Instantiate(AoEPrefab, hitOnTerrain, caster.transform.rotation);
            return true;
        }
    }
}