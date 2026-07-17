using System.Collections;
using UnityEngine;

public class BlockedStreetTrigger : MonoBehaviour
{
    bool triggered;

    public RevealZone revealZone;

    private void OnTriggerEnter(Collider other)
    {
        if (triggered)
            return;

        if (!other.CompareTag("Player"))
            return;

        triggered = true;

        StartCoroutine(Sequence());
    }

    IEnumerator Sequence()
    {
        yield return new WaitForSeconds(1f);
        DialogueManager.Instance.Say(
            "Ha? May nakaharang.");

        yield return new WaitForSeconds(2.5f);

        PhoneManager.Instance.ReceiveMessage(
            "Anong nangyari?");

        NotificationManager.Instance.ShowNotification(
            "Anong nangyari?");

        yield return new WaitForSeconds(3f);

        PhoneManager.Instance.ReceiveMessage(
            "Picture nga.");

        NotificationManager.Instance.ShowNotification(
            "Picture nga.");

        yield return new WaitForSeconds(2f);

        ObjectiveManager.Instance.SetObjective(
            "PhotographBlockage",
            "Take a picture of the blockage.");
    }
}