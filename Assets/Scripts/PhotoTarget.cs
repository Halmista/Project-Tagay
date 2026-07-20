using UnityEngine;

public class PhotoTarget : MonoBehaviour
{
    public Monster monster;

    public string objectiveID;

    public RevealZone revealZone;

    public BodyBagBehaviour bodyBag;

    public void Photograph()
    {
        Debug.Log("Photographed: " + gameObject.name);

        if (revealZone != null)
            revealZone.LockReveal();

        // Special reaction for the body bag
        if (bodyBag != null)
        {
            bodyBag.Squirm();
        }

        // Normal clue/body bag photo target
        if (monster == null)
        {
            if (!string.IsNullOrEmpty(objectiveID) &&
                ObjectiveManager.Instance.IsCurrentObjective(objectiveID))
            {
                ObjectiveManager.Instance.CompleteObjective();
            }

            return;
        }

        // Monster photo target
        bool defeated = monster.Disperse();

        if (defeated &&
            !string.IsNullOrEmpty(objectiveID) &&
            ObjectiveManager.Instance.IsCurrentObjective(objectiveID))
        {
            ObjectiveManager.Instance.CompleteObjective();
        }
    }
}