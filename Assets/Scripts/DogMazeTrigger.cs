using UnityEngine;

public class DogMazeMonster : MonoBehaviour
{
    public Transform checkpoint;

    void OnTriggerEnter(Collider other)
    {
        if (!other.CompareTag("Player"))
            return;

        CharacterController cc = other.GetComponent<CharacterController>();

        if (cc != null)
            cc.enabled = false;

        other.transform.SetPositionAndRotation(
            checkpoint.position,
            checkpoint.rotation);

        if (cc != null)
            cc.enabled = true;

        DialogueManager.Instance.Say(
            "Kailangan kong umiwas..."
        );
    }
}