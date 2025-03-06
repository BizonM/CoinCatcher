using DG.Tweening;
using UnityEngine;
using UnityEngine.Rendering.Universal;

public class Coin : MonoBehaviour
{
    [SerializeField] private int coinValue = 1;
    [SerializeField] private SpriteRenderer coinRenderer;
    [SerializeField] private CircleCollider2D circleCollider2D;
    [SerializeField] private LayerMask playerLayer;
    
    [SerializeField] Light2D coinLight;
    
    [SerializeField] private AudioSource coinAudioSource;
    [SerializeField] private AudioClip coinSound;
    

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if ((playerLayer.value & (1 << collision.gameObject.layer)) != 0)
        {
            CoinTaken();
        }
    }

    private void CoinTaken()
    {
        Sequence sequence = DOTween.Sequence();

        sequence
            .AppendCallback(() => circleCollider2D.enabled = false)
            .AppendCallback(() => GameEvents.Current.AddScore(coinValue))
            .AppendCallback(() => coinAudioSource.PlayOneShot(coinSound))
            .AppendCallback(() => coinRenderer.enabled = false)
            .Append(DOTween.To(() => coinLight.intensity, x => coinLight.intensity = x, 0f, 1f))
            .AppendCallback(() => Destroy(gameObject));

    }
}
