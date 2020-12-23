using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ManageAirCraft : MonoBehaviour
{
    public GameObject[] spawnPoint;
    public GameObject[] targets;
    public int TargetLeft;
    public GameObject AntiAirRaidCenter;
    public GameObject[] airCraftObject;
    public AirCraft currentAirCraft;
    private int indexAir = 0;
    public bool[] Lines;
    public float heightLines = 8;
    public int score;
    public Text scoreText;
    public Text waveNumber;
    public Text waveOnline;
    public GameObject wavePanel;
    public float searchCountdown=1;
    public float timeBetwenWave = 5;
    public float waveCountDown;
    public enum StateSpawn { COUNTING,SPAWNING,WAITING}
    private StateSpawn state = StateSpawn.COUNTING;
    [System.Serializable]
    public class Wave
    {
        public int number;
        public int enemyCount;
        public float spawnRate;
        public float enemyHealth;
        public float enemySpeed;
        public float firePower;
        public float fireRate;
    }
    public Wave wave;

    [System.Serializable]
    public class speedVariable
    {
        public float baseLevel;
        public float hardLevel;
        public float f1Factor;
        public float f1Power;
        public float f2Factor;
        public float f2Power;
        public float f3Deno;
        public float f1;
        public float f2;
        public float f3;
    }
    public speedVariable SpeedV;
    [System.Serializable]
    public class healthVariable
    {
        public float baseLevel;
        public float hardLevel;
        public float f1Factor;
        public float f1Power;
        public float f2Factor;
        public float f2Power;
        public float f3Deno;
        public float f1;
        public float f2;
        public float f3;
    }
    public healthVariable HealthV;

    [System.Serializable]
    public class powerVariable
    {
        public float baseLevel;
        public float hardLevel;
        public float f1Factor;
        public float f1Power;
        public float f2Factor;
        public float f2Power;
        public float f3Deno;
        public float f1;
        public float f2;
        public float f3;
    }
    public powerVariable PowerV;

    [System.Serializable]
    public class FireRateVariable
    {
        public float baseLevel;
        public float hardLevel;
        public float f1Factor;
        public float f1Power;
        public float f2Factor;
        public float f2Power;
        public float f3Deno;
        public float f1;
        public float f2;
        public float f3;
    }
    public FireRateVariable FireRateV;

    // Start is called before the first frame update
    void Start()
    {
        TargetLeft = targets.Length+1;
        waveCountDown = timeBetwenWave;
    }

    // Update is called once per frame
    void Update()
    {
        if (state == StateSpawn.WAITING)
        {
            //check if enemies are still alive
            if (!EnemyIsAlive())
            {
                //begin a new wave
                WaveCompleted();
            }
            else return;
        }
        if (waveCountDown < 0)
        {
            if (state != StateSpawn.SPAWNING)
            {
                StartCoroutine(SpawnWave());
            }
        }
        else
        {
            waveCountDown -= Time.deltaTime;
        }

        if (TargetLeft == 0)
        {
            GameOver();
        }
        scoreText.text = score.ToString();

    }
    IEnumerator SpawnWave()
    {
        state = StateSpawn.SPAWNING;
        wavePanel.SetActive(false);
        waveOnline.text = wave.number.ToString();
        //do in wave
        Debug.Log("spawning wave:" + wave.number);
        for (int i = 0; i < wave.enemyCount; i++)
        {
            SpawnEnemy();
            yield return new WaitForSeconds(wave.spawnRate);
        }
        state = StateSpawn.WAITING;
        yield break;
    }
    public void SpawnEnemy()
    {
        int AirType = Random.Range(0, airCraftObject.Length);
        int Point = Random.Range(0, spawnPoint.Length);

        GameObject cloneAirCraft = Instantiate(airCraftObject[AirType], spawnPoint[Point].transform.position, spawnPoint[Point].transform.rotation);
        currentAirCraft = cloneAirCraft.GetComponent<AirCraft>();
        currentAirCraft.currentLine = 0;
        currentAirCraft.health = wave.enemyHealth;
        currentAirCraft.speed = wave.enemySpeed+currentAirCraft.baseSpeed;
        if (Point == 0)/// spawnpoint=leftpoint
        {
            currentAirCraft.direction = AirCraft.Direction.west;
        }
        if (Point == 1)///  spawnpoint=rightpoint
        {
            currentAirCraft.direction = AirCraft.Direction.east;
        }
        currentAirCraft.leftPoint = spawnPoint[0].transform;
        currentAirCraft.rightPoint = spawnPoint[1].transform;
        currentAirCraft.letMove = true;
    }

    void WaveCompleted()///begin a new wave
    {
        Debug.Log("wave completed");
        waveNumber.text = wave.number.ToString();
        wavePanel.SetActive(true);
        state = StateSpawn.COUNTING;
        waveCountDown = timeBetwenWave;
        initialNextWave();
        wave.number++;
    }

    void initialNextWave()
    {
        SpeedV.f1 = 1 + SpeedV.f1Factor * Mathf.Pow(wave.number - SpeedV.baseLevel, SpeedV.f1Power);
        SpeedV.f2 = 1 + SpeedV.f2Factor * Mathf.Pow(wave.number - SpeedV.baseLevel, SpeedV.f2Power);
        SpeedV.f3 = Mathf.Min(1, (Mathf.Max(wave.number, SpeedV.hardLevel + SpeedV.baseLevel) - (SpeedV.hardLevel - SpeedV.baseLevel)) / SpeedV.f3Deno);
        wave.enemySpeed = 1 + (SpeedV.f1 - 1) * (1 - SpeedV.f3) + (SpeedV.f2 - 1) * SpeedV.f3;
        HealthV.f1 = 1 + HealthV.f1Factor * Mathf.Pow(wave.number - HealthV.baseLevel, HealthV.f1Power);
        HealthV.f2 = 1 + HealthV.f2Factor * Mathf.Pow(wave.number - HealthV.baseLevel, HealthV.f2Power);
        HealthV.f3 = Mathf.Min(1, (Mathf.Max(wave.number, HealthV.hardLevel + HealthV.baseLevel) - (HealthV.hardLevel - HealthV.baseLevel)) / HealthV.f3Deno);
        wave.enemyHealth = 1 + (HealthV.f1 - 1) * (1 - HealthV.f3) + (HealthV.f2 - 1) * HealthV.f3;
        PowerV.f1 = 1 + PowerV.f1Factor * Mathf.Pow(wave.number - PowerV.baseLevel, PowerV.f1Power);
        PowerV.f2 = 1 + PowerV.f2Factor * Mathf.Pow(wave.number - PowerV.baseLevel, PowerV.f2Power);
        PowerV.f3 = Mathf.Min(1, (Mathf.Max(wave.number, PowerV.hardLevel + PowerV.baseLevel) - (PowerV.hardLevel - PowerV.baseLevel)) / PowerV.f3Deno);
        wave.firePower = 1 + (PowerV.f1 - 1) * (1 - PowerV.f3) + (PowerV.f2 - 1) * PowerV.f3;
        FireRateV.f1 = 1 + FireRateV.f1Factor * Mathf.Pow(wave.number - FireRateV.baseLevel, FireRateV.f1Power);
        FireRateV.f2 = 1 + FireRateV.f2Factor * Mathf.Pow(wave.number - FireRateV.baseLevel, FireRateV.f2Power);
        FireRateV.f3 = Mathf.Min(1, (Mathf.Max(wave.number, FireRateV.hardLevel + FireRateV.baseLevel) - (FireRateV.hardLevel - FireRateV.baseLevel)) / FireRateV.f3Deno);
        wave.fireRate = 1 + (FireRateV.f1 - 1) * (1 - FireRateV.f3) + (FireRateV.f2 - 1) * FireRateV.f3;

        Artillery[] artilleries = FindObjectsOfType<Artillery>();
        foreach (var item in artilleries)
        {
            item.firePower = wave.firePower;
            item.fireRate = wave.fireRate;
        }
    }

    bool EnemyIsAlive()
    {
        searchCountdown -=Time.deltaTime;
        if (searchCountdown <= 0f)
        {
            searchCountdown = 1;
            if (GameObject.FindGameObjectWithTag("AirCraft") == null)
            {
                return false;
            }
        }
        return true;
    }

    public void GameOver()
    {
        Debug.Log("GameOver");
        //Time.timeScale = 0;
    }
}
