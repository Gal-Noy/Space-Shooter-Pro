using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Laser : MonoBehaviour
{
    [SerializeField]
    private float _speed = 8.0f;

    private Player shootingPlayer;

    private void Start()
    {
        switch (gameObject.tag)
        {
            case "Laser":
                shootingPlayer = GameObject.Find("Player").GetComponent<Player>();
                break;
            case "Laser_1":
                shootingPlayer = GameObject.Find("Player_1").GetComponent<Player>();
                break;
            case "Laser_2":
                shootingPlayer = GameObject.Find("Player_2").GetComponent<Player>();
                break;
        }
    }
    void Update()
    {
        transform.Translate(Vector3.up * _speed * Time.deltaTime);
        if (transform.position.y > 8f)
        {
            if (transform.parent != null)
                Destroy(transform.parent.gameObject);
            Destroy(this.gameObject);
        }
    }

    public void IncreasePlayerScore(int score)
    {
        shootingPlayer.IncreaseScore(score);
    }
}
