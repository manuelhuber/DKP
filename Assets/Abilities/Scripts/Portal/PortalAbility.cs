using Abilities.Indicators.Scripts.Components;
using Control;
using UnityEngine;

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
            get { return SpellTargetingType.Point; }
        }

        public override bool OnActivation(GameObject c) {
            ActivatePointTarget(c);
            enterPortalSet = false;
            exitPortal = null;
            exitPortal = null;
            return false;
        }

        public override bool OnLeftClickUp(ClickLocation click, GameObject caster) {
            var splat = caster.GetComponentInChildren<SplatManager>();
            if (!enterPortalSet) {
                enterPortal = Instantiate(EnterPortalPrefab, splat.GetSpellCursorPosition(), Quaternion.identity);
                enterPortalSet = true;
                return false;
            } else {
                exitPortal = Instantiate(ExitPortalPrefab, splat.GetSpellCursorPosition(), Quaternion.identity);
                enterPortal.AddComponent<EnterPortal>().Exit = exitPortal;
                Destroy(enterPortal, Duration);
                Destroy(exitPortal, Duration);
                CancelTargeting(caster);
                return true;
            }
        }

        public override void OnCancel(GameObject caster) {
            Destroy(enterPortal);
            Destroy(exitPortal);
            CancelTargeting(caster);
        }
    }
}