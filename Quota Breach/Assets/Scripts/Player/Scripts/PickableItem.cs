using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
[RequireComponent(typeof(Collider))]
public class PickableItem : MonoBehaviour
{
    [Header("Item Settings")]
    public string itemName = "Item";
    public float throwForce = 10f;

    [Header("Hold Settings")]
    public Vector3 holdLocalPosition;
    public Vector3 holdLocalRotation;

    [Header("Highlight")]
    public Color highlightColor = Color.yellow;

    private Rigidbody rb;
    private Collider col;
    private Renderer rend;
    private Color[] originalColors;

    private void Awake()
    {
        rb = GetComponent<Rigidbody>();
        col = GetComponent<Collider>();
        rend = GetComponentInChildren<Renderer>();

        originalColors = new Color[rend.materials.Length];
        for (int i = 0; i < rend.materials.Length; i++)
            originalColors[i] = rend.materials[i].color;
    }

    public void SetHighlight(bool state)
    {
        if (state)
        {
            for (int i = 0; i < rend.materials.Length; i++)
                rend.materials[i].color = highlightColor;
        }
        else
        {
            for (int i = 0; i < rend.materials.Length; i++)
                rend.materials[i].color = originalColors[i];
        }
    }

    public void OnPickedUp(Transform hand)
    {
        rb.isKinematic = true;
        col.enabled = false;

        transform.SetParent(hand);
        transform.localPosition = holdLocalPosition;
        transform.localEulerAngles = holdLocalRotation;
    }

    public void OnThrown(Vector3 pos, Vector3 dir)
    {
        transform.SetParent(null);
        transform.position = pos;

        rb.isKinematic = false;
        col.enabled = true;

        rb.AddForce(dir.normalized * throwForce, ForceMode.VelocityChange);
    }
}

