using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// This could possibly be done with smart colliders, but this is the safe way
public class PotionLabel : MonoBehaviour
{
    [SerializeField] Potion potion;

    private void OnMouseUp()
    {
        potion.PotionWasClicked();
    }
}
