using UnityEngine;

#if UNITY_UI
using UnityEngine.UI;
#endif

/// <summary>
/// Rice grain particle with physics simulation
/// Converted from Godot RiceParticle.gd for Unity 6.2+
/// Compatible with Unity 6.x versions
/// </summary>
public class RiceParticle : MonoBehaviour
{
    [Header("Physics")]
    private Vector2 velocity = Vector2.zero;
    private float rotationSpeed = 0f;
    private float lifetime = 1.8f;
    private float gravity = 400f;
    private float airResistance = 0.98f;
    private float elapsedTime = 0f;

    [Header("Visual")]
    #if UNITY_UI
    public UnityEngine.UI.Image particleImage;  // Or use SpriteRenderer
    #else
    public SpriteRenderer particleSprite;  // Fallback to SpriteRenderer
    #endif

    void Start()
    {
        // Random rotation speed
        rotationSpeed = UnityEngine.Random.Range(-4f, 4f);
        
        // Random size variation
        float sizeVariation = UnityEngine.Random.Range(0.85f, 1.15f);
        transform.localScale = Vector3.one * sizeVariation;

        // Set pure white color
        #if UNITY_UI
        if (particleImage != null)
        {
            particleImage.color = Color.white;
        }
        #else
        if (particleSprite != null)
        {
            particleSprite.color = Color.white;
        }
        #endif

        // Auto-destroy after lifetime
        Destroy(gameObject, lifetime);
    }

    void Update()
    {
        elapsedTime += Time.deltaTime;

        // Apply physics
        velocity.y += gravity * Time.deltaTime;
        velocity *= airResistance;

        // Update position (convert to world space)
        Vector3 movement = new Vector3(velocity.x, velocity.y, 0) * Time.deltaTime;
        transform.position += movement;

        // Rotate
        transform.Rotate(0, 0, rotationSpeed * Time.deltaTime * 57.3f); // Convert to degrees

        // Fade out (stays opaque for 70% of lifetime)
        float fadeStart = lifetime * 0.7f;
        if (elapsedTime > fadeStart)
        {
            float fadeProgress = (elapsedTime - fadeStart) / (lifetime - fadeStart);
            float alpha = 1f - (fadeProgress * 0.6f);  // Only fade to 40% min

            #if UNITY_UI
            if (particleImage != null)
            {
                Color col = particleImage.color;
                col.a = alpha;
                particleImage.color = col;
            }
            #else
            if (particleSprite != null)
            {
                Color col = particleSprite.color;
                col.a = alpha;
                particleSprite.color = col;
            }
            #endif
        }
    }

    public void SetVelocity(Vector2 vel)
    {
        velocity = vel;
    }
}

