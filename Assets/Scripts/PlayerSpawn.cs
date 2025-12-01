using Fusion;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerSpawn : SimulationBehaviour, IPlayerJoined
{
    [SerializeField] NetworkPrefabRef _playerPrefab;

    [SerializeField] Transform[] _spawnPoints;

    int _myIndex;

    bool _initialized;

    public void PlayerJoined(PlayerRef player)
    {
        var currentClients = Runner.SessionInfo.PlayerCount;

        if (_initialized && currentClients > 1)
        {
            CreatePlayer(_myIndex);
            _initialized = false;
            return;
        }

        if (player == Runner.LocalPlayer)
        {
            if (currentClients < 2) 
            {
                _myIndex = currentClients - 1;
                _initialized = true;
            }
            else 
            {
               
                if (Runner.SessionInfo.PlayerCount > _spawnPoints.Length) return;

                CreatePlayer(currentClients - 1);
            }
        }
    }

    void CreatePlayer(int spawnIndex)
    {
        var spawnPoint = _spawnPoints[spawnIndex];

        Runner.Spawn(_playerPrefab, spawnPoint.position, spawnPoint.rotation);
    }
}
