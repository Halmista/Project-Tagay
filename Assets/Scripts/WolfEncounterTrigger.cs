using System.Collections;
using UnityEngine;

public class WolfEncounterTrigger : MonoBehaviour
{
    public FirstPersonController player;

    public Monster wolfMonster;

    public RevealZone revealZone;

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
        // Stop the player for a moment
        player.SetMovement(false);

        DialogueManager.Instance.Say("...");

        yield return new WaitForSeconds(2f);

        player.SetMovement(true);

        if (revealZone != null)
            revealZone.OnReveal += RevealWolf;

        ObjectiveManager.Instance.SetObjective(
            "PhotographWolf",
            "Drive away the creature."
        );
    }

    void RevealWolf()
    {
        if (revealZone != null)
            revealZone.OnReveal -= RevealWolf;

        StartCoroutine(StartWolf());
    }

    IEnumerator StartWolf()
    {
        yield return new WaitForSeconds(1f);

        wolfMonster.SetPhotographable(true);

        wolfMonster.StartChasing(player.transform);
    }
}