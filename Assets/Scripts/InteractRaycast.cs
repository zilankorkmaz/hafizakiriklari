using UnityEngine;

public class InteractRaycast : MonoBehaviour
{
    public float interactDistance = 10f;

    void Update()
    {
        if(Input.GetKeyDown(KeyCode.E))
        {
            Debug.Log("E tuşuna basıldı");
        }
        Ray ray = new Ray(transform.position, transform.forward);

        RaycastHit hit;

        Debug.DrawRay(ray.origin, ray.direction * interactDistance, Color.red);

        if (Physics.Raycast(ray, out hit, interactDistance))
        {
            Debug.Log("Çarpılan obje: " + hit.collider.name);
            
        }

    }
}
