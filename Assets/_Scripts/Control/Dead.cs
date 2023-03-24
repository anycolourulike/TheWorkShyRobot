using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Rambler.Combat;
using Rambler.Core;
using Rambler.Movement;
using Rambler.Control;
using UnityEngine.Animations.Rigging;

public class Dead : IState
{
    private readonly AIController _aICon;
    public CombatTarget _combatTarget;
    public Mover _mover;
    public Fighter _fighter;

    public Dead(AIController aIController, CombatTarget combatTarget, Mover mover, Fighter fighter)
    {
        _aICon = aIController;
        _combatTarget = combatTarget;
        _mover = mover;
        _fighter = fighter;
    }

    public void OnEnter()
    {
        _combatTarget.enabled = false;  
        _mover.enabled = false;
        _fighter.enabled = false;
        _aICon.NotPatrolling();
        _aICon.NotIdle();
        _aICon.AttackingFalse();
        _aICon.isFollowingPlayer = false;
        

    }

    public void OnExit()
    {
        
    }

    public void Tick()
    {
       
    }
}
