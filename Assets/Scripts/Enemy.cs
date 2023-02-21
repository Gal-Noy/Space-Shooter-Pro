using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : MonoBehaviour
{
    [SerializeField]
    private float _speed = 4f;

    private Animator mAnimator;

    private bool isDead = false;
    [SerializeField]
    private AudioSource explosionSound;

    [SerializeField]
    private GameObject _laserPrefab;

    [SerializeField]
    private AudioSource laserSound;

    void Start()
    {
        mAnimator = GetComponent<Animator>();
        explosionSound = GetComponent<AudioSource>();

        StartCoroutine(FireLaser());
    }

    void Update()
    {
        HandleMovement();
    }

    void HandleMovement()
    {
        transform.Translate(Vector3.down * _speed * Time.deltaTime);
        if (transform.position.y <= -4f)
            transform.position = new Vector3(Random.Range(-8f, 8f), 6, 0);
    }

    IEnumerator FireLaser()
    {
        yield return new WaitForSeconds(2f);
        while (!isDead)
        {
            laserSound.Play();
            Vector3 laserPosition = transform.position + new Vector3(-0.1175f, -1.05f, 0);
            Instantiate(_laserPrefab, laserPosition, transform.rotation);
            yield return new WaitForSeconds(Random.Range(1f, 4f));
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (!isDead)
        {
            if (collision.CompareTag("Player"))
            {
                Player player = collision.transform.GetComponent<Player>();
                if (player != null)
                    player.Damage();
                Die();
            }
            else if (collision.CompareTag("Laser") || collision.CompareTag("Laser_1") || collision.CompareTag("Laser_2"))
            {
                Laser laser = collision.transform.GetComponent<Laser>();
                Destroy(collision.gameObject);
                if (laser != null)
                    laser.IncreasePlayerScore(Random.Range(5, 16));
                Die();
            }
        }
    }

    void Die()
    {
        isDead = true;
        mAnimator.SetTrigger("OnEnemyDeath");
        _speed = 0.5f;
        explosionSound.Play();
        Destoy();
    }

    void Destoy()
    {
        foreach (Transform child in gameObject.transform)
            Destroy(child.gameObject);
        Destroy(GetComponent<Collider2D>());
        Destroy(this.gameObject, 2.8f);
    }
}
