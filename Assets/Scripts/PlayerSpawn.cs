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
            if (currentClients < 2) //Si somos el primer nos guardamos el indice de la posicion en la que va a spawnear.
            {
                _myIndex = currentClients - 1;
                _initialized = true;
            }
            else //Si ya somos el segundo cliente entrante
            {
                //Si hay mas jugadores que puntos de spawn, retorno
                if (Runner.SessionInfo.PlayerCount > _spawnPoints.Length) return;

                CreatePlayer(currentClients - 1);
            }

            #region CameraTarget (hecho en el player)
            //var newPlayer = Runner.Spawn(_playerPrefab, new Vector3(Random.Range(-2f, 2f), 1, 0), Quaternion.identity);

            //var cam = Camera.main;

            //if (cam.TryGetComponent(out CameraFollow component))
            //{
            //    component.SetTarget(newPlayer.transform);
            //}
            #endregion
        }
    }

    void CreatePlayer(int spawnIndex)
    {
        //Obtengo el punto de spawn
        var spawnPoint = _spawnPoints[spawnIndex];

        //Instantiate NO
        Runner.Spawn(_playerPrefab, spawnPoint.position, spawnPoint.rotation);
    }
}
