using System.Collections;
using UnityEngine;

public class CameraManager : MonoBehaviour
{
    public static CameraManager Instance { get; private set; }
    public const float k_MaxCamVerticalAngle = 25f;
    public const float k_MaxCamHorizontalAngle = 15f;
    [SerializeField] public const float k_CamRotationTime = 2f;
    public bool m_CamMovement = false;

    Coroutine m_LookAtCoroutine = null;

    
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
        GameManager.InputChanged += SetTarget;
    }

    private void OnDestroy()
    {

        GameManager.InputChanged -= SetTarget;
    }

    public IEnumerator SmoothMove(Quaternion baseRot, Quaternion targetRot)
    {
        float moveTime = 0;
        while (moveTime < Mathf.PI) {
            Debug.Log("moveTime 1 : " + moveTime);
            moveTime += Time.deltaTime * k_CamRotationTime;
            Debug.Log("moveTime 2 : " + moveTime);
            moveTime = Mathf.Clamp(moveTime, 0, Mathf.PI);
            Debug.Log("moveTime 3 : " + moveTime);
            float lerpTime = SmoothSin(moveTime);

            // Quaternion nextRot = Quaternion.Lerp(baseRot, targetRot, (k_CamRotationTime - moveTime) / k_CamRotationTime);
            Quaternion nextRot = Quaternion.Lerp(baseRot, targetRot, moveTime);

            transform.rotation = nextRot;
            yield return null;
        }
    }

    private void SetTarget(Vector2 input)
    {
        if (!m_CamMovement) return;
        if (m_LookAtCoroutine != null) StopCoroutine(m_LookAtCoroutine);
        m_LookAtCoroutine = StartCoroutine(SmoothMove(transform.rotation, Quaternion.Euler(new Vector3(-input.y * k_MaxCamVerticalAngle, input.x * k_MaxCamHorizontalAngle))));
    }

    public static float SmoothSin(float x)
    {
        return .5f * Mathf.Sin(x - Mathf.PI / 2f) + .5f;
    }
}
