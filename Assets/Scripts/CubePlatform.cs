using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Fusion;

public class CubePlatform : NetworkBehaviour
{
    public void dead()
    {
        Runner.Despawn(Object);
    }
}
