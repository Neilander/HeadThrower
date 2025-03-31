using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SignControl : MonoBehaviour
{
    public Transform player;
    void Update()
    {
        transform.position = player.transform.position + new Vector3(0, 1.02f, 0);
    }
}
