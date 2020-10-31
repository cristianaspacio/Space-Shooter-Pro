﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : MonoBehaviour
{
    [SerializeField]
    private float _speed = 2.0f;
    private float _fireRate = 3.0f;
    private float _canFire = -1f;
    private Player _player;
    [SerializeField]
    private GameObject _laserPrefab;
    // Start is called before the first frame update

    private Animator _anim;

    private AudioSource _audioSource;
    void Start()
    {
        _player = GameObject.Find("Player").GetComponent<Player>();
        _audioSource = GetComponent<AudioSource>();
        
        if(_audioSource == null)
        {
            Debug.LogError("Audio Source is NULL.");
        }
        if (_player == null)
        {
            Debug.LogError("_player is NULL");
        }
        _anim = GetComponent<Animator>();
        if(_anim == null)
        {
            Debug.LogError("_anim is NULL");
        }

        
    }

    // Update is called once per frame
    void Update()
    {
        if(Time.time >_canFire)
        {
            _fireRate = Random.Range(3f, 7f);
            _canFire = Time.time + _fireRate;
            GameObject enemyLaser = Instantiate(_laserPrefab, transform.position, Quaternion.identity);
            Laser[] lasers= enemyLaser.GetComponentsInChildren<Laser>();
            for(int i = 0; i < 2; i++)
            {
                lasers[i].AssignEnemyLaser();
                
            }
        }
        CalculateMovement();
    }

    private void CalculateMovement()
    {
        //move down 4 meters per second
        transform.Translate(Vector3.down * _speed * Time.deltaTime);
        //if bottom of screen respawn at top at a new random x position
        if (transform.position.y <= -6.0f)
        {
            float randomX = Random.Range(-9.0f, 9.0f);
            transform.position = new Vector3(randomX, 8, 0);
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.tag == "Player")
        {
            Player player = other.transform.GetComponent<Player>();

            if (player != null)
            {
                player.Damage();
            }
            _anim.SetTrigger("OnEnemyDeath");
            _audioSource.Play();
            _speed = 0f;
            Destroy(this.gameObject, 2.8f);

        }

        if (other.tag == "Laser")
        {
            if(_player != null)
            {
                _player.AddScore(10);
            }
            _anim.SetTrigger("OnEnemyDeath");
            Destroy(other.gameObject);
            _audioSource.Play();
            _speed = 0f;
            Destroy(this.GetComponent<BoxCollider2D>());
            Destroy(this.gameObject, 2.8f);
        }
    }

    

}