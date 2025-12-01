using Fusion;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerAni : NetworkBehaviour
{
    [SerializeField] NetworkMecanimAnimator _mecAnim;

    public void MoveAnimation(bool onMove)
    {
        _mecAnim.Animator.SetBool("Move", onMove);
    }
}
