using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SignControl : MonoBehaviour
{
    public Transform player;
    public Vector3 偏移;
    void Update()
    {
        transform.position = player.transform.position + 偏移;
    }
}
