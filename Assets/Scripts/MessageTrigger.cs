using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MessageTrigger : MonoBehaviour
{
    [System.Serializable]
    public class ChatMessage
    {
        [TextArea]
        public string message;

        [Tooltip("Seconds to wait before sending the next message.")]
        public float delayAfter = 2f;
    }

    [Header("Story")]
    public string storyEvent;

    [Header("Conversation")]
    public List<ChatMessage> messages = new List<ChatMessage>();

    [Header("Objective")]
    public string objectiveID;

    [TextArea]
    public string objectiveText;

    private bool triggered;

    

    private void OnTriggerEnter(Collider other)
    {
        if (triggered)
            return;

        if (!other.CompareTag("Player"))
            return;

        triggered = true;

        StartCoroutine(SendConversation());
    }

    IEnumerator SendConversation()
    {
        foreach (ChatMessage chat in messages)
        {
            PhoneManager.Instance.ReceiveMessage(chat.message);
            NotificationManager.Instance.ShowNotification(chat.message);

            yield return new WaitForSeconds(chat.delayAfter);
        }

        if (!string.IsNullOrEmpty(objectiveID))
        {
            ObjectiveManager.Instance.SetObjective(
                objectiveID,
                objectiveText);
        }

        if (!string.IsNullOrEmpty(storyEvent))
        {
            StoryManager.Instance.TriggerEvent(storyEvent);
        }
    }
}