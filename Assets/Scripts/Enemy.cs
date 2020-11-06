using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : MonoBehaviour
{
    [SerializeField]
    private float _speed = 2.0f;
    private float _fireRate = 3.0f;
    private float _canFire = -1f;
    private GameObject _playerObject;
    private Player _player;
    [SerializeField]
    private GameObject _laserPrefab;
    private bool _canShootLaser;
    private bool _moveRight = true;
    // Start is called before the first frame update

    private Animator _anim;

    private AudioSource _audioSource;

    [SerializeField]
    private bool _isUnique = false;
    [SerializeField]
    private GameObject[] _uniqueEnemyLasers;

    private bool _isShieldActive = false;
    [SerializeField]
    private GameObject _shieldPrefab;
    void Start()
    {
        _playerObject = GameObject.Find("Player");
        _player = _playerObject.GetComponent<Player>();
        _audioSource = GetComponent<AudioSource>();
        _canShootLaser = true;
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
        if (_isUnique)
        {
            UniqueLaser();
            UniqueMovement();
        }
        else if(_playerObject != null && Vector3.Distance(_playerObject.transform.position, transform.position) <= 3.0f)
        {
            MoveTowardsPlayer();
        }
        else
        {
            ShootLaser();
            CalculateMovement();
        }
    }

    private void CalculateMovement()
    {
        if(_moveRight)
        {
            transform.Translate(Vector3.right * _speed * Time.deltaTime);
        }
        else
        {
            transform.Translate(Vector3.left * _speed * Time.deltaTime);
        }

        //move down 4 meters per second
        if(transform.position.x <= -9.0f)
        {
            _moveRight = true;
        }
        else if(transform.position.x >= 9.0f)
        {
            _moveRight = false;
        }

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

            if (_isShieldActive == true)
            {
                DamageShield();
            }
            else
            {
                _anim.SetTrigger("OnEnemyDeath");
                _audioSource.Play();
                _speed = 0f;
                _canShootLaser = false;
                Destroy(this.GetComponent<BoxCollider2D>());
                Destroy(this.gameObject, 2.8f);
            }

        }

        if (other.tag == "Laser")
        {
            if(_player != null)
            {
                _player.AddScore(10);
            }

            if(_isShieldActive == true)
            {
                DamageShield();
                Destroy(other.gameObject);
            }
            else
            {
                _anim.SetTrigger("OnEnemyDeath");
                Destroy(other.gameObject);
                _audioSource.Play();
                _speed = 0f;
                Destroy(this.GetComponent<BoxCollider2D>());
                _canShootLaser = false;
                Destroy(this.gameObject, 2.8f);
            }
        }
    }

    private void ShootLaser()
    {
        if (Time.time > _canFire && _canShootLaser)
        {
            _fireRate = Random.Range(3f, 7f);
            _canFire = Time.time + _fireRate;
            GameObject enemyLaser = Instantiate(_laserPrefab, transform.position, Quaternion.identity);
            Laser[] lasers = enemyLaser.GetComponentsInChildren<Laser>();
            for (int i = 0; i < 2; i++)
            {
                lasers[i].AssignEnemyLaser();

            }
        }
    }

    public void SetUnique()
    {
        _isUnique = true;
    }

    private void UniqueLaser()
    {
        if (Time.time > _canFire && _canShootLaser)
        {
            _fireRate = Random.Range(3f, 7f);
            _canFire = Time.time + _fireRate;
            int angle = 0;
            for(int i = 0; i < 4; i ++)
            {
                _uniqueEnemyLasers[i] = Instantiate(_laserPrefab, transform.position, Quaternion.Euler(new Vector3(0,0, angle+290.0f)));
                Laser[] lasers = _uniqueEnemyLasers[i].GetComponentsInChildren<Laser>();
                for (int j = 0; j < 2; j++)
                {
                    lasers[j].AssignEnemyLaser();
                }
                angle += 45;
            }
            
        }
    }

    private void UniqueMovement()
    {
        if (_moveRight)
        {
            transform.Translate(Vector3.right * _speed * Time.deltaTime);
        }
        else
        {
            transform.Translate(Vector3.left * _speed * Time.deltaTime);
        }

        if (transform.position.x <= -9.0f)
        {
            _moveRight = true;
        }
        else if (transform.position.x >= 9.0f)
        {
            _moveRight = false;
        }

        if(transform.position.y >= 2)
        {
            transform.Translate(Vector3.down * _speed * Time.deltaTime);
        }
    }

    private void DamageShield()
    {
        _isShieldActive = false;
        _shieldPrefab.SetActive(false);
    }

    public void SetShield()
    {
        _isShieldActive = true;
        _shieldPrefab.SetActive(true);
    }

    private void MoveTowardsPlayer()
    {
        transform.position = Vector3.MoveTowards(transform.position, _playerObject.transform.position, _speed * Time.deltaTime);
    }
}
