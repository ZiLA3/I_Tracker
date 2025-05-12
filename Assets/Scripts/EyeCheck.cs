using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class EyeCheck : MonoBehaviour
{
    [SerializeField] Vector3 eyePointOffset;
    [SerializeField] Vector3 handPointOffset;

    [SerializeField] Transform leftEye;
    [SerializeField] Transform rightEye;
    [SerializeField] Transform[] handPoints;
    
    const int eyePointCounts = 2;
    const int handPointCounts = 21;
    
    // Test
    Interactable currentInteractable;
    [SerializeField] float sphereCastRadius = 0.2f;
    [SerializeField] float maxDistance = 20f;
    [SerializeField] Vector3 drawGizmosOffset;
    [SerializeField] bool drawGizmos = true;
    HashSet<Transform> interactedObjects;

    private void Start()
    {
        interactedObjects = new HashSet<Transform>();
        currentInteractable = null;
    }

    private void Update() 
    {
        InteractCheck();

        if (Input.GetKeyDown(KeyCode.Space)) 
        {
            if(currentInteractable != null)
            {
                currentInteractable.Interact(); 
                interactedObjects.Add(currentInteractable.transform.root);
            }
        }

        Debug.Log(currentInteractable);
    }

    // 조건 좌우 눈이 바라보는 대상 일치
    // 이미 상호작용한 오브젝트는 제외

    private void InteractCheck() 
    {
        RaycastHit leftEyeHit;
        RaycastHit rightEyeHit;

        bool[] hitObject = { false, false }; // 0: left, 1: right


        if (Physics.SphereCast(leftEye.position, sphereCastRadius, leftEye.forward, out leftEyeHit, maxDistance))
        {
            if (leftEyeHit.transform.root.GetComponent<Interactable>()) 
            {
                hitObject[0] = true;
            }
        }
           
        if (Physics.SphereCast(rightEye.position, sphereCastRadius, rightEye.forward, out rightEyeHit, maxDistance))
        {
            if (rightEyeHit.transform.root.GetComponent<Interactable>())
            {
                hitObject[1] = true;
            }
        }

        if (hitObject[0] && hitObject[1])
        {
            if (leftEyeHit.transform.root == rightEyeHit.transform.root)
            {
                if (!interactedObjects.Contains(leftEyeHit.transform.root))
                {
                    currentInteractable = leftEyeHit.transform.root.GetComponent<Interactable>();
                    return;
                }
            }
        }

        currentInteractable = null;
    }

    private void OnDrawGizmos()
    {
        if (!drawGizmos)
            return;
        
        Gizmos.color = Color.magenta;

        Gizmos.DrawWireSphere(leftEye.position + drawGizmosOffset, sphereCastRadius);
        Gizmos.DrawWireSphere(rightEye.position + drawGizmosOffset, sphereCastRadius);
    }
}
