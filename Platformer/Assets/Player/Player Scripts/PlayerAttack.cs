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
            TryAttack("Bow");

        if (keyboard.xKey.wasPressedThisFrame)
            TryAttack("Fireball");

        if (keyboard.bKey.wasPressedThisFrame)
            TryAttack("Sword");
    }

    public void TryAttack(string attackName)
    {
        Debug.Log("Try Att " + attackName);
        if (isAttacking || globalCooldownTimer > 0f) return;
        AttackBase attack = attacks.FirstOrDefault(a => a.attackName == attackName);
        if (attack == null) return;
        Debug.Log("Attack is not null");
        StartCoroutine(HandleAttack(attack));
    }

    private IEnumerator HandleAttack(AttackBase attack)
    {
        isAttacking = true;

        if (attack.castTime > 0f)
            yield return new WaitForSeconds(attack.castTime);

        yield return StartCoroutine(attack.Execute(transform, enemyLayer));

        globalCooldownTimer = attack.playerCooldown;
        isAttacking = false;
    }
}