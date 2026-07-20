using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IntroMessages : MonoBehaviour
{
    public FirstPersonController player;
    [System.Serializable]
    public class IntroMessage
    {
        [TextArea]
        public string text;

        public float delayAfter = 3f;
    }

    public List<IntroMessage> messages = new();

    public string startingObjectiveID;

    [TextArea]
    public string startingObjectiveText;

    IEnumerator Start()
    {
        player.SetMovement(false);
        foreach (IntroMessage msg in messages)
        {
            PhoneManager.Instance.ReceiveMessage(msg.text);
            NotificationManager.Instance.ShowNotification(msg.text,
            NotificationSender.Jun);

            yield return new WaitForSeconds(msg.delayAfter);
        }

        if (!string.IsNullOrEmpty(startingObjectiveID))
        {
            ObjectiveManager.Instance.SetObjective(
                startingObjectiveID,
                startingObjectiveText);
        }

        player.SetMovement(true);
    }
}