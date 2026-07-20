using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MessageTrigger : MonoBehaviour
{
    [Header("Completion")]
    public bool completeCurrentObjective;
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
    public List<ChatMessage> messages = new();
   
    [Header("Dialogue")]

    [TextArea]
    public string dialogue;

    public float dialogueDelay = 2f;

    [Header("Objective")]
    public string objectiveID;

    [TextArea]
    public string objectiveText;

    private bool triggered;

    

    private void OnTriggerEnter(Collider other)
    {
        Debug.Log("Entered by: " + other.name);
        if (triggered)
            return;

        if (!other.CompareTag("Player"))
            return;

        triggered = true;

        StartCoroutine(SendConversation());
    }

    IEnumerator SendConversation()
    {
        if (!string.IsNullOrEmpty(dialogue))
        {
            DialogueManager.Instance.Say(dialogue);

            yield return new WaitForSeconds(dialogueDelay);
        }

        foreach (ChatMessage chat in messages)
        {
            PhoneManager.Instance.ReceiveMessage(chat.message);
            NotificationManager.Instance.ShowNotification(chat.message, NotificationSender.Jun);

            yield return new WaitForSeconds(chat.delayAfter);
        }

        if (!string.IsNullOrEmpty(objectiveID))
        {
            ObjectiveManager.Instance.SetObjective(
                objectiveID,
                objectiveText);
        }

        if (completeCurrentObjective)
        {
            ObjectiveManager.Instance.CompleteObjective();
        }

        if (!string.IsNullOrEmpty(storyEvent))
        {
            Debug.Log("Triggering story event: " + storyEvent);
            StoryManager.Instance.TriggerEvent(storyEvent);
        }
    }
}