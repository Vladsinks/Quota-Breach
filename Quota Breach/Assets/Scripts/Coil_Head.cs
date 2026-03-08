using UnityEngine;

public class Coil_Head : MonoBehaviour
{
    public Transform player;
    public float moveSpeed = 5f;
    public float rotationSpeed = 10f;

    public AudioClip[] _stopSounds; 
    public AudioSource _walkSound; 
    public AudioSource audioSource;  
    
    private Animator anim;
    private bool isVisible;
    private bool wasVisibleLastFrame; // Флаг для отслеживания момента остановки

    void Start() {
        anim = GetComponent<Animator>();
    }

    void Update() {
        if (!isVisible) {
            // ВРАГ ДВИЖЕТСЯ
            MoveAndRotate();
            wasVisibleLastFrame = false;
            _walkSound.UnPause(); 
        } 
        else {
            // ВРАГ ОСТАНОВИЛСЯ
            // Если в прошлом кадре мы еще двигались (не был виден), а сейчас видны
            if (!wasVisibleLastFrame) {
                StopAction();
            }
            wasVisibleLastFrame = true;
            _walkSound.Pause();
        }
    }

    void MoveAndRotate() {
        Vector3 direction = (player.position - transform.position).normalized;
        direction.y = 0;
        
        if (direction != Vector3.zero) {
            Quaternion lookRotation = Quaternion.LookRotation(direction);
            transform.rotation = Quaternion.Slerp(transform.rotation, lookRotation, Time.deltaTime * rotationSpeed);
        }

        transform.position += transform.forward * moveSpeed * Time.deltaTime;
    }

    void StopAction() {
        // Проигрываем анимацию один раз. 
        // "StopPose" — это название триггера или самой анимации
        anim.Play("Coil_Head_Head", 0, 0f); 

        AudioClip clip = _stopSounds[Random.Range(0, _stopSounds.Length)];
        audioSource.PlayOneShot(clip);
    }

    void OnBecameVisible() { isVisible = true; }
    void OnBecameInvisible() { isVisible = false; }
}
