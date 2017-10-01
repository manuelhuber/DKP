using Abilities.Indicators.Scripts.Components;
using Control;
using UnityEngine;
using UnityEngine.AI;
using Util;

namespace Abilities.Scripts {
    public class Teleport : Ability {
        public GameObject LeavePrefab;

        private GameObject casterMarker;

        public override SpellTargetingType IndicatorType {
            get { return SpellTargetingType.Point; }
        }

        public override bool OnActivation(GameObject c) {
            PointTarget(c);
            casterMarker = Instantiate(LeavePrefab, c.transform);
            return false;
        }

        public override bool OnLeftClickUp(ClickLocation click, GameObject caster) {
            DestroyCasterMarkerAnimated();

            caster.GetComponent<NavMeshAgent>().Warp(WarpLocation(caster));

            var waypoints = caster.GetComponent<WaypointHandler>();
            if (waypoints != null) waypoints.DestroyCurrentWaypoint();

            CancelTargeting(SpellTargeting.TargetPrefab, caster);

            return true;
        }

        public override void OnCancel(GameObject caster) {
            CancelTargeting(SpellTargeting.TargetPrefab, caster);
            Destroy(casterMarker);
        }

        private static Vector3 WarpLocation(GameObject caster) {
            var splat = caster.GetComponentInChildren<SplatManager>();
            var warpLocation = PositionUtil.ClosesNavMeshPosition(splat.GetSpellCursorPosition());
            return warpLocation;
        }

        private void DestroyCasterMarkerAnimated() {
            casterMarker.transform.SetParent(null, true);
            casterMarker.GetComponent<Animator>().SetTrigger("Die");
            Destroy(casterMarker, 0.5f);
        }
    }
}