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

    public IFindRocks(AIController aIController, BoulderGennie boulderGennie)
    {
        aIController = aIController;
        boulderGennie = boulderGennie;
        
    }

    public void OnEnter()
    {
       
    }

    public void OnExit()
    {
        
    }

    public void Tick()
    {
        //Play Idle Anim or Guard Anim
    }

}
