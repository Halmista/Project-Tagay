using UnityEngine;

public class PhotoTarget : MonoBehaviour
{
    public Monster monster;

    public string objectiveID;

    public void Photograph()
    {
        if (monster != null)
            monster.Disperse();

        if (!string.IsNullOrEmpty(objectiveID) &&
            ObjectiveManager.Instance.IsCurrentObjective(objectiveID))
        {
            ObjectiveManager.Instance.CompleteObjective();
        }
    }
}