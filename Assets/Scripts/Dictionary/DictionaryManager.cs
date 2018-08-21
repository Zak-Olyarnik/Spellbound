using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using System;
using System.IO;
using System.Xml.Serialization;

public class DictionaryManager : MonoBehaviour {

    private string[] allWords; /* Holds the entire English language */
    private string[] gradeLevelWords; /* Holds the grade-level words */
    private string[] letterCollections; /* Holds the list of letter collections generated from grade-level words */
    private int[] collectionWordCounts; /* Holds the number of words each collection can make */
    private const string FULL_WORD_LIST_PATH = "Words/full_list_curated";
    private const string GRADE_LEVEL_WORD_LIST_PATH = "Words/gradeLevelWords";
    private const string LETTER_COLLECTION_LIST_PATH = "Words/collectionsWithCount";

    public bool didLoadSuccessfully { get; private set; }

    public static DictionaryManager Instance { get; private set; }

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(this.gameObject);
            LoadAllFiles();
        }
        else
        {
            Debug.Log("Duplicate instance of WordManager! Destroying " + this.name);
            Destroy(this.gameObject);
        }
    }

    private void LoadAllFiles()
    {
        string[] separator = new string[] { "\r\n" };
        /* Load the full list of words */
        TextAsset fullList = Resources.Load<TextAsset>(FULL_WORD_LIST_PATH);
        if (fullList == null)
        {
            Debug.LogError("Failed to find " + FULL_WORD_LIST_PATH + " in Resource folder!");
            didLoadSuccessfully = false;
        }
        else
        {
            allWords = fullList.text.Split(separator, StringSplitOptions.None);
            Debug.Log("Loaded " + allWords.Length + " words into master word list");
        }

        /* Load the grade level words */
        TextAsset gradeLevelList = Resources.Load<TextAsset>(GRADE_LEVEL_WORD_LIST_PATH);
        if (gradeLevelList == null)
        {
            Debug.LogError("Failed to find " + GRADE_LEVEL_WORD_LIST_PATH + " in Resource folder!");
            didLoadSuccessfully = false;
        }
        else
        {
            gradeLevelWords = gradeLevelList.text.Split(separator, StringSplitOptions.None);
            Debug.Log("Loaded " + gradeLevelWords.Length + " words into grade level word list");
        }

        /* Load the letter collections */
        TextAsset letterCollectionList = Resources.Load<TextAsset>(LETTER_COLLECTION_LIST_PATH);
        if (letterCollectionList == null)
        {
            Debug.LogError("Failed to find " + LETTER_COLLECTION_LIST_PATH + " in Resource folder!");
            didLoadSuccessfully = false;
        }
        else
        {
            string[] combinations = letterCollectionList.text.Split(separator, StringSplitOptions.None);
            letterCollections = new string[combinations.Length];
            collectionWordCounts = new int[combinations.Length];
            for (int i = 0; i < combinations.Length; i++)
            {
                string[] row = combinations[i].Split(',');
                letterCollections[i] = row[0];
                collectionWordCounts[i] = int.Parse(row[1].Trim());
            }

                Debug.Log("Loaded " + letterCollections.Length + " letter collections");
        }
    }

    public bool ValidWord(string word)
    {
        return allWords.Where(w => w.ToLower() == word.ToLower()).Count() > 0;
    }

    public List<string> GetAllWordsForLetterCollection(string collection)
    {
        List<string> words = new List<string>();
        foreach(string word in gradeLevelWords)
        {
            if (collection.ContainsChars(word))
                words.Add(word);
        }
        return words;
    }

    public string RandomLetterSelection()
    {
        return letterCollections[UnityEngine.Random.Range(0, letterCollections.Length)];
    }

    public string RandomLetterSelection(int lowerBound, int upperBound)
    {
        List<int> indicesToChooseFrom = new List<int>();
        for (int i = 0; i < letterCollections.Length; i++)
        {
            if (collectionWordCounts[i] >= lowerBound && collectionWordCounts[i] <= upperBound)
                indicesToChooseFrom.Add(i);
        }
        if (indicesToChooseFrom.Count == 0)
        {
            Debug.LogWarning("Could not find a letter collection with a number of words within the given bounds (" + lowerBound + "," + upperBound + "). Choosing a random letter collection.");
            return RandomLetterSelection();
        }

        return letterCollections[indicesToChooseFrom[UnityEngine.Random.Range(0, indicesToChooseFrom.Count)]];
    }

    public string RandomLetterSelection(int lowerBound)
    {
        List<int> indicesToChooseFrom = new List<int>();
        for (int i = 0; i < letterCollections.Length; i++)
        {
            if (collectionWordCounts[i] >= lowerBound)
                indicesToChooseFrom.Add(i);
        }
        if (indicesToChooseFrom.Count == 0)
        {
            Debug.LogWarning("Could not find a letter collection with a number of words above the given bound (" + lowerBound + "). Choosing a random letter collection.");
            return RandomLetterSelection();
        }

        return letterCollections[indicesToChooseFrom[UnityEngine.Random.Range(0, indicesToChooseFrom.Count)]];
    }
}
