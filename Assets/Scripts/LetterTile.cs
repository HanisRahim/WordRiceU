using UnityEngine;
using System.Collections;

/// <summary>
/// Letter tile component with click detection and animations
/// Converted from Godot LetterTile.gd for Unity 6.2+
/// Simplified version using basic Unity components only
/// </summary>
public class LetterTile : MonoBehaviour
{
    [Header("Properties")]
    public char Letter { get; private set; } = 'A';
    
    [Header("References")]
    public SpriteRenderer backgroundSprite;
    public TextMesh letterText;
    
    [Header("Settings")]
    public Vector2 tileSize = new Vector2(80, 80);
    
    // State
    private bool isSelected = false;
    private bool isFlying = false;
    private float originalRotation;
    private bool isClickable = true;

    // Colors
    private static readonly Color COLOR_NORMAL = new Color(0.96f, 0.93f, 0.84f, 1f);
    private static readonly Color COLOR_SELECTED = new Color(1f, 0.85f, 0.4f, 1f);
    private static readonly Color COLOR_TEXT = new Color(0.24f, 0.16f, 0.09f, 1f);

    // Event
    public System.Action<LetterTile> OnTileClicked;

    void Start()
    {
        SetupVisual();
        originalRotation = Random.Range(-15f, 15f);
        transform.rotation = Quaternion.Euler(0, 0, originalRotation);
    }

    public void SetLetter(char letter)
    {
        Letter = letter;
        if (letterText != null)
        {
            letterText.text = letter.ToString();
        }
    }

    public void SetClickable(bool clickable)
    {
        isClickable = clickable;
    }

    void SetupVisual()
    {
        if (backgroundSprite != null)
        {
            backgroundSprite.color = COLOR_NORMAL;
        }

        if (letterText != null)
        {
            letterText.text = Letter.ToString();
            letterText.fontSize = 42;
            letterText.color = COLOR_TEXT;
            letterText.anchor = TextAnchor.MiddleCenter;
            letterText.alignment = TextAlignment.Center;
        }
    }

    // Basic Unity click detection (works without UI package)
    void OnMouseDown()
    {
        if (!isClickable || isSelected || isFlying)
            return;

        OnTileClicked?.Invoke(this);
    }

    void OnMouseEnter()
    {
        if (!isClickable || isSelected || isFlying)
            return;

        transform.localScale = Vector3.one * 1.1f;
    }

    void OnMouseExit()
    {
        if (!isClickable || isSelected || isFlying)
            return;

        transform.localScale = Vector3.one;
    }

    public void MarkAsSelected()
    {
        isSelected = true;
        
        if (backgroundSprite != null)
        {
            backgroundSprite.color = COLOR_SELECTED;
        }
        
        transform.rotation = Quaternion.identity;
        transform.localScale = Vector3.one * 1.05f;
    }

    public void PlayShakeAnimation()
    {
        StartCoroutine(ShakeCoroutine());
    }

    IEnumerator ShakeCoroutine()
    {
        Vector3 originalPos = transform.position;
        float duration = 0.3f;
        float elapsed = 0f;

        while (elapsed < duration)
        {
            float offset = Mathf.Sin(elapsed * 30f) * 10f;
            transform.position = originalPos + new Vector3(offset, 0, 0);
            elapsed += Time.deltaTime;
            yield return null;
        }

        transform.position = originalPos;
    }

    public IEnumerator FlyToSlot(Vector3 targetPosition)
    {
        isFlying = true;
        Vector3 startPos = transform.position;
        Quaternion startRot = transform.rotation;
        Vector3 startScale = transform.localScale;
        
        float duration = 0.6f;
        float elapsed = 0f;

        while (elapsed < duration)
        {
            float t = elapsed / duration;
            // Cubic ease in-out
            t = t < 0.5f ? 4f * t * t * t : 1f - Mathf.Pow(-2f * t + 2f, 3f) / 2f;

            transform.position = Vector3.Lerp(startPos, targetPosition, t);
            transform.rotation = Quaternion.Lerp(startRot, Quaternion.identity, t);
            transform.localScale = Vector3.Lerp(startScale, Vector3.one * 0.9f, t);
            
            // Fade out
            if (backgroundSprite != null)
            {
                Color col = backgroundSprite.color;
                col.a = Mathf.Lerp(1f, 0.5f, t);
                backgroundSprite.color = col;
            }
            if (letterText != null)
            {
                Color col = letterText.color;
                col.a = Mathf.Lerp(1f, 0.5f, t);
                letterText.color = col;
            }

            elapsed += Time.deltaTime;
            yield return null;
        }

        gameObject.SetActive(false);
    }
}
