using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Book : MonoBehaviour
{
    [SerializeField] private SpriteRenderer sprite;
    [SerializeField] private Text text;
    private ParticleSystem sparkle;
    private bool isUsed = false;

    public bool Used
    { get { return isUsed; } }

    void Start()
    {
        sparkle = GetComponent<ParticleSystem>();
    }

    public void SetText(string s)
    {
        text.text = s;
        isUsed = true;
    }

    public void Restore(bool neededToAdvance)
    {
        isUsed = false;
        text.text = "";
        if(neededToAdvance)
            sparkle.Play();
        else
            sparkle.Stop();

        int i = Random.Range(0, Constants.bookSprites.Length);
        sprite.sprite = Constants.bookSprites[i];
    }
}
