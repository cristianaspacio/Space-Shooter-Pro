﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpawnManager : MonoBehaviour
{
    [SerializeField]
    private GameObject _enemyPrefab;
    [SerializeField]
    private GameObject _enemyContainer;
    [SerializeField]
    private GameObject[] _powerups;
    [SerializeField]
    private int _waveNumber = 1;

    private bool _stopSpawning = false;

    [SerializeField]
    private GameObject _bossPrefab;
    // Start is called before the first frame update
    void Start()
    {
       
    }

    public void StartSpawning()
    {
        StartCoroutine(WaveSystem());
        StartCoroutine(SpawnEnemyRoutine());
        StartCoroutine(SpawnPowerupRoutine());
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private IEnumerator SpawnEnemyRoutine()
    {
        yield return new WaitForSeconds(3.0f);
        while(_stopSpawning == false)
        {
            for(int i = 0; i < _waveNumber; i++)
            {
                float rand = Random.value;
                float randomX = Random.Range(-9.0f, 9.0f);
                GameObject newEnemy = Instantiate(_enemyPrefab, new Vector3(randomX, 8, 0), Quaternion.identity);
                newEnemy.transform.parent = _enemyContainer.transform;
                if(rand > 0.8f)
                {
                    newEnemy.GetComponent<Enemy>().SetUnique();
                }
                else if(rand > 0.6f)
                {
                    newEnemy.GetComponent<Enemy>().SetShield();
                }
                else if(rand > 0.5f)
                {
                    newEnemy.GetComponent<Enemy>().SetAvoid();
                }

            }
            yield return new WaitForSeconds(5.0f);
            
        }
    }

    IEnumerator SpawnPowerupRoutine()
    {
        yield return new WaitForSeconds(3.0f);
        while (_stopSpawning == false) 
        {
            float rand = Random.value;
            Vector3 spawnPoint = new Vector3(Random.Range(-9.0f, 9.0f), 8, 0);
            if(rand > 0.8f)
            {
                if(Random.value > 0.5f)
                {
                    Instantiate(_powerups[6], spawnPoint, Quaternion.identity);
                }
                else
                {
                    Instantiate(_powerups[7], spawnPoint, Quaternion.identity);
                }
            }
            else if(rand > 0.7f)
            {
                Instantiate(_powerups[4], spawnPoint, Quaternion.identity);
            }
            else if(rand > 0.6f)
            {
                Instantiate(_powerups[5], spawnPoint, Quaternion.identity); 
            }
            else if(rand > 0.5f)
            {
                int randomPowerup = Random.Range(0, 3);
                Instantiate(_powerups[randomPowerup], spawnPoint, Quaternion.identity);
            }
            else
            {
                Instantiate(_powerups[3], spawnPoint, Quaternion.identity);
            }
            yield return new WaitForSeconds(Random.Range(3.0f, 7.0f));
        }
        
    }

    public void OnPlayerDeath()
    {
        _stopSpawning = true;
    }

    IEnumerator WaveSystem()
    {
        while(_stopSpawning == false)
        {
            yield return new WaitForSeconds(20.0f);
            _waveNumber++;
            if(_waveNumber >= 3)
            {
                _stopSpawning = true;
                BossEncounter();
            }
        }
    }

    private void BossEncounter()
    {
        GameObject boss = Instantiate(_bossPrefab, new Vector3(0, 8, 0), Quaternion.identity);
        boss.transform.parent = _enemyContainer.transform;
    }
}
