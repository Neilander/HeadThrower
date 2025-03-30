using System.Collections;
using System.Collections.Generic;
using UnityEngine;


    public class StepPlat : BaseInteraction
    {
        public bool isOn;
        public Sprite offSprite;
        public Sprite onSprite;


        private SpriteRenderer rend;

        void Awake()
        {
            rend = GetComponent<SpriteRenderer>();
        }

        void OnCollisionEnter2D(Collision2D other)
        {
            //Debug.Log(1);
            if (other.collider.tag != "Player")
                return;

            ChangeSprite();
        }

        void ChangeSprite()
        {
            isOn = !isOn;

            rend.sprite = isOn ? onSprite : offSprite;
        }
    }
