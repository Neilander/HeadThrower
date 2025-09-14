using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class 相机中心点更新 : MonoBehaviour
{
    public Transform 人物_身体;
    public Transform 人物_头;

    [SerializeField]
    Vector3 Pos_Cam;

    void Awake()
    {
        Pos_Cam = new Vector3(
            (人物_身体.position.x + 人物_头.position.x) / 2f,
            (人物_身体.position.y + 人物_头.position.y) / 2f,
            0
        );

        transform.position = Pos_Cam;
    }

    //检测并计算相机的中心点
    void Update()
    {
        Pos_Cam = new Vector3(
            (人物_身体.position.x + 人物_头.position.x) / 2f,
            (人物_身体.position.y + 人物_头.position.y) / 2f,
            0
        );
        transform.position = Pos_Cam;
    }
}
