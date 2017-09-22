using Control;
using UnityEngine;
using Util;

namespace Abilities.Scripts {
    [CreateAssetMenu(menuName = "Spells/Portal")]
    public class Portal : Ability {
        public GameObject EnterPortalPrefab;
        public GameObject ExitPortalPrefab;
        public float CastingDistance;
        public float PortalDistance;

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
            if (enterPortalSet) return true;
            enterPortalSet = true;
            exitPortal = Instantiate(EnterPortalPrefab);
            return false;
        }

        public override void OnCancel() {
            Destroy(enterPortal);
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