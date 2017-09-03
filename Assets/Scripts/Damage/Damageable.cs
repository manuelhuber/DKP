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
        private int _hitpoints;

        private int Hitpoints {
            get { return _hitpoints; }
            set { _hitpoints = Mathf.Min(value, MaxHitpoints); }
        }

        private void Awake() {
            Hitpoints = MaxHitpoints;
            initCanvas();
        }

        public void CauseDamage(int dmg) {
            modifyHitpoints(-dmg);
        }

        public void Heal(int amount) {
            modifyHitpoints(amount);
        }

        private void initCanvas() {
            canvas = new GameObject {name = "UnitCanvas"};
            canvas.transform.localScale = new Vector3(0.03f, 0.03f, 0.03f);
            var transformPosition = canvas.transform.position;
            transformPosition.y = OffsetTop;
            canvas.transform.position = transformPosition;


            canvas.transform.SetParent(transform, false);
            canvas.AddComponent<Billboarding>();
            canvas.AddComponent<Canvas>();

            healthbar = Instantiate(HealthbarPrefab, canvas.transform, false).GetComponent<Slider>();
            healthbar.maxValue = MaxHitpoints;
            healthbar.value = Hitpoints;
        }

        private void modifyHitpoints(int amount) {
            Hitpoints += amount;
            healthbar.value = Hitpoints;
        }
    }
}