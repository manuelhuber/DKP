using System;
using System.Collections.Generic;
using Control;
using NUnit.Framework.Constraints;
using UnityEngine;
using UnityEngine.UI;
using Util;

namespace UI {
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
            abs.ForEach(aa => DisplayAbility(aa.Ability));
        }

        public void DisplayAbility(Ability ab) {
            var ability = Instantiate(AbilityPrefab, folder);
            ability.GetComponentInChildren<Image>().sprite = ab.Icon;
            var hotkey = UnityUtil.FindComponentInChildrenWithTag<Text>(ability, "HotkeyText");
            hotkey.text = ab.Hotkey.ToString();

//            var cooldown = UnityUtil.FindComponentInChildrenWithTag<Text>(ability, "CooldownText");
//            cooldown.text = ab.CooldownRemaining.ToString();


            var rect = ability.GetComponent<RectTransform>();
            var offsetX = renderedAbilities.Count * (rect.rect.height + 10);
            UnityUtil.SetAnchoredPosition(rect, offsetX, 0);
            renderedAbilities.Add(ability);
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