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
            "Kailan pa naharangan yung daan dito?");

        yield return new WaitForSeconds(2.5f);

        PhoneManager.Instance.ReceiveMessage(
            "Kadadaan ko lang kanina dyan.");

        NotificationManager.Instance.ShowNotification(
            "Kadadaan ko lang kanina dyan.", NotificationSender.Jun);

        yield return new WaitForSeconds(3f);

        PhoneManager.Instance.ReceiveMessage(
            "Picturan mo nga.");

        NotificationManager.Instance.ShowNotification(
            "Picturan mo nga.", NotificationSender.Jun);

        yield return new WaitForSeconds(3f);

        PhoneManager.Instance.ReceiveMessage(
            "Matagal ka lang di nakabalik dito sa atin.");

        NotificationManager.Instance.ShowNotification(
            "Matagal ka lang di nakabalik dito sa atin.", NotificationSender.Jun);

        yield return new WaitForSeconds(3f);

        PhoneManager.Instance.ReceiveMessage(
            "Naliligaw ka na kaagad.");

        NotificationManager.Instance.ShowNotification(
            "Naliligaw ka na kaagad.", NotificationSender.Jun);

        yield return new WaitForSeconds(2f);

        ObjectiveManager.Instance.SetObjective(
            "PhotographBlockage",
            "Take a picture of the wooden planks blockage.");
    }
}