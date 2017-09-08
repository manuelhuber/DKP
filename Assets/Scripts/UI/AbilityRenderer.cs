using System.Collections.Generic;
using Control;
using UnityEngine;
using UnityEngine.UI;
using Util;

namespace UI {
    public class AbilityRenderer : MonoBehaviour {
        public GameObject AbilityPrefab;

        private readonly List<GameObject> renderedAbilities = new List<GameObject>();
        private Transform folder;

        public void DisplayAbilities(List<Ability> abilities) {
            renderedAbilities.ForEach(Destroy);
            renderedAbilities.Clear();
            if (abilities == null) return;
            abilities.ForEach(DisplayAbility);
        }

        public void DisplayAbility(Ability ab) {
            var ability = Instantiate(AbilityPrefab, folder);
            ability.GetComponentInChildren<Image>().sprite = ab.Icon;
            var hotkey = ability.GetComponentInChildren<Text>();
            hotkey.text = ab.Hotkey.ToString();

            var rect = ability.GetComponent<RectTransform>();
            var offsetX = renderedAbilities.Count * (rect.rect.height + 10);
            UnityUtil.SetAnchoredPosition(rect, offsetX, 0);
            renderedAbilities.Add(ability);
        }

        private void Start() {
            folder = GameObject.FindWithTag("AbilityFolder").transform;
        }
    }
}