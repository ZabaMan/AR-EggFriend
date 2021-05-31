using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Photon.Pun;
using TMPro;
using UnityEngine;

public class MinigameManager : MonoBehaviour
{
    [SerializeField] private float spawnOuter, spawnInner;
    
    private int score = 0;
    private bool gameActive = false;
    private bool gameOwner = false;
    private EggFriend eggFriend;
    private Dictionary<string, EggFriend> eggTargets = new Dictionary<string, EggFriend>();
    [SerializeField] private GameObject enemyGO;
    private List<Enemy> enemyList = new List<Enemy>();

    [SerializeField] private GameObject hungerUI, gameUI;
    [SerializeField] private TextMeshProUGUI currentScore, highScore;
    [SerializeField] private GameObject bullet;
    private bool canShoot = true;
    [SerializeField] private GameObject minigameNetworker;
    private GameObject minigameNetworkerGO;


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
        var offset = ARCentralManager.Project.NetworkManager.AR_Origin.InverseTransformPoint(Camera.main.transform.position);
        object[] arr = new object[1];
        arr[0] = offset;

        var b = PhotonNetwork.Instantiate("Bullet", Camera.main.transform.position, Camera.main.transform.rotation, 0, arr);
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

    public void StartGame(EggFriend egg, Dictionary<string, EggFriend> _eggFriendsOnline)
    {
        gameActive = true;
        gameOwner = true;
        eggFriend = egg;
        eggTargets.Clear();
        eggTargets.Add(PhotonNetwork.NickName, egg);
        foreach (var onlineEggs in _eggFriendsOnline)
        {
            eggTargets.Add(onlineEggs.Key, onlineEggs.Value);
        }

        minigameNetworkerGO = PhotonNetwork.Instantiate("MinigameOnline", Vector3.zero, Quaternion.identity);
        StartCoroutine(SpawnEnemies());
        
        eggFriend.HungerEnabled(false);
        hungerUI.SetActive(false);
        gameUI.SetActive(true);
        highScore.text = PlayerPrefs.GetInt("HighScore").ToString();
        score = 0;
        currentScore.text = score.ToString();
    }

    public void JoinGame()
    {
        gameActive = true;
        eggFriend = ARCentralManager.Project.GameManager.GetEggFriend().EggFriend;
        eggFriend.HungerEnabled(false);
        hungerUI.SetActive(false);
        gameUI.SetActive(true);
        highScore.text = PlayerPrefs.GetInt("HighScore").ToString();
        score = 0;
        currentScore.text = score.ToString();
    }

    public void EndGame()
    {
        if (gameOwner)
        {
            foreach (var enemy in enemyList)
            {
                if (enemy.gameObject)
                    PhotonNetwork.Destroy(enemy.gameObject);
            }
            enemyList.Clear();
            PhotonNetwork.Destroy(minigameNetworkerGO);
        }

        if (score > PlayerPrefs.GetInt("HighScore"))
        {
            PlayerPrefs.SetInt("HighScore", score);
            PlayerPrefs.Save();
        }

        
        eggFriend.HungerEnabled(true);
        hungerUI.SetActive(true);
        gameUI.SetActive(false);
        gameActive = false;
    }

    private void SpawnDrill()
    {
        var targetEgg = RandomEgg();
        var rPos = GetRandomPositionAroundEgg(eggTargets[targetEgg].transform);
        var offset = ARCentralManager.Project.NetworkManager.AR_Origin.InverseTransformPoint(rPos);
        object[] arr = new object[2];
        arr[0] = targetEgg;
        arr[1] = offset;
        Enemy enemy = PhotonNetwork.Instantiate("Enemy", rPos, Quaternion.identity,0, arr).GetComponent<Enemy>();
        enemy.Setup(eggTargets[targetEgg].transform);
        enemyList.Add(enemy);
    }

    public void KillDrill(Enemy enemy)
    {
        if (gameOwner)
        {
            enemyList.Remove(enemy);
            PhotonNetwork.Destroy(enemy.gameObject);
        }
    }

    public void AddScore()
    {
        score++;
        currentScore.text = score.ToString();
    }

    private string RandomEgg()
    {
        var rEgg = Random.Range(0, eggTargets.Count);
        return eggTargets.ElementAt(rEgg).Key;
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
