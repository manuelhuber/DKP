using Damage;

namespace Boss.Zylindron {
    public class ZylindronDeath : Damageable {
        protected override void Die() {
            base.Die();
            Destroy(gameObject, 5);
        }
    }
}