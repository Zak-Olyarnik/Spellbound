using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class GameLoopData {

    public string LetterCollection { get; private set; }
    private Dictionary<string, bool> Words;
    private List<string> bonusWords;
    public int NumberOfWordsToFind;

    public GameLoopData(string letterCollection)
    {
        LetterCollection = letterCollection;
        List<string> possibleWords = DictionaryManager.Instance.GetAllWordsForLetterCollection(LetterCollection);
        Words = new Dictionary<string, bool>();
        foreach (string word in possibleWords)
            Words.Add(word, false);
        bonusWords = new List<string>();
    }

    public void AddFoundWord(string word)
    {
        word = word.ToLower();
        if (Words.ContainsKey(word))
            Words[word] = true;
        else
        {
            if (!bonusWords.Contains(word))
                bonusWords.Add(word);
        }
    }

    public bool IsWordFound(string word)
    {
        return GetAllFoundWords().Contains(word.ToLower());
    }

    public int TotalNumberOfWordsFound()
    {
        return Words.Count(w => w.Value) + bonusWords.Count;
    }

    public int GradeLevelWordsCount()
    {
        return Words.Count + bonusWords.Count;
    }

    public List<string> GetAllFoundWords()
    {
        List<string> result = Words.Where(w => w.Value).Select(w => w.Key).ToList();
        result.AddRange(bonusWords);
        result.Sort();
        return result;
    }

    public List<string> GetMissedGradeLevelWords()
    {
        return Words.Where(w => !w.Value).Select(w => w.Key).ToList();
    }
}
