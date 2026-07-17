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

        // Aling Nena speaks
        DialogueManager.Instance.Say(
            "Jami: Magsasara na kami. Anong kailangan mo?");

        yield return new WaitForSeconds(2f);

        // Officer thinks
        DialogueManager.Instance.Say(
            "Chichirya lang, pampulutan.");
        
        yield return new WaitForSeconds(1f);

        DialogueManager.Instance.Say(
           "Ay... hindi ko alam kung anong gustong brand ni Jun.");

        yield return new WaitForSeconds(5f);

        PhoneManager.Instance.ReceiveMessage(
            "Sendan mo na lang ako ng picture kung anong meron sila.");

        NotificationManager.Instance.ShowNotification(
            "Sendan mo na lang ako ng picture kung anong meron sila.");

        yield return new WaitForSeconds(2f);

        ObjectiveManager.Instance.SetObjective(
            "PhotographChips",
            "Open your phone's camera [C] and take a picture of the chips [Left-Click].");
    }
}