using JetBrains.Annotations;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Laser : MonoBehaviour
{
    [SerializeField]
    private float _speed = 8.0f;
    private bool _isEnemyLaser;
    private bool _isHoming = false;
    private GameObject _target = null;
    // Update is called once per frame
    void Update()
    {
        if (_isHoming == true && _target != null)
        {
            MoveTowardsTarget();
        }
        else if (_isEnemyLaser == false)
        {
            MoveUp();
        }
        else
        {
            MoveDown();
        }
    }

    private void MoveUp()
    {
        transform.Translate(Vector3.up * _speed * Time.deltaTime);

        if (transform.position.y >= 8.0f || transform.position.y <= -5.0f || transform.position.x >= 11.0f || transform.position.x <= -11.0f)
        {
            if (transform.parent != null)
            {
                Destroy(transform.parent.gameObject);
            }
            Destroy(gameObject);
        }
    }

    private void MoveDown()
    {
        transform.Translate(Vector3.down * _speed * Time.deltaTime);

        if (transform.position.y <= -5.0f)
        {
            if (transform.parent != null)
            {
                Destroy(transform.parent.gameObject);
            }
            Destroy(gameObject);
        }
    }

    private void MoveTowardsTarget()
    {
        if(_target != null)
        {
            transform.position = Vector3.MoveTowards(transform.position, _target.transform.position, _speed * Time.deltaTime);
        }
        else
        {
            FindClosestTarget("Enemy");
        }
        
        if(_target.GetComponent<Collider2D>() == null)
        {
            _target = null;
        }
    }

    public void AssignEnemyLaser()
    {
        _isEnemyLaser = true;
        this.tag = "Enemy_Laser";
        
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.tag == "Player" && _isEnemyLaser)
        {
            collision.GetComponent<Player>().Damage();
            Destroy(gameObject);
        }
        if(collision.tag == "Powerup" && _isEnemyLaser)
        {
            Destroy(gameObject);
        }

    }

    private void OnBecameInvisible()
    {
        if (transform.parent != null)
        {
            Destroy(transform.parent.gameObject);
        }
        Destroy(gameObject);
    }

    public void SetHomingActive(string targetName)
    {
        _isHoming = true;
        FindClosestTarget(targetName);
    }

    private void FindClosestTarget(string targetName)
    {
        GameObject[] potentialTargets;
        potentialTargets = GameObject.FindGameObjectsWithTag(targetName);
        float distance = Mathf.Infinity;
        foreach (GameObject potentialTarget in potentialTargets)
        {
            Vector3 diff = potentialTarget.transform.position - transform.position;
            float curDistance = diff.sqrMagnitude;
            if(curDistance < distance)
            {
                _target = potentialTarget;
                distance = curDistance;
            }
        }
    }
}
