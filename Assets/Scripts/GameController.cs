using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using System.Linq;

public class GameController : MonoBehaviour
{
    [SerializeField] private Cauldron cauldron;
    [SerializeField] private Potion[] potions;
    [SerializeField] private Book[] books;
    [SerializeField] private Text timerText;
    [SerializeField] private Text roundStartText;
    [SerializeField] private GameObject resultsScreen;
    [SerializeField] private GameObject blackScreen;
    [SerializeField] private Text foundWords1;
    [SerializeField] private Text foundWords2;
    [SerializeField] private Text missedWords1;
    [SerializeField] private Text missedWords2;
    private List<string> lettersInPlay = new List<string>();
    private int timer;
    [SerializeField] private Sprite winSprite;
    [SerializeField] private Sprite loseSprite;
    [SerializeField] private Sprite nextSprite;
    [SerializeField] private Sprite replaySprite;
    [SerializeField] private Image arrowButton;
    [SerializeField] private GameObject roundStartClouds;
    [SerializeField] private GameObject roundEndClouds;
    public List<GameLoopData> History;

    public static GameController Instance { get; private set; }
    private static int round = 0;
    private int roundWords = 0;
    private int roundMultiplier = 3;

    void Awake()
    {
        if (Instance == null)
            Instance = this;
        else
        {
            Debug.Log("Duplicate GameController instance! Destroy! Destroy!");
            Destroy(this.gameObject);
        }
    }

    // Use this for initialization
    void Start()
    {
        History = new List<GameLoopData>();
        StartCoroutine(StartRound(0));
    }

    IEnumerator StartRound(float fadeTime)
    {
        AudioManager.Instance.PlayEffect(AudioManager.SoundEffects.SmokeBlowing);
        yield return new WaitForSeconds(fadeTime);

        resultsScreen.SetActive(false);

        round++;
        roundWords = (5 + (int)System.Math.Floor(round / 3.0f));    // calculate # of words needed based on round

        /* Choose a random letter selection */
        string letterCollection = DictionaryManager.Instance.RandomLetterSelection(roundWords * roundMultiplier);
        Debug.Log("Chosen letters: " + letterCollection);

        GameLoopData gameData = new GameLoopData(letterCollection);
        gameData.NumberOfWordsToFind = roundWords;

        /* Set the timer's value to: (#ofWordsToFind + #extraWordsFoundLastRound) * Max(5, 10 - roundLevel) */
        int bonusWords = 0;
        if (History.Count > 0)
            bonusWords = Mathf.Max(History[History.Count - 1].TotalNumberOfWordsFound() - History[History.Count - 1].NumberOfWordsToFind, 0);

        timer = (roundWords + bonusWords) * Mathf.Max(5, 11 - round);
        timerText.text = timer.ToString();

        History.Add(gameData);

        ResetAll();

        // display round start message (with smoke particle effect)
        roundStartText.text = "Round " + round + "\nMake " + roundWords + " Words!";
        StartCoroutine(FadeInText(Time.realtimeSinceStartup));

        InvokeRepeating("TimerTick", 8, 1);
    }

    public IEnumerator ValidateWord(string word, int potionCount)
    {
        if (!History[History.Count - 1].IsWordFound(word))   // first check if word has already been created
        {
            bool valid = DictionaryManager.Instance.ValidWord(word);

            if (valid)
            {
                Debug.Log("valid");
                //TODO: animate word moving up into sky
                /* Play the validWord sound effect */
                AudioManager.Instance.PlayEffect(AudioManager.SoundEffects.ValidWord);
                /* Add the found word to the history */
                History[History.Count - 1].AddFoundWord(word);
                StartCoroutine(AddWordToBook(word));
                // add extra time for creating long words and using blends
                if (word.Length >= 4)
                {
                    AddTime(word.Length + word.Length - potionCount);
                }
                cauldron.InstantUpdateSprite();
                yield return new WaitForSeconds(0.75f);
                cauldron.Clear();
            }
            else
            {
                Debug.Log("not valid");
                //TODO: animate cauldron exploding
                /* Play the invalidWord sound effect */
                AudioManager.Instance.PlayEffect(AudioManager.SoundEffects.InvalidWord);
                yield return null;
                cauldron.Clear();
            }
        }
        else
        {
            Debug.Log("already made");
            //TODO: animate cauldron exploding
            /* Play the reusedWord sound effect */
            AudioManager.Instance.PlayEffect(AudioManager.SoundEffects.ReusedWord);
            yield return null;
            cauldron.Clear();
        }
    }



