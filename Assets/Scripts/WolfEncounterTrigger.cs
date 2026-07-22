using System.Collections;
using UnityEngine;

public class WolfEncounterTrigger : MonoBehaviour
{
    public FirstPersonController player;

    public Monster wolfMonster;

    //public RevealZone revealZone;

    bool explainedThreeShots;

    bool triggered;

    public DogPatrolBehaviour dogBehaviour;

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

        //wolfMonster.SetPhotographable(false);

        DialogueManager.Instance.Say("...");

        yield return new WaitForSeconds(2f);

        DialogueManager.Instance.Say("Putangina, ano na naman ito?");

        yield return new WaitForSeconds(2f);

        player.SetMovement(true);

        ObjectiveManager.Instance.SetObjective(
            "ObserveWolf",
            "Observe the monster."
        );

        StartCoroutine(StartWolf());
    }

    /*void RevealWolf()
    {
        Debug.Log("RevealWolf called");
        if (revealZone != null)
            revealZone.OnReveal -= RevealWolf;

        StartCoroutine(StartWolf());
    }*/

    IEnumerator StartWolf()
    {
        yield return new WaitForSeconds(1f);
        StoryManager.Instance.TriggerEvent("WolfEncounter");
        dogBehaviour.ResumePatrol();

        
    }


}