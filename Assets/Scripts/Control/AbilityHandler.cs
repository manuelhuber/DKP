using System;
using System.Collections.Generic;
using System.Linq;
using Spells;
using UI;
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
            if (!active || activeAbility == null || !activeAbility.Ability.OnLeftClickUp(click)) return false;
            activeAbility.RemainingCooldown = activeAbility.Ability.Cooldown;
            activeAbility = null;
            return true;
        }

        private void Update() {
            abilities.Where(a => a.RemainingCooldown > 0).ToList()
                .ForEach(ability =>
                    ability.RemainingCooldown = Math.Max(ability.RemainingCooldown - Time.deltaTime, 0));
            if (!active) return;
            if (activeAbility != null) activeAbility.Ability.OnUpdate();
            if (Input.GetKey(KeyCode.Escape) && activeAbility != null) {
                activeAbility.Ability.OnCancel();
                activeAbility = null;
            }
            // Check user input to activate abliity
            ActiveAbility newActiveAbility = null;
            abilities.ForEach(ability => {
                var mod = ability.Ability.Modifier == 0 || Input.GetKey(ability.Ability.Modifier);
                if (!(ability.RemainingCooldown <= 0) || !mod || !Input.GetKey(ability.Ability.Hotkey)) return;
                newActiveAbility = ability;
            });
            if (newActiveAbility == null) return;
            if (activeAbility != null) activeAbility.Ability.OnCancel();
            activeAbility = newActiveAbility;
            if (!activeAbility.Ability.OnActivation(gameObject)) return;
            activeAbility.RemainingCooldown = activeAbility.Ability.Cooldown;
            activeAbility = null;
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