using System.Collections;
using UnityEngine;

// Defines the different phases of the game
public enum Phase {
    Intro,
    Wait,
    Draw,
    Finished
}

public class GameInstance : MonoBehaviour {
    // Singleton instance for easy access
    public static GameInstance Instance { get; private set; }
    
    // Keeps track of the elapsed time since the game started
    private float elapsedTime;
    
    // The current phase of the game (Intro, Wait, Draw, Finished)
    private Phase phase;
    
    // The time at which the round should start (after the Intro phase)
    private float roundStartTime = 1.0f;
    
    // Random time for how long to wait in the Wait phase before transitioning to Draw
    private float randomWaitTime;

    // UI element to display an exclamation mark when the Draw phase starts
    [SerializeField] GameObject exclamation;

    private void Awake() {
        // Ensure only one instance of GameInstance exists
        if (Instance == null) {
            Instance = this;
        } else {
            Destroy(gameObject); // Destroy duplicate instances
        }
    }
    
    private void Start() {
        // Randomly pick a wait time between 2 and 5 seconds for the Wait phase
        randomWaitTime = UnityEngine.Random.Range(2.0f, 5.0f);
        
        // Hide the exclamation mark at the beginning
        exclamation.SetActive(false);
    }

    private void Update() {
        // Increment the elapsed time by the time passed since the last frame
        elapsedTime += Time.deltaTime;

        // Switch between game phases and call respective handling methods
        switch (phase) {
            case Phase.Intro:
                HandleIntroPhase();
                break;
            case Phase.Wait:
                HandleWaitPhase();
                break;
            case Phase.Draw:
                HandleDrawPhase();
                break;
            case Phase.Finished:
                HandleFinishedPhase();
                break;
        }
    }

    // Returns the current game phase
    public Phase GetPhase() {
        return phase;
    }

    // Ends the game and sets the phase to Finished
    public void SetGameFinish() {
        phase = Phase.Finished;
    }

    // Called when an attack is successful
    public void AttackedSuccessfully(bool isPlayer) {
        if (isPlayer) {
            // If the player attacked successfully, defeat the NPC
            Samurai_NPC.Instance.Defeated();
        } else {
            // If the NPC attacked successfully, defeat the player
            Samurai_Player.Instance.Defeated();
        }
        // Hide the exclamation mark after the attack
        exclamation.SetActive(false);
    }

    // Handles the logic for the Intro phase
    private void HandleIntroPhase() {
        // Check if the elapsed time is greater than the round start time
        if (elapsedTime > roundStartTime) {
            // Transition to the Wait phase
            phase = Phase.Wait;
            
            // Start the coroutine to handle the Wait phase (only called once when phase changes)
            StartCoroutine(OnWaitPhaseFinish());
        }
    }

    // Handles the logic for the Wait phase
    private void HandleWaitPhase() {
        // The Wait phase is managed by the coroutine, no additional logic needed here.
    }

    // Coroutine to transition from the Wait phase to the Draw phase
    private IEnumerator OnWaitPhaseFinish() {
        // Wait for a random amount of time before transitioning to the Draw phase
        yield return new WaitForSeconds(randomWaitTime);
        
        // Transition to the Draw phase after the wait is over
        phase = Phase.Draw;
        
        // Show the exclamation mark to signal the Draw phase
        exclamation.SetActive(true);
        
        // Log the transition to the Draw phase for debugging
        Debug.Log("Phase is now DRAW!");
    }

    // Handles the logic for the Draw phase
    private void HandleDrawPhase() {
        // Logic for the Draw phase is handled by player and NPC scripts.
        // Currently empty.
    }

    // Handles the logic for the Finished phase
    private void HandleFinishedPhase() {
        // Logic for the Finished phase is handled externally.
        // Currently empty.
    }
    
    // Reset function
    public void ResetGame() {
        elapsedTime = 0.0f;
        phase = Phase.Intro;
        randomWaitTime = UnityEngine.Random.Range(2.0f, 5.0f);
        exclamation.SetActive(false);

        Samurai_NPC.Instance.Reset();
        Samurai_Player.Instance.Reset();
    }
}
