using UnityEngine;
using System.Collections;

namespace LuminousVector
{
	public class Game : MonoBehaviour
	{


		public static Game GAME;
		public static Game INSTANCE
		{
			get
			{
				if (!GAME)
				{
					GAME = FindObjectOfType<Game>() as Game;
					if (!GAME)
					{
						Debug.Log("Game Manager not found");
					}
					else
						GAME.Init();
				}
				return GAME;
			}
		}

		void Awake()
		{
			DontDestroyOnLoad(gameObject);
			if (FindObjectOfType<Game>() as Game != this)
				Destroy(gameObject);
		}

		public static bool IS_PAUSED
		{
			get
			{
				return INSTANCE._isPaused;
			}
		}

		private bool _isPaused = false;

		public void Init()
		{

		}
		
		public static void Pause()
		{
			INSTANCE._isPaused = true;
			foreach (Pauseable p in FindObjectsOfType<Pauseable>())
				p.Pause();
		}

		public static void UnPause()
		{
			INSTANCE._isPaused = false;
			foreach (Pauseable p in FindObjectsOfType<Pauseable>())
				p.UnPause();
		}
	}
}
