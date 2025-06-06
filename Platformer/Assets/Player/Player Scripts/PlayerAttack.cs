using UnityEngine;
using System;
using System.Collections;
using System.Linq;
using UnityEngine.InputSystem;

public class PlayerAttack : MonoBehaviour
{
    [SerializeField] private LayerMask enemyLayer;
    [SerializeField] private AttackBase[] attacks;

    private bool isAttacking;
    private float globalCooldownTimer;

    private void Update()
    {
        globalCooldownTimer -= Time.deltaTime;

        // Example Input (Replace with your actual input handling)
        var keyboard = Keyboard.current;
        if (keyboard == null) return;

        if (keyboard.zKey.wasPressedThisFrame)
            TryAttack(attacks[0]);
        if (keyboard.xKey.wasPressedThisFrame)
            TryAttack(attacks[1]);
        if (keyboard.cKey.wasPressedThisFrame)
            TryAttack(attacks[2]);
        if (keyboard.vKey.wasPressedThisFrame)
            TryAttack(attacks[3]);
    }

    public void TryAttack(AttackBase attack)
    {
        if (isAttacking || globalCooldownTimer > 0f) return;
        StartCoroutine(HandleAttack(attack));
    }

    private IEnumerator HandleAttack(AttackBase attack)
    {
        isAttacking = true;

        if (attack.castTime > 0f)
            yield return new WaitForSeconds(attack.castTime);

        yield return StartCoroutine(attack.Execute(transform.parent, enemyLayer));

        globalCooldownTimer = attack.playerCooldown;
        yield return new WaitForSeconds(attack.playerCooldown);
        isAttacking = false;
    }
}