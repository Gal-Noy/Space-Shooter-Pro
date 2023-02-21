using System.Collections;
using System.Collections.Generic;
using System.Security.Cryptography;
using UnityEngine;
using UnityEngine.InputSystem.OnScreen;

public class Player : MonoBehaviour
{
    // Stats
    [SerializeField]
    private float _speed = 5f;
    [SerializeField]
    private int lives = 3;
    [SerializeField]
    private int score = 0;
    [SerializeField]
    private int playerId;

    // Utils
    private SpawnManager spawnManager;
    private UIManager uiManager;
    private Animator animator;

    // Laser
    private bool isFiring;
    private float lastShotTime;
    [SerializeField]
    private GameObject _laserPrefab;
    [SerializeField]
    private float _fireRate = 0.3f;


    // Powerups
    private bool tripleShotActive = false;
    [SerializeField]
    private GameObject tripleShot;

    private bool shieldActive = false;
    [SerializeField]
    private GameObject shieldVisualizer;

    // Damage
    [SerializeField]
    private GameObject[] damage;
    private int nextDamage;

    // Moving animations
    private bool isMovingLeft = false;
    private bool isMovingRight = false;

    // Sound FX
    [SerializeField]
    private AudioSource laserSound;

    private bool isCoop;

    void Start()
    {
        isCoop = playerId != 0;
        PositionPlayer();
        spawnManager = GameObject.Find("Spawn_Manager").GetComponent<SpawnManager>();
        animator = GetComponent<Animator>();
        nextDamage = (int) Random.Range(0, 2);

        if (!isCoop)
            uiManager = GameObject.Find("Canvas").GetComponent<UIManager>();
        else
            uiManager = GameObject.Find("Canvas").GetComponent<UIManagerCoop>();
    }

    void PositionPlayer()
    {
        switch (playerId)
        {
            case 0:
                transform.position = new Vector3(0, 0, 0);
                break;
            case 1:
                transform.position = new Vector3(-3, 0, 0);
                break;
            case 2:
                transform.position = new Vector3(3, 0, 0);
                break;
        }
    }

    void Update()
    {
        CalculateMovement();
        HandleFire();
    }

    void CalculateMovement()
    {
        float horizontalInput = Input.GetAxis(playerId == 0 ? "Horizontal" : "Horizontal" + playerId);
        float verticallInput = Input.GetAxis(playerId == 0 ? "Vertical" : "Vertical" + playerId);

        Vector3 direction = new Vector3(horizontalInput, verticallInput, 0);

        transform.Translate(direction * _speed * Time.deltaTime);

        KeepBounds();

        HandleMovingAnimation(horizontalInput);
    }

    void KeepBounds()
    {
        transform.position = new Vector3(transform.position.x, Mathf.Clamp(transform.position.y, -3.8f, 0), 0);

        if (transform.position.x >= 10f)
            transform.position = new Vector3(-10f, transform.position.y, 0);
        else if (transform.position.x <= -10f)
            transform.position = new Vector3(10f, transform.position.y, 0);
    }

    void HandleMovingAnimation(float moveInput)
    {
        if (moveInput < 0 && !isMovingLeft)
        {
            animator.SetTrigger("MoveLeft");
            isMovingLeft = true;
            isMovingRight = false;
        }
        else if (moveInput > 0 && !isMovingRight)
        {
            animator.SetTrigger("MoveRight");
            isMovingLeft = false;
            isMovingRight = true;
        }
        else if (moveInput == 0 && (isMovingLeft || isMovingRight))
        {
            if (isMovingLeft)
            {
                animator.SetTrigger("MoveLeftReverse");
            }
            else if (isMovingRight)
            {
                animator.SetTrigger("MoveRightReverse");
            }

            animator.SetTrigger("Idle");
            isMovingLeft = false;
            isMovingRight = false;
        }
    }

    void HandleFire()
    {
        if (playerId < 2)
        {
            if (Input.GetKeyDown(KeyCode.Space) && !isFiring)
            {
                isFiring = true;
                StartCoroutine(FireLaser());
            }
            if (Input.GetKeyUp(KeyCode.Space))
            {
                isFiring = false;
            }
        }
        else if (playerId == 2)
        {
            if (Input.GetKeyDown(KeyCode.Keypad0) && !isFiring)
            {
                isFiring = true;
                StartCoroutine(FireLaser());
            }
            if (Input.GetKeyUp(KeyCode.Keypad0))
            {
                isFiring = false;
            }
        }
        
    }

    IEnumerator FireLaser()
    {
        while (isFiring)
        {
            float timeSinceLastShot = Time.time - lastShotTime;
            if (timeSinceLastShot >= _fireRate)
            {
                laserSound.Play();
                lastShotTime = Time.time;
                Vector3 laserPosition = transform.position + new Vector3(0, 1.05f, 0);
                Instantiate(!tripleShotActive ? _laserPrefab : tripleShot, laserPosition, transform.rotation);
            }
            yield return null;
        }
    }

    public void Damage()
    {
        if (shieldActive)
        {
            DeactivateShield();
            return;
        }

        if (!isCoop)
            uiManager.UpdateLives(--lives);
        else
            ((UIManagerCoop)uiManager).UpdateLives(--lives, playerId);

        if (lives > 0)
        {
            damage[nextDamage].gameObject.SetActive(true);
            nextDamage = 1 - nextDamage;
        }
        else
        {
            Destroy(this.gameObject);
            spawnManager.OnPlayerDeath(playerId);
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("EnemyLaser"))
        {
            Destroy(collision.gameObject);
            Damage();
        }
    }

    public void IncreaseScore(int points)
    {
        score += points;
        
        if (!isCoop)
            uiManager.UpdateScore(score);
        else
            ((UIManagerCoop) uiManager).UpdateScore(score, playerId);
    }

    // Triple Shot powerup
    public void ActivateTripleShot()
    {
        tripleShotActive = true;
        StartCoroutine(TripleShotPowerDownRoutine());
    }

    IEnumerator TripleShotPowerDownRoutine()
    {
        yield return new WaitForSeconds(5.0f);
        tripleShotActive = false;
    }

    // Speed powerup
    public void ActivateSpeedBoost()
    {
        _speed *= 1.5f;
        StartCoroutine(SpeedPowerDownRoutine());
    }

    IEnumerator SpeedPowerDownRoutine()
    {
        yield return new WaitForSeconds(5.0f);
        _speed /= 1.5f;
    }

    // Shield powerup
    public void ActivateShield()
    {
        shieldActive = true;
        shieldVisualizer.gameObject.SetActive(true);
    }

    public void DeactivateShield()
    {
        shieldActive = false;
        shieldVisualizer.gameObject.SetActive(false);
    }

}