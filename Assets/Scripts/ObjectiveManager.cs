using TMPro;
using UnityEngine;
using System;
using DG.Tweening;
using System.Collections;


public class ObjectiveManager : MonoBehaviour
{
    public static ObjectiveManager Instance;

    public static Action<string> OnObjectiveCompleted;

    public TMP_Text objectiveTitleText;
    public TMP_Text objectiveText;

    private string currentObjectiveID;
    private string currentObjectiveText;
    bool completing;

    void Awake()
    {
        Instance = this;
    }

    public void SetObjective(string id, string text)
    {
        currentObjectiveID = id;
        currentObjectiveText = text;

        objectiveTitleText.text = "OBJECTIVE";
        objectiveText.text = text;
    }

    public bool IsCurrentObjective(string id)
    {
        return currentObjectiveID == id;
    }

    public void CompleteObjective()
    {
        if (completing)
            return;

        StartCoroutine(CompleteRoutine());
    }

    IEnumerator CompleteRoutine()
    {
        completing = true;

        string completedID = currentObjectiveID;

        objectiveTitleText.text = "OBJECTIVE COMPLETE";

        Transform t = objectiveTitleText.transform;

        t.DOKill();
        t.localScale = Vector3.one;

        Sequence seq = DOTween.Sequence();

        seq.Append(t.DOScale(1.15f, 0.15f));
        seq.Append(t.DOScale(1f, 0.15f));

        yield return new WaitForSeconds(2f);
        
        objectiveTitleText.text = "OBJECTIVE";
        objectiveText.text = "";

        currentObjectiveID = "";
        currentObjectiveText = "";

        completing = false;

        OnObjectiveCompleted?.Invoke(completedID);
    }
}