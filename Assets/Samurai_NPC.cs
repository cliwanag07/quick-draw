using System.Collections;
using UnityEngine;

public class Samurai_NPC : Samurai {
    // Singleton instance to allow easy access to the NPC samurai
    public static Samurai_NPC Instance { get; private set; }

    // Delay before the NPC attacks after the draw phase starts
    private float attackDelay;

    // Ensures the attack coroutine only starts once per round
    private bool hasStartedAttack = false;
    
    private void Awake() {
        // Ensure only one instance of Samurai_NPC exists
        if (Instance == null) {
            Instance = this;
        } else {
            Destroy(gameObject); // Destroy duplicate instances
        }
    }

    protected override void Start() {
        base.Start(); // Call base class Start() for initial setup
        
        // Randomize the attack delay between 0.1s and 0.3s
        attackDelay = UnityEngine.Random.Range(0.1f, 0.3f);
    }

    private void Update() {
        // If the draw phase starts and the attack hasn't started yet
        if (!hasStartedAttack && GameInstance.Instance.GetPhase() == Phase.Draw) {
            hasStartedAttack = true; // Mark that attack has started
            StartCoroutine(WaitToAttack()); // Start attack coroutine
        }
    }

    // Coroutine to wait for attackDelay before attacking
    private IEnumerator WaitToAttack() {
        yield return new WaitForSeconds(attackDelay);
        Attack(); // Trigger attack after delay
    }
}