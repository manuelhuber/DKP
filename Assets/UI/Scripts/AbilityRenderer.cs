using System;
using System.Collections.Generic;
using Control;
using UnityEngine;
using UnityEngine.UI;
using Util;

namespace UI.Scripts {
    public class AbilityRenderer : MonoBehaviour {
        public GameObject AbilityPrefab;

        private readonly List<GameObject> renderedAbilities = new List<GameObject>();
        private List<ActiveAbility> abilities;
        private Transform folder;

        public void DisplayAbilities(List<ActiveAbility> abs) {
            renderedAbilities.ForEach(Destroy);
            renderedAbilities.Clear();
            abilities = abs;
            if (abs == null) return;
            abs.ForEach(DisplayAbility);
        }

        public void DisplayAbility(ActiveAbility ability) {
            var abilityIcon = Instantiate(AbilityPrefab, folder);
            abilityIcon.GetComponentInChildren<Image>().sprite = ability.Ability.Icon;
            var hotkey = UnityUtil.FindComponentInChildrenWithTag<Text>(abilityIcon, "HotkeyText");
            hotkey.text = ability.Hotkey.ToString();

            var rect = abilityIcon.GetComponent<RectTransform>();
            var offsetX = renderedAbilities.Count * (rect.rect.height + 10);
            UnityUtil.SetAnchoredPosition(rect, offsetX, 0);
            renderedAbilities.Add(abilityIcon);
        }

        private void Start() {
            folder = GameObject.FindWithTag("AbilityFolder").transform;
        }


        private void Update() {
            var i = 0;
            if (abilities == null) return;
            abilities.ForEach(ability => {
                var cooldown = Math.Round(ability.RemainingCooldown, 1, MidpointRounding.AwayFromZero);
                UnityUtil.FindComponentInChildrenWithTag<Text>(renderedAbilities[i], "CooldownText").text =
                    cooldown > 0 ? cooldown.ToString("n1") : "";
                i++;
            });
        }
    }
}