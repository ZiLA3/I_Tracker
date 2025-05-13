using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class EyeCheck : MonoBehaviour
{
    [Header("Eye Point Offset")]
    [SerializeField] Vector3 transformOffset;
    [SerializeField] Vector3 handPointOffset;

    [Header("Eye Casting")]
    [SerializeField] float sphereCastRadius = 0.2f;
    [SerializeField] float maxDistance = 20f;
    [SerializeField] bool drawGizmos = true;
    [SerializeField] Vector3 drawGizmosOffset;

    Interactable currentInteractable;
    HashSet<Transform> interactedObjects;

    private void Start()
    {
        interactedObjects = new HashSet<Transform>();
        currentInteractable = null;
    }

    private void Update() 
    {
        transform.position = new Vector3(FaceLandmark.EyePoint.x, FaceLandmark.EyePoint.y) + transformOffset;

        InteractCheck();

        if (Input.GetKeyDown(KeyCode.Space)) 
        {
            if(currentInteractable != null)
            {
                currentInteractable.Interact(); 
                interactedObjects.Add(currentInteractable.transform.root);
            }
        }
    }

    // 조건 좌우 눈이 바라보는 대상 일치
    // 이미 상호작용한 오브젝트는 제외

    private void InteractCheck() 
    {
        RaycastHit leftEyeHit;


        if (Physics.SphereCast(transform.position, sphereCastRadius, transform.forward, out leftEyeHit, maxDistance))
        {
            if (leftEyeHit.transform.root.GetComponent<Interactable>()) 
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

        Gizmos.DrawWireSphere(transform.position + drawGizmosOffset, sphereCastRadius);
    }
}
