using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EggFriend : MonoBehaviour
{
    [SerializeField] private float eatDistance;
    [SerializeField] private float walkDistance;
    
    private Transform _cameraTransform;
    public List<Color> randomColours;
    private Animator _animator;

    public bool foodAvailable;
    public Transform foodLocation;
    
    [SerializeField] private SpriteRenderer body;
    [SerializeField] private SpriteRenderer face;
    [SerializeField] private Sprite smile, eat1, eat2;

    private int currentHunger = 935;
    [SerializeField] private RectTransform textMask, barMask;

    [SerializeField] private bool hunger = true;
    
    // Start is called before the first frame update
    void Start()
    {
        _cameraTransform = Camera.main.transform;
        body.color = randomColours[(int) Random.Range(0, randomColours.Count)];
        _animator = body.GetComponent<Animator>();
        //StartCoroutine(GetHungry());
    }

    public void SetUI(RectTransform text, RectTransform bar)
    {
        textMask = text;
        barMask = bar;
        StartCoroutine(GetHungry());
    }

    private IEnumerator GetHungry()
    {
        while (hunger)
        {
            currentHunger-=5;
            textMask.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, currentHunger);
            barMask.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, currentHunger);
            yield return new WaitForSeconds(1.0f);
        }
    }

    public void HungerEnabled(bool set)
    {
        hunger = set;
        if (hunger)
        {
            StartCoroutine(GetHungry());
        }
    }
    
    // Update is called once per frame
    void Update()
    {
        Transform target;
        if (foodAvailable)
            target = foodLocation;
        else
            target = _cameraTransform;
        
        transform.LookAt(target);
        
        var xRotation = transform.eulerAngles.x;
        if (xRotation > 180) xRotation = xRotation - 360;
        xRotation = Mathf.Clamp(xRotation, -25, 25);
        if (xRotation < 0) xRotation = 360 + xRotation;
        transform.eulerAngles = new Vector3(xRotation, transform.eulerAngles.y, 0);

        if (foodAvailable)
        {
            if (Vector3.Distance(transform.position, foodLocation.position) <= walkDistance && Vector3.Distance(transform.position, foodLocation.position) > eatDistance && walking != true)
            {
                _animator.SetBool("walk", true);
                walking = true;
                StartCoroutine(Walking());
            }
            else if (Vector3.Distance(transform.position, foodLocation.position) <= eatDistance || Vector3.Distance(transform.position, foodLocation.position) > walkDistance)
            {
                walking = false;
            }
            
            if (Vector3.Distance(transform.position, foodLocation.position) <= eatDistance && eating != true)
            {
                eating = true;
                StartCoroutine(Eating());
            }
            else if (Vector3.Distance(transform.position, foodLocation.position) > eatDistance)
            {
                eating = false;
            }
        }
    }

    private bool eating = false;
    private bool walking = false;

    private IEnumerator Eating()
    {
        while (eating == true)
        {
            currentHunger += 50;
            if (currentHunger > 935)
                currentHunger = 935;
            face.sprite = eat1;
            yield return new WaitForSeconds(1.0f);
            face.sprite = eat2;
            yield return new WaitForSeconds(1.0f);

        }

        face.sprite = smile;
    }
    
    private IEnumerator Walking()
    {
        while (walking == true)
        {
            transform.position = Vector3.MoveTowards(transform.position, foodLocation.position, 0.0001f);
            yield return null;
        }
        _animator.SetBool("walk", false);
        
    }

    public void SetFood(Transform food)
    {
        foodAvailable = true;
        foodLocation = food;
    }

    public void RemoveFood()
    {
        foodAvailable = false;
        foodLocation = null;
    }
}
