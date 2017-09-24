using System;
using System.Collections.Generic;
using System.Linq;
using UI;
using UI.Scripts;
using UnityEngine;

namespace Control {
    public class AbilityHandler : MonoBehaviour {
        public List<Ability> AbilityScripts;

        private List<ActiveAbility> abilities = new List<ActiveAbility>();

        public bool Active {
            get { return active; }
            set {
                abilityRenderer.DisplayAbilities(value ? abilities : null);
                if (!value) {
                    if (activeAbility != null) activeAbility.Ability.OnCancel();
                    activeAbility = null;
                }
                active = value;
            }
        }

        private bool active;
        private ActiveAbility activeAbility;
        private AbilityRenderer abilityRenderer;

        public bool OnLeftClickDown(ClickLocation click) {
            // Not active or no active ability -> do nothing
            if (!active || activeAbility == null) return false;
            // By convention the ability can't end on a click down
            activeAbility.Ability.OnLeftClickDown(click);
            // if there's an active ability always return true to prevent the selection-box from startin
            return true;
        }

        public bool OnLeftClickUp(ClickLocation click) {
            if (!active || activeAbility == null) return false;
            var abilityDone = activeAbility.Ability.OnLeftClickUp(click);
            if (!abilityDone) return true;
            activeAbility.RemainingCooldown = activeAbility.Ability.Cooldown;
            activeAbility = null;
            return true;
        }

        private void Update() {
            ReduceCooldowns();
            if (!active) return;
            if (activeAbility != null) activeAbility.Ability.OnUpdate();

            // Check cancel
            if (Input.GetKey(KeyCode.Escape) && activeAbility != null) {
                activeAbility.Ability.OnCancel();
                activeAbility = null;
            }

            var newActiveAbility = CheckAbilityActivation();
            if (newActiveAbility == null) return;
            // cancel previous active ability 
            if (activeAbility != null) activeAbility.Ability.OnCancel();
            activeAbility = newActiveAbility;

            if (!activeAbility.Ability.OnActivation(gameObject)) return;
            // If the ability is done after hotkey press set cooldown and be done
            activeAbility.RemainingCooldown = activeAbility.Ability.Cooldown;
            activeAbility = null;
        }

        private ActiveAbility CheckAbilityActivation() {
            ActiveAbility newActiveAbility = null;
            abilities.ForEach(ability => {
                var mod = ability.Ability.Modifier == 0 || Input.GetKey(ability.Ability.Modifier);
                if (!(ability.RemainingCooldown <= 0) || !mod || !Input.GetKeyDown(ability.Ability.Hotkey)) return;
                newActiveAbility = ability;
            });
            return newActiveAbility;
        }

        private void ReduceCooldowns() {
            abilities.Where(a => a.RemainingCooldown > 0).ToList()
                .ForEach(ability =>
                    ability.RemainingCooldown = Math.Max(ability.RemainingCooldown - Time.deltaTime, 0));
        }

        private void Start() {
            abilityRenderer = RaidUi.GetAbilityRenderer();
            abilities = AbilityScripts.Select(a => new ActiveAbility {
                Ability = a,
                RemainingCooldown = 0
            }).ToList();
        }
    }
}