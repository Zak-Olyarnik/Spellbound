using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Cloud : MonoBehaviour
{
    // cloud behaves as a state machine
    public enum State { RiseFromCauldron, Bob, SlideOver, SlideBack, MoveToBook };

    [SerializeField] private float width;
    [SerializeField] private float speed;
    [SerializeField] private float amplitude;
    [SerializeField] private Text text;
    [SerializeField] private SpriteRenderer sprite;
    private Vector2 targetPosition;
    private Vector2 cauldronPosition;
    private State state;
    private float randomBobStart = 0;

    public string Text
    { set { text.text = value; } }

    public Vector2 TargetPosition
    { set { targetPosition = value; } }

    void Awake()
    {
        cauldronPosition = transform.parent.transform.position;
        int i = Random.Range(0, Constants.cloudSprites.Length);
        sprite.sprite = Constants.cloudSprites[i];
        randomBobStart = Random.Range(0, 10.0f);
    }

    public void SetState(State newState, int totalClouds)
    {
        switch (newState)
        {
            case (State.RiseFromCauldron):
                targetPosition = new Vector2((0.5f * width) * (totalClouds - 1), 2.5f) + cauldronPosition;
                break;
            case (State.Bob):
                // targetPosition recalculated each frame
                break;
            case (State.SlideOver):
                targetPosition = new Vector2(transform.position.x - (0.5f * width), transform.position.y);
                break;
            case (State.SlideBack):
                targetPosition = new Vector2(transform.position.x + (0.5f * width), transform.position.y);
                break;
            case (State.MoveToBook):
                // targetPosition set manually in this case
                break;
            default:
                break;
        }
        state = newState;
    }


    void Update()
    {
        switch(state)
        {
            case (State.RiseFromCauldron):
                transform.position = Vector3.MoveTowards(transform.position, targetPosition, Time.deltaTime * speed);
                break;
            case (State.Bob):
                float newY = 0.25f * Mathf.Sin(Time.time * 0.5f + randomBobStart);
                transform.position = new Vector3(transform.position.x, newY);// + transform.position.y);
                break;
            case (State.SlideOver):
                transform.position = Vector3.MoveTowards(transform.position, targetPosition, Time.deltaTime * speed);
                break;
            case (State.SlideBack):
                transform.position = Vector3.MoveTowards(transform.position, targetPosition, Time.deltaTime * speed);
                break;
            case (State.MoveToBook):
                transform.position = Vector3.MoveTowards(transform.position, targetPosition, Time.deltaTime * speed/2.0f);
                transform.localScale = transform.localScale * 0.98f;
                break;
        }
        if (state != State.MoveToBook && IsWithin(transform.position, targetPosition))
            SetState(State.Bob, 0);
    }


    private bool IsWithin(Vector3 obj1, Vector3 obj2)
    {
        float tolerance = 0.05f;
        if ((Mathf.Abs(obj1.x - obj2.x) < tolerance) && (Mathf.Abs(obj1.y - obj2.y) < tolerance))
            return true;
        else
            return false;
    } 
}
