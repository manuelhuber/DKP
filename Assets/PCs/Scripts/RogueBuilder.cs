using Damage.Melee;
using UnityEngine;

namespace PCs.Scripts {
    public class RogueBuilder : CharacterBuilder {
        public override GameObject ModifyHero(GameObject hero) {
            var melee = hero.GetComponent<MeleeAttack>();
            melee.DefaultAttack = MeleeAttackVariants.Backstab(1.5);
            return hero;
        }
    }
}