using System.Linq;
using UnityEngine;
using Util;

namespace Damage {
    public class PlayerHealth : Damageable {
        protected override void Die() {
            UnityUtil.FindComponentInChildrenWithTag<Animator>(gameObject, "PlayerAnimation").SetTrigger("Die");
        }
    }
}