using UnityEngine;

public class RoadBlockTrigger : MonoBehaviour
{
    [TextArea]
    public string dialogue = "Kausapin ko muna si Mang Nestor.";

    float cooldown;

    void Update()
    {
        if (cooldown > 0)
            cooldown -= Time.deltaTime;
    }

    void OnTriggerEnter(Collider other)
    {
        if (!other.CompareTag("Player"))
            return;

        if (cooldown > 0)
            return;

        cooldown = 2f;

        DialogueManager.Instance.Say(dialogue);
    }
}