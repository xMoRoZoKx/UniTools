using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ConnectableMonoBehaviour : MonoBehaviour
{
    public Connections connections = new Connections();
    private void OnDestroy()
    {
        connections.DisconnectAll();
    }
}
