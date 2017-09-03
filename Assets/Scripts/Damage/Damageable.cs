using Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Damage {
    public class Damageable : MonoBehaviour {
        public int MaxHitpoints = 100;
        public GameObject HealthbarPrefab;
        public float OffsetTop;

        private GameObject canvas;
        private Slider healthbar;
        private int hitpoints;

        private int Hitpoints {
            get { return hitpoints; }
            set { hitpoints = Mathf.Min(value, MaxHitpoints); }
        }

        private void Awake() {
            Hitpoints = MaxHitpoints;
            InitCanvas();
        }

        public void CauseDamage(int dmg) {
            ModifyHitpoints(-dmg);
        }

        public void Heal(int amount) {
            ModifyHitpoints(amount);
        }

        private void InitCanvas() {
            // Create a new local canvas that's attached to this gameobject
            canvas = new GameObject {name = "UnitCanvas"};
            canvas.transform.localScale = new Vector3(0.03f, 0.03f, 0.03f);
            var transformPosition = canvas.transform.position;
            transformPosition.y = OffsetTop;
            canvas.transform.position = transformPosition;
            canvas.transform.SetParent(transform, false);
            canvas.AddComponent<Canvas>();
            // Make the canvas face the camera
            canvas.AddComponent<Billboarding>();

            healthbar = Instantiate(HealthbarPrefab, canvas.transform, false).GetComponent<Slider>();
            healthbar.maxValue = MaxHitpoints;
            healthbar.value = Hitpoints;
        }

        private void ModifyHitpoints(int amount) {
            Hitpoints += amount;
            healthbar.value = Hitpoints;
        }
    }
}