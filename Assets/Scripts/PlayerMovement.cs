using Fusion;
using Fusion.Addons.Physics;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovement : NetworkCharacterController
{
    public event Action<float> OnMovement;

    PlayerAni playerAnimation;

    private void Awake()
    {
        playerAnimation = GetComponentInChildren<PlayerAni>();
    }

    public override void Move(Vector3 direction)
    {
        if (direction.magnitude > 0)
        {
            playerAnimation.MoveAnimation(true);
        }
        else playerAnimation.MoveAnimation(false);

        var deltaTime = Runner.DeltaTime;
        var previousPos = transform.position;
        var moveVelocity = Velocity;

        direction = direction.normalized;

        if (Grounded && moveVelocity.y < 0)
        {
            moveVelocity.y = 0f;
        }

        moveVelocity.y += gravity * Runner.DeltaTime;

        var horizontalVel = default(Vector3);
        horizontalVel.x = moveVelocity.x;

        if (direction == default)
        {
            horizontalVel = Vector3.Lerp(horizontalVel, default, braking * deltaTime);
        }
        else
        {
            horizontalVel = Vector3.ClampMagnitude(horizontalVel + direction * acceleration * deltaTime, maxSpeed);

            //var rotationValue = 0;

            //if (Mathf.Sign(direction.x) < 0)
            //{
            //    rotationValue = 180;
            //}
            //else
            //{
            //    rotationValue = 0;
            //}
            //transform.rotation = Quaternion.Euler(Vector3.up * rotationValue);
            transform.rotation = Quaternion.Euler(Vector3.up * (Mathf.Sign(direction.x) < 0 ? 180 : 0));

        }

        moveVelocity.x = horizontalVel.x;

        _controller.Move(moveVelocity * deltaTime);

        Velocity = (transform.position - previousPos) * Runner.TickRate;
        Grounded = _controller.isGrounded;

        //OnMovement(Velocity.x);
    }
}
