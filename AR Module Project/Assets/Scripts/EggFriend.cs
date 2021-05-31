using System.Collections;
using System.Collections.Generic;
using Photon.Pun;
using TMPro;
using UnityEngine;

public class EggFriend : MonoBehaviourPun, IPunInstantiateMagicCallback
{
    [SerializeField] private float eatDistance;
    [SerializeField] private float walkDistance;
    
    private Transform _cameraTransform;
    public List<Color> randomColours;
    private Animator _animator;

    public bool foodAvailable;
    public List<Transform> foodLocation = new List<Transform>();
    
    [SerializeField] private SpriteRenderer body;
    [SerializeField] private SpriteRenderer face;
    [SerializeField] private Sprite smile, eat1, eat2;

    private int currentHunger = 935;
    [SerializeField] private RectTransform textMask, barMask;

    [SerializeField] private bool hunger = true;

    public int eggColour;

    
    public void OnPhotonInstantiate(PhotonMessageInfo info)
    {
        transform.parent = ARCentralManager.Project.NetworkManager.AR_Origin;
    }
    
    // Start is called before the first frame update
    void OnEnable()
    {
        _cameraTransform = Camera.main.transform;
        _animator = body.GetComponent<Animator>();

        if (!photonView.IsMine)
        {
            ARCentralManager.Project.NetworkManager.WaitBeforeLoadEggProperties(this, photonView);
            ARCentralManager.Project.GameManager.AddEggFriendsToOnlineList(photonView.Owner.NickName, this);
        }
    }

    public void SetColourManually(int c)
    {
        eggColour = c;
        body.color = randomColours[c];
    }
    
    
    public void SetColourRandomly()
    {
        eggColour = (int) Random.Range(0, randomColours.Count);
        body.color = randomColours[eggColour];
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
        Vector3 target;
        if (foodAvailable)
            target = GetClosestFood().position;
        else
            target = _cameraTransform.position;
        
        transform.LookAt(target);

        var xRotation = transform.eulerAngles.x;
        if (xRotation > 180) xRotation = xRotation - 360;
        xRotation = Mathf.Clamp(xRotation, -25, 25);
        if (xRotation < 0) xRotation = 360 + xRotation;
        transform.eulerAngles = new Vector3(xRotation, transform.eulerAngles.y, 0);

        if (foodAvailable)
        {
            if (photonView.IsMine)
            {
                if (Vector3.Distance(transform.position, target) <= walkDistance &&
                    Vector3.Distance(transform.position, target) > eatDistance && walking != true)
                {
                    _animator.SetBool("walk", true);
                    walking = true;
                    StartCoroutine(Walking(target));
                }
                else if (Vector3.Distance(transform.position, target) <= eatDistance ||
                         Vector3.Distance(transform.position, target) > walkDistance)
                {
                    walking = false;
                }
            }

            if (Vector3.Distance(transform.position, target) <= eatDistance && eating != true)
            {
                eating = true;
                StartCoroutine(Eating());
            }
            else if (Vector3.Distance(transform.position, target) > eatDistance)
            {
                eating = false;
            }
        }
        else
        {
            walking = false;
            eating = false;
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
    
    private IEnumerator Walking(Vector3 target)
    {
        while (walking == true)
        {
            transform.position = Vector3.MoveTowards(transform.position, target, 0.001f);
            yield return null;
        }
        _animator.SetBool("walk", false);
        
    }

    public void SetFood(Transform food)
    {
        foodAvailable = true;
        foreach (var foods in foodLocation)
        {
            if (Vector3.Distance(foods.position, food.position) < 0.01f)
            {
                print("DESTROYED FOOD");
                Destroy(food.gameObject);
                return;
            }
        }
        foodLocation.Add(food);
    }

    private Transform GetClosestFood()
    {
        Transform closestFood = foodLocation[0];
        foreach (var food in foodLocation)
        {
            if (Vector3.Distance(transform.position, food.position) <
                Vector3.Distance(transform.position, closestFood.position))
            {
                closestFood = food;
            }
        }

        return closestFood;
    }

    public void RemoveFood()
    {
        foodAvailable = false;
        foodLocation = null;
    }

    
}
