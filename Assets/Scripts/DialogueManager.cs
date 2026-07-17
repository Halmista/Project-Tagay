using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using DG.Tweening;

public class DialogueManager : MonoBehaviour
{
    public static DialogueManager Instance;

    public TMP_Text dialogueText;
    public CanvasGroup dialogueCanvas;

    Queue<string> dialogueQueue = new();

    bool showing;

    void Awake()
    {
        if (Instance == null)
            Instance = this;
        else
            Destroy(gameObject);
    }

    public void Say(string line)
    {
        dialogueQueue.Enqueue(line);

        if (!showing)
            StartCoroutine(ProcessQueue());
    }

    IEnumerator ProcessQueue()
    {
        showing = true;

        while (dialogueQueue.Count > 0)
        {
            string line = dialogueQueue.Dequeue();

            dialogueText.text = line;

            dialogueCanvas.DOKill();
            dialogueCanvas.DOFade(1f, 0.3f);

            float duration =
                Mathf.Max(2f, line.Length * 0.06f);

            yield return new WaitForSeconds(duration);

            dialogueCanvas.DOFade(0f, 0.3f);

            yield return new WaitForSeconds(0.35f);
        }

        showing = false;
    }
}