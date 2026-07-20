using System.Collections;
using UnityEngine;

public class StoreArrivalTrigger : MonoBehaviour
{
    bool triggered;

    private void OnTriggerEnter(Collider other)
    {
        if (triggered)
            return;

        if (!other.CompareTag("Player"))
            return;

        triggered = true;

        StartCoroutine(StoreSequence());
    }

    IEnumerator StoreSequence()
    {
        DialogueManager.Instance.Say(
           "Jami, pwede pa ba ako bumili?");

        yield return new WaitForSeconds(2f);

        // Jami speaks
        DialogueManager.Instance.Say(
            "Jami: Uy long time no see, ser.");

        yield return new WaitForSeconds(1f);

        DialogueManager.Instance.Say(
           "Jami: Papunta na ako sa bahay ni Jun. Sinasara ko lang yung tindahan.");

        yield return new WaitForSeconds(1f);
        
        DialogueManager.Instance.Say(
           "Jami: Anong kailangan mo?");

        yield return new WaitForSeconds(2f);

        // Officer thinks
        DialogueManager.Instance.Say(
            "Chichirya lang, pandagdag pulutan.");
        
        yield return new WaitForSeconds(7f);

        PhoneManager.Instance.ReceiveMessage(
            "Picturan mo kung anong meron sila.");

        NotificationManager.Instance.ShowNotification(
            "Picturan mo kung anong meron sila", NotificationSender.Jun);

        yield return new WaitForSeconds(2f);

        ObjectiveManager.Instance.SetObjective(
            "PhotographChips",
            "Open your phone's camera [C] and take a picture of the chips [Left-Click].");
    }
}