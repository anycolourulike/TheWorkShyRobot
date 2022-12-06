using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Rambler.Combat;
using Rambler.Core;
using Rambler.Movement;
using Rambler.Control;

public class IFindRocks : IState
{
    private readonly AIController aIController;
    private readonly BoulderGennie boulderGennie;
    private readonly Mover mover;

    public IFindRocks(AIController aIController, BoulderGennie boulderGennie, Mover mover)
    {
        aIController = aIController;
        boulderGennie = boulderGennie;
        mover = mover;        
    }

    public void OnEnter()
    {
        BoulderGennie.findRocks.Invoke();
    }

    public void OnExit()
    {
        
    }

    public void Tick()
    {
       
    }  

}
