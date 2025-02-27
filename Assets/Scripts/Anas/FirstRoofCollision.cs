using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FirstRoofCollision : MonoBehaviour
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
        //if(Enemy.GetComponent<NewCarController>() != null)
        //{
        //    Enemy.GetComponent<NewCarController>().enabled = false;
        //}
        //else if (Enemy.GetComponent<OpponentController>() != null)
        //{
        //    Enemy.GetComponent<OpponentController>().enabled = false;
        //}
        impactAudio.Play();
        EnemyRoof.SetActive(false);
        GameData.SecondPlayerStarts++;
        Player.GetComponent<NewCarController>().DestroyPlayerCar();
        GameManager.Instance.Endlevel();
    }
}


