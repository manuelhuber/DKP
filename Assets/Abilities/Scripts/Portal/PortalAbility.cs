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

        private GameObject caster;
        private GameObject enterPortal;
        private GameObject exitPortal;

        // 1 = setting enter portal
        // 2 = setting exit portal
        private bool enterPortalSet;

        public override bool OnActivation(GameObject c) {
            enterPortalSet = false;
            caster = c;
            enterPortal = Instantiate(EnterPortalPrefab);
            enterPortal.transform.position = EnterPortalPosition();
            return false;
        }

        public override bool OnLeftClickUp(ClickLocation click) {
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

        public override void OnCancel() {
            Destroy(enterPortal);
            Destroy(exitPortal);
            caster = null;
        }

        public override void OnUpdate() {
            if (caster == null) {
                if (enterPortal != null) Destroy(enterPortal);
                return;
            }
            if (!enterPortalSet && enterPortal != null) enterPortal.transform.position = EnterPortalPosition();
            else if (enterPortalSet && exitPortal != null) exitPortal.transform.position = ExitPortalPosition();
        }

        public Vector3 EnterPortalPosition() {
            return PositionUtil.GetValidNavMeshPosition(caster.transform.position, CastingDistance);
        }

        public Vector3 ExitPortalPosition() {
            return PositionUtil.GetValidNavMeshPosition(enterPortal.transform.position, PortalDistance);
        }
    }
}