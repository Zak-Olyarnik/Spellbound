using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class Potion : MonoBehaviour
{
    [SerializeField] private Cauldron cauldron;
    [SerializeField] private SpriteRenderer sprite;
    [SerializeField] private Text label;
    private string letter = "";
    private Vector2 startPosition;
    private Animator animator;
    private int animationNumber;
    private float animationTriggerChance = 0.05f; /* Chance per second to trigger a random potion animation */
    private bool playAnimations = true;

    public string Letter
    { get { return letter; } }

    public void Init(string s)
    {
        letter = s;
        label.text = letter;
        int i = Random.Range(0, Constants.potionSprites.Length);
        sprite.sprite = Constants.potionSprites[i];
        gameObject.transform.parent.gameObject.SetActive(true);
    }

    private void OnMouseUp()
    {
        if (UnityEngine.EventSystems.EventSystem.current.IsPointerOverGameObject())
            return;
        Potion flyingPotion = Instantiate(gameObject,gameObject.transform.parent).GetComponent<Potion>();
        flyingPotion.ToInfinityAndBeyond(letter);
    }

    public void ToInfinityAndBeyond(string s)
    {
        letter = s;
        StartCoroutine(MoveToCauldron());
    }

    //public void Restore()   // "Reset" is a protected keyword
    //{
    //    //if (animationCR != null)
    //    //{
    //    //    StopCoroutine(animationCR);
    //    //    animationCR = null;
    //    //}
    //    transform.position = startPosition;
    //    transform.rotation = Quaternion.identity;
    //    sprite.color = new Color(sprite.color.r, sprite.color.g, sprite.color.b, 1f);
    //}

    void Awake()
    {
        startPosition = transform.position;
        animator = GetComponent<Animator>();
    }

    void Start()
    {
        animationNumber = 0;
        StartCoroutine(RandomPotionAnimations());
    }

    private IEnumerator RandomPotionAnimations()
    {
        while (playAnimations)
        {
            if (animationNumber == 0 && Random.Range(0f, 1f) <= animationTriggerChance)
            {
                animationNumber = Random.Range(1, 6);
                animator.SetInteger("animationNumber", animationNumber);
                yield return new WaitForSeconds(2f);
                animationNumber = 0;
                animator.SetInteger("animationNumber", animationNumber);
            }
            else
                yield return new WaitForSeconds(Random.Range(1f, 5f));
        }
    }

    private IEnumerator MoveToCauldron()
    {
        /* Turn off animations, as this is a throwing potion */
        playAnimations = false;
        animator.Rebind();
        animator.enabled = false;
        /* Trigger a throw sound effect */
        AudioManager.Instance.PlayEffect(AudioManager.SoundEffects.PotionThrow);
        Vector2 destination = cauldron.gameObject.transform.position;
        Quaternion rotation = Quaternion.Euler(new Vector3(0f, 0f, Random.Range(1f, 30f)));
        foreach (Vector2 position in CoolStuff.PositionOverParabola(startPosition, cauldron.GetPotionArcMidpoint(), destination, .5f))
        {
            transform.position = position;
            transform.Rotate(rotation.eulerAngles);
            yield return null;
        }

        /* Trigger a bottle break sound effect */
        AudioManager.Instance.PlayEffect(AudioManager.SoundEffects.PotionBreak);
        /* The potion is now at the cauldron */
        cauldron.AddPotion(this);
        /* Destroy the duplicate potion */
        Destroy(this.gameObject);
    }
}
