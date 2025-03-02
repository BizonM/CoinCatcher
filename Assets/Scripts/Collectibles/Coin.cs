
using System.Collections;
using DG.Tweening;
using UnityEngine;
using UnityEngine.Rendering.Universal;

public class Coin : MonoBehaviour
{
    [SerializeField] private int coinValue = 1;
    [SerializeField] private SpriteRenderer coinRenderer;
    [SerializeField] private CircleCollider2D circleCollider2D;
    [SerializeField] Light2D coinLight;
    [SerializeField] private AudioSource coinAudioSource;
    [SerializeField] private AudioClip coinSound;
    

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            coinAudioSource.PlayOneShot(coinSound);
            GameManager.Instance.addScore(coinValue);
            StartCoroutine(LightOutAndDestroy());
        }
    }

    private IEnumerator LightOutAndDestroy()
    {
        DOTween.To(() => coinLight.intensity, x => coinLight.intensity = x, 0, 0.2f)
            .SetEase(Ease.InOutSine);
        coinRenderer.enabled = false;
        circleCollider2D.enabled = false;
        yield return new WaitForSeconds(0.2f);
        Destroy(gameObject);
    }
}
