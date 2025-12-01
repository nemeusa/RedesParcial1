using Fusion;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class GameManager : NetworkBehaviour
{

    public static GameManager Instance { get; private set; }

    [field: SerializeField] public Color TeamOneColor { get; private set; }
    [field: SerializeField] public Color TeamTwoColor { get; private set; }


    private Dictionary<PlayerRef, Player> _clients;
    public List<Player> myPlayers;

    private Dictionary<Player, Color> _teamDictionary;

    [SerializeField] private GameObject _winImage;
    [SerializeField] private GameObject _loseImage;
    //[SerializeField] private TimeManager _timeManager;

    bool _hasInitialized;

    public event Action OnStartGame;



    private void Awake()
    {
        Instance = this;
        _clients = new Dictionary<PlayerRef, Player>();
        myPlayers = new List<Player>();
        _teamDictionary = new Dictionary<Player, Color>();
    }

    public override void Spawned()
    {
        _hasInitialized = true;

        //OnStartGame += ResetPlayers;
        OnStartGame += DisableGameOver;
    }

    public void AddPlayer(Player player)
    {
        var newPlayer = player.Object.InputAuthority;

        if (_clients.ContainsKey(newPlayer)) return;

        _clients.Add(newPlayer, player);
        myPlayers.Add(player);

        if (!_hasInitialized) return;

        SetPlayerTeam(player);

        //if (_clients.Count >= 2)
        //{
        //    OnStartGame += _timeManager.RPC_StartCountdown;

        //    OnStartGame?.Invoke();
        //}
    }

    private void SetUpPlayers()
    {
        foreach (var player in myPlayers)
        {
            SetPlayerTeam(player);
        }

    }

    private void SetIdColor(Player player)
    {

        if (HasStateAuthority)
        {
            _teamDictionary.TryAdd(player, player.HasInputAuthority ? TeamOneColor : TeamTwoColor);
        }
        else
        {
            _teamDictionary.TryAdd(player, player.HasInputAuthority ? TeamTwoColor : TeamOneColor);
        }
    }

    private void SetPlayerTeam(Player player)
    {
        SetIdColor(player);

        var colorRef = _teamDictionary[player];

        player.RPC_SetTeam(colorRef);
        //StartCoroutine(CallSetID(player));
    }

    //[Rpc(RpcSources.All, RpcTargets.StateAuthority)]
    //public void RPC_GameOver()
    //{
    //    var winnerColor = ColorManager.Instance.GetWinner();

    //    var winner = _teamDictionary.Keys.First();

    //    foreach (var pair in _teamDictionary)
    //    {
    //        if (pair.Value == winnerColor)
    //        {
    //            winner = pair.Key;
    //        }
    //    }

    //    RPC_ShowGameOverImage(winner.Object);
    //}

    [Rpc]
    private void RPC_ShowGameOverImage(NetworkObject winner)
    {
        if (winner.InputAuthority == Runner.LocalPlayer)
        {
            _winImage.SetActive(true);
        }
        else
        {
            _loseImage.SetActive(true);
        }
    }


    public void PlayerDead(Player player)
    {
        if (myPlayers.Contains(player))
        {
            _loseImage.SetActive(true);
            myPlayers.Remove(player);
        }

        if (myPlayers.Count <= 1)
        {
            RPC_ShowVictory(myPlayers[0].Object.StateAuthority);
        }
    }

    [Rpc]
    void RPC_ShowVictory([RpcTarget] PlayerRef client)
    {
        _winImage.SetActive(true);
    }

    private void DisableGameOver()
    {
        _loseImage.SetActive(false);
        _winImage.SetActive(false);
    }


    [Rpc]
    public void RPC_GameOver(Player player)
    {
        var winnerColor = ColorManager.Instance.GetWinner();

        var winner = _teamDictionary.Keys.First();

        foreach (var pair in _teamDictionary)
        {
            if (pair.Value == winnerColor)
            {
                winner = pair.Key;
            }
        }

        if (myPlayers.Count <= 1)
        {
            _winImage.SetActive(true);
        }
        if (myPlayers.Contains(player))
        {
            _loseImage.SetActive(true);
        }
    }
}
