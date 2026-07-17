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

        CharacterController controller = other.GetComponent<CharacterController>();

        if (controller != null)
            controller.enabled = false;

        other.transform.position = respawnPoint.position;
        other.transform.rotation = respawnPoint.rotation;

        if (controller != null)
            controller.enabled = true;

        // Stop the monster from chasing
        monster.StopChasing();
    }
}