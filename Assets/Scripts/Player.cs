using Fusion;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : NetworkBehaviour
{
    PlayerMovement _movement;
    [SerializeField] WeaponArm _weapon;

    [SerializeField] List<GameObject> _materials;

    Vector3 initialPost;

    private bool _doublejumped;

    Vector2 _mouseDir;

    private Vector3 _weaponDir;

    private Camera _myCam;
    [SerializeField] private LayerMask _mouseMask;



    private void Awake()
    {
        _movement = GetComponent<PlayerMovement>();
        _weapon = GetComponentInChildren<WeaponArm>();
        _weaponDir = _weapon.transform.position;
        _myCam = Camera.main;
    }

    public override void Spawned()
    {
        initialPost = transform.position;

        GameManager.Instance.AddPlayer(this);



    }

    public override void Render()
    {

        if (Input.GetKeyDown(KeyCode.Space))
        {
            if (_movement.Grounded == true)
            {
                _movement.Jump(false, 15);
            }
            else if (_doublejumped == false)
            {
                _movement.Jump(true, 15);
                _doublejumped = true;
            }

        }



        if (Input.GetKeyDown(KeyCode.Mouse0))
        {
            _weapon.Shoot();
        }
    }

    private void Update()
    {
        SetMouseDir();
        SetWeaponDir();
    }

    public override void FixedUpdateNetwork()
    {
        _movement.Move(Vector3.right * Input.GetAxis("Horizontal"));

        _weapon.RPC_Rotate(_weaponDir);

        //if (inputData.buttons.IsSet(PlayerButtons.Shot))
        //if (GameManager.Instance.myPlayers.Count < 2) return;


        if (_movement.Grounded == true)
        {
            _doublejumped = false;
        }
    }


    private void SetMouseDir()
    {
        if (!_myCam) return;

        if (!Physics.Raycast(_myCam.ScreenPointToRay(Input.mousePosition), out RaycastHit hitInfo
                , Mathf.Infinity, _mouseMask)) return;

        var mousePos = hitInfo.point;

        mousePos.z = 0;

        _mouseDir = mousePos - _weapon.transform.position;
    }

    private void SetWeaponDir()
    {
        if (!Physics.Raycast(_myCam.ScreenPointToRay(Input.mousePosition), out RaycastHit hitInfo
                , Mathf.Infinity, _mouseMask)) return;

        _mouseDir = hitInfo.point;

        //_mouseDir.z = 0;

        _weaponDir = _mouseDir - new Vector2(_weapon.transform.position.x, _weapon.transform.position.y);
    }

    public void Death()
    {
        GameManager.Instance.RPC_GameOver(this);
        //GameManager.Instance.PlayerDead(this);
        Runner.Despawn(Object);
    }


    [Rpc]
    public void RPC_SetTeam(Color color)
    {
        foreach (var mat in _materials)
            mat.GetComponent<Renderer>().material.color = color;
        _weapon.SetColor(color);
    }

}
