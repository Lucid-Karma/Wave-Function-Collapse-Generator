using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyController : MonoBehaviour
{
    enum EnemyState
    {
        Rotate,
        Fire
    }
    EnemyState enemyState;

    protected List<GameObject> _bullets = new();
    public GameObject bullet;
    public Transform muzzle;
    protected DynamicPool bulletPool;
    private float _timer;
    public float speedTime;
    private int maxBulletCount = 5;  //difficultyManager
    private int bulletCounter;

    public float rotationSpeed;
    private float rotationTimer;
    private float rotationAngle;

    private GameObject _player;
    public float playerRadius;    //difficultyManager
    private Quaternion rotationGoal;
    private Quaternion finalRotation;

    private void Start()
    {
        bulletPool = new();
        _player = GameObject.FindGameObjectWithTag("Player");
        enemyState = EnemyState.Fire;
    }

    void FixedUpdate()
    {
        switch (enemyState)
        {
            case EnemyState.Fire:
                Fire(); 
                break;
            case EnemyState.Rotate:
                RotateEnemy();
                break;
        }
    }

    public virtual void Fire()
    {
        if (bulletCounter >= maxBulletCount)
        {
            bulletCounter = 0;
            enemyState = EnemyState.Rotate;
            return;
        }

        if (_timer >= speedTime)
        {
            bulletPool.GetObject(muzzle, bullet, _bullets);
            bulletCounter++;
            _timer = 0;
        }

        _timer += 0.02f;
    }

    private void RotateEnemy()
    {
        transform.Rotate(Vector3.up, Random.Range(0, 360));
        KeepFiring();
    }

    private void RotateConstantly()
    {
        transform.Rotate(Vector3.up * rotationSpeed * Time.fixedDeltaTime);
    }

    Vector3 randomPosition;
    private void KeepFiring()
    {
        enemyState = EnemyState.Fire;
    }

    private void RotateToPlayer()
    {
        randomPosition = _player.transform.position + Random.insideUnitSphere * playerRadius;

        Vector3 direction = (randomPosition - transform.position).normalized;
        rotationGoal = Quaternion.LookRotation(direction);
        finalRotation = Quaternion.Slerp(transform.rotation, rotationGoal, rotationSpeed); // Smooth rotation change
        transform.rotation = new Quaternion(finalRotation.x, 0, finalRotation.y, 0);
    }
}
