using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HitPlat : MonoBehaviour
{
    public bool isOn;
    public Sprite offSprite;
    public Sprite onSprite;


    private SpriteRenderer rend;

    void Awake()
    {
        rend = GetComponent<SpriteRenderer>();
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        //Debug.Log(1);
        if (other.tag != "Bullet")
            return;

        ChangeSprite();
    }

    void ChangeSprite()
    {
        isOn = !isOn;

        rend.sprite = isOn ? onSprite : offSprite;
    }
}