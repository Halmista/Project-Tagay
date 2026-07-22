using TMPro;
using DG.Tweening;
using System.Collections;
using Unity.VisualScripting.Dependencies.Sqlite;
using UnityEngine;

public class StoryManager : MonoBehaviour
{
    public static StoryManager Instance;
    public FirstPersonController player;
    public GameObject jamiNPC;
    public Transform jamiExitPoint;

    public LightFlicker porchLight;

    public Light[] windowLights;

    public Light[] porchLights;

    public GameObject[] windowGlows;

    public LightFlicker entranceStreetLight;

    [Header("House Horror")]
    public GameObject oldManGhost;
    public LightFlicker houseLight;
    public GameObject roadBlock;

    [Header("Body Bag Encounter")]
    public GameObject bodyBag;
    //public Monster wormMonster;
    public ParticleSystem bodyBagSmoke;

    [Header("Jun Reveal")]
    public GameObject junCorpse;
    public Light spotlight;
    public LightFlicker endingSpotlight;

    [Header("Ending")]
    public CanvasGroup fadeCanvas;
    public Transform startingCheckpoint;

    public Transform currentCheckpoint;

    public Transform dogCheckpoint;
    public Monster wolfMonster;

    public TMP_Text endingText;

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
        Debug.Log("TriggerEvent: " + eventName);

