using System.Collections.Generic;
using Control;
using UnityEngine;
using UnityEngine.UI;

namespace UI {
    public class AbilityRenderer : MonoBehaviour {
        public GameObject AbilityPrefab;

        private List<GameObject> renderedAbilities = new List<GameObject>();
        private GameObject canvas;
        private Transform folder;

        public void DisplayAbilities(List<Ability> abilities) {
            renderedAbilities.ForEach(Destroy);
            renderedAbilities.Clear();
            if (abilities == null) return;
            abilities.ForEach(DisplayAbility);
        }

        public void DisplayAbility(Ability ab) {
            var ability = Instantiate(AbilityPrefab);
            renderedAbilities.Add(ability);
            ability.transform.SetParent(folder);
            ability.GetComponentInChildren<Image>().sprite = ab.Icon;
            var hotkey = ability.GetComponentInChildren<Text>();
            hotkey.text = ab.Hotkey.ToString();
            var pos = new Vector3 {
                y = 15,
                x = 100
            };
            ability.transform.position = pos;
        }

        private void Start() {
            canvas = GameObject.FindWithTag("Canvas");
            var foo = new GameObject("Abilities");
            foo.transform.SetParent(canvas.transform);
            folder = foo.transform;
        }
    }
}