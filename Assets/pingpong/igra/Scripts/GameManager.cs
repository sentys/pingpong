
using System.Collections;
using TMPro;
using UnityEngine;
using Random = UnityEngine.Random;

namespace pingpong.igra
{
    public class GameManager : MonoBehaviour
    {
        #region Public
        [Header("Values")]
        public float movementSpeed = 10;
        public float ballStartSpeed = 5;
        public float ballAcceleration = 0.2f;
        public bool hideLevelInMainMenu;
        public InputType inputType;


        public SaveProvider saveProvider = SaveProvider.PlayerPrefs;

        [Header("Audio")]
        public AudioClip highScoreClip;    
        public AudioClip gameOverClip;    
        public AudioSource audioSource;
        
        [Header("References")]
        // Список экранов, 0 - главное меню, 1 - худ, 2 - гейм овер
        public GameObject[] gameScreens;
        public GameObject levelHolder;
        public Transform ball;
        public Transform player;
        public TextMeshProUGUI[] scoreTexts;
        public TextMeshProUGUI highScoreText;
        public TextMeshProUGUI mainMenuHighScoreText;
        #endregion
        
        #region Private
        private int _score;
        private int _highScore;
        private bool _isGameOver = true;
        private bool _achievedHighScore;
        private Rigidbody2D _ballRb;
        private string _originalHighScoreText;
        #endregion
    
        #region Enums
        public enum SaveProvider
        {
            PlayerPrefs,
            UltimateSaveAndLoad
        }

        public enum InputType
        {
            Keyboard,
            Rotate,
            Slider
        }
        #endregion

        private void Awake()
        {
            _ballRb = ball.GetComponent<Rigidbody2D>();
            
            // Получить текст рекорда(HighScore) чтобы изменить {score} в ручную
            _originalHighScoreText = mainMenuHighScoreText.text;
            
            ChangeScreen(0);
            
            // Загрузить рекорд
            if (saveProvider == SaveProvider.PlayerPrefs)
            {
                _highScore = PlayerPrefs.GetInt("HighScore", 0);
            }
            else
            {
                // Место для кода UltimateSaveAndLoad, если добавите позже
            }

            // Обновить рекорд
            highScoreText.text = _highScore.ToString();
            mainMenuHighScoreText.text = _originalHighScoreText.Replace("{score}", _highScore.ToString());
        }

        public void PlayGame()
        {
            StartCoroutine(Play());
        }

        private void Update()
        {
            if (!_isGameOver)
            {
                // Управление
                if (inputType == InputType.Keyboard)
                {
                    player.Rotate(new Vector3(0, 0, movementSpeed * Input.GetAxis("Horizontal") * Time.fixedDeltaTime));
                }
                else if (inputType == InputType.Rotate) 
                {
                    Vector3 mousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
                    Vector2 target = new Vector2(
                        mousePosition.x - transform.position.x,
                        mousePosition.y - transform.position.y
                    );
                    player.up = target;
                }
                else
                {
                    Vector3 mousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
                    Vector3 target = new Vector3(0,0, transform.position.x - mousePosition.x);
                    player.eulerAngles = target * 60;
                }
            }
        }

        private void FixedUpdate()
        {
            // Движение шара
            if (!_isGameOver)
            {
                float ballAccel = ballAcceleration * Time.fixedDeltaTime;
                _ballRb.linearVelocity += new Vector2(Mathf.Sign(_ballRb.linearVelocity.x) * ballAccel,
                    Mathf.Sign(_ballRb.linearVelocity.y) * ballAccel);
            }
        }

        // Добавление очков
        public void AddScore(int value)
        {
            _score += value;

            //Update all the score texts
            for (int i = 0; i < scoreTexts.Length; i++)
            {
                scoreTexts[i].text = _score.ToString();
            }

            // Проверка рекорда
            if (_score > _highScore)
            {
                _highScore = _score;

                // Поскольку оно вызывается каждый раз когда Score выше HighScore нам не нужен звук для воспроизведения каждый раз
                if (!_achievedHighScore)
                {
                    _achievedHighScore = true;
                    audioSource.clip = highScoreClip;
                    audioSource.Play();
                }

                highScoreText.text = _highScore.ToString();
                
                // Сохранение рекорда   
                if (saveProvider == SaveProvider.PlayerPrefs)
                {
                    PlayerPrefs.SetInt("HighScore", _highScore);
                    PlayerPrefs.Save();
                }
                else
                {
                    // Место для кода UltimateSaveAndLoad, если добавите позже
                }
            }
        }
        
        // Вызывается шаром когда он проходит через барьер
        public void GameOver()
        {
            if (!_isGameOver)
            {
                // Гейм овер
                ChangeScreen(2);
                _isGameOver = true;

                // Обновление основного текста рекорда
                mainMenuHighScoreText.text = _originalHighScoreText.Replace("{score}", _highScore.ToString());
                
                // Звук гейм овера
                audioSource.clip = gameOverClip;
                audioSource.Play();
            }
        }

        public void ChangeScreen(int number)
        {
            for (int i = 0; i < gameScreens.Length; i++)
            {
                // Включаем целевой экран, если его номер равен требуемому номеру экрана
                gameScreens[i].SetActive(i == number);
            }

            // Если загружается главное меню
            if (number == 0)
            {
                _isGameOver = true;
                ResetBall();
            }

            // Если эта опция включена, скрывает отображение уровня в главном меню
            if (hideLevelInMainMenu)
            {
                levelHolder.SetActive(number > 0);
            }
        }

        // Сбрасывает уровень и ожидает определенное время при сбросе
        IEnumerator Play()
        {
            // Сброс всех значений матча
            _score = 0;
            _isGameOver = false;
            _achievedHighScore = false;
            AddScore(0); // Это не добавляет очков, так как мы передаем 0, но обновляет тексты
            ResetBall();

            // Включает экран HUD
            ChangeScreen(1);

            // Ожидание перед добавлением случайной силы мячу
            yield return new WaitForSecondsRealtime(0.5f);
            _ballRb.linearVelocity = new Vector2(ballStartSpeed * (Random.Range(0, 2) == 0 ? 1 : -1), ballStartSpeed * (Random.Range(0, 2) == 0 ? 1 : -1));
        }

        private void ResetBall()
        {
            // Сброс позиции мяча
            if (_ballRb != null)
            {
                _ballRb.linearVelocity = Vector2.zero;
            }
            ball.transform.position = Vector3.zero;
        }
    }
}
