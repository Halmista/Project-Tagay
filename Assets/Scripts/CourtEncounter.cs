using System.Collections;
using UnityEngine;

public class CourtEncounter : MonoBehaviour
{
    public GameObject normalCourt;
    public GameObject horrorCourt;

    public Material blackSkybox;

    public Transform mazeCheckpoint;
    public Monster[] mazeDogs;

    Material originalSkybox;

    bool triggered;

    void Start()
    {
        originalSkybox = RenderSettings.skybox;
    }

    void OnTriggerEnter(Collider other)
    {
        Debug.Log("Something entered: " + other.name);

        if (triggered)
            return;

        if (!other.CompareTag("Player"))
            return;

        Debug.Log("Player entered!");

        triggered = true;

        StartCoroutine(BeginEncounter());
    }

    IEnumerator BeginEncounter()
    {
        DialogueManager.Instance.Say("Kailangan ko na lang lakarin papunta sa kabilang parte ng court.");
        yield return new WaitForSeconds(6f);

        RenderSettings.skybox = blackSkybox;
        DynamicGI.UpdateEnvironment();

        normalCourt.SetActive(false);
        horrorCourt.SetActive(true);

        yield return new WaitForSeconds(2f);

        DialogueManager.Instance.Say("...");

        yield return new WaitForSeconds(2f);

        DialogueManager.Instance.Say("Nawala silang lahat...?");

        yield return new WaitForSeconds(2f);

        DialogueManager.Instance.Say("...");

        yield return new WaitForSeconds(2f);

        ObjectiveManager.Instance.SetObjective(
            "WalkPastDogMaze",
            "Carefully make your way through the basketball court."
        );

        foreach (Monster dog in mazeDogs)
            dog.SetPhotographable(false);
    }
}