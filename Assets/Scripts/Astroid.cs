using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Astroid : MonoBehaviour
{
    [SerializeField]
    private float rotateSpeed = 20f;

    [SerializeField]
    private GameObject explosionPrefab;

    [SerializeField]
    private SpawnManager spawnManager;

    void Start()
    {
        spawnManager = GameObject.Find("Spawn_Manager").GetComponent<SpawnManager>();
    }

    // Update is called once per frame
    void Update()
    {
        transform.Rotate(Vector3.forward* rotateSpeed * Time.deltaTime);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Laser") || collision.CompareTag("Laser_1") || collision.CompareTag("Laser_2"))
        {
           Instantiate(explosionPrefab, transform.position, Quaternion.identity);
           Destroy(collision.gameObject);
           spawnManager.StartSpawning(); // Start the game!
           Destroy(this.gameObject, 0.25f);
        }
    }
}
