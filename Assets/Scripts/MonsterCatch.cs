using UnityEngine;

public class MonsterCatch : MonoBehaviour
{
    public Transform respawnPoint;

    private Monster monster;

    void Awake()
    {
        monster = GetComponentInParent<Monster>();
    }

    void OnTriggerEnter(Collider other)
    {
        if (!other.CompareTag("Player"))
            return;

        PlayerLives lives = other.GetComponent<PlayerLives>();

        if (lives != null)
        {
            if (!lives.TakeHit())
            {
                // Player still has lives remaining
                return;
            }
        }

        CharacterController controller = other.GetComponent<CharacterController>();

        if (controller != null)
            controller.enabled = false;

        other.transform.position = respawnPoint.position;
        other.transform.rotation = respawnPoint.rotation;

        if (controller != null)
            controller.enabled = true;

        monster.StopChasing();
    }
}