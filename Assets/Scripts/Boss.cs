using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Boss : MonoBehaviour
{
    private float _speed = 1.0f;
    [SerializeField]
    private GameObject _laserPrefab;
    private float startAngle = 90f, endAngle = 270f;
    private bool _isAlive = true;
    private float _angle = 0f;
    // Start is called before the first frame update
    void Start()
    {
        StartCoroutine(PhaseChange());
    }

    // Update is called once per frame
    void Update()
    {
        transform.position = Vector3.MoveTowards(transform.position, new Vector3(0, 3.0f, 0), _speed * Time.deltaTime);
    }

    private void ShootLaser1()
    {
        float angleStep = (endAngle - startAngle) / 10;
        float angle = startAngle;

        for(int i = 0; i < 10 + 1; i++)
        {
            float dirX = transform.position.x + Mathf.Sin((angle * Mathf.PI) / 180f);
            float dirY = transform.position.y + Mathf.Cos((angle * Mathf.PI) / 180f);

            Vector3 moveVector = new Vector3(dirX, dirY, 0f);
            Vector2 dir = (moveVector - transform.position).normalized;
            GameObject laser = Instantiate(_laserPrefab, transform.position, transform.rotation);
            laser.GetComponent<Laser>().SetDir(dir);

            angle += angleStep;
        }
    }

    private void ShootLaser2()
    {
        float dirX = transform.position.x + Mathf.Sin((_angle * Mathf.PI) / 180f);
        float dirY = transform.position.y + Mathf.Cos((_angle * Mathf.PI) / 180f);

        Vector3 moveVector = new Vector3(dirX, dirY, 0f);
        Vector2 dir = (moveVector - transform.position).normalized;

        GameObject laser = Instantiate(_laserPrefab, transform.position, transform.rotation);
        laser.GetComponent<Laser>().SetDir(dir);

        _angle += 10f;
    }

    IEnumerator PhaseChange()
    {
        while(_isAlive)
        {
            InvokeRepeating("ShootLaser1", 2f, 2f);
            yield return new WaitForSeconds(20f);
            CancelInvoke("ShootLaser1");
            InvokeRepeating("ShootLaser2", 2f, 0.1f);
            yield return new WaitForSeconds(20f);
            CancelInvoke("ShootLaser2");
        }
    }
}
