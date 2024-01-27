using System.Collections;
using UnityEngine;

[RequireComponent(typeof(Animator))]
public class SpeechBubble : MonoBehaviour
{
    private Animator animator;
    private SpriteRenderer sprite;

    private void Start()
    {
        animator = GetComponent<Animator>();
        sprite = GetComponent<SpriteRenderer>();
        sprite.enabled = false;
    }

    public void TalkForSeconds(float time)
    {
        if (animator.GetBool("Talking")) return;
        StartCoroutine(Talk(time));
    }

    private IEnumerator Talk(float time)
    {
        sprite.enabled = true;
        animator.SetBool("Talking", true);
        yield return new WaitForSeconds(time);
        animator.SetBool("Talking", false);
        yield return new WaitForSeconds(1f);
        sprite.enabled = false;
    }
}
