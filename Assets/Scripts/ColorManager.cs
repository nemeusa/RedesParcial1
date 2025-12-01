using Fusion;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class ColorManager : NetworkBehaviour
{
    public static ColorManager Instance { get; private set; }

    [SerializeField] private TextMeshProUGUI _teamOneText;
    [SerializeField] private TextMeshProUGUI _teamTwoText;
    //[Networked, OnChangedRender(nameof(UpdateText))] TextMeshProUGUI _teamOneText { get; set; }
    //[Networked, OnChangedRender(nameof(UpdateText))] TextMeshProUGUI _teamTwoText { get; set; }


    [Networked, OnChangedRender(nameof(UpdateText))] int TeamOneFalls { get; set; }
    [Networked, OnChangedRender(nameof(UpdateText))] int TeamTwoFalls { get; set; }

    private Color _teamOneColor;
    private Color _teamTwoColor;

    private void Awake()
    {
        Instance = this;
    }

    private void Start()
    {
        //UpdateText(_teamOneText, TeamOneFalls);
        //UpdateText(_teamTwoText, TeamTwoFalls);


        _teamOneColor = GameManager.Instance.TeamOneColor;
        _teamTwoColor = GameManager.Instance.TeamTwoColor;

        GameManager.Instance.OnStartGame += Restart;
    }

    public override void Spawned()
    {
        UpdateText();
    }

    public void AddBlock(PlayerLife player)
    {
        if (player.HasInputAuthority)
        {
            TeamOneFalls++;
        }
        else
        {
            TeamTwoFalls++;
        }

        //UpdateText(_teamOneText, TeamOneFalls);
        //UpdateText(_teamTwoText, TeamTwoFalls);
        UpdateText();
    }

    public void UpdateText()
    {
        _teamOneText.text = $"{TeamOneFalls}";
        _teamTwoText.text = $"{TeamTwoFalls}";
        Debug.Log("actualiza contador");
    }

    public Color GetWinner()
    {
        return TeamOneFalls < TeamTwoFalls ? _teamOneColor : _teamTwoColor;
    }

    private void Restart()
    {
        TeamOneFalls = 0;
        TeamTwoFalls = 0;

        UpdateText();
        //UpdateText(_teamOneText, TeamOneFalls);
        //UpdateText(_teamTwoText, TeamTwoFalls);
    }
}
