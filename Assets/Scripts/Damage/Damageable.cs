using System;
using System.Collections.Generic;
using System.Linq;
using Generic;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.UI;

[Serializable]
public class DamageInterceptor {
    public int Order = 5;
    public Func<int, int> Interceptor;
}

namespace Damage {
    [RequireComponent(typeof(Team))]
    public class Damageable : MonoBehaviour {
        public int MaxHitpoints = 100;
        public GameObject HealthbarPrefab;
        public GameObject CombatTextPrefab;
        public float CanvasOffsetTop;

        private GameObject canvas;
        private readonly List<Slider> healthbars = new List<Slider>();
        private int hitpoints;
        private bool dead;
        private readonly List<DamageInterceptor> damageInterceptors = new List<DamageInterceptor>();

        private int Hitpoints {
            get { return hitpoints; }
            set { hitpoints = Mathf.Min(value, MaxHitpoints); }
        }

        public void AddHealthbar(Slider bar) {
            bar.maxValue = MaxHitpoints;
            bar.value = hitpoints;
            healthbars.Add(bar);
        }

        public void AddDamageInterceptor(DamageInterceptor interceptor) {
            damageInterceptors.Add(interceptor);
        }

        public void RemoveDamageInterceptor(DamageInterceptor interceptor) {
            damageInterceptors.Remove(interceptor);
        }

        public bool IsDead() {
            return dead;
        }

        public void Revive(int amount) {
            dead = false;
            Hitpoints = amount;
        }

        public void ModifyHitpoints(int initialAmount) {
            if (dead) return;
            var orderedInterceptors = damageInterceptors.OrderBy(interceptor => interceptor.Order);

            var amount =
                orderedInterceptors.Aggregate(initialAmount, (acc, interceptor) => interceptor.Interceptor(acc));
            Hitpoints += amount;
            healthbars.ForEach(bar => bar.value = Hitpoints);
            var textObject = Instantiate(CombatTextPrefab, canvas.transform, false);
            textObject.GetComponent<Text>().text = amount.ToString();
            textObject.GetComponent<Animator>().SetTrigger(amount < 0 ? "Hit" : "Heal");
            Destroy(textObject, 3);
            if (Hitpoints <= 0) CoreDie();
        }

        /// <summary>
        /// Can be overriden to allow custom code on death
        /// </summary>
        protected virtual void Die() {
            var textObject = Instantiate(CombatTextPrefab, canvas.transform, false);
            textObject.GetComponent<Text>().text = "X.X DEAD";
            Destroy(textObject, 3);
        }

        /// <summary>
        /// This needs to be called always
        /// </summary>
        private void CoreDie() {
            dead = true;
            Die();
        }

        private void Awake() {
            Hitpoints = MaxHitpoints;
            InitCanvas();
        }

        /// <summary>
        /// Generates a local canvas above the gameobject for healthbar and combat text
        /// </summary>
        private void InitCanvas() {
            // Create a new local canvas that's attached to this gameobject
            canvas = new GameObject {name = "UnitCanvas"};
            canvas.transform.localScale = new Vector3(0.03f, 0.03f, 0.03f);
            var transformPosition = canvas.transform.position;
            transformPosition.y = CanvasOffsetTop;
            canvas.transform.position = transformPosition;
            canvas.transform.SetParent(transform, false);
            canvas.AddComponent<Canvas>();
            // Make the canvas face the camera
            canvas.AddComponent<Billboarding>();

            var healthbar = Instantiate(HealthbarPrefab, canvas.transform, false).GetComponent<Slider>();
            healthbar.maxValue = MaxHitpoints;
            healthbar.value = Hitpoints;
            healthbars.Add(healthbar);
        }
    }
}