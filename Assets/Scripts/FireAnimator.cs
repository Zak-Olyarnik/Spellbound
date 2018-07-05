using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class FireAnimator : MonoBehaviour
{
    [SerializeField] SpriteRenderer image;
    [SerializeField] Sprite[] fireSprites;
    [SerializeField] private float changeTime;
    private int index;

    void UpdateImage()
    {
        index = Random.Range(0, fireSprites.Length);
        image.sprite = fireSprites[index];
    }

    void Start()
    {
        InvokeRepeating("UpdateImage", changeTime, changeTime);
    }
}
