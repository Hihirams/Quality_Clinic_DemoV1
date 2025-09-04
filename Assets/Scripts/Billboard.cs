using UnityEngine;

public class Billboard : MonoBehaviour
{
    Camera cam;
    Vector3 baseScale;

    [Header("Auto-escala")]
    public float sizePerMeter = 0.03f;
    public float minScale = 0.3f, maxScale = 2.5f;

    void Start() => baseScale = transform.localScale;

    void LateUpdate()
    {
        if (!cam) cam = Camera.main;
        if (!cam) return;

        // mirar a cámara
        transform.rotation = Quaternion.LookRotation(transform.position - cam.transform.position);

        // mantener tamaño aparente
        float dist = Vector3.Distance(transform.position, cam.transform.position);
        float k = Mathf.Clamp(dist * sizePerMeter, minScale, maxScale);
        transform.localScale = baseScale * k;
    }
}
