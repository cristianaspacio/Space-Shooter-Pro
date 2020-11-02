using System.Collections;
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

    private bool _stopSpawning = false;
    // Start is called before the first frame update
    void Start()
    {
       
    }

    public void StartSpawning()
    {
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
            float randomX = Random.Range(-9.0f, 9.0f);
            GameObject newEnemy = Instantiate(_enemyPrefab, new Vector3(randomX, 8, 0),  Quaternion.identity);
            newEnemy.transform.parent = _enemyContainer.transform;
            yield return new WaitForSeconds(5.0f);

        }
    }

    IEnumerator SpawnPowerupRoutine()
    {
        yield return new WaitForSeconds(3.0f);
        while (_stopSpawning == false) 
        {
            Vector3 spawnPoint = new Vector3(Random.Range(-9.0f, 9.0f), 8, 0);
            int randomPowerup = Random.Range(0, 5);
            Instantiate(_powerups[randomPowerup], spawnPoint, Quaternion.identity);
            yield return new WaitForSeconds(Random.Range(3.0f, 7.0f));
        }
        
    }

    public void OnPlayerDeath()
    {
        _stopSpawning = true;
    }
}
