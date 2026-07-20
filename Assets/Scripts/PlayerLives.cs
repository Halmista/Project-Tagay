using UnityEngine;

public class PlayerLives : MonoBehaviour
{
    public int maxHits = 1;

    int hitsRemaining;

    void Start()
    {
        hitsRemaining = maxHits;
    }

    public void ResetHits(int hits)
    {
        maxHits = hits;
        hitsRemaining = hits;
    }

    public bool TakeHit()
    {
        hitsRemaining--;
        return hitsRemaining <= 0;
    }
}