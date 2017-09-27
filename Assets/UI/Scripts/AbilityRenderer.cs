using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using Control;
using UnityEngine;
using UnityEngine.UI;
using Util;

namespace UI.Scripts {
    public class AbilityRenderer : MonoBehaviour {
        public GameObject AbilityPrefab;

        private readonly List<GameObject> renderedAbilities = new List<GameObject>();
        private Transform folder;

        public void DisplayAbilities(List<ActiveAbility> abs) {
            renderedAbilities.ForEach(Destroy);
            renderedAbilities.Clear();
            if (abs == null) return;
            abs.ForEach(DisplayAbility);
        }

        public void DisplayAbility(ActiveAbility ability) {
            var abilityUi = Instantiate(AbilityPrefab, folder);
            var rect = abilityUi.GetComponent<RectTransform>();
            abilityUi.GetComponent<RenderAbility>().Render(ability);
            var offsetX = renderedAbilities.Count * (rect.rect.height + 10);
            UnityUtil.SetAnchoredPosition(rect, offsetX, 0);
            renderedAbilities.Add(abilityUi);
        }

        private void Start() {
            folder = GameObject.FindWithTag("AbilityFolder").transform;
        }
    }
}