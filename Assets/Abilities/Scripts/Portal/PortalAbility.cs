using Control;
using UnityEngine;
using Util;

namespace Abilities.Scripts.Portal {
    public class PortalAbility : Ability {
        public GameObject EnterPortalPrefab;
        public GameObject ExitPortalPrefab;
        public float CastingDistance;
        public float PortalDistance;
        public float Duration;

        private GameObject enterPortal;
        private GameObject exitPortal;

        private bool enterPortalSet;

        public override SpellTargetingType IndicatorType {
            get { return SpellTargetingType.TwoPoints; }
        }

        public override bool OnActivation(GameObject c) {
            enterPortalSet = false;
            enterPortal = Instantiate(EnterPortalPrefab);
            enterPortal.transform.position = EnterPortalPosition(c);
            exitPortal = null;
            return false;
        }

        public override bool OnLeftClickUp(ClickLocation click, GameObject caster) {
            if (!enterPortalSet) {
                enterPortalSet = true;
                exitPortal = Instantiate(ExitPortalPrefab);
                return false;
            }
            var enterScript = enterPortal.AddComponent<EnterPortal>();
            enterScript.Exit = exitPortal;
            Destroy(enterPortal, Duration);
            Destroy(exitPortal, Duration);
            return true;
        }

        public override void OnCancel(GameObject caster) {
            Destroy(enterPortal);
            Destroy(exitPortal);
        }

        public override void OnUpdate(GameObject caster) {
            if (!enterPortalSet && enterPortal != null) enterPortal.transform.position = EnterPortalPosition(caster);
            else if (enterPortalSet && exitPortal != null) exitPortal.transform.position = ExitPortalPosition(caster);
        }

        public Vector3 EnterPortalPosition(GameObject caster) {
            return PositionUtil.GetValidNavMeshPositionFromCursor(caster.transform.position, CastingDistance);
        }

        public Vector3 ExitPortalPosition(GameObject caster) {
            return PositionUtil.GetValidNavMeshPositionFromCursor(enterPortal.transform.position, PortalDistance);
        }
    }
}