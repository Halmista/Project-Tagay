using DG.Tweening;
using System.Collections;
using UnityEngine;

public class StoryManager : MonoBehaviour
{
    public static StoryManager Instance;
    public FirstPersonController player;
    public GameObject jamiNPC;
    public Transform jamiExitPoint;

    public LightFlicker entranceStreetLight;

    void Awake()
    {
        Instance = this;
    }
    void OnEnable()
    {
        ObjectiveManager.OnObjectiveCompleted += ObjectiveCompleted;
    }

    void OnDisable()
    {
        ObjectiveManager.OnObjectiveCompleted -= ObjectiveCompleted;
    }

    public void TriggerEvent(string eventName)
    {
        switch (eventName)
        {
            case "MangNestorArrival":
                StartCoroutine(MangNestorArrival());
                break;

            case "HouseInside":
                StartCoroutine(HouseInside());
                break;
        }
    }

    void ObjectiveCompleted(string id)
    {
        switch (id)
        {
            case "Move":
                StartCoroutine(MoveSequence());
                break;

            case "OpenPhone":
                StartCoroutine(OpenPhoneSequence());
                break;

            case "PhotographChips":

                StartCoroutine(ChipsSequence());

                break;
            
            case "PhotographBlockage":
                StartCoroutine(BlockageSequence());
                break;

            case "TakeGhostPhoto":
                StartCoroutine(GhostSequence());
                break;
        }
    }

    IEnumerator MoveSequence()
    {
        DialogueManager.Instance.Say(
            "Kailangan ko na talagang bilisan.");

        yield return new WaitForSeconds(2.5f);

        PhoneManager.Instance.ReceiveMessage(
            "Bukas ka na ba dadating? Haha.");

        NotificationManager.Instance.ShowNotification(
            "Bukas ka na ba dadating? Haha.");

        yield return new WaitForSeconds(1f);
        player.SetMovement(false);
        ObjectiveManager.Instance.SetObjective(
            "OpenPhone",
            "Open your phone to check Jun's messages. [TAB]");
    }

    IEnumerator OpenPhoneSequence()
    {
        //player.SetMovement(false);
        yield return new WaitForSeconds(1.5f);
        DialogueManager.Instance.Say(
           "Kulit nito ni Jun.");

        yield return new WaitForSeconds(2f);
        DialogueManager.Instance.Say(
            "Ganito ba kadilim dito dati?");

        yield return new WaitForSeconds(2f);

        StartCoroutine(entranceStreetLight.ScareFlicker());

        yield return new WaitForSeconds(2f);

        DialogueManager.Instance.Say(
            "Hala.");

        yield return new WaitForSeconds(3f);

        ObjectiveManager.Instance.SetObjective(
            "TurnOnFlashlight",
            "Turn on your phone's flashlight [F] and continue moving");
        player.SetMovement(true);
    }

    IEnumerator ChipsSequence()
    {
        yield return new WaitForSeconds(1f);
        PhoneManager.Instance.ReceiveMessage(
            "Ay wala yung gusto kong chichirya.");

        NotificationManager.Instance.ShowNotification(
            "Ay wala yung gusto kong chichirya.");

        yield return new WaitForSeconds(3f);

        PhoneManager.Instance.ReceiveMessage(
            "Wag na lang, bumili na pala si Lisa.");

        NotificationManager.Instance.ShowNotification(
            "Wag na lang, bumili na pala si Lisa.");

        yield return new WaitForSeconds(3f);

        PhoneManager.Instance.ReceiveMessage(
            "Ambagal mo, maubusan ka na dito!");

        NotificationManager.Instance.ShowNotification(
            "Ambagal mo, maubusan ka na dito!");

        yield return new WaitForSeconds(2f);

        DialogueManager.Instance.Say(
            "Haha taena nito ni Jun. Siya 'tong nagpapabili ng pampulutan tapos ako yung mabagal.");
       
        ObjectiveManager.Instance.SetObjective(
           "Move Forward",
           "Continue going to the next streetlight and then turn right.");

        yield return new WaitForSeconds(3f);
        DialogueManager.Instance.Say(
            "Jami: Ariel, ang tagal mo nang hindi napapagawi dito.");
        
        yield return new WaitForSeconds(1f);
        Animator anim = jamiNPC.GetComponent<Animator>();

        // Face the exit first
        Vector3 direction = jamiExitPoint.position - jamiNPC.transform.position;
        direction.y = 0f;

        Quaternion targetRotation = Quaternion.LookRotation(direction);

        jamiNPC.transform
            .DORotateQuaternion(targetRotation, 0.5f)
            .SetEase(Ease.OutSine)
            .OnComplete(() =>
            {
                anim.SetBool("Walking", true);

                jamiNPC.transform.DOMove(jamiExitPoint.position, 2.5f)
                    .SetEase(Ease.Linear)
                    .OnComplete(() =>
                    {
                        anim.SetBool("Walking", false);
                        jamiNPC.SetActive(false);
                    });
            });

       
       
    }

