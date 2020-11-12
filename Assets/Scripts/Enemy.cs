using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;

public class Enemy : MonoBehaviour
{
    [SerializeField]
    private float _speed = 3.0f;
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
    

    private bool _isShieldActive = false;
    [SerializeField]
    private GameObject _shieldPrefab;

    private bool _canShootAtPowerup = true;
    private bool _avoidingLaser = false;
    private bool _avoidEnemyType = false;
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
        RaycastHit2D hit = Physics2D.Raycast(transform.position, Vector2.down, Mathf.Infinity, LayerMask.GetMask("Powerup"));

        if(_avoidEnemyType == true)
        {
            AvoidLasers();
        }
        if (hit.collider != null)
        {
            StartCoroutine(ShootatPowerUp());
        }
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
            if(_avoidingLaser == false)
            {
                CalculateMovement();
            }
            _avoidingLaser = false;
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
        if (Time.time > _canFire && _canShootLaser && _playerObject != null)
        {
            GameObject enemyLaser;
            _fireRate = Random.Range(3f, 7f);
            _canFire = Time.time + _fireRate;
            float dist = Mathf.Abs(_playerObject.transform.position.x - transform.position.x);
            if(dist <= 2 && _playerObject.transform.position.y > transform.position.y)
            {
                enemyLaser = Instantiate(_laserPrefab, transform.position, Quaternion.Euler(0,0,180));
            }
            else
            {
                enemyLaser = Instantiate(_laserPrefab, transform.position, Quaternion.identity);
            }

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
        if (Time.time > _canFire && _canShootLaser && _playerObject != null)
        {
            GameObject enemyLaser;
            _fireRate = Random.Range(3f, 7f);
            _canFire = Time.time + _fireRate;
            int angle = 0;
            for(int i = 0; i < 5; i ++)
            {
                enemyLaser = Instantiate(_laserPrefab, transform.position, Quaternion.Euler(0,0, angle-90));
                Laser[] lasers = enemyLaser.GetComponentsInChildren<Laser>();
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

    private IEnumerator ShootatPowerUp()
    {
        if(_canShootAtPowerup)
        {
            GameObject enemyLaser = Instantiate(_laserPrefab, transform.position, Quaternion.identity);
            Laser[] lasers = enemyLaser.GetComponentsInChildren<Laser>();
            for (int i = 0; i < 2; i++)
            {
                lasers[i].AssignEnemyLaser();
            }
            _canShootAtPowerup = false;
            yield return new WaitForSeconds(1.0f);
            _canShootAtPowerup = true;
        }
    }

    private void AvoidLasers()
    {
        Collider2D[] hitColliders = Physics2D.OverlapCircleAll(transform.position, 2.5f);
        foreach (var hitCollider in hitColliders)
        {
            if (hitCollider.tag == "Laser")
            {
                _avoidingLaser = true;
                transform.position = Vector3.MoveTowards(transform.position, hitCollider.transform.position, -1 * _speed * Time.deltaTime);
            }
        }
    }

    public void SetAvoid()
    {
        _avoidEnemyType = true;
    }
}
