using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class MinigameManager : MonoBehaviour
{
    [SerializeField] private float spawnOuter, spawnInner;
    
    private int score = 0;
    private bool gameActive = false;
    private EggFriend egg;
    [SerializeField] private GameObject enemyGO;
    private List<Enemy> enemyList = new List<Enemy>();

    [SerializeField] private GameObject hungerUI, gameUI;
    [SerializeField] private TextMeshProUGUI currentScore, highScore;
    [SerializeField] private GameObject bullet;
    private bool canShoot = true;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if(!gameActive) 
            return;
        
        if ((Input.touchCount > 0 && Input.GetTouch(0).phase == TouchPhase.Began || Input.GetMouseButtonDown(0)) && canShoot)
        {
            ShootBullet();
            canShoot = false;
            Invoke("ReloadShoot", 0.25f);
        }
    }

    private void ShootBullet()
    {
        var b = Instantiate(bullet, Camera.main.transform.position, Camera.main.transform.rotation);
        Destroy(b, 10.0f);
    }

    private void ReloadShoot()
    {
        canShoot = true;
    }

    private IEnumerator SpawnEnemies()
    {
        while (gameActive)
        {
            SpawnDrill();
            yield return new WaitForSeconds(10.0f / (score == 0 ? 1 : score));
        }
    }

    public void StartGame(EggFriend egg)
    {
        gameActive = true;
        this.egg = egg;
        StartCoroutine(SpawnEnemies());
        
        egg.HungerEnabled(false);
        hungerUI.SetActive(false);
        gameUI.SetActive(true);
    }

    public void EndGame()
    {
        foreach (var enemy in enemyList)
        {
            Destroy(enemy.gameObject);
        }
        enemyList.Clear();
        
        egg.HungerEnabled(true);
        hungerUI.SetActive(true);
        gameUI.SetActive(false);
        gameActive = false;
    }

    private void SpawnDrill()
    {
        Enemy enemy = Instantiate(enemyGO, GetRandomPositionAroundEgg(egg.transform), Quaternion.identity).GetComponent<Enemy>();
        enemy.Setup(egg.transform, this);
        enemyList.Add(enemy);
    }

    public void KillDrill(Enemy enemy)
    {
        enemyList.Remove(enemy);
        Destroy(enemy.gameObject);
        score++;
        currentScore.text = score.ToString();
    }

    private Vector3 GetRandomPositionAroundEgg(Transform eggTransform)
    {
        var x = GetRandom1DPoint(eggTransform.position.x);
        var y = GetRandom1DPoint(eggTransform.position.y);
        var z = GetRandom1DPoint(eggTransform.position.z);
        return new Vector3(x, y, z);
    }

    private float GetRandom1DPoint(float xyz)
    {
        var xLow = Random.Range(xyz - spawnOuter, xyz - spawnInner);
        var xHigh = Random.Range(xyz + spawnInner, xyz + spawnOuter);
        float x;
        if (Random.Range(0, 1.0f) < 0.5f)
            x = xLow;
        else
            x = xHigh;

        return x;
    }
}
