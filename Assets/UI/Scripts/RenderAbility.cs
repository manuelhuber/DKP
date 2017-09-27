using System;
using System.Text.RegularExpressions;
using Control;
using UnityEngine;
using UnityEngine.UI;
using Util;

namespace UI.Scripts {
    public class RenderAbility : MonoBehaviour {
        public Text Hotkey;
        public Text Cooldown;
        public Text TooltipName;
        public Text TooltipCooldown;
        public Text TooltipDescription;
        public Image Icon;
        [HideInInspector] public ActiveAbility Ability;

        public void Render(ActiveAbility ability) {
            Ability = ability;
            Icon.sprite = ability.Ability.Icon;
            Hotkey.text = ability.Hotkey.ToString();
            TooltipName.text = ability.Ability.name;
            TooltipCooldown.text = "Cooldown: " + ability.Ability.Cooldown;
            TooltipDescription.text = CreateTooltip(ability.Ability);
        }

        public void Update() {
            var cooldown = Math.Round(Ability.RemainingCooldown, 1, MidpointRounding.AwayFromZero);
            Cooldown.text = cooldown > 0 ? cooldown.ToString("n1") : "";
        }

        private string CreateTooltip(Ability ability) {
            return Regex.Replace(ability.Tooltip, @"\{\{(.*?)\}\}",
                match => ability.GetPropValue(match.Groups[1].Value).ToString());
        }
    }
}