    public void ResetAll()
    {
        cauldron.Clear();   // TODO: depending on how we animate, we may need to create a separate function here
        ResetBooks();
        ResetPotions();
        lettersInPlay.Clear();
    }

    void ResetPotions()
    {
        /* Add all letters and blends */
        int lastSingleLetterIndex = -1;
        foreach (string letters in Constants.letters)
        {
            if (History[History.Count - 1].LetterCollection.ContainsChars(letters))
            {
                lettersInPlay.Add(letters.ToLower());
                if (letters.Length == 1)
                    lastSingleLetterIndex++;
            }
        }
        Debug.Log("Giving potions: " + lettersInPlay.ToDelimitedString());

        // shuffle list of single letters, then shuffle list of blends separately
            // this ensures that we always distribute all letters before blends, if
            // we run out of potion space
        for (int i = 0; i <= lastSingleLetterIndex; i++)
        {
            string temp = lettersInPlay[i];
            int randomIndex = Random.Range(i, lastSingleLetterIndex+1);
            lettersInPlay[i] = lettersInPlay[randomIndex];
            lettersInPlay[randomIndex] = temp;
        }

        for (int i = lastSingleLetterIndex+1; i < lettersInPlay.Count; i++)
        {
            string temp = lettersInPlay[i];
            int randomIndex = Random.Range(i, lettersInPlay.Count);
            lettersInPlay[i] = lettersInPlay[randomIndex];
            lettersInPlay[randomIndex] = temp;
        }

        //// create list of potion indices to choose from
        //List<int> indices = new List<int>();
        //for (int i = 0; i < Mathf.Min(potions.Length, lettersInPlay.Count); i++)
        //{
        //    indices.Add(i);
        //}

        //// assign letters (in order) to potions (randomly)
        //for (int i = 0; i < Mathf.Min(potions.Length, lettersInPlay.Count); i++)
        //{
        //    int chosenIndex = Random.Range(0, indices.Count);
        //    potions[indices[chosenIndex]].Init(lettersInPlay[i]);
        //    indices.RemoveAt(chosenIndex);
        //}

        // turn off excess potions
        //for(int i = 0; i<indices.Count; i++)
        //{
        //    potions[indices[i]].gameObject.transform.parent.gameObject.SetActive(false);
        //}

        // assign letters to potions
        //for (int i = 0; i < Mathf.Min(potions.Length, lettersInPlay.Count); i++)
        //{
        //    potions[i].Init(lettersInPlay[i]);
        //}


        // this version does single letters followed by blends, with the blends always
            // starting on a new shelf
        int firstBlendPotion = ((lastSingleLetterIndex / 4) + 1) * 4;
        int potionIndex = 0, letterIndex = 0;
        while(potionIndex < potions.Length && letterIndex < lettersInPlay.Count)
        {
            potions[potionIndex].Init(lettersInPlay[letterIndex]);

            if (letterIndex == lastSingleLetterIndex)
                potionIndex = firstBlendPotion;
            else
                potionIndex++;
            letterIndex++;

        }


        // turn off excess potions
        //for (int i = lettersInPlay.Count; i < potions.Length; i++)
        //{
        //    potions[i].gameObject.transform.parent.gameObject.SetActive(false);
        //}

        for (int i = lastSingleLetterIndex + 1; i < firstBlendPotion; i++)
        {
            potions[i].gameObject.transform.parent.gameObject.SetActive(false);
        }
        for (int i = potionIndex; i < potions.Length; i++)
        {
            potions[i].gameObject.transform.parent.gameObject.SetActive(false);
        }



    }

    void ResetBooks()
    {
        for(int i=0; i<books.Length; i++)
        {
            books[i].Restore(i < roundWords);
        }
    }

    public IEnumerator AddWordToBook(string word)
    {
        /* Add the word to a free book */
        foreach (Book book in books)
        {
            if (!book.Used)
            {
                cauldron.SendCloudsToBook(book.transform.position);
                yield return new WaitForSeconds(0.75f);
                book.SetText(word);
                break;
            }
        }
    }

    void TimerTick()
    {
        timer--;
        timerText.text = timer.ToString();
        if (timer <= 0)
        {
            CancelInvoke("TimerTick");
            StartCoroutine(EndGame());
        }
    }