    IEnumerator BlockageSequence()
    {
        yield return new WaitForSeconds(0.5f);

        DialogueManager.Instance.Say(
            "...");

        yield return new WaitForSeconds(1.5f);

        DialogueManager.Instance.Say(
            "Nawawala?"

        );

        yield return new WaitForSeconds(2.5f);

        DialogueManager.Instance.Say(
            "Ano yun...?"
        );

        yield return new WaitForSeconds(2f);

        DialogueManager.Instance.Say(
            "Bakit may dugo...?"
        );

        yield return new WaitForSeconds(3f);

        PhoneManager.Instance.ReceiveMessage(
            "Parang wala namang problema.");

        NotificationManager.Instance.ShowNotification(
            "Parang wala namang problema.");

        yield return new WaitForSeconds(3f);

        PhoneManager.Instance.ReceiveMessage(
            "Baka pagod lang yan...");

        NotificationManager.Instance.ShowNotification(
            "Baka pagod lang yan...");

        yield return new WaitForSeconds(2f);

        DialogueManager.Instance.Say(
            "Tangina anong nangyayari dito.."
        );

        yield return new WaitForSeconds(2f);

        ObjectiveManager.Instance.SetObjective(
            "ContinueStreet",
            "Continue moving forward.");
    }

    IEnumerator GhostSequence()
    {
        yield return new WaitForSeconds(1f);

        DialogueManager.Instance.Say(
            "Tangina!"
        );

        yield return new WaitForSeconds(2.5f);

        DialogueManager.Instance.Say(
            "Ano yon?!"
        );

        yield return new WaitForSeconds(3f);

        PhoneManager.Instance.ReceiveMessage(
            "Ayos ka lang? Na kina Mang Nestor ka na?"
        );

        NotificationManager.Instance.ShowNotification(
            "Ayos ka lang? Na kina Mang Nestor ka na?"
        );

        yield return new WaitForSeconds(2f);

        DialogueManager.Instance.Say(
            "Nababaliw na ba ako?!"
        );

        yield return new WaitForSeconds(2.5f);

        ObjectiveManager.Instance.SetObjective(
            "MangNestor",
            "Go to Mang Nestor's house."
        );
    }

    IEnumerator MangNestorArrival()
    {
        ObjectiveManager.Instance.CompleteObjective();

        yield return new WaitForSeconds(2f);

        DialogueManager.Instance.Say(
            "Mang Nestor?"
        );

        yield return new WaitForSeconds(3f);

        DialogueManager.Instance.Say(
            "Mukhang bukas naman..."
        );

        yield return new WaitForSeconds(2f);

        ObjectiveManager.Instance.SetObjective(
            "EnterHouse",
            "Go inside Mang Nestor's house."
        );
    }

    IEnumerator HouseInside()
    {
        ObjectiveManager.Instance.CompleteObjective();

        yield return new WaitForSeconds(2f);

        DialogueManager.Instance.Say(
            "Mang Nestor?"
        );

        yield return new WaitForSeconds(2.5f);

        PhoneManager.Instance.ReceiveMessage(
            "Nandiyan ba siya?"
        );

        NotificationManager.Instance.ShowNotification(
            "Nandiyan ba siya?"
        );

        yield return new WaitForSeconds(3f);

        DialogueManager.Instance.Say(
            "Wala dito..."
        );

        yield return new WaitForSeconds(2f);

        PhoneManager.Instance.ReceiveMessage(
            "Hanapin mo nga."
        );

        NotificationManager.Instance.ShowNotification(
            "Hanapin mo nga."
        );

        yield return new WaitForSeconds(2f);

        ObjectiveManager.Instance.SetObjective(
            "SearchHouse",
            "Look around the house."
        );
    }
}