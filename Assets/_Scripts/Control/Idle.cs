﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Rambler.Combat;
using Rambler.Core;
using Rambler.Movement;
using Rambler.Control;

public class Idle : IState
{
    private readonly AIController _aIController;

    public Idle(AIController aIController)
    {
        _aIController = aIController;
    }

    public void OnEnter()
    {
        _aIController.IsIdle();
    }

    public void OnExit()
    {
        _aIController.NotIdle();
    }

    public void Tick()
    {
       //Play Idle Anim or Guard Anim
    }
}
