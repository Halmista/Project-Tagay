using UnityEngine;

public class BodyBagTrigger : MonoBehaviour
{
    public BodyBagBehaviour bodyBag;

    bool triggered;

    void OnTriggerEnter(Collider other)
    {
        if (triggered)
            return;

        if (!other.CompareTag("Player"))
            return;

        triggered = true;
        bodyBag.Squirm();
    }
}