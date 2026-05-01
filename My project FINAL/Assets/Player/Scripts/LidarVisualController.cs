using UnityEngine;
using System.Collections;

public class LidarVisualController : MonoBehaviour
{
    [Header("Настройки материала")]
    public Material lidarMaterial;
    
    [Header("Обычное состояние")]
    public Color normalColor = Color.white;
    [ColorUsage(true, true)] // Включает HDR пикер в инспекторе
    public Color normalEmission = Color.black; 

    [Header("Состояние тревоги")]
    public Color dangerColor = Color.red;
    [ColorUsage(true, true)]
    public Color dangerEmission = Color.red * 4f; // Умножаем на 4 для яркости

    private Coroutine flashCoroutine;
    private bool isWarningActive = false;

    void Start()
    {
        // Включаем поддержку Emission на материале (на всякий случай)
        lidarMaterial.EnableKeyword("_EMISSION");
        ResetMaterial();
    }

    public void StartWarning()
    {
        if (!isWarningActive)
        {
            isWarningActive = true;
            flashCoroutine = StartCoroutine(FlashRoutine());
        }
    }

    public void StopWarning()
    {
        isWarningActive = false;
        if (flashCoroutine != null) StopCoroutine(flashCoroutine);
        ResetMaterial();
    }

    private void ResetMaterial()
    {
        lidarMaterial.SetColor("_Color", normalColor);
        lidarMaterial.SetColor("_EmissionColor", normalEmission);
    }

    IEnumerator FlashRoutine()
    {
        while (isWarningActive)
        {
            // Скорость мигания (10f)
            float t = (Mathf.Sin(Time.time * 10f) + 1f) / 2f;
            
            // Интерполяция основного цвета
            Color currentColor = Color.Lerp(normalColor, dangerColor, t);
            // Интерполяция свечения
            Color currentEmission = Color.Lerp(normalEmission, dangerEmission, t);

            lidarMaterial.SetColor("_Color", currentColor);
            lidarMaterial.SetColor("_EmissionColor", currentEmission);
            
            yield return null; 
        }
    }

    // Чтобы материал не остался красным после выхода из игры в редакторе
    void OnDisable()
    {
        ResetMaterial();
    }
}
