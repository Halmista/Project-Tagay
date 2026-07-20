using UnityEngine;

public class BodyBagCompleteTrigger : MonoBehaviour
{
    bool triggered;

    private void OnTriggerEnter(Collider other)
    {
        if (triggered)
            return;

        if (!other.CompareTag("Player"))
            return;

        triggered = true;

        if (ObjectiveManager.Instance.IsCurrentObjective("WalkPastBodyBag"))
        {
            ObjectiveManager.Instance.CompleteObjective();
        }

        StoryManager.Instance.TriggerEvent("AfterBodyBag");
    }
}