    public void AddTime(int time)
    {
        timer += time;
    }

    IEnumerator EndGame()
    {
        AudioManager.Instance.PlayEffect(AudioManager.SoundEffects.SmokeBlowingShort);
        roundEndClouds.SetActive(true);
        yield return new WaitForSeconds(2);
        resultsScreen.SetActive(true);
        if(History[History.Count - 1].TotalNumberOfWordsFound() >= roundWords)
        {
            resultsScreen.GetComponent<SpriteRenderer>().sprite = winSprite;
            arrowButton.sprite = nextSprite;
        }
        else
        {
            resultsScreen.GetComponent<SpriteRenderer>().sprite = loseSprite;
            arrowButton.sprite = replaySprite;
            round = 0;
        }

        // reset text
        foundWords1.text = "";
        foundWords2.text = "";
        missedWords1.text = "";
        missedWords2.text = "";

        // display word lists. if > 32 words are found, the rest will be auto-truncated
        int found = 0, missed = 0;
        foreach (string foundWord in History[History.Count - 1].GetAllFoundWords())
        {
            found++;
            if (found <= 16)
                foundWords1.text = foundWords1.text + "\n" + foundWord;
            else
                foundWords2.text = foundWords2.text + "\n" + foundWord;
        }

        // this hot garbage shuffles the missed words so we don't always get just the A's,
            // then resorts the selected sub-list afterwards
        List<string> missedWords = History[History.Count - 1].GetMissedGradeLevelWords();
        if (missedWords.Count > 32)
        {
            // it's either this or write a new static class to put the extension in
            for (int i = 0; i < missedWords.Count - 1; i++)
            {
                string tmp1 = missedWords[i];
                int j = Random.Range(i, missedWords.Count - 1);
                missedWords[i] = missedWords[j];
                missedWords[j] = tmp1;
            }
            missedWords.RemoveRange(32, missedWords.Count - 32);
            missedWords.Sort();
        }

        foreach (string missedWord in missedWords)
        {
            missed++;
            if (missedWords.Count <= 16 || missed >= 16)
                missedWords1.text = missedWords1.text + "\n" + missedWord;
            else
                missedWords2.text = missedWords2.text + "\n" + missedWord;
        }
    }

    private IEnumerator FadeInText(float startTime)
    {
        float timer = (Time.realtimeSinceStartup - startTime);
        float fracJourney = timer / 30f;
        roundStartText.color = Color.Lerp(roundStartText.color, new Color(0, 0, 0, 1), fracJourney);
        if (timer > 3.5f)
        {
            StopCoroutine(FadeInText(startTime));
            blackScreen.SetActive(false);
            StartCoroutine(FadeOutText(Time.realtimeSinceStartup));
            yield break;
        }
        yield return new WaitForSecondsRealtime(0.05f);
        StartCoroutine(FadeInText(startTime));
    }

    private IEnumerator FadeOutText(float startTime)
    {
        float timer = (Time.realtimeSinceStartup - startTime);
        float fracJourney = timer / 30f;
        roundStartText.color = Color.Lerp(roundStartText.color, new Color(0, 0, 0, 0), fracJourney);
       if (timer > 3.5f)
        {
            StopCoroutine(FadeOutText(startTime));
            roundStartText.color = new Color(0, 0, 0, 0);
            yield break;
        }
        yield return new WaitForSecondsRealtime(0.05f);
        StartCoroutine(FadeOutText(startTime));
    }

    public void PlayAgainClick()
    {
        roundStartClouds.SetActive(true);
        StartCoroutine(StartRound(2));
    }

    public void QuitClick()
    { Application.Quit(); }

    public void UndoClick()
    {
        AudioManager.Instance.PlayEffect(AudioManager.SoundEffects.UndoButton);
        cauldron.Undo();
    }

    public void MenuClick()
    {
        StartCoroutine(SwitchToMainMenu());
    }

    private IEnumerator SwitchToMainMenu()
    {
        AudioManager.Instance.PlayEffect(AudioManager.SoundEffects.MenuButton);
        roundEndClouds.SetActive(true);
        yield return new WaitForSeconds(2f);

        /* Clear out round and history information */
        round = 0;
        History.Clear();
        SceneManager.LoadScene("Menu");
    }
}
