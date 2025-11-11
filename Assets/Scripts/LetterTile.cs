using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using TMPro;
using System.Collections;

/// <summary>
/// Letter tile component with click detection and animations
/// Converted from Godot LetterTile.gd for Unity 6.2+
/// Compatible with Unity 6.x versions
/// </summary>
public class LetterTile : MonoBehaviour, IPointerClickHandler, IPointerEnterHandler, IPointerExitHandler
{
    [Header("Properties")]
    public char Letter { get; private set; } = 'A';
    
    [Header("References")]
    public Image backgroundImage;
    public TextMeshProUGUI letterLabel;
    
    [Header("Settings")]
    public Vector2 tileSize = new Vector2(80, 80);
    
    // State
    private bool isSelected = false;
    private bool isFlying = false;
    private float originalRotation;
    private bool isClickable = true;

    // Colors
    private static readonly Color COLOR_NORMAL = new Color(0.96f, 0.93f, 0.84f, 1f);  // f5ecd7
    private static readonly Color COLOR_SELECTED = new Color(1f, 0.85f, 0.4f, 1f);    // ffd966
    private static readonly Color COLOR_TEXT = new Color(0.24f, 0.16f, 0.09f, 1f);     // 3d2817

    // Event
    public System.Action<LetterTile> OnTileClicked;

    void Start()
    {
        SetupVisual();
        originalRotation = UnityEngine.Random.Range(-15f, 15f);
        transform.rotation = Quaternion.Euler(0, 0, originalRotation);
    }

    public void SetLetter(char letter)
    {
        Letter = letter;
        if (letterLabel != null)
        {
            letterLabel.text = letter.ToString();
        }
    }

    public void SetClickable(bool clickable)
    {
        isClickable = clickable;
    }

    void SetupVisual()
    {
        if (backgroundImage != null)
        {
            backgroundImage.color = COLOR_NORMAL;
        }

        if (letterLabel != null)
        {
            letterLabel.text = Letter.ToString();
            letterLabel.fontSize = 42;
            letterLabel.color = COLOR_TEXT;
            letterLabel.alignment = TextAlignmentOptions.Center;
        }
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        // Check if clickable (VS mode uses this for AI tiles)
        if (!isClickable || isSelected || isFlying)
            return;

        OnTileClicked?.Invoke(this);
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        if (!isClickable || isSelected || isFlying)
            return;

        transform.localScale = Vector3.one * 1.1f;
        Cursor.SetCursor(null, Vector2.zero, CursorMode.Auto); // Can set custom cursor
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        if (!isClickable || isSelected || isFlying)
            return;

        transform.localScale = Vector3.one;
    }

    public void MarkAsSelected()
    {
        isSelected = true;
        if (backgroundImage != null)
        {
            backgroundImage.color = COLOR_SELECTED;
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
            if (backgroundImage != null)
            {
                Color col = backgroundImage.color;
                col.a = Mathf.Lerp(1f, 0.5f, t);
                backgroundImage.color = col;
            }

            elapsed += Time.deltaTime;
            yield return null;
        }

        gameObject.SetActive(false);
    }
}

