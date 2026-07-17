using System.Collections;
using UnityEngine;

public class GhostEncounterTrigger : MonoBehaviour
{
    public LightFlicker encounterStreetLight;
    public Monster ghost;

    bool triggered;

    private void OnTriggerEnter(Collider other)
    {
        if (triggered)
            return;

        if (!other.CompareTag("Player"))
            return;

        triggered = true;

        StartCoroutine(Encounter());
    }

    IEnumerator Encounter()
    {
        // Flicker the light first
        yield return StartCoroutine(encounterStreetLight.ScareFlicker());
        ghost.gameObject.SetActive(true);

        yield return new WaitForSeconds(0.5f);

        DialogueManager.Instance.Say(
            "..."

        );

        yield return new WaitForSeconds(1f);

        DialogueManager.Instance.Say(
            "Ano yan...?"
        );

        yield return new WaitForSeconds(1f);

        DialogueManager.Instance.Say(
            "Shet wala akong baril ngayon."
        );

        yield return new WaitForSeconds(1f);
        DialogueManager.Instance.Say(
            "Shet."
        );

        yield return new WaitForSeconds(1f);
        DialogueManager.Instance.Say(
            "Shet!"
        );

        yield return new WaitForSeconds(1f);
        DialogueManager.Instance.Say(
            "Baka gumana yung camera??"
        );

        yield return new WaitForSeconds(2f);

        ghost.StartChasing(GameObject.FindGameObjectWithTag("Player").transform);

        ObjectiveManager.Instance.SetObjective(
            "TakeGhostPhoto",
            "Take a picture of the ghost.");
    }
}