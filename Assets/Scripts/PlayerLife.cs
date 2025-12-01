using Fusion;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerLife : NetworkBehaviour
{

    [SerializeField] byte _maxLife;
    [Networked, OnChangedRender(nameof(LifeChanged))] int CurrentLife { get; set; }

    [SerializeField] int _currentSpawns = 3;
    [SerializeField] float _respawnTime = 2.5f;

    TickTimer _respawnTimer;

    NetworkBool IsDead { get; set; }

    public event Action<bool> OnDeadStateUpdate;

    Player player;

    [SerializeField] GameObject loseScreen;

    private void Awake()
    {
        player = GetComponent<Player>();
    }

    public override void Spawned()
    {
        CurrentLife = _maxLife;
    }


    void LifeChanged()
    {
        //Actualizar barras de vida
    }

    [Rpc(RpcSources.All, RpcTargets.StateAuthority)]
    public void RPC_TakeDamage(int damage)
    {
        if (IsDead) return;

        if (CurrentLife < damage)
        {
            damage = CurrentLife;
        }

        CurrentLife -= damage;

        if (CurrentLife != 0) return;

        _currentSpawns--;

        ColorManager.Instance.AddBlock(this);

        if (_currentSpawns == 0)
        { 
            player.Death();
            //DisconnectObject();
            return;
        }

        _respawnTimer = TickTimer.CreateFromSeconds(Runner, _respawnTime);

        Debug.Log("muerto xd");
        IsDead = true;
    }

    public override void FixedUpdateNetwork()
    {
        if (_respawnTimer.Expired(Runner))
        {
            Debug.Log("procesando");
            _respawnTimer = TickTimer.None;
            CurrentLife = _maxLife;
            IsDead = false;
            OnDeadStateUpdate?.Invoke(IsDead);
            Debug.Log("se reseteo");
        }
    }

    void DisconnectObject()
    {
        if (!HasInputAuthority)
        {
            Runner.Disconnect(Object.InputAuthority);
        }

        Runner.Despawn(Object);
    }
}
