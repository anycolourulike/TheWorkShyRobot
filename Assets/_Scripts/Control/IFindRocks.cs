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
       //As soon as boulders land assign nearestBoulder
       mover.StartMoveAction(boulderGennie.nearestBoulder.transform.position, 5);
       //pick up boulder
       //remove picked up boulder from gennie list
       //throw boulder at player
       //repeat until no boulders left
       //jump attack
    }

    public void OnExit()
    {
        
    }

    public void Tick()
    {
       
    }  

}
