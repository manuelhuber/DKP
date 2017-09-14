using Damage;

namespace Boss.Zylindron {
    public class ZylindronDeath : Damageable {
        protected override void Die() {
            Destroy(gameObject, 5);
        }
    }
}