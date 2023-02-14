﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Rambler.Combat;
using Rambler.Core;
using Rambler.Movement;
using Rambler.Control;

public class Attack : IState
{

    private readonly AIController _aIController;
    public GameObject _closestTarget;
    private readonly Mover _mover;
    public CombatTarget _otherCombatTarget;
    private readonly FieldOfView _FOV;    
    public  float _TimerForNextAttack;
    public float _timeSinceLastSawPlayer;
    private readonly float _suspicionTime;
    private readonly float _coolDown;
    public CapsuleCollider _capsuleCol;
    public Fighter _fighter;

    public Attack(AIController aIController, Fighter fighter, Mover mover, FieldOfView FOV, float TimerForNextAttack, float timeSinceLastSawPlayer, float suspicionTime, float coolDown)
    {
        _aIController = aIController;
        _FOV = FOV;
        _TimerForNextAttack = TimerForNextAttack;
        _timeSinceLastSawPlayer = timeSinceLastSawPlayer;
        _suspicionTime = suspicionTime;
        _coolDown = coolDown;
        _fighter = fighter;
        _mover = mover; 
    }

    void AttackTimer()
    {
        if (_FOV.canSeePlayer == true)
        { 
            if (_TimerForNextAttack > 0)
            {
                _TimerForNextAttack -= Time.deltaTime;
            }
            else if (_TimerForNextAttack <= 0)
            {
                if (_capsuleCol != null)
                {
                    _aIController.AttackBehaviour();
                    _TimerForNextAttack = _coolDown;
                }
            }
        }         
    }
    
    private void UpdateTimers()
    {
        _timeSinceLastSawPlayer += Time.deltaTime;
    }

    public void OnEnter()
    { 
        _aIController.TargetHealthCheck();
        _aIController.isAttacking = true;
        _timeSinceLastSawPlayer = 2f;
        _mover.CancelNav();             
    }

    public void OnExit()
    {
        _aIController.isAttacking = false;
        _fighter.TargetCap = null;
        _fighter.CombatTarget = null;
        _fighter.EnemyPos = null;
    }

    public void Tick()
    {        
        AttackTimer();
        UpdateTimers();
    }
}
