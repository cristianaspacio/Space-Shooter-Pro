using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    //public or private reference 
    //data type (int,float,bool,string)
    //every variable has a name
    //optional value assigned
    [SerializeField]
    private float _speed = 5.0f;
    [SerializeField]
    private GameObject _laserPrefab;
    [SerializeField]
    private float _fireRate = 0.5f;
    private float _canFire = -1.0f;
    [SerializeField]
    private int _ammoCount = 15;
    [SerializeField]
    private int _lives = 3;
    private SpawnManager _spawnManager;
    [SerializeField]
    private bool _isTripleShotActive = false;
    [SerializeField]
    private GameObject _tripleShotPrefab;
    [SerializeField]
    private bool _isMultiDirectionalShotActive = false;
    [SerializeField]
    GameObject[] _multiDirectionalLasers;
    [SerializeField]
    private bool _isSpeedBoostActive = false;
    [SerializeField]
    private bool _isShieldActive = false;

    [SerializeField]
    private GameObject _shield;
    private SpriteRenderer _shieldSpriteRenderer;
    private int _shieldStrength;

    [SerializeField]
    private int _score;
    private UIManager _uiManager;

    [SerializeField]
    private GameObject _rightEngine, _leftEngine;
    [SerializeField]
    private AudioClip _laserSound;
    [SerializeField]
    private AudioClip _explosionSound;
    private AudioSource _audioSource;


    // Start is called before the first frame update
    void Start()
    {
        transform.position = new Vector3(0, 0, 0);
        _spawnManager = GameObject.Find("Spawn_Manager").GetComponent<SpawnManager>();
        _uiManager = GameObject.Find("Canvas").GetComponent<UIManager>();
        _audioSource = GetComponent<AudioSource>();
        _shieldSpriteRenderer = _shield.GetComponent<SpriteRenderer>();
        if(_shieldSpriteRenderer == null)
        {
            Debug.LogError("Shield is NULL.");
        }
        if(_audioSource == null)
        {
            Debug.LogError("The Audio Source is NULL.");
        }
        if (_spawnManager == null)
        {
            Debug.LogError("The Spawn Manager is NULL");
        }
        if(_uiManager == null)
        {
            Debug.LogError("The UI Manager is NULL");
        }

        _audioSource.clip = _laserSound;

    }

    // Update is called once per frame
    void Update()
    {

        CalculateMovement();

        if(Input.GetKeyDown(KeyCode.Space) && Time.time > _canFire)
        {
            ShootLaser();
        }
    }
    void CalculateMovement()
    {
        float horizontalInput = Input.GetAxis("Horizontal");
        float verticalInput = Input.GetAxis("Vertical");
        Vector3 direction = new Vector3(horizontalInput, verticalInput, 0);

        if (Input.GetKey(KeyCode.LeftShift))
        {
            transform.Translate(direction * _speed * 2 * Time.deltaTime);
        }
        else
        {
            transform.Translate(direction * _speed * Time.deltaTime);
        }
        
        
        


        transform.position = new Vector3(transform.position.x, Mathf.Clamp(transform.position.y, -3.8f, 0));

        if (transform.position.x >= 11)
        {
            transform.position = new Vector3(-11, transform.position.y, 0);
        }
        else if (transform.position.x <= -11)
        {
            transform.position = new Vector3(11, transform.position.y, 0);
        }
    }

    void ShootLaser()
    {
        _canFire = Time.time + _fireRate;

        if (_isTripleShotActive == true && _ammoCount > 0)
        {
            _ammoCount--;
            Instantiate(_tripleShotPrefab, transform.position, Quaternion.identity);
            _audioSource.Play();
            _uiManager.ChangeAmmo(_ammoCount);
        }
        else if(_isMultiDirectionalShotActive && _ammoCount > 0)
        {
            _ammoCount--;

            Instantiate(_laserPrefab, transform.position + new Vector3(0, 1.05f, 0), Quaternion.identity);
            Instantiate(_laserPrefab, transform.position + new Vector3(-1.05f, 1.05f, 0), Quaternion.Euler(new Vector3(0, 0, 45)));
            Instantiate(_laserPrefab, transform.position + new Vector3(-1.05f, 0, 0), Quaternion.Euler(new Vector3(0, 0, 90)));
            Instantiate(_laserPrefab, transform.position + new Vector3(-1.05f, -1.05f, 0), Quaternion.Euler(new Vector3(0, 0, 135)));
            Instantiate(_laserPrefab, transform.position + new Vector3(0, -1.05f, 0), Quaternion.Euler(new Vector3(0, 0, 180)));
            Instantiate(_laserPrefab, transform.position + new Vector3(1.05f, -1.05f, 0), Quaternion.Euler(new Vector3(0, 0, 225)));
            Instantiate(_laserPrefab, transform.position + new Vector3(1.05f, 0, 0), Quaternion.Euler(new Vector3(0, 0, 270)));
            Instantiate(_laserPrefab, transform.position + new Vector3(1.05f, 1.05f, 0), Quaternion.Euler(new Vector3(0, 0, 315)));

            _audioSource.Play();
            _uiManager.ChangeAmmo(_ammoCount);
        }
        else if(_ammoCount > 0)
        {
            _ammoCount--;
            Instantiate(_laserPrefab, transform.position + new Vector3(0, 1.05f, 0), Quaternion.identity);
            _audioSource.Play();
            _uiManager.ChangeAmmo(_ammoCount);
        }
       
    }

    public void Damage()
    {
        if(_isShieldActive == true)
        {
            DamageShield();
            return;
        }
        _lives--;

        if(_lives == 2)
        {
            _rightEngine.SetActive(true);
            
        }
        else if (_lives == 1)
        {
            _leftEngine.SetActive(true);
        }

        _uiManager.UpdatesLives(_lives);

        if(_lives < 1)
        {
            _spawnManager.OnPlayerDeath();
            _audioSource.clip = _explosionSound;
            _audioSource.Play();
            Destroy(this.gameObject);
        }
    }

    public void TripleShotActive ()
    {
        _isTripleShotActive = true;
        StartCoroutine(TripleShotPowerDown());
    }

    IEnumerator TripleShotPowerDown()
    {
         yield return new WaitForSeconds(8.0f);
         _isTripleShotActive = false;
    }

    public void SpeedBoostActive()
    {
        _isSpeedBoostActive = true;
        _speed *= 2;
        StartCoroutine(SpeedBoostPowerDown());
    }

    IEnumerator SpeedBoostPowerDown()
    {
        yield return new WaitForSeconds(8.0f);
        _isSpeedBoostActive = false;
        _speed = 5.0f;
    }

    public void ShieldActive()
    {
        _shieldSpriteRenderer.color = Color.white;
        _isShieldActive = true;
        _shield.SetActive(true);
        _shieldStrength = 3;
    }

    public void RefillAmmo()
    {
        _ammoCount = 15;
        _uiManager.ChangeAmmo(_ammoCount);
    }

    public void AddScore(int points)
    {
        _score += points;
        _uiManager.ChangeScore(_score);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision.tag == "Enemy_Laser")
        {
            Destroy(collision.gameObject);
            Damage();
        }
    }

    private void DamageShield()
    {
        _shieldStrength--;
        switch (_shieldStrength)
        {
            case 2:
                _shieldSpriteRenderer.color = Color.yellow;
                break;
            case 1:
                _shieldSpriteRenderer.color = Color.red;
                break;
            case 0:
                _isShieldActive = false;
                _shield.SetActive(false);
                break;
            default:
                break;
        }
    }

    public void HealthPickUp()
    {
        if(_lives < 3)
        {
            _lives++;
            switch(_lives)
            {
                case 2:
                    _leftEngine.SetActive(false);
                    break;
                case 3:
                    _rightEngine.SetActive(false);
                    break;
                default:
                    break;
            }
            _uiManager.UpdatesLives(_lives);
        }
    }

    public void MultiShotActive()
    {
        _isMultiDirectionalShotActive = true;
        _isTripleShotActive = false;
        StartCoroutine(MultiShotPowerDown());
    }

    IEnumerator MultiShotPowerDown()
    {
        yield return new WaitForSeconds(5.0f);
        _isMultiDirectionalShotActive = false;
    }
}
