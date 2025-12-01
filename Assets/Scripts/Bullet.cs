using Fusion.Addons.Physics;
using Fusion;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bullet : NetworkBehaviour
{
    [SerializeField] NetworkRigidbody3D _netRb;
    [SerializeField] float _initialImpulse;

    [SerializeField] float _lifeTime;
    [SerializeField] byte _damage;

    TickTimer _lifeTimer;

    [SerializeField] private float _launchForce;
    [SerializeField] private NetworkTransform _bulletTransform;


    //public override void Spawned()
    //{
    //    _netRb.Rigidbody.AddForce(transform.right * _initialImpulse, ForceMode.VelocityChange);

    //    _lifeTimer = TickTimer.CreateFromSeconds(Runner, _lifeTime);
    //}

    //public override void FixedUpdateNetwork()
    //{
    //    if (!_lifeTimer.ExpiredOrNotRunning(Runner)) return;

    //    DespawnObject();
    //}


    public override void Spawned()
    {
        _netRb = GetComponent<NetworkRigidbody3D>();

        _lifeTimer = TickTimer.CreateFromSeconds(Runner, _lifeTime);
    }

    public override void FixedUpdateNetwork()
    {
        _bulletTransform.transform.right = _netRb.Rigidbody.velocity.normalized;

        if (!_lifeTimer.ExpiredOrNotRunning(Runner)) return;
        DespawnObject();
    }

    public void SetBullet(Vector3 dir)
    {
        _netRb.Rigidbody.AddForce(dir.normalized * _launchForce, ForceMode.VelocityChange);
    }


    void DespawnObject()
    {
        Runner.Despawn(Object);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (!HasStateAuthority) return;

        if (other.gameObject.CompareTag("Cube"))
        {
            
            var cubeNO = other.gameObject.GetComponent<NetworkObject>();

            if (cubeNO != null)
            {
                if (Object.HasStateAuthority)
                {
                    Runner.Despawn(cubeNO);
                }
            }
        }

        if (other.TryGetComponent(out PlayerLife enemyHealth))
        {
            if (!enemyHealth.HasStateAuthority)
                enemyHealth.RPC_TakeDamage(_damage);
        }

        if (other.gameObject.CompareTag("Cube") || other.gameObject.CompareTag("Walls"))
            DespawnObject();
    }
}
