using UnityEngine;

public class Billboard : MonoBehaviour
{
    Camera cam;
    Vector3 baseScale;

    [Header("Auto-escala")]
    public float sizePerMeter = 0.06f;   // controla tamaño según distancia
    public float minScale = 0.6f;
    public float maxScale = 2.5f;

    void Start()
    {
        baseScale = transform.localScale;
    }

    void LateUpdate()
    {
        if (!cam) cam = Camera.main;
        if (!cam) return;

        // Hacer que mire a la cámara
        transform.rotation = Quaternion.LookRotation(transform.position - cam.transform.position);

        // Mantener tamaño legible
        float dist = Vector3.Distance(transform.position, cam.transform.position);
        float k = Mathf.Clamp(dist * sizePerMeter, minScale, maxScale);
        transform.localScale = baseScale * k;
    }
}
