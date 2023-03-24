using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Rambler.Core;
using Rambler.Combat;
using Rambler.Control;
using Rambler.Movement;

public class IReload : IState
{

    private readonly AIController _aIController;
    private readonly Fighter _fighter;

    public IReload(AIController aIController, Fighter fighter)
    {
        _aIController = aIController;
        _fighter = fighter;
    }

    
    public void OnEnter()
    {
        _fighter.canAttack = false;
        _aIController.AttackingFalse();
        _aIController.NotIdle();
        _fighter.ReloadSeq();
        //ActiveWeapon.Reload();
        //AIRigLayer.SetTrigger("Reload");
    }

    public void OnExit()
    {
        _fighter.canAttack = true;
    }

    public void Tick()
    {
        
    }
}
