using UnityEngine;

public class BotController : MonoBehaviour
{
    [SerializeField] Transform m_Target;
    [SerializeField] Transform m_LegBone;
    [SerializeField] float m_LegTrackingSpeed;
    [SerializeField] float m_LegMaxTurnAngle;

    private void LateUpdate()
    {
        Quaternion currentLocalRotation = m_LegBone.localRotation; // Store local position
        m_LegBone.localRotation = Quaternion.identity; // Reset local position
        Vector3 targetLocalLookDir = m_LegBone.InverseTransformDirection(m_Target.position - m_LegBone.position); // Local direction vector
        targetLocalLookDir = Vector3.RotateTowards(Vector3.forward, targetLocalLookDir, Mathf.Deg2Rad * m_LegMaxTurnAngle, 0); // Angle limit
        Quaternion towardTarget = Quaternion.LookRotation(targetLocalLookDir, transform.up); // Local direction Quatertnion
        m_LegBone.rotation = Quaternion.Slerp(currentLocalRotation, towardTarget, 1 - Mathf.Exp(-m_LegTrackingSpeed * Time.deltaTime)); // Smoothing

    }
}
