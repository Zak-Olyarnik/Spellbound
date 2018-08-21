using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Constants : MonoBehaviour
{
    [SerializeField] private Sprite[] serializedPotionSprites;
    public static Sprite[] potionSprites;   // your move, Unity
    [SerializeField] private Sprite[] serializedBookSprites;
    public static Sprite[] bookSprites;
    [SerializeField] private Sprite[] serializedCloudSprites;
    public static Sprite[] cloudSprites;
    public static string[] letters = new string[] {
        "A", "E", "I", "O", "U",         //vowels, ind 0-4 
        "B", "C", "D", "F", "G", "H", "K", "L", "M", "N", "P", "Q", "R", "S", "T", "V", "W", "X", "Y", "Z",    // consonants, ind 5-24
        "BL", "BR", "CH", "CK", "CL", "CR", "DR", "FL", "FR", "GH", "GL", "GR", "NG", "PH", "PL", "PR", "QU", "SC", "SH", "SK", "SL", "SM", "SN", "SP", "ST", "SW", "TH", "TR", "TW", "WH", "WR",   // digraphs, ind 25-55
        "NTH", "SCH", "SCR", "SHR", "SPL", "SPR", "SQU", "STR", "THR",  // trigraphs, ind 56-64
        "AI", "AU", "AW", "AY", "EA", "EE", "EI", "EU", "EW", "EY", "IE", "OI", "OO","OU", "OW", "OY" }; // vowel digraphs, ind 65-80
    //TODO: "ER", "AR", "OR", etc., "ING", "ED"...
    public static Color[] potionColors = new Color[] { Color.blue, Color.green, Color.red, Color.yellow };


    private void Awake()
    {
        potionSprites = serializedPotionSprites;
        bookSprites = serializedBookSprites;
        cloudSprites = serializedCloudSprites;
    }
}
