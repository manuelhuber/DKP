namespace Damage {
    public class Zylindron : Damageable {
        protected override void Die() {
            Destroy(gameObject, 5);
        }
    }
}