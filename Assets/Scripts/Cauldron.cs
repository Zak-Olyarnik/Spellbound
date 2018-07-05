using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Cauldron : MonoBehaviour
{
    [SerializeField] Text label;
    private string word = "";
    private List<Potion> potionsInWord = new List<Potion>();
    [SerializeField] private GameObject cloud;
    private List<Cloud> clouds = new List<Cloud>();
    private GameController gameController;
    [SerializeField] private GameObject particleGameObject;
    [SerializeField] private GameObject potionArcMidpoint;
    private ParticleSystem bubbleParticle;
    private float emissionRate;
    private float increasePerPotion = 2f;
    [SerializeField] SpriteRenderer currentSprite;
    [SerializeField] Sprite[] sprites;

    public static Cauldron Instance { get; private set; }

    void Awake()
    {
        if (Instance == null)
            Instance = this;
        else
        {
            Debug.Log("Duplicate Cauldron instance! Destroy! Destroy!");
            Destroy(this.gameObject);
        }
    }

    private void Start()
    {
        gameController = GameController.Instance;
        bubbleParticle = particleGameObject.GetComponent<ParticleSystem>();
        emissionRate = 1f;
        UpdateBubbleEmission();
        UpdateSprite();
    }

    public void AddPotion(Potion potion)
    {
        potionsInWord.Add(potion);
        AddCloud(potion.Letter);
        word += potion.Letter;

        /* Increase the bubble emmision */
        emissionRate += increasePerPotion;
        UpdateBubbleEmission();
        UpdateSprite();
    }

    private void AddCloud(string letter)
    {
        Cloud newCloud = Instantiate(cloud, transform).GetComponent<Cloud>();
        clouds.Add(newCloud);
        newCloud.Text = letter;
        newCloud.SetState(Cloud.State.RiseFromCauldron, clouds.Count);

        for(int i=0; i<clouds.Count-1; i++)
        {
            clouds[i].SetState(Cloud.State.SlideOver, clouds.Count);
        }
    }

    public void SendCloudsToBook(Vector2 bookPosition)
    {
        foreach (Cloud cloud in clouds)
        {
            cloud.SetState(Cloud.State.MoveToBook, clouds.Count);
            cloud.TargetPosition = bookPosition;
        }
    }

    void OnMouseUp()
    {
        StartCoroutine(gameController.ValidateWord(word, potionsInWord.Count));
    }

    public void Undo()
    {
        // Check that there is a letter to undo
        if (word.Length > 0)
        {
            // Remove last potion from stack
            Potion potionToRemove = potionsInWord[potionsInWord.Count - 1];
            potionsInWord.RemoveAt(potionsInWord.Count - 1);
            word = word.Substring(0, word.Length - potionToRemove.Letter.Length);  // Substring is (startIndex, length)

            // Remove the last cloud
            Cloud cloudToRemove = clouds[clouds.Count - 1];
            clouds.RemoveAt(clouds.Count - 1);
            Destroy(cloudToRemove.gameObject);

            foreach(Cloud cloud in clouds)
            {
                cloud.SetState(Cloud.State.SlideBack, clouds.Count);
            }

            emissionRate -= increasePerPotion;
            UpdateBubbleEmission();
            UpdateSprite();
        }
    }

    public void Clear()
    {
        for (int i = potionsInWord.Count; i > 0; i--)
        {
            Undo();
        }
    }

    public void UpdateBubbleEmission()
    {
        ParticleSystem.EmissionModule emission = bubbleParticle.emission;
        emission.rateOverTime = emissionRate;
    }

    private void UpdateSprite()
    {
        int index = potionsInWord.Count;
        if (index > 5) index = 5;
        currentSprite.sprite = sprites[index];
    }

    public void InstantUpdateSprite()
    {
        currentSprite.sprite = sprites[0];
    }

    public Vector2 GetPotionArcMidpoint()
    {
        return potionArcMidpoint.transform.position;
    }
}
