using System.Collections.Generic;
using Control;
using Damage;
using Damage.Common;
using Damage.Melee;
using Damage.Range;
using UnityEngine;
using UnityEngine.AI;

namespace PCs.Scripts {
    public enum AttackType {
        Melee,
        Ranged
    }

    public class CharacterBuilder : ScriptableObject {
        public int Hitpoints;
        public float Speed;
        public List<Ability> Abilities;

        [HideInInspector] public AttackType AttackType;
        [HideInInspector] public float AttackRange;
        [HideInInspector] public float AttackIntervall;
        [HideInInspector] public int AttackDamage;
        [HideInInspector] public float MeleeAnimationOffset;
        [HideInInspector] public GameObject ProjectilePrefab;

        public GameObject MakeHero(GameObject hero) {
            hero.GetComponent<Damageable>().MaxHitpoints = Hitpoints;
            hero.GetComponent<NavMeshAgent>().speed = Speed;
            hero.GetComponent<AbilityHandler>().AbilityScripts = Abilities;
            AddAttack(hero);
            ModifyHero(hero);
            return hero;
        }

        public virtual GameObject ModifyHero(GameObject hero) {
            return hero;
        }

        private void AddAttack(GameObject hero) {
            Attack attack;
            if (AttackType == AttackType.Melee) {
                attack = hero.AddComponent<MeleeAttack>();
                ((MeleeAttack) attack).AttackDamage = AttackDamage;
                ((MeleeAttack) attack).AnimationOffset = MeleeAnimationOffset;
            } else {
                attack = hero.AddComponent<RangeAttack>();
                ((RangeAttack) attack).ProjectilePrefab = ProjectilePrefab;
            }
            attack.AttackInterval = AttackIntervall;
            attack.Range = AttackRange;
        }
    }
}