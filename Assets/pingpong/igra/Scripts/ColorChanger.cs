
// Динамично меняет цвет фона

using System.Collections;
using UnityEngine;

namespace pingpong.igra
{
    public class ColorChanger : MonoBehaviour
    {
        public Color[] colors;
        public Camera cam;

        public float minWaitTime = 6, maxWaitTime = 12;
        public float smoothChange = 25;

        // Автоматически обновляется из кода
        private Color _selectedColor;
        private int _selectedColorNumber;

        private void Awake()
        {
            // Выбор цвета на первый из массива цветов
            _selectedColor = colors[0];

            // Запустить перезарядку смены цветов
            StartCoroutine(WaitBeforeChangingColors());
        }

        private void Update()
        {
            UpdateColors();
        }

        void ChangeColor()
        {
            // Выбирает рандомный цвет и запускает перезарядку
            _selectedColorNumber = Random.Range(0, colors.Length);
            StartCoroutine(WaitBeforeChangingColors());
        }

        IEnumerator WaitBeforeChangingColors()
        {
            // Ждёт рандомное время между минимальным и максимальным значением
            yield return new WaitForSecondsRealtime(Random.Range(minWaitTime, maxWaitTime));

            ChangeColor();
        }

        private float _vel;
        private float _time;
        void UpdateColors()
        {
            // Изменяет значение времени от 0 до 1 ссылаясь на значение SmoothChange
            _time = Mathf.SmoothDamp(0, 1, ref _vel, smoothChange * Time.fixedDeltaTime);
            if (_time >= 1) _time = 0; //Whenever the time reaches 1, reset it back to 0

            // Плавный переход
            _selectedColor = Color.Lerp(_selectedColor, colors[_selectedColorNumber], _time);

            // Новый цвет для камеры
            cam.backgroundColor = _selectedColor;
        }
    }
}