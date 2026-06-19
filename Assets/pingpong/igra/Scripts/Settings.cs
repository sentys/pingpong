using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Audio;



namespace pingpong.igra
{
	public class Settings : MonoBehaviour
	{
		[Header("Sound")]
		public AudioMixer SoundsMixer;
		public string SoundMixerVolumeName = "Master";
		public int SoundVolume = 1;

		[Header("Sound UI")]
		public Image SoundImage;
		public Sprite SoundOnSprite;
		public Sprite SoundOffSprite;

		public GameManager gameManager;

		void Start()
		{
			//Получаем сохраненное состояние звука (Вкл/Выкл)
			if (gameManager.saveProvider == GameManager.SaveProvider.PlayerPrefs)
			{
				SoundVolume = PlayerPrefs.GetInt("Volume", SoundVolume);
			}
			else
			{

			}

			//Устанавливаем загруженную громкость в аудиомикшер, используя имя группы аудиомикшера.
			//Если загруженная громкость равна 1, то устанавливаем громкость микшера на 0 (макс)
			//Если загруженная громкость равна 0, то устанавливаем громкость микшера на -80 (мин)
			SoundsMixer.SetFloat(SoundMixerVolumeName, SoundVolume == 1 ? 0 : -80);

			//Наконец, обновляем кнопку звука
			UpdateSoundImage();
		}

		//Вызывается из кнопки звука
		public void ChangeVolume()
		{
			//Если SoundVolume равен 1 (включен), то устанавливаем его в 0 (выключен) и наоборот
			if (SoundVolume == 1)
				SoundVolume = 0;
			else
				SoundVolume = 1;

			//Устанавливаем загруженную громкость в аудиомикшер, используя имя группы аудиомикшера.
			//Если загруженная громкость равна 1, то устанавливаем громкость микшера на 0 (макс)
			//Если загруженная громкость равна 0, то устанавливаем громкость микшера на -80 (мин)
			SoundsMixer.SetFloat(SoundMixerVolumeName, SoundVolume == 1 ? 0 : -80);

			//Сохраняем новую громкость
			if (gameManager.saveProvider == GameManager.SaveProvider.PlayerPrefs)
			{
				PlayerPrefs.SetInt("Volume", SoundVolume);
			}
			else
			{

			}

			//Наконец, обновляем кнопку звука
			UpdateSoundImage();
		}

		void UpdateSoundImage()
		{
			//Если SoundVolume равен 1 (включен), то устанавливаем спрайт изображения на SoundOnSprite
			//Если SoundVolume не равен 1 (выключен), то устанавливаем спрайт изображения на SoundOffSprite
			if (SoundVolume == 1)
				SoundImage.sprite = SoundOnSprite;
			else
				SoundImage.sprite = SoundOffSprite;
		}
	}
}