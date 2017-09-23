using System.Collections.Generic;
using Control;
using Damage;
using Damage.Melee;
using Damage.Range;
using UnityEngine;
using UnityEngine.AI;

namespace Raid.Builders {
    public enum AttackType {
        Melee,
        Ranged
    }

    public class CharacterBuilder : ScriptableObject {
        public int Hitpoints;
        public float Speed;
        public List<Ability> Abilities;

        public AttackType AttackType;
        public float AttackRange;
        public float AttackIntervall;
        [HideInInspector] public int AttackDamage;
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
            } else {
                attack = hero.AddComponent<RangeAttack>();
                ((RangeAttack) attack).ProjectilePrefab = ProjectilePrefab;
            }
            attack.AttackInterval = AttackIntervall;
            attack.Range = AttackRange;
        }
    }
}