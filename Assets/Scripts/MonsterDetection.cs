using UnityEngine;

public class MonsterDetection : MonoBehaviour
{
    private Monster monster;

    void Awake()
    {
        monster = GetComponentInParent<Monster>();
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
            monster.StartChasing(other.transform);
    }

    void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
            monster.StopChasing();
    }
}