using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class Powerup : MonoBehaviour
{
    [SerializeField]
    private float _speed = 3.0f;
    [SerializeField] // 0 = TripleShot, 1 = Speed, 2 = Shield, 3 = Ammo
    private int _powerupID;
    // Start is called before the first frame update
    [SerializeField]
    private AudioClip _audioClip;

    [SerializeField]
    GameObject _player;
    void Start()
    {
        _player = GameObject.Find("Player");
        if(_player == null)
        {
            Debug.LogError("Player is null.");
        }
    }

    // Update is called once per frame
    void Update()
    {
        if(Input.GetKey(KeyCode.C))
        {
            transform.position = Vector3.MoveTowards(transform.position, _player.transform.position, _speed * Time.deltaTime * 2);
        }
        else
        {
            transform.Translate(Vector3.down * _speed * Time.deltaTime);
        }

        if(transform.position.y <= -7.0f)
        {
            Destroy(this.gameObject);
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision.tag == "Player")
        {
            Player player = collision.transform.GetComponent<Player>();

            AudioSource.PlayClipAtPoint(_audioClip, transform.position);
            if(player != null)
            {
                switch (_powerupID)
                {
                    case 0:
                        player.TripleShotActive();
                        break;
                    case 1:
                        player.SpeedBoostActive();
                        break;
                    case 2:
                        player.ShieldActive();
                        break;
                    case 3:
                        player.RefillAmmo();
                        break;
                    case 4:
                        player.HealthPickUp();
                        break;
                    case 5:
                        player.NegativePowerupActive();
                        break;
                    case 6:
                        player.MultiShotActive();
                        break;
                    case 7:
                        player.HomingProjectileActive();
                        break;
                    default:
                        break;
                }
               
                Destroy(this.gameObject);
            }
        }
        if(collision.tag == "Enemy_Laser")
        {
            Destroy(this.gameObject);
        }
    }

}