        switch (eventName)
        {
            case "MangNestorArrival":
                StartCoroutine(MangNestorArrival());
                break;

            case "HouseInside":
                StartCoroutine(HouseInside());
                break;

            case "CrowdEncounter":
                StartCoroutine(CrowdEncounter());
                break;

            case "BodyBagEncounter":
                StartCoroutine(BodyBagEncounter());
                break;

            case "AfterBodyBag":
                StartCoroutine(AfterBodyBag());
                break;

            case "MarkHouseConversation":
                StartCoroutine(MarkHouseConversation());
                break;

            case "WolfEncounter":
                StartCoroutine(WolfEncounter());
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

            case "InspectHouse":
                StartCoroutine(HouseRevealSequence());
                break;

            case "PhotographBlockage2":
                StartCoroutine(Blockage2Sequence());
                break;

            case "PhotographCrowd":
                StartCoroutine(CrowdRevealSequence());
                break;

            case "PhotographTable":
                StartCoroutine(JunReveal());
                break;
        }
    }

    IEnumerator MoveSequence()
    {
        DialogueManager.Instance.Say(
            "Hindi na nga naka-duty pero late pa rin.");
        yield return new WaitForSeconds(1f);
        
        NotificationManager.Instance.ShowNotification(
           "Napasa mo na ba yung listahan mo?"
       , NotificationSender.Unknown);

        yield return new WaitForSeconds(2f);

        PhoneManager.Instance.ReceiveMessage(
            "Bukas ka na ba dadating? Haha.");

        NotificationManager.Instance.ShowNotification(
            "Bukas ka na ba dadating? Haha.",
            NotificationSender.Jun);

        yield return new WaitForSeconds(1f);
        player.SetMovement(false);
        ObjectiveManager.Instance.SetObjective(
            "OpenPhone",
            "Open your phone to check Jun's messages. [TAB]");
    }

    IEnumerator OpenPhoneSequence()
    {
        DialogueManager.Instance.Say(
           "Kulit nito ni Jun.");

        //yield return new WaitForSeconds(1f);
        //DialogueManager.Instance.Say(
        //   "Shet, nakalimutan ko yung powerbank pala.");
        
        //yield return new WaitForSeconds(1f);
       // DialogueManager.Instance.Say(
        //   "Ambilis pa naman ma-drain ng battery ng phone ko.");

        yield return new WaitForSeconds(1f);
        DialogueManager.Instance.Say(
            "Grabe hindi na nagbago, andilim pa rin dito.");

        StartCoroutine(entranceStreetLight.ScareFlicker());

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
            "Wag na lang pala, bumili na si Lisa.");

        NotificationManager.Instance.ShowNotification(
            "Wag na lang pala, bumili na pala si Lisa.", NotificationSender.Jun);

        yield return new WaitForSeconds(2f);

        DialogueManager.Instance.Say(
             "2% nabawas kaagad sa isang picture?. Tsk...");

        yield return new WaitForSeconds(4f);

        DialogueManager.Instance.Say(
            "Wala daw palang gusto si Jun. Pasensya na sa abala Jami.");
       
        yield return new WaitForSeconds(2f);
        
        DialogueManager.Instance.Say(
            "Jami: Sige lang, kita na lang tayo mamaya. Ingat sila sayo. Ser.");

        yield return new WaitForSeconds(2f);
        
        DialogueManager.Instance.Say(
            "Haha...");

        yield return new WaitForSeconds(3f);

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

        ObjectiveManager.Instance.SetObjective(
           "Move Forward",
           "Continue going to the next streetlight and then turn right.");

    }

    IEnumerator BlockageSequence()
    {
        player.SetMovement(false);

        DialogueManager.Instance.Say(
            "...");

        yield return new WaitForSeconds(1.5f);

        DialogueManager.Instance.Say(
            "Nawala?"
        );

        yield return new WaitForSeconds(2.5f);

        DialogueManager.Instance.Say(
            "Ano yun...?"
        );

        yield return new WaitForSeconds(2f);

        DialogueManager.Instance.Say(
            "Bakit may dugo...?"
        );
        player.SetMovement(true);
        yield return new WaitForSeconds(3f);

        PhoneManager.Instance.ReceiveMessage(
            "Parang wala namang problema.");

        NotificationManager.Instance.ShowNotification(
            "Parang wala namang problema.", NotificationSender.Jun);

        yield return new WaitForSeconds(3f);

        PhoneManager.Instance.ReceiveMessage(
            "Baka pagod lang yan...");

        NotificationManager.Instance.ShowNotification(
            "Baka pagod lang yan...", NotificationSender.Jun);

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
            "Na kina Mang Nestor ka na?"
        );

        NotificationManager.Instance.ShowNotification(
            "Na kina Mang Nestor ka na?"
        , NotificationSender.Jun);

        yield return new WaitForSeconds(2f);

        DialogueManager.Instance.Say(
            "Nababaliw na ba ako?!"
        );

        yield return new WaitForSeconds(2f);
        NotificationManager.Instance.ShowNotification(
           "Paalala lang, yung listahan."
       , NotificationSender.Unknown);

        yield return new WaitForSeconds(2f);
        NotificationManager.Instance.ShowNotification(
           "Bente mil din yon. Kahit yung small time."
       , NotificationSender.Unknown);

        yield return new WaitForSeconds(2.5f);

        ObjectiveManager.Instance.SetObjective(
            "MangNestor",
            "Go to Mang Nestor's house."
        );
    }

    IEnumerator MangNestorArrival()
    {
        ObjectiveManager.Instance.CompleteObjective();
        player.SetMovement(false);
        yield return new WaitForSeconds(2f);

        DialogueManager.Instance.Say(
            "Mang Nestor?"
        );

        yield return new WaitForSeconds(3f);

        DialogueManager.Instance.Say(
            "Bukas yung pinto niya..."
        );

        yield return new WaitForSeconds(2f);

        ObjectiveManager.Instance.SetObjective(
            "EnterHouse",
            "Go inside Mang Nestor's house."
        );
        player.SetMovement(true);
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
            "Nandiyan ba siya?",
            NotificationSender.Jun);

        yield return new WaitForSeconds(3f);

        DialogueManager.Instance.Say(
         "Wala siya dito, baka may pinuntahan."
        );

        yield return new WaitForSeconds(3f);

        PhoneManager.Instance.ReceiveMessage(
            "Pero iniwan niyang nakabukas yung pinto?"
        );

        NotificationManager.Instance.ShowNotification(
            "Pero iniwan niyang nakabukas yung pinto?",
            NotificationSender.Jun);
        
        yield return new WaitForSeconds(3f);

        DialogueManager.Instance.Say(
            "May punto ka."
        );

        yield return new WaitForSeconds(3f);

        ObjectiveManager.Instance.SetObjective(
            "InspectHouse",
            "Investigate. Take a picture of the room."
        );
    }

    IEnumerator HouseRevealSequence()
    {

        yield return new WaitForSeconds(2f);

        DialogueManager.Instance.Say(
            "Hala... anong nagyari?"
        );

        yield return new WaitForSeconds(2f);

        StartCoroutine(houseLight.ScareFlicker());
        
        Transform playerTransform = player.transform;

        Vector3 spawnPos =
            playerTransform.position +
            playerTransform.forward * 1.5f;

        spawnPos.y = playerTransform.position.y;

        oldManGhost.transform.position = spawnPos;
        Vector3 lookPosition = player.transform.position;
        lookPosition.y = oldManGhost.transform.position.y; // Keep him upright

        oldManGhost.transform.LookAt(lookPosition);
        oldManGhost.SetActive(true);

        yield return new WaitForSeconds(2f);
        DialogueManager.Instance.Say(
            "???: Bakit...Ariel..."
        );
        yield return new WaitForSeconds(2f);
        oldManGhost.SetActive(false);

        DialogueManager.Instance.Say(
            "Mang Nestor...?!"
        );

        yield return new WaitForSeconds(3f);

        PhoneManager.Instance.ReceiveMessage(
            "Pre, nandito na pala si Mang Nestor."
        );

        NotificationManager.Instance.ShowNotification(
            "Pre, nandito na pala si Mang Nestor.",
            NotificationSender.Jun);

        yield return new WaitForSeconds(3f);

        PhoneManager.Instance.ReceiveMessage(
            "Mukhang nagkasalisi lang kayo."
        );

        NotificationManager.Instance.ShowNotification(
            "Mukhang nagkasalisi lang kayo.",
            NotificationSender.Jun);

        yield return new WaitForSeconds(3f);

        DialogueManager.Instance.Say(
            "Na-malikmata lang ba ako...?"
        );
        
        yield return new WaitForSeconds(3f);
        roadBlock.SetActive(false);
        PhoneManager.Instance.ReceiveMessage(
            "Pwede ka bang kumatok kina Mark?"
        );

        NotificationManager.Instance.ShowNotification(
            "Pwede ka bang kumatok kina Mark?",
            NotificationSender.Jun);

        yield return new WaitForSeconds(3f);

        PhoneManager.Instance.ReceiveMessage(
            "Imbitahan mo rin"
        );

        NotificationManager.Instance.ShowNotification(
           "Imbitahan mo rin",
            NotificationSender.Jun);

        yield return new WaitForSeconds(3f);

        ObjectiveManager.Instance.SetObjective(
            "Blockage2",
            "Stop by Mark's house on the way to the party."
        );
    }

    IEnumerator Blockage2Sequence()
    {
        yield return new WaitForSeconds(1f);

        DialogueManager.Instance.Say(
            "Bakit andaming eskinita na may harang??"
        );

        yield return new WaitForSeconds(2.5f);

        NotificationManager.Instance.ShowNotification(
            "Good job. I-dedeposito namin yung pera bukas.",
         NotificationSender.Unknown);

        yield return new WaitForSeconds(2f);

        ObjectiveManager.Instance.SetObjective(
            "VisitMark",
            "Continue to Mark's house."
        );
    }

    IEnumerator CrowdEncounter()
    {
        player.SetMovement(false);

        DialogueManager.Instance.Say(
            "May mga tao..."
        );

        yield return new WaitForSeconds(2f);

        DialogueManager.Instance.Say(
            "??: May bagong nabiktima na naman..."
        );

        yield return new WaitForSeconds(2.5f);

        DialogueManager.Instance.Say(
            "??: Nakakatakot nang lumabas sa gabi."
        );

        yield return new WaitForSeconds(2.5f);

        DialogueManager.Instance.Say(
            "??: Kahit hindi ano, sinasalvage..."
        );

        yield return new WaitForSeconds(3f);

        DialogueManager.Instance.Say(
            "..."
        );


        yield return new WaitForSeconds(2f);

        player.SetMovement(true);

        PhoneManager.Instance.ReceiveMessage(
            "Pre, anong nangyari?"
        );

        NotificationManager.Instance.ShowNotification(
            "Pre, anong nangyari?",
            NotificationSender.Jun);

        yield return new WaitForSeconds(3f);

        ObjectiveManager.Instance.SetObjective(
            "PhotographCrowd",
            "Take a picture of the scene."
        );
    }

    IEnumerator CrowdRevealSequence()
    {
        yield return new WaitForSeconds(1f);

        DialogueManager.Instance.Say(
            "..."
        );

        yield return new WaitForSeconds(2f);

        DialogueManager.Instance.Say(
            "May na-salvage."
        );

        yield return new WaitForSeconds(2.5f);

        DialogueManager.Instance.Say(
            "Kailangan ko sigurong tawagan yung istasyon."
        );

        yield return new WaitForSeconds(2.5f);

        ObjectiveManager.Instance.SetObjective(
            "InspectBodyBag",
            "Approach the body bag."
        );
    }

    IEnumerator BodyBagEncounter()
    {
        player.SetMovement(false);

        DialogueManager.Instance.Say(
            "..."
            );

        bodyBag.transform.DOPunchRotation(
            new Vector3(0, 0, 8),
            0.3f,
            8,
            1);

        yield return new WaitForSeconds(1f);

        bodyBag.transform.DOShakeRotation(
            1.2f,
            25f,
            30);

        bodyBag.transform.DOShakePosition(
            1.2f,
            0.08f,
            30);

        bodyBag.transform.DOScale(1.15f, 0.8f)
            .SetEase(Ease.OutBack);

        yield return new WaitForSeconds(1.2f);

        DialogueManager.Instance.Say(
           "Puta. Bakit yan gumagalaw?"
           );
        
        yield return new WaitForSeconds(1.2f);

        DialogueManager.Instance.Say(
           "Baka hindi ito totoo katulad ng ibang nakita ko ngayong gabi..."
           );

        player.SetMovement(true);

        ObjectiveManager.Instance.SetObjective(
            "WalkPastBodyBag",
            "Walk past the body bag."
        );
    }

    IEnumerator AfterBodyBag()
    {
        yield return new WaitForSeconds(1f);

        DialogueManager.Instance.Say(
          "...Puta. Hindi nawala pero gumagalaw pa rin."
          );

        yield return new WaitForSeconds(1f);

        DialogueManager.Instance.Say(
            "Kalma, Ariel...ilusyon lang yun."
         );

        PhoneManager.Instance.ReceiveMessage(
            "Malapit na ba sa bahay ni Mark?"
        );

        NotificationManager.Instance.ShowNotification(
            "Malapit na ba sa bahay ni Mark?",
            NotificationSender.Jun);

        yield return new WaitForSeconds(3f);

        NotificationManager.Instance.ShowNotification(
            "Mukhang madali lang sayo mag-lista.",
         NotificationSender.Unknown);

        yield return new WaitForSeconds(2f);

        NotificationManager.Instance.ShowNotification(
            "Madami naman sa area niyo.",
         NotificationSender.Unknown);

        ObjectiveManager.Instance.SetObjective(
            "VisitMarkHouse",
            "Go to Mark's house."
        );
    }

    IEnumerator MarkHouseConversation()
    {
        player.SetMovement(false);

        yield return new WaitForSeconds(2f);

        DialogueManager.Instance.Say(
            "Ate Baby... si Mark po?"
        );

        yield return new WaitForSeconds(3f);

        DialogueManager.Instance.Say(
            "Birthday ni Jun... hinahanap po siya sa inuman."
        );

        yield return new WaitForSeconds(3f);

        DialogueManager.Instance.Say(
            "Ate Baby: ..."
        );

        yield return new WaitForSeconds(2f);

        DialogueManager.Instance.Say(
            "Ate Baby: Ariel..."
        );

        yield return new WaitForSeconds(2f);

        DialogueManager.Instance.Say(
            "Ate Baby: Matagal nang wala si Mark."
        );

        yield return new WaitForSeconds(1.5f);

        DialogueManager.Instance.Say(
            "Wala na si Mark??"
        );

        yield return new WaitForSeconds(2f);

        DialogueManager.Instance.Say(
            "Kelan pa po?"
        );

        yield return new WaitForSeconds(2f);

        // Conversation ends...
        yield return StartCoroutine(TurnOffHouseLights());

        yield return new WaitForSeconds(2f);

        DialogueManager.Instance.Say(
            "Ate Baby...?"
        );

        yield return new WaitForSeconds(2f);

        player.SetMovement(true);

        ObjectiveManager.Instance.CompleteObjective();

        yield return new WaitForSeconds(1f);

        PhoneManager.Instance.ReceiveMessage(
            "Pre... may dumating dito."
        );
        NotificationManager.Instance.ShowNotification(
            "Pre... may dumating dito.",
            NotificationSender.Jun);

        yield return new WaitForSeconds(2.5f);

        PhoneManager.Instance.ReceiveMessage(
            "Mga pulis..."
        );
        NotificationManager.Instance.ShowNotification(
            "Mga pulis...",
            NotificationSender.Jun);

        yield return new WaitForSeconds(2.5f);

        PhoneManager.Instance.ReceiveMessage(
            "Bakit nandito sila?"
        );
        NotificationManager.Instance.ShowNotification(
            "Bakit nandito sila?",
            NotificationSender.Jun);

        yield return new WaitForSeconds(3f);

        DialogueManager.Instance.Say(
            "Jun..."
        );

        yield return new WaitForSeconds(1.5f);

        // New objective
        ObjectiveManager.Instance.SetObjective(
            "GoToJunHouse",
            "Go."
        );

        StoryManager.Instance.TriggerEvent("AfterMarkHouse");
    }

    IEnumerator TurnOffHouseLights()
    {
        if (porchLight != null)
            yield return StartCoroutine(porchLight.ScareFlicker());

        // Turn off window lights one by one
        for (int i = 0; i < windowLights.Length; i++)
        {
            if (windowLights[i] != null)
                windowLights[i].enabled = false;

            if (i < windowGlows.Length && windowGlows[i] != null)
                windowGlows[i].SetActive(false);

            yield return new WaitForSeconds(0.15f);
        }

        // Turn off porch lights last
        foreach (Light light in porchLights)
        {
            if (light != null)
                light.enabled = false;
        }
    }
    /*
#if UNITY_EDITOR
    [ContextMenu("DEBUG - Dog Encounter")]
    public void DebugDogEncounter()
    {
        StopAllCoroutines();

        CharacterController cc = player.GetComponent<CharacterController>();

        if (cc != null)
            cc.enabled = false;

        player.transform.SetPositionAndRotation(
            dogCheckpoint.position,
            dogCheckpoint.rotation);

        if (cc != null)
            cc.enabled = true;

        ObjectiveManager.Instance.SetObjective(
            "PhotographWolf",
            "Drive away the creature using your camera."
        );

        wolfMonster.SetPhotographable(true);
        wolfMonster.StartChasing(player.transform);
    }
#endif
    */
    IEnumerator WolfEncounter()
    {
        Debug.Log("WolfEncounter coroutine started");
        DialogueManager.Instance.Say(
          "Parang binibisita niya yung mga pinto."
         );
        yield return new WaitForSeconds(1.5f);
        DialogueManager.Instance.Say(
          "Baka pwede ko siyang lagpasan."
         );
        // Give the player time to look at the dog.
        yield return new WaitForSeconds(2f);

        ObjectiveManager.Instance.SetObjective(
            "WalkPastWolf",
            "Carefully walk past the creature without using your camera."
        );
    }

    IEnumerator JunReveal()
    {
        player.SetMovement(false);

        yield return new WaitForSeconds(1.5f);

        DialogueManager.Instance.Say(
           "Jun: Antagal mo."
        );

        yield return new WaitForSeconds(2f);

        DialogueManager.Instance.Say(
            "Jun: Ayan na parte mo."
        );

        yield return new WaitForSeconds(2f);

        DialogueManager.Instance.Say(
            "Jun: Tagay."
        );

        yield return new WaitForSeconds(2f);


        // Flicker before Jun appears
        StartCoroutine(endingSpotlight.ScareFlicker());

        yield return new WaitForSeconds(0.8f);

        // Fade Jun in
        junCorpse.SetActive(true);
        
        //StartCoroutine(endingSpotlight.ScareFlicker());

        yield return new WaitForSeconds(3f);

        DialogueManager.Instance.Say(
            "..."
        );

        yield return new WaitForSeconds(2f);

        DialogueManager.Instance.Say(
            "Jun: Bakit, Ariel?"
        );

        yield return new WaitForSeconds(2.5f);
        
        DialogueManager.Instance.Say(
           "Jun: Bakit mo kami binenta?"
        );

        yield return new WaitForSeconds(2.5f);

        DialogueManager.Instance.Say(
            "Hindi..."
        );

        yield return new WaitForSeconds(2f);

        DialogueManager.Instance.Say(
            "Sorry! Hindi ko alam na magiging ganito!"
        );
        yield return new WaitForSeconds(2.5f);

        DialogueManager.Instance.Say(
            "Jun: Ariel. Alam mo."
        );
        yield return new WaitForSeconds(2f);
        
        DialogueManager.Instance.Say(
            "Jun: Alam mo kung anong mangyayari."
        );
        yield return new WaitForSeconds(4f);

        DialogueManager.Instance.Say(
            "Sorry Jun!"
        );

        yield return new WaitForSeconds(2f);
        StartCoroutine(endingSpotlight.ScareFlicker());

        DialogueManager.Instance.Say(
            "JUN!"
        );

        yield return new WaitForSeconds(2f);

        yield return StartCoroutine(EndingSequence());
        
    }

    IEnumerator EndingSequence()
    {
        player.SetMovement(false);


        CharacterController cc =
            player.GetComponent<CharacterController>();

        if (cc != null)
            cc.enabled = false;

        player.transform.SetPositionAndRotation(
            startingCheckpoint.position,
            startingCheckpoint.rotation);

        if (cc != null)
            cc.enabled = true;
        yield return new WaitForSeconds(1f);

        yield return StartCoroutine(
            IntroMessages.Instance.PlayIntro()
        );

        yield return new WaitForSeconds(1f);

        DialogueManager.Instance.Say(
            "Hindi..."
        );

        yield return new WaitForSeconds(2f);
       
        // Fade to black
        yield return fadeCanvas
            .DOFade(1f, 2f)
            .WaitForCompletion();

        endingText.gameObject.SetActive(true);
        endingText.text = "Hustisya.";

        yield return new WaitForSeconds(5f);

        Application.Quit();
    }
}