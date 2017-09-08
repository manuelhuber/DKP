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
            return false;
        }

        public bool OnLeftClickUp(ClickLocation click) {
            return false;
        }

        private void Update() {
            if (!active || gcdEnd > Time.time) return;
            Abilities.ForEach(ability => {
                var mod = ability.Modifier == 0 || Input.GetKey(ability.Modifier);
                if (mod && Input.GetKey(ability.Hotkey)) {
                    activeAbility = ability;
                }
            });
            if (activeAbility == null) return;
            gcdEnd = Time.time + GlobalCooldown;
            if (activeAbility.OnActivation(gameObject)) activeAbility = null;
        }

        private void Start() {
            abilityRenderer = RaidUi.GetAbilityRenderer();
        }
    }
}