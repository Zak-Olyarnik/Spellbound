using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System.IO;
using System;
using System.Linq;

public class ToolMenuOptions {

    private const string WORD_LIST_PATH = "/Resources/Words/DictionarySource";
    private const string GRADE_LEVEL_LIST_PATH = "/Resources/Words/gradeLevelWords.txt";
    private const string COLLECTION_LIST_PATH = "/Resources/Words/letterCollections.txt";
    private const string COLLECTION_WITH_COUNT_PATH = "/Resources/Words/collectionsWithCount.csv";
    private static char[] VALID_LETTERS =
    {
        'a', 'b', 'c', 'd', 'e', 'f', 'g', 'h', 'i', 'j', 'k', 'l', 'm', 'n', 'o', 'p', 'q', 'r', 's', 't', 'u', 'v', 'w', 'x', 'y', 'z'
    };

	[MenuItem("Tools/Generate Letter Collection Database", priority = 1)]
    private static void CreateLetterCollectionDatabase()
    {
        Debug.Log("<color=blue>Generating Letter Collection Database...</color>");
        /* Load the full list of words */
        List<string> wordList = LoadAllWords();
        Debug.Log(wordList.Count + " words loaded");

        /* Save this list of words as the grade level list */
        IOHelper<string>.ToTextFile(wordList.ToDelimitedString(Environment.NewLine), Application.dataPath + GRADE_LEVEL_LIST_PATH);

        /* Sort the letters of each word */
        List<string> sortedList = new List<string>();
        foreach (string word in wordList)
        {
            string sortedWord = new string(word.Distinct().OrderBy(c => c).ToArray());
            sortedList.Add(sortedWord);
        }
        Debug.Log("Sorted words into collections of letters...");

        /* Filter down to distinct collections of letters */
        List<string> filteredList = sortedList.Distinct().ToList();
        Debug.Log("Filtered letter collections down to: " + filteredList.Count);

        /* Find all collections of letters that are not a subset of another collection */
        List<string> uniqueCollections = new List<string>();
        foreach (string letters in filteredList)
        {
            bool unique = true;
            foreach (string wordToCheck in filteredList)
            {
                if (letters.Length >= wordToCheck.Length)
                    continue;
                if (wordToCheck.ContainsChars(letters))
                {
                    unique = false;
                    break;
                }
            }

            if (unique)
                uniqueCollections.Add(letters);
        }
        Debug.Log(uniqueCollections.Count + " unique collections found.");

        /* Sort the list of unique collections */
        uniqueCollections.Sort();

        /* Count the number of words each collection can make */
        List<string> collectionsWithCounts = new List<string>();
        foreach (string collection in uniqueCollections)
        {
            int count = 0;
            foreach (string letters in filteredList)
            {
                if (collection.ContainsChars(letters))
                    count++;
            }
            collectionsWithCounts.Add(collection + ", " + count);
        }
        /* Save the list of collections with their counts to a separate csv file */
        IOHelper<string>.ToTextFile(collectionsWithCounts.ToDelimitedString(Environment.NewLine), Application.dataPath + COLLECTION_WITH_COUNT_PATH);
        Debug.Log("Collections with count saved to: " + Application.dataPath + COLLECTION_WITH_COUNT_PATH);

        /* Save this list of unique collections to a text file */
        IOHelper<string>.ToTextFile(uniqueCollections.ToDelimitedString(Environment.NewLine), Application.dataPath + COLLECTION_LIST_PATH);
        Debug.Log("Collections saved to: " + Application.dataPath + COLLECTION_LIST_PATH);

        
    }

    private static List<string> LoadAllWords()
    {
        List<string> wordList = new List<string>();
        foreach (string path in Directory.GetFiles(Application.dataPath + WORD_LIST_PATH))
        {
            wordList.AddRange(IOHelper<string>.LoadTextFile(path, Environment.NewLine));
        }
        Debug.Log(wordList.Count + " words loaded");

        /* Remove any words that have illegal characters */
        int numRemoved = 0;
        for (int i = wordList.Count - 1; i >= 0; i--)
        {
            foreach (char letter in wordList[i])
            {
                if (!VALID_LETTERS.Contains(letter))
                {
                    numRemoved++;
                    wordList.RemoveAt(i);
                    break;
                }
            }
        }
        int count = wordList.Count;
        Debug.Log("Removed " + numRemoved + " words containing illegal characters (" + count + " words left)");

        /* Sort the list of words alphabetically */
        wordList.Sort();

        /* Remove duplicates */
        wordList = wordList.Distinct().ToList();
        Debug.Log((count - wordList.Count) + " duplicate words removed (" + wordList.Count + " words left)");

        return wordList;
    }

    [MenuItem("Tools/Analyse Letter Collection Database", priority = 1)]
    private static void PrintLetterCollectionStats()
    {
        /* Load the list of letter collections and their word counts */
        string[] collectionsWithCounts = IOHelper<string>.LoadTextFile(Application.dataPath + COLLECTION_WITH_COUNT_PATH, Environment.NewLine);
        string[] collections = new string[collectionsWithCounts.Length];
        int[] counts = new int[collectionsWithCounts.Length];
        for (int i = 0; i < collectionsWithCounts.Length; i++)
        {
            string[] components = collectionsWithCounts[i].Split(',');
            collections[i] = components[0];
            counts[i] = int.Parse(components[1].Trim());
        }

        /* Print out stats about collections sizes */
        int minCollection = collections.Min(c => c.Length);
        int maxCollection = collections.Max(c => c.Length);
        double averageCollection = collections.Average(c => c.Length);

        Debug.Log("Minimum collection size: " + minCollection + " (" + collections.Where(c => c.Length == minCollection).ToDelimitedString() + ")");
        Debug.Log("Maximum collection size: " + maxCollection + " (" + collections.Where(c => c.Length == maxCollection).ToDelimitedString() + ")");
        Debug.Log("Average collection size: " + averageCollection);

        /* Print out stats about number of words collections can make */
        int minWords = counts.Min();
        int maxWords = counts.Max();
        Debug.Log("Minimum number of words in a collection: " + minWords + " (" + collections.Where( (c, index) => counts[index] == minWords).ToDelimitedString() + ")");
        Debug.Log("Maximum number of words in a collection: " + maxWords + " (" + collections.Where((c, index) => counts[index] == maxWords).ToDelimitedString() + ")");
        Debug.Log("Average number of words in a collection: " + counts.Average());

        /* Calculate the potion stats */
        int minPotionIndex = 0, maxPotionIndex = 0;
        int minPotionCount = 1000, maxPotionCount = 0, sumPotions = 0;
        for (int i = 0; i < collections.Length; i++)
        {
            int count = 0;
            foreach (string phoneme in Constants.letters)
            {
                if (collections[i].ContainsChars(phoneme))
                    count++;
            }

            if (count < minPotionCount)
            {
                minPotionCount = count;
                minPotionIndex = i;
            }
            if (count > maxPotionCount)
            {
                maxPotionCount = count;
                maxPotionIndex = i;
            }
            sumPotions += count;
        }

        Debug.Log("Minimum number of potions given: " + minPotionCount + " (" + collections[minPotionIndex] + ")");
        Debug.Log("Maximum number of potions given: " + maxPotionCount + " (" + collections[maxPotionIndex] + ")");
        Debug.Log("Average number of potions given: " + (sumPotions / collections.Length));
    }

}
