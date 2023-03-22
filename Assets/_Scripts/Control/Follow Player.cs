using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Rambler.Control;
using Rambler.Movement;

public class FollowPlayer : IState
{
    private readonly AIController _aIController;
    private readonly GameObject _player;
    float followPlayerTimer = 2f;
    public Mover _mover;

    public FollowPlayer(AIController aIController, GameObject player, Mover mover)
    {
        _aIController = aIController;
        _player = player;
        _mover = mover;
    }
    
    void PlayerFollow()
    {                  
        Vector3 playerDirection = _player.transform.position - _aIController.gameObject.transform.position;
        Vector3 randPos = (_player.transform.position + (_aIController.gameObject.transform.forward * 8.5f))
                          + new Vector3(UnityEngine.Random.Range( -1.5f, 5.5f), 0f,
                          UnityEngine.Random.Range(-1.5f, 3.5f)); 
                              

        if(playerDirection.magnitude > 4f)
        {
            _mover.MoveTo(destination: randPos, speedFraction: 7f); 
        }             
    }

    public void OnEnter()
    {
        _aIController.FolllowingPlayer();
    } 

    public void OnExit()
    {
        _aIController.NotFollowingPlayer();
    }

    public void Tick()
    {
        followPlayerTimer += Time.deltaTime;
        if (followPlayerTimer >= 1f) 
        {
            followPlayerTimer = followPlayerTimer % 1f;         
            PlayerFollow();
        }
    }
}
