using Fusion;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Lava : NetworkBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        if (!HasStateAuthority) return;

        if (other.TryGetComponent(out PlayerLife enemyHealth))
        {
            enemyHealth.RPC_TakeDamage(100);
        }
    }
}
