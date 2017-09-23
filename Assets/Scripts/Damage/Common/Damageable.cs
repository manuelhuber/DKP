﻿using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Raid;
using UnityEngine;
using UnityEngine.UI;

[Serializable]
public class DamageInterceptor {
    public int Order = 5;
    public Func<int, int> Interceptor;
}

namespace Damage {
    [RequireComponent(typeof(Team))]
    public class Damageable : MonoBehaviour {
        public GameObject HealthbarPrefab;
        public GameObject CombatTextPrefab;
        public float CanvasOffsetTop;

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
        private int maxHitpoints = 100;

        private int Hitpoints {
            get { return hitpoints; }
            set { hitpoints = Mathf.Min(value, MaxHitpoints); }
        }

        public void DisplayText(string text) {
            var textObject = Instantiate(CombatTextPrefab, canvas.transform, false);
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
            StartCoroutine(RemoveInterceptorAfterTime(interceptor, duration));
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
            ModifyHitpoints(amount > 0 ? amount : MaxHitpoints);
            var immune = new DamageInterceptor {
                Interceptor = (int x) => 0,
                Order = 1
            };
            AddDamageInterceptorWithDuration(immune, vulnerabilityDuartion);
        }

        public void ModifyHitpoints(int initialAmount) {
            if (dead) return;
            var orderedInterceptors = damageInterceptors.OrderBy(interceptor => interceptor.Order);

            var amount =
                orderedInterceptors.Aggregate(initialAmount, (acc, interceptor) => interceptor.Interceptor(acc));
            if (amount == 0) return;
            Hitpoints += amount;
            healthbars.ForEach(bar => bar.value = Hitpoints);
            var textObject = Instantiate(CombatTextPrefab, canvas.transform, false);
            textObject.GetComponent<Text>().text = amount.ToString();
            textObject.GetComponent<Animator>().SetTrigger(amount < 0 ? "Hit" : "Heal");
            Destroy(textObject, 3);
            if (Hitpoints <= 0) Die();
            else OnHit(amount);
        }

        /// <summary>
        /// Can be overriden to allow custom code on hit
        /// </summary>
        protected virtual void OnHit(int amount) {
        }


        /// <summary>
        /// This needs to be called always
        /// </summary>
        protected virtual void Die() {
            dead = true;
            TargetManager.RemoveTarget(gameObject);
        }

        private IEnumerator RemoveInterceptorAfterTime(DamageInterceptor interceptor, float duration) {
            yield return new WaitForSeconds(duration);
            RemoveDamageInterceptor(interceptor);
        }

        private void Awake() {
            Hitpoints = MaxHitpoints;
        }

        /// <summary>
        /// Generates a local canvas above the gameobject for healthbar and combat text
        /// </summary>
        protected virtual void Start() {
            canvas = GetComponentInChildren<Canvas>().gameObject;
            AddHealthbar(GetComponentInChildren<Slider>());
            TargetManager.AddTarget(gameObject);
        }
    }
}