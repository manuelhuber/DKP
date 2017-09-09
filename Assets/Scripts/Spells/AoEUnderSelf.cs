using Control;
using Damage;
using UnityEngine;
using Util;

namespace Spells {
    [CreateAssetMenu(menuName = "Spells/AoEUnderSelf")]
    public class AoEUnderSelf : Ability {
        public AreaOfEffect AoEPrefab;

        public override bool OnActivation(GameObject cas) {
            Vector3 hitOnTerrain;
            PositionUtil.ProjectOnTerrainFromPosition(cas.transform.position, out hitOnTerrain);
            Instantiate(AoEPrefab, hitOnTerrain, cas.transform.rotation);
            return true;
        }
    }
}