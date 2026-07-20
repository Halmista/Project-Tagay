using System.Collections;
using UnityEngine;

public class GhostEncounterTrigger : MonoBehaviour
{
    public LightFlicker encounterStreetLight;
    public Monster ghost;
    public FirstPersonController player;

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
        ghost.SetPhotographable(false);
        ghost.gameObject.SetActive(true);

        yield return new WaitForSeconds(0.5f);

        DialogueManager.Instance.Say(
            "..."

        );

        yield return new WaitForSeconds(1f);

        DialogueManager.Instance.Say(
            "Tangina."
        );
        player.SetMovement(false);
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
        player.SetMovement(true);
        yield return new WaitForSeconds(1f);
        DialogueManager.Instance.Say(
            "Baka gumana yung camera??"
        );

        yield return new WaitForSeconds(2f);
        player.GetComponent<PlayerLives>().ResetHits(1);
        ghost.StartChasing(GameObject.FindGameObjectWithTag("Player").transform);
        ghost.SetPhotographable(true);
        yield return new WaitForSeconds(3f);

        ObjectiveManager.Instance.SetObjective(
            "TakeGhostPhoto",
            "Take a picture of the ghost.");
    }
}