using UnityEngine;
using UnityEngine.AI;

public class CoilHeadAI : MonoBehaviour
{
    public Transform player;

    [Header("Анимация остановки (один раз)")]
    public AnimationClip stopAnimation;

    private NavMeshAgent agent;
    private Animation anim;
    private bool hasPlayedStopAnim = false;

    void Start()
    {
        agent = GetComponent<NavMeshAgent>();

        anim = GetComponent<Animation>();
        if (anim == null)
            anim = gameObject.AddComponent<Animation>();

        if (stopAnimation != null)
        {
            anim.AddClip(stopAnimation, "StopAnim");
            anim.wrapMode = WrapMode.Once;
        }
    }

    void Update()
    {
        if (player == null) return;

        // Постоянно идём к игроку
        agent.SetDestination(player.position);

        // Проверяем скорость агента
        float speed = agent.velocity.magnitude;

        // Если движется
        if (speed > 0.1f)
        {
            hasPlayedStopAnim = false;
        }
        else
        {
            // Стоит — играем анимацию один раз
            if (!hasPlayedStopAnim && stopAnimation != null)
            {
                anim.Play("StopAnim");
                hasPlayedStopAnim = true;
            }
        }
    }
}
