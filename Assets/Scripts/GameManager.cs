using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static UnityEngine.CullingGroup;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }
    public Vector2 PlayerInput {  get; private set; }
    public const float k_mvtThreshold = .1f;
    public static Action<Vector2> InputPressed;
    public static Action<Vector2> InputChanged;

    Vector2 newPlayerInput;

    private void Awake()
    {
        // Keep the Cameraman when loading new scenes
        DontDestroyOnLoad(gameObject);

        // Singleton checks
        if (Instance == null) { // If there is no instance of Cameraman yet, then this one becomes the only instance
            Instance = this;
        } else {                // If a Cameraman instance already exists, destroy the new one
            Debug.LogWarning(gameObject.name + " instance already exists, destroying the duplicate");
            Destroy(gameObject);
            return;
        }
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        newPlayerInput = new Vector2(UnityEngine.Input.GetAxisRaw("Horizontal"), UnityEngine.Input.GetAxisRaw("Vertical"));
        if (newPlayerInput != PlayerInput) InputChanged?.Invoke(newPlayerInput);
        PlayerInput = newPlayerInput;

        if (PlayerInput.x <= k_mvtThreshold &&
            PlayerInput.x >= -k_mvtThreshold &&
            PlayerInput.y <= k_mvtThreshold &&
            PlayerInput.y >= -k_mvtThreshold) {
            return;
        }
        
        PlayerInput.Normalize();
        InputPressed?.Invoke(PlayerInput);
    }
}
