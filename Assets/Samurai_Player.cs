using UnityEngine;

public class Samurai_Player : Samurai {
    // Singleton instance to allow easy access to the player samurai
    public static Samurai_Player Instance { get; private set; }
    
    private void Awake() {
        // Ensure only one instance of Samurai_Player exists
        if (Instance == null) {
            Instance = this;
        } else {
            Destroy(gameObject); // Destroy duplicate instances
        }
    }

    private void Update() {
        // Check if the player presses the spacebar
        if (Input.GetKeyDown(KeyCode.Space)) {
            Attack(); // Trigger the attack action
        }
    }
}