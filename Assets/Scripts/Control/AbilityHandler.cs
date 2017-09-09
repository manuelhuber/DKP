using System.Collections.Generic;
using UI;
using UnityEngine;

namespace Control {
    public class AbilityHandler : MonoBehaviour {
        public List<Ability> Abilities;

        public bool Active {
            get { return active; }
            set {
                abilityRenderer.DisplayAbilities(value ? Abilities : null);
                active = value;
            }
        }

        private bool active;
        private float GlobalCooldown = 0.5f;
        private float gcdEnd;
        private Ability activeAbility;
        private AbilityRenderer abilityRenderer;

        public bool OnLeftClickDown(ClickLocation click) {
            // Not active or no active ability -> do nothing
            if (!active || activeAbility == null) return false;
            // By convention the ability can't end on a click down
            activeAbility.OnLeftClickDown(click);
            // if there's an active ability always return true to prevent the selection-box from startin
            return true;
        }

        public bool OnLeftClickUp(ClickLocation click) {
            if (!active || activeAbility == null || !activeAbility.OnLeftClickUp(click)) return false;
            activeAbility = null;
            return true;
        }

        private void Update() {
            if (!active || gcdEnd > Time.time) return;
            Abilities.ForEach(ability => {
                var mod = ability.Modifier == 0 || Input.GetKey(ability.Modifier);
                if (mod && Input.GetKey(ability.Hotkey)) {
                    activeAbility = ability;
                }
            });
            if (activeAbility == null || !activeAbility.OnActivation(gameObject)) return;
            activeAbility = null;
            gcdEnd = Time.time + GlobalCooldown;
        }

        private void Start() {
            abilityRenderer = RaidUi.GetAbilityRenderer();
        }
    }
}