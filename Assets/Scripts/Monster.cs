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
    Animator animator;

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

    [Header("Photography")]
    public bool canBePhotographed = true;
    public int photosRemaining = 3;

    [Header("Attack")]
    public bool usesAttack = false;
    public float attackRange = 1.8f;
    public float attackCooldown = 2f;

    float attackTimer;
    bool attacking;

    
    void Start()
    {
        baseY = transform.position.y;
        originalLocalPos = model.localPosition;
        animator = model.GetComponent<Animator>();
    }

    void Update()
    {
        UpdateTwitch();

        if (!chasing || player == null)
            return;

        if (usesAttack)
        {
            attackTimer -= Time.deltaTime;

            float distance = Vector3.Distance(transform.position, player.position);

            if (distance <= attackRange)
            {
                if (!attacking && attackTimer <= 0f)
                    StartCoroutine(AttackRoutine());

                return;
            }
        }
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

        if (animator != null)
            animator.SetBool("Chasing", true);
    }

    public void StopChasing()
    {
        chasing = false;
        player = null;

        if (animator != null)
            animator.SetBool("Chasing", false);
    }
    public void SetPhotographable(bool value)
    {
        canBePhotographed = value;
    }
    void OnEnable()
    {
        ActiveMonsters.Add(this);
    }

    void OnDisable()
    {
        ActiveMonsters.Remove(this);
    }
    public bool Disperse()
    {
        if (!canBePhotographed)
            return false;

        photosRemaining--;
        Debug.Log("Monster photographed. Photos remaining: " + photosRemaining);
        // Monster survives
        if (photosRemaining > 0)
        {
            if (animator != null)
            {
                animator.SetInteger("HitStage", 3 - photosRemaining);
                animator.SetTrigger("Hit");
                animator.SetBool("Chasing", false);
            }

            chasing = false;

            StartCoroutine(ResumeChaseRoutine());

            moveSpeed += 0.5f;

            return false;
        }

        // Monster dies
        chasing = false;
        player = null;
        
        if (animator != null)
        {
            animator.SetBool("Chasing", false);
            animator.SetTrigger("Die");
        }

        StartCoroutine(DeathRoutine());
        return true;
    }

    IEnumerator ResumeChaseRoutine()
    {
        yield return new WaitForSeconds(0.8f);

        if (photosRemaining > 0)
        {
            chasing = true;

            if (animator != null)
                animator.SetBool("Chasing", true);
        }
    }
    IEnumerator DeathRoutine()
    {
        // Wait for the death animation
        yield return new WaitForSeconds(1.5f);

        model.DOKill();
        transform.DOKill();

        smoke.transform.SetParent(null);
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

    IEnumerator AttackRoutine()
    {
        attacking = true;
        chasing = false;

        if (animator != null)
        {
            animator.SetBool("Chasing", false);
            animator.SetTrigger("Attack");
        }

        yield return new WaitForSeconds(0.9f); // Match your attack animation length

        if (Vector3.Distance(transform.position, player.position) > attackRange)
        {
            attackTimer = attackCooldown;
            attacking = false;

            chasing = true;
            animator.SetBool("Chasing", true);

            yield break;
        }

        PlayerLives lives = player.GetComponent<PlayerLives>();

        if (lives != null)
        {
            if (lives.TakeHit())
            {
                CharacterController controller =
                    player.GetComponent<CharacterController>();

                if (controller != null)
                    controller.enabled = false;

                player.position = respawnPoint.position;
                player.rotation = respawnPoint.rotation;

                if (controller != null)
                    controller.enabled = true;

                StopChasing();

                yield break;
            }
        }

        attackTimer = attackCooldown;

        attacking = false;

        if (photosRemaining > 0)
        {
            chasing = true;
            animator.SetBool("Chasing", true);
        }
    }
}