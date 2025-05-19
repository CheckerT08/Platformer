using UnityEngine;
using System;
using System.Collections;
using System.Linq;

public class PlayerAttack : MonoBehaviour
{
    [SerializeField] private Player player;
    [SerializeField] private LayerMask enemyLayer;
    [SerializeField] private Attack[] attacks;

    private bool isAttacking;
    private float globalCooldownTimer;

    private void Update()
    {
        globalCooldownTimer -= Time.deltaTime;

        // Example Input (Replace with your actual input handling)
        if (Input.GetKeyDown(KeyCode.Z))
            TryAttack("Slash");
        if (Input.GetKeyDown(KeyCode.X))
            TryAttack("Fireball");
    }

    public void TryAttack(string attackName)
    {
        if (isAttacking || globalCooldownTimer > 0f) return;

        Attack attack = attacks.FirstOrDefault(a => a.attackName == attackName);
        if (attack == null) return;

        StartCoroutine(HandleAttack(attack));
    }

    private IEnumerator HandleAttack(Attack attack)
    {
        isAttacking = true;

        // Optional: Add cast time delay
        if (attack.castTime > 0f)
            yield return new WaitForSeconds(attack.castTime);

        yield return StartCoroutine(attack.Execute(transform, enemyLayer));

        globalCooldownTimer = attack.playerCooldown;
        isAttacking = false;
    }
}
