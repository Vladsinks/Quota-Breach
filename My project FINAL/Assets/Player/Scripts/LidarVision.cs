using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class TagMaterialPair
{
    public string tag;
    public Material material;
}

public class LidarVision : MonoBehaviour
{
    [Header("Radar Settings")]
    public float radarRange = 50f;
    public float spreadAngle = 30f; 
    public int raysPerFrame = 10; 
    public LayerMask detectionMask;

    [Header("Visualization")]
    public GameObject hitPointPrefab; 
    public Transform rayOrigin;
    public Color rayColor = Color.green; 

    [Header("Tag Materials")]
    public List<TagMaterialPair> tagMaterials = new List<TagMaterialPair>();
    public Material defaultMaterial; 

    [Header("Optimization")]
    public int maxPoints = 500; 
    public float pointLifeTime = 5f; 

    private Queue<GameObject> hitPoints = new Queue<GameObject>(); 

    void Update()
    {
        if (Input.GetMouseButton(0))
        {
            EmitRadarRays();
        }
        else if (Input.GetKeyDown(KeyCode.C))
        {
            ClearAllPoints();
        }
    }

    void EmitRadarRays()
    {
        for (int i = 0; i < raysPerFrame; i++)
        {
            
            Vector3 direction = GetRandomDirectionInCone(rayOrigin.forward, spreadAngle);

            Ray ray = new Ray(rayOrigin.position, direction);
            RaycastHit hit;

            if (Physics.Raycast(ray, out hit, radarRange, detectionMask))
            {
                Material materialToUse = GetMaterialForTag(hit.collider.tag);
                CreateHitPoint(hit.point, materialToUse);
            }

            
            Debug.DrawRay(ray.origin, ray.direction * radarRange, rayColor, 0.1f);
        }
    }

    Material GetMaterialForTag(string tag)
    {
        foreach (var pair in tagMaterials)
        {
            if (pair.tag == tag)
                return pair.material;
        }
        return defaultMaterial;
    }

    Vector3 GetRandomDirectionInCone(Vector3 baseDirection, float maxAngle)
    {
        
        float randomAngle = Random.Range(0f, maxAngle);
        Vector3 randomAxis = UnityEngine.Random.onUnitSphere;

       
        return Quaternion.AngleAxis(randomAngle, randomAxis) * baseDirection;
    }

    void CreateHitPoint(Vector3 position, Material material)
    {
        
        if (hitPoints.Count >= maxPoints)
        {
            Destroy(hitPoints.Dequeue());
        }

        
        GameObject hitPoint = Instantiate(hitPointPrefab, position, Quaternion.identity);

        
        if (material != null)
        {
            var renderer = hitPoint.GetComponent<Renderer>();
            if (renderer != null)
            {
                renderer.material = material;
            }
        }

        hitPoints.Enqueue(hitPoint);

        
        Destroy(hitPoint, pointLifeTime);
    }

    void ClearAllPoints()
    {
        while (hitPoints.Count > 0)
        {
            Destroy(hitPoints.Dequeue());
        }
    }
}