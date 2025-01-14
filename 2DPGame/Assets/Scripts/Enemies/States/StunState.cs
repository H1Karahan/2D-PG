﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StunState : State
{
    D_StunState stateData;

    protected bool isStunTimeOver;
    protected bool isGrounded;
    protected bool isMovementStopped;
    protected bool performCloseRangeAction;
    protected bool isPlayerMinAgroRAnge;

    public StunState(Entity etity, FiniteStateMachine stateMachine, string animBoolName, D_StunState stateData) : base(etity, stateMachine, animBoolName)
    {
        this.stateData = stateData;
    }

    public override void DoChecks()
    {
        base.DoChecks();
        isGrounded = entity.CheckGround();
        performCloseRangeAction = entity.CheckPlayerInCloseRangeAction();
        isPlayerMinAgroRAnge = entity.CheckPlayerInMinAgroRange();
    }

    public override void Enter()
    {
        base.Enter();

        isStunTimeOver = false;
        isMovementStopped = false;
        entity.SetVelocity(stateData.stunKnockBackSpeed, stateData.stunKnockBackAngle, entity.lastDamageDirection);
    }

    public override void Exit()
    {
        base.Exit();
        entity.ResetStunResistance();
    }

    public override void LogicUpdate()
    {
        base.LogicUpdate();

        if (Time.time >= startTime + stateData.stunTime)
        {
            isStunTimeOver = true;
        }

        if (isGrounded && Time.time > startTime + stateData.stunKnockBackTime && !isMovementStopped) 
        {
            entity.SetVelocity(0f);
            isMovementStopped = true;
        }
    }

    public override void PhysicsUpdate()
    {
        base.PhysicsUpdate();
    }
}
