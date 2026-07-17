using DG.Tweening;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Monster : MonoBehaviour
{
    public float moveSpeed = 2f;
    public Transform respawnPoint;
    public Transform photoTarget;
    private Transform player;
    private bool chasing = false;
    public ParticleSystem smoke;
    public Renderer[] renderers;
    public static List<Monster> ActiveMonsters = new();

    public Transform model;
    private Vector3 originalLocalPos;
    private Tween twitchTween;
    public float floatHeight = 0.2f;
    public float floatSpeed = 2f;
    private float baseY;

    [Header("Twitch")]
    public float farTwitchStrength = 0.03f;
    public float nearTwitchStrength = 0.12f;

    public float farTwitchInterval = 0.35f;
    public float nearTwitchInterval = 0.08f;

    private float twitchTimer;
    private Vector3 visualOffset;

    void Start()
    {
        baseY = transform.position.y;
        originalLocalPos = model.localPosition;
    }

    void Update()
    {
        UpdateTwitch();

        if (!chasing || player == null)
            return;

        Vector3 target = player.position;
        target.y = baseY;

        transform.position = Vector3.MoveTowards(
            transform.position,
            target,
            moveSpeed * Time.deltaTime);

        Vector3 pos = transform.position;
        //pos.y = baseY + Mathf.Sin(Time.time * floatSpeed) * floatHeight;

        transform.LookAt(player);

    }

    void UpdateTwitch()
    {
        float strength;
        float interval;

        if (!chasing || player == null)
        {
            // Idle twitch
            strength = 0.015f;
            interval = 0.6f;
        }
        else
        {
            float distance = Vector3.Distance(transform.position, player.position);
            float t = Mathf.InverseLerp(6f, 1.5f, distance);

            strength = Mathf.Lerp(0.02f, 0.08f, t);
            interval = Mathf.Lerp(0.35f, 0.08f, t);
        }

        twitchTimer += Time.deltaTime;

        if (twitchTimer >= interval)
        {
            twitchTimer = 0;

            Vector3 randomOffset =
                 transform.right * Random.Range(-1f, 1f) * strength +
                 transform.up * Random.Range(-0.2f, 0.2f) * strength;

            randomOffset.y *= 0.3f;

            twitchTween?.Kill();
            model.localPosition = originalLocalPos;

            twitchTween = model
                .DOLocalMove(originalLocalPos + randomOffset, 0.06f)
                .SetLoops(2, LoopType.Yoyo)
                .SetEase(Ease.OutQuad);
        }
    }

    public void StartChasing(Transform target)
    {
        player = target;
        chasing = true;
    }

    public void StopChasing()
    {
        chasing = false;
        player = null;
    }
    void OnEnable()
    {
        ActiveMonsters.Add(this);
    }

    void OnDisable()
    {
        ActiveMonsters.Remove(this);
    }
    public void Disperse()
    {
        chasing = false;
        player = null;
        model.DOKill();       // Kill every tween on the model
        transform.DOKill();   // Kill every tween on the root
        smoke.transform.SetParent(null); // Smoke stays behind
        smoke.Play();
        smoke.transform.DOKill();
        Destroy(smoke.gameObject, 1f);

        StartCoroutine(DisappearRoutine());
    }

    IEnumerator DisappearRoutine()
    {
        float interval = 0.06f;

        for (int i = 0; i < 4; i++)
        {
            foreach (Renderer r in renderers)
                r.enabled = false;

            yield return new WaitForSeconds(interval);

            foreach (Renderer r in renderers)
                r.enabled = true;

            yield return new WaitForSeconds(interval);
        }

        foreach (Renderer r in renderers)
            r.enabled = false;

        yield return new WaitForSeconds(0.15f);
        transform.DOKill();
        Destroy(gameObject);
    }
}