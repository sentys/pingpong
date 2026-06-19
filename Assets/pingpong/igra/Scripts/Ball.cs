
using UnityEngine;

namespace pingpong.igra
{
    public class Ball : MonoBehaviour
    {
        public GameManager gameManager;
        public AudioSource hitAudioSource;
        
        // Вызывается когда шарик проходит через триггер границы
        private void OnTriggerEnter2D(Collider2D col)
        {
            if (col.CompareTag("Border"))
            {
                gameManager.GameOver();
            }
        }

        // Добавляет одно очко каждый раз когда мяч бьёт игрока
        private void OnCollisionEnter2D(Collision2D col)
        {
            hitAudioSource.Play();
            
            if (col.collider.CompareTag("Player"))
            {
                gameManager.AddScore(1);
            }
        }
    }
}