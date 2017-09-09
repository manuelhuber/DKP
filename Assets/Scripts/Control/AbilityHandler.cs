using System.Collections.Generic;
using Spells;
using UI;
using UnityEngine;

namespace Control {
    public class AbilityHandler : MonoBehaviour {
        public List<GameObject> AbilityPrefabs;

        private readonly List<Ability> abilities = new List<Ability>();

        public bool Active {
            get { return active; }
            set {
                abilityRenderer.DisplayAbilities(value ? abilities : null);
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
            if (Input.GetKey(KeyCode.Escape) && activeAbility != null) {
                activeAbility.OnCancel();
                activeAbility = null;
            }
            if (!active || gcdEnd > Time.time || activeAbility != null) return;
            abilities.ForEach(ability => {
                var mod = ability.Modifier == 0 || Input.GetKey(ability.Modifier);
                if (mod && Input.GetKey(ability.Hotkey)) {
                    activeAbility = ability;
                }
            });
            if (activeAbility == null || !activeAbility.OnActivation(gameObject)) return;
            activeAbility = null;
            gcdEnd = Time.time + GlobalCooldown;
        }

        private void Awake() {
            AbilityPrefabs.ForEach(o => abilities.Add(Instantiate(o).GetComponent<Ability>()));
        }

        private void Start() {
            abilityRenderer = RaidUi.GetAbilityRenderer();
        }
    }
}