using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Asteroid : MonoBehaviour
{
    // Start is called before the first frame update
    [SerializeField]
    GameObject _explosionPrefab;


    private SpawnManager _spawnManager;
    void Start()
    {
        _spawnManager = GameObject.Find("Spawn_Manager").GetComponent<SpawnManager>();
        
        if(_spawnManager == null)
        {
            Debug.LogError("_spawnManager is NULL.");
        }
    }

    // Update is called once per frame
    void Update()
    {
        transform.Rotate(0, 0, -15f * Time.deltaTime);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {

        if(collision.tag == "Laser")
        {
            Destroy(collision.gameObject);
            Instantiate(_explosionPrefab, transform.position, Quaternion.identity);
            _spawnManager.StartSpawning();
            this.GetComponent<CircleCollider2D>().enabled = false;
            Destroy(this.gameObject, 0.5f);
        }
    }
}
