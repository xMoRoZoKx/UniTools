using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ConnectableMonoBehaviour : MonoBehaviour
{
    [HideInInspector] public Connections connections = new Connections();

    protected virtual void OnDestroy()
    {
        connections.DisconnectAll();
    }
}
