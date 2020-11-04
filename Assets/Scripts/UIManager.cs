using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{
    [SerializeField]
    private Text _scoreText;
    [SerializeField]
    private Text _ammoText;
    [SerializeField]
    private Image _livesImg;
    [SerializeField]
    private Sprite[] _liveSprites;
    [SerializeField]
    private GameObject _gameOverText;
    [SerializeField]
    private GameObject _restartText;

    [SerializeField]
    private GameObject _thrusterBar;
    private Image _thrusterBarImage;

    private GameManager _gameManager;
    // Start is called before the first frame update
    void Start()
    {
        _scoreText.text = "Score: " + 0;
        _ammoText.text = "Ammo: " + 15;
        _gameOverText.SetActive(false);
        _restartText.SetActive(false);
        _gameManager = GameObject.Find("Game_Manager").GetComponent<GameManager>();
        _thrusterBarImage = _thrusterBar.GetComponent<Image>();
        if(_gameManager == null)
        {
            Debug.LogError("_gameManager is NULL.");
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void UpdateThrusters(float amount)
    {
        _thrusterBarImage.fillAmount = amount / 100f;
    }

    public void ChangeAmmo(int ammo)
    {
        _ammoText.text = "Ammo: " + ammo;
    }

    public void ChangeScore(int score)
    {
        _scoreText.text = "Score: " + score;
    }

    public void UpdatesLives(int currentLives)
    {
        _livesImg.sprite = _liveSprites[currentLives];

        if(currentLives == 0)
        {
            _restartText.SetActive(true);
            StartCoroutine(GameOverFlicker());
            _gameManager.GameOver();
        }
    }


    IEnumerator GameOverFlicker()
    {
        while(true)
        {
            _gameOverText.SetActive(true);
            yield return new WaitForSeconds(1.0f);
            _gameOverText.SetActive(false);
            yield return new WaitForSeconds(1.0f);
        }
    }

    
 }
