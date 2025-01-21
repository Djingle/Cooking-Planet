using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using static UnityEngine.CullingGroup;

public class PlanetManager : MonoBehaviour
{
    public static PlanetManager Instance { get; private set; }
    private Rigidbody m_rigidbody;
    public float m_speed = 5;

    private void Awake()
    {
        // Keep the Cameraman when loading new scenes
        DontDestroyOnLoad(gameObject);

        // Singleton checks
        if (Instance == null) { // If there is no instance of Cameraman yet, then this one becomes the only instance
            Instance = this;
        } else {                // If a Cameraman instance already exists, destroy the new one
            Debug.LogWarning("Cameraman Instance already exists, destroying the duplicate");
            Destroy(gameObject);
            return;
        }

        GameManager.InputPressed += OnInputPressed;
    }

    private void Start()
    {
        m_rigidbody = GetComponent<Rigidbody>();
    }

    private void OnDestroy()
    {
        GameManager.InputPressed -= OnInputPressed;
    }

    void OnInputPressed(Vector2 input)
    {
        Vector3 torque = new Vector3(-input.y * m_speed, input.x * m_speed, 0);
        //Debug.Log("torque : " + torque);
        m_rigidbody.AddTorque(torque);
    }
}
