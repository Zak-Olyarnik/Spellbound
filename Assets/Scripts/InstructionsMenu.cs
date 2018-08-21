using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class InstructionsMenu : MonoBehaviour
{
    [SerializeField] SpriteRenderer image;
    [SerializeField] Sprite[] sprites;
    [SerializeField] Image leftImage;
    [SerializeField] Image rightImage;
    [SerializeField] Sprite[] directionalSprites;
    [SerializeField] GameObject menu;
    
    private int index;

    public void NextImage()
    {
        index++;
        if (index == sprites.Length - 1)
        {
            rightImage.sprite = directionalSprites[3];
        }
        else if (index >= sprites.Length)
        {
            menu.SetActive(true);
            gameObject.SetActive(false);
            return;
        }
        image.sprite = sprites[index];
        leftImage.sprite = directionalSprites[1];
    }

    public void PrevImage()
    {
        index--;
        if (index == 0)
        {
            leftImage.sprite = directionalSprites[0];
        }
        else if (index < 0)
        {
            menu.SetActive(true);
            gameObject.SetActive(false);
            return;
        }
        image.sprite = sprites[index];
        rightImage.sprite = directionalSprites[2];
    }

    void OnEnable()
    {
        index = 0;
        image.sprite = sprites[index];
    }
}