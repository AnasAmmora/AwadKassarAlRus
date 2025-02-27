using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SecondRoofCollision : MonoBehaviour
{
    [SerializeField] private GameObject Player;
    [SerializeField] private GameObject Enemy;
    [SerializeField] private GameObject EnemyRoof;
    [SerializeField] private AudioSource impactAudio;

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.GetComponent<NewCarController>() != null)
        {
            CollisionAction();
        }
        if (collision.gameObject.GetComponent<OpponentController>() != null)
        {
            CollisionAction();
        }
        if (collision.gameObject.CompareTag("Ground"))
        {
            CollisionAction();
        }
    }
    private void CollisionAction()
    {
        //if (Enemy.GetComponent<NewCarController>() != null)
        //{
        //    Enemy.GetComponent<NewCarController>().enabled = false;
        //}
        EnemyRoof.SetActive(false);
        if (Player.GetComponent<NewCarController>() != null)
        {
            Player.GetComponent<NewCarController>().DestroyPlayerCar();
        }
        //else if (Player.GetComponent<OpponentController>() != null)
        //{
        //    Player.GetComponent<OpponentController>().DestroyPlayerCar();
        //}
        impactAudio.Play();
        GameData.FirstPlayerStarts++;
        GameManager.Instance.Endlevel();
    }
}
