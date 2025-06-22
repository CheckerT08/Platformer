using UnityEngine;
using System;
using System.Collections;
using System.Linq;

public class PlayerAttack : MonoBehaviour
{
    public AttackBase[] attacks;

    public int primaryAttackID, rangedAttackID, dashAttackID, specialAttackID;

    private bool isAttacking;
    private float globalCooldownTimer;

    private void Start()
    {
        primaryAttackID = Game.Save.Get<int>(Game.Save.primaryAttackLocation);
        rangedAttackID = Game.Save.Get<int>(Game.Save.rangedAttackLocation);
        dashAttackID = Game.Save.Get<int>(Game.Save.dashAttackLocation);
        specialAttackID = Game.Save.Get<int>(Game.Save.specialAttackLocation);
    }

    private void Update()
    {
        globalCooldownTimer -= Time.deltaTime;

        // Example Input (Replace with your actual input handling)
        if (Input.GetKeyDown(KeyCode.Z))
            TryAttack(attacks[0]);
        if (Input.GetKeyDown(KeyCode.X))
            TryAttack(attacks[1]);
        if (Input.GetKeyDown(KeyCode.C))
            TryAttack(attacks[2]);
        if (Input.GetKeyDown(KeyCode.V))
            TryAttack(attacks[3]);
    }

    public void TryAttack(AttackBase attack)
    {
        print("Try");
        if (isAttacking || globalCooldownTimer > 0f) return;
        StartCoroutine(HandleAttack(attack));
    }

    private IEnumerator HandleAttack(AttackBase attack)
    {
        print("handle");
        isAttacking = true;

        if (attack.castTime > 0f)
            yield return new WaitForSeconds(attack.castTime);

        yield return StartCoroutine(attack.Execute(transform.parent, Game.Layer.enemy));

        globalCooldownTimer = attack.playerCooldown;
        yield return new WaitForSeconds(attack.playerCooldown);
        isAttacking = false;
    }
}