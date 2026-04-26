using UnityEngine;
using System.Collections;

public class ChangeColor : MonoBehaviour
{
    private Color color;

    private IEnumerator Start() 
    {
        if(this.gameObject.CompareTag("Wall")) color = Color.grey;
        if(this.gameObject.CompareTag("Scrap")) color = Color.yellow;
        if(this.gameObject.CompareTag("Enemy")) color = Color.red;

        Renderer r = GetComponent<Renderer>();
        if (r == null) yield break;

        r.material.color = color;
        yield return new WaitForSeconds(1f);
        r.material.color = Color.black;
        Destroy(this); 
    }
}
