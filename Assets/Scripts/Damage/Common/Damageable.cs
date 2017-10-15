using System;
using System.Collections.Generic;
using System.Linq;
using Damage.Common;
using Generic;
using Raid;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.Serialization;
using UnityEngine.UI;

[Serializable]
public class DamageInterceptor {
    public int Order = 5;
    public Func<int, int> Interceptor;
}

namespace Damage {
    [RequireComponent(typeof(Team))]
    public class Damageable : DkpMonoBehaviour {
        [SerializeField] private GameObject canvasPrefab;
        [SerializeField] private GameObject combatTextPrefab;
        public float CanvasOffsetTop = 1.5f;
        public int CanvasWidth = 75;
        public bool Untargetable;

        public bool Targetable {
            get { return !IsDead() && !Untargetable; }
        }

        public int MaxHitpoints {
            get { return maxHitpoints; }
            set {
                var dif = value - maxHitpoints;
                maxHitpoints = value;
                hitpoints += dif;
            }
        }

        private GameObject canvas;
        private readonly List<Slider> healthbars = new List<Slider>();
        private int hitpoints;
        private bool dead;
        private readonly List<DamageInterceptor> damageInterceptors = new List<DamageInterceptor>();
        [SerializeField] private int maxHitpoints = 100;

        private int Hitpoints {
            get { return hitpoints; }
            set { hitpoints = Mathf.Min(value, MaxHitpoints); }
        }

        public void MakeUntargetableFor(float duration) {
            Untargetable = true;
            DoAfterDelay(() => Untargetable = false, duration);
        }

        public void DisplayText(string text) {
            var textObject = Instantiate(combatTextPrefab, canvas.transform, false);
            textObject.GetComponent<Text>().text = text;
            Destroy(textObject, 3);
        }

        public void AddHealthbar(Slider bar) {
            bar.maxValue = MaxHitpoints;
            bar.value = hitpoints;
            healthbars.Add(bar);
        }

        public void AddDamageInterceptor(DamageInterceptor interceptor) {
            damageInterceptors.Add(interceptor);
        }

        public void AddDamageInterceptorWithDuration(DamageInterceptor interceptor, float duration) {
            damageInterceptors.Add(interceptor);
            DoAfterDelay(() => RemoveDamageInterceptor(interceptor), duration);
        }

        public void RemoveDamageInterceptor(DamageInterceptor interceptor) {
            damageInterceptors.Remove(interceptor);
        }

        public bool IsDead() {
            return dead;
        }

        public void Revive(int amount, float vulnerabilityDuartion) {
            if (!dead) return;
            dead = false;
            TargetManager.Instance.AddTarget(gameObject);
            Heal(amount > 0 ? amount : MaxHitpoints);
            var immune = new DamageInterceptor {
                Interceptor = (int x) => 0,
                Order = 1
            };
            AddDamageInterceptorWithDuration(immune, vulnerabilityDuartion);
            ToggleCommonComponents(true);
        }

        public void Heal(int amount) {
            ModifyHitpoints(amount);
        }

        public void Damage(int amount) {
            ModifyHitpoints(-amount);
        }

        /// <summary>
        /// Damage or heal the target.
        /// Negative numbers deal damage
        /// Positive numbers heal
        /// The amount will be modified depending on the units damage modifiers
        /// </summary>
        /// <param name="initialAmount">
        /// The unmodified amount
        /// </param>
        public void ModifyHitpoints(int initialAmount) {
            if (dead) return;
            var amount = damageInterceptors
                .OrderBy(interceptor => interceptor.Order)
                .Aggregate(initialAmount, (acc, interceptor) => interceptor.Interceptor(acc));
            if (amount == 0) return;
            Hitpoints += amount;
            healthbars.ForEach(bar => bar.value = Hitpoints);
            var textObject = Instantiate(combatTextPrefab, canvas.transform, false);
            textObject.GetComponent<Text>().text = amount.ToString();
            textObject.GetComponent<Animator>().SetTrigger(amount < 0 ? "Hit" : "Heal");
            Destroy(textObject, 3);
            if (Hitpoints <= 0) Die();
            else OnHit(amount);
        }

        /// <summary>
        /// Can be overriden to allow custom code when receiving damage or heal
        /// </summary>
        protected virtual void OnHit(int amount) {
        }

        private void InitCanvas() {
            // Create a new local canvas that's attached to this gameobject
            canvas = Instantiate(canvasPrefab, gameObject.transform, false);

            // Set width
            var rectTransform = canvas.GetComponent<RectTransform>();
            rectTransform.sizeDelta = new Vector2(CanvasWidth, rectTransform.sizeDelta[1]);

            // Change vertical position
            var transformPosition = canvas.transform.localPosition;
            transformPosition.y = CanvasOffsetTop;
            canvas.transform.localPosition = transformPosition;

            AddHealthbar(canvas.GetComponentInChildren<Slider>());
        }


        /// <summary>
        /// This needs to be called always
        /// </summary>
        protected virtual void Die() {
            dead = true;
            ToggleCommonComponents(false);
            TargetManager.Instance.RemoveTarget(gameObject);
        }

        private void ToggleCommonComponents(bool value) {
            var agent = GetComponent<NavMeshAgent>();
            if (agent != null) agent.enabled = value;
            var attack = GetComponent<Attack>();
            if (attack != null) attack.enabled = value;
            var body = GetComponent<Rigidbody>();
            if (body != null) body.isKinematic = !value;
        }

        private void Awake() {
            Hitpoints = MaxHitpoints;
            InitCanvas();
        }

        /// <summary>
        /// Generates a local canvas above the gameobject for healthbar and combat text
        /// </summary>
        protected virtual void Start() {
            TargetManager.Instance.AddTarget(gameObject);
        }
    }
}