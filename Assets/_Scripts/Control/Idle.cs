using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Rambler.Combat;
using Rambler.Core;
using Rambler.Movement;
using Rambler.Control;

public class Idle : IState
{
    private readonly AIController _aIController;
    private readonly Fighter _fighter;

    public Idle(AIController aIController, Fighter fighter)
    {
        _aIController = aIController;
        _fighter = fighter;
    }

    public void OnEnter()
    {
        _aIController.IsIdle();
        _fighter.canAttack = false;
    }

    public void OnExit()
    {
        _aIController.NotIdle();
        _fighter.canAttack = true;
        _aIController.AttackingTrue();
    }

    public void Tick()
    {
       //Play Idle Anim or Guard Anim
    }
}
