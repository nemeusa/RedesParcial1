using Fusion;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WeaponArm : NetworkBehaviour
{
    [SerializeField] private NetworkPrefabRef _bulletPrefab;
    [SerializeField] private Transform _bulletSpawnPoint;
    [SerializeField] private Color _myColor;

    public void SetColor(Color playerColor)
    {
        _myColor = playerColor;
    }

    public void Shoot()
    {
        if (!HasStateAuthority) return;
        var newBullet = Runner.Spawn(_bulletPrefab, _bulletSpawnPoint.position, Quaternion.identity).GetComponent<Bullet>();

        //var dir = mousePos - _bulletSpawnPoint.position; ----> _bulletSpawnPoint.right

        //newBullet.RPC_SetBulletColor(_myColor);
        newBullet.SetBullet(_bulletSpawnPoint.right);
    }

    [Rpc]
    public void RPC_Rotate(Vector2 point)
    {
        transform.right = (Vector3)point;
    }

}
