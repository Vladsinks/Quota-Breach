using UnityEngine;

public class Interact : MonoBehaviour
{
    [SerializeField] private float distance = 10f;  
    [SerializeField] private GameObject Player; 
    [SerializeField] private GameObject Aim; //типо прицел 

    void Update()
    {
        Ray ray = new Ray(transform.position, transform.forward);
        RaycastHit hit;

        if (Physics.Raycast(ray, out hit, distance))
        {
            if (hit.collider.CompareTag("Scrap") || hit.collider.CompareTag("Door"))
            {
                Aim.SetActive(true);

                if (Input.GetKey(KeyCode.E) && hit.collider.CompareTag("Scrap"))
                {
                    Destroy(hit.collider.gameObject);
                    Player.GetComponentInChildren<InGameMenu>().Money += 1f;
                    Aim.SetActive(false);
                }

                if (Input.GetKey(KeyCode.E) && hit.collider.CompareTag("Door"))
                {
                    Destroy(hit.collider.gameObject);
                    Player.GetComponentInChildren<InGameMenu>().RoomCount += 1;
                    Aim.SetActive(false);
                }
            }
            else Aim.SetActive(false);
        }
    }
}
