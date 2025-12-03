using UnityEngine;

[CreateAssetMenu(fileName = "KamikazeAttack", menuName = "Game/Enemy/Attack/Kamikaze")]
public class KamikazeAttackSO : EnemyAttackSO
{
    [SerializeField] private float explosionRadius = 2.0f;
    [SerializeField] private int damage = 1;
    [SerializeField] private float triggerDistance = 0.5f;

    public override float ExecuteAttack(IActor attacker, EnemyWeapon weapon, IActor target, EnemyDataSO data)
    {
        EnemyMovement movement = attacker.GetAttachedComponent<EnemyMovement>();
        if (movement == null) return 0.1f;

        // FIX: Cast to the new State Class
        KamikazeState state = movement.RuntimeState as KamikazeState;

        // Check if state exists and has a locked target
        if (state == null || !state.lockedTarget.HasValue) return 0.1f;

        float dist = Vector3.Distance(attacker.GetTransform().position, state.lockedTarget.Value);

        if (dist <= triggerDistance)
        {
            Explode(attacker);
        }

        return 0.1f;
    }

    private void Explode(IActor attacker)
    {
        Collider[] hits = Physics.OverlapSphere(attacker.GetTransform().position, explosionRadius);
        foreach (var hit in hits)
        {
            if (hit.CompareTag("Player"))
            {
                IDamageHandler handler = hit.GetComponentInParent<IDamageHandler>();
                if (handler != null)
                {
                    handler.HandleDamage(new DamageInfo(damage, attacker));
                }
            }
        }

        IDamageHandler selfHealth = attacker.GetAttachedComponent<HealthComponent>();
        if (selfHealth != null)
        {
            selfHealth.HandleDamage(new DamageInfo(9999, attacker));
        }
    }
}