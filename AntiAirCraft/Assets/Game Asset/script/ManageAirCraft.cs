using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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
    public int power;
    public float searchCountdown=1;
    public float timeBetwenWave = 5;
    public float waveCountDown;
    public enum StateSpawn { COUNTING,SPAWNING,WAITING}
    private StateSpawn state = StateSpawn.COUNTING;
    public int nextWave=0;
    [System.Serializable]
    public class Wave
    {
        public int number;
        public int count;
        public float rate;
    }
    public Wave wave;

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
            GameOwer();
        }
    }
    IEnumerator SpawnWave()
    {
        state = StateSpawn.SPAWNING;
        //do in wave
        Debug.Log("spawning wave:" + wave.number);
        for (int i = 0; i < wave.count; i++)
        {
            SpawnEnemy();
            yield return new WaitForSeconds(wave.rate);
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
        currentAirCraft.power = 100;
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
        state = StateSpawn.COUNTING;
        waveCountDown = timeBetwenWave;

        nextWave++;
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

    public void GameOwer()
    {
        Time.timeScale = 0;
    }
}
