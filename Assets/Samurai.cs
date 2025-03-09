using UnityEngine;
using System.Collections;

public class Samurai : MonoBehaviour {
    // Starting, ending, and attack positions of the object
    [SerializeField] private Vector3 startPos;
    [SerializeField] private Vector3 endPos;
    [SerializeField] private Vector3 attackPos;

    // Curve controlling movement easing (e.g., ease-in, ease-out)
    [SerializeField] private AnimationCurve movementCurve;

    // Cross to display when attack is invalid
    [SerializeField] private GameObject cross;

    // Sprites for different states
    [SerializeField] private Sprite idleSprite;
    [SerializeField] private Sprite attackSprite;
    [SerializeField] private Sprite failSprite;

    // SpriteRenderer for changing sprites
    private SpriteRenderer spriteRenderer;

    // Duration of sliding and attacking
    private float slideDuration = 1.0f;
    private float attackDuration = 0.2f;

    // Ensures attack is only processed once per round
    private bool pressedDraw = false;

    // Called when the script starts
    protected virtual void Start() {
        // Get SpriteRenderer component
        spriteRenderer = GetComponent<SpriteRenderer>();
        if (spriteRenderer == null) {
            Debug.LogWarning("Samurai: No SpriteRenderer attached");
        }
        
        // Hide the cross initially
        cross?.SetActive(false);

        // Start moving from startPos to endPos over slideDuration using movementCurve
        StartCoroutine(MoveCoroutine(startPos, endPos, slideDuration));
    }
    
    // Coroutine to move the object from 'start' to 'end' over 'duration' using 'movementCurve'
    private IEnumerator MoveCoroutine(Vector3 start, Vector3 end, float duration) {
        float time = 0f; // Elapsed time tracker

        // Start at the correct position
        transform.position = start;

        while (time < duration) {
            // t is the normalized time (0 to 1)
            float t = time / duration;

            // Evaluate the curve to get easing value
            float curveValue = movementCurve.Evaluate(t);

            // Lerp between start and end positions based on curveValue
            transform.position = Vector3.LerpUnclamped(start, end, curveValue);

            // Increase time based on time passed since last frame
            time += Time.deltaTime;

            // Wait until next frame
            yield return null;
        }

        // Ensure object is exactly at end position at the end
        transform.position = end;
    }

    // Called when the player or AI initiates an attack
    protected void Attack() {
        if (pressedDraw) return; // Only allow attack once per round

        Debug.Log("attack!");

        switch (GameInstance.Instance.GetPhase()) {
            case Phase.Wait:
                // Invalid phase for attack — show cross and block further attempts
                cross?.SetActive(true);
                pressedDraw = true;
                break;

            case Phase.Draw:
                // Valid attack phase — process attack
                pressedDraw = true;
                GameInstance.Instance.SetGameFinish(); // Mark the round as finished
                
                // Switch to attack sprite
                spriteRenderer.sprite = attackSprite;

                // Move to attack position over attackDuration
                StartCoroutine(MoveCoroutine(endPos, attackPos, attackDuration));

                // Notify game instance if the attack was successful
                GameInstance.Instance.AttackedSuccessfully(this == Samurai_Player.Instance);
                break;
        }
    }

    // Called when the samurai is defeated
    public void Defeated() {
        // Switch to fail sprite
        spriteRenderer.sprite = failSprite;

        // Hide the cross
        cross?.SetActive(false);
    }
    
    // Reset samurai when game reset
    public virtual void Reset() {
        spriteRenderer.sprite = idleSprite;
        cross?.SetActive(false);
        pressedDraw = false;
        StartCoroutine(MoveCoroutine(startPos, endPos, slideDuration));
    }
}
