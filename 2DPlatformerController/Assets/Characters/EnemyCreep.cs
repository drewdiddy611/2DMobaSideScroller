﻿using Assets.Attributes;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyCreep : PhysicsObjectBasic, IDamagable
{
    #region Properties
    public DamagableAttributes damagableAttributes = new DamagableAttributes();
    public VitalityAttributes vitalityAttributes = new VitalityAttributes();
    DamageManager dmgManager = new DamageManager();
    private SpriteRenderer spriteRenderer;
    private Animator animator;
    
    #endregion

    // Use this for initialization
    private void Start()
    {
        TheCollisionDetector.CollisionDetectedEvent += TheCollisionDetector_CollisionDetectedEvent;
       
        spriteRenderer = GetComponent<SpriteRenderer>();
        animator = GetComponent<Animator>();
        
        Physics2D.IgnoreLayerCollision(14, 14);
        Physics2D.IgnoreLayerCollision(15, 15);
    }

    protected override void ComputeVelocity()
    {
        var move = Vector2.right;

        if(vitalityAttributes.Team == TAGS.Team2)
            move = Vector2.left;


         bool flipSprite = (spriteRenderer.flipX ? (vitalityAttributes.Team == TAGS.Team2) : (vitalityAttributes.Team == TAGS.Team1));
         if (!flipSprite)
         {
             spriteRenderer.flipX = !spriteRenderer.flipX;
         }

        //animator.SetBool("grounded", TheCollisionDetector.IsGrounded);
        // animator.SetFloat("velocityX", Mathf.Abs(velocity.x) / maxSpeed);
        //print(":::::::" + Mathf.Abs(velocity.x) / maxSpeed);
        TargetVelocity = move * movementAttributes.MaxSpeed;
    }

    private void TheCollisionDetector_CollisionDetectedEvent(RaycastHit2D secondaryCollider, Rigidbody2D primaryCollider)
    {
        // Attack(secondaryCollider, primaryCollider);
        var possibleTarget = secondaryCollider.rigidbody.gameObject.GetComponent<IDamagable>();
        if (targets.Count < GetDamagableAttributes().Cleave  && !targets.Contains(possibleTarget))
        {
            targets.Add(possibleTarget);
        }
        foreach (var item in targets)
        {
            Attack(item, primaryCollider);
        }
    }

    public DamagableAttributes GetDamagableAttributes()
    {
        return damagableAttributes;
    }

    public VitalityAttributes GetVitalityAttributes()
    {
        return vitalityAttributes;
    }

    private void Update()
    {
        
    }
    private void Attack(IDamagable trgt, Rigidbody2D primaryCollider)
    {
        dmgManager.DistributeDamageWithInvincible(trgt.GetVitalityAttributes(), damagableAttributes, this.damagableAttributes.AttackDamage);
        //trgt.GetVitalityAttributes(), damagableAttributes( this.damagableAttributes.AttackDamage);
        animator.SetBool("basicAttack", true);
      //  secondaryCollider.rigidbody.gameObject.GetComponent<IDamagable>().TakeDamage(this.damagableAttributes.AttackDamage);
    }

    public void TakeDamage(int damage)
    {
        var shouldBeDestroyed = dmgManager.DistributeDamageWithInvincible(vitalityAttributes, damagableAttributes, damage);

        if (shouldBeDestroyed)
        {
            Destroy(this.gameObject);
        }
    }

}
