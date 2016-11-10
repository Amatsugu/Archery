using UnityEngine;
using System.Collections;

namespace LuminousVector
{

	public class Pause : MonoBehaviour
	{
		void Start()
		{
			EventManager.StartListening(GameEvent.GAME_PAUSED, () => Cursor.lockState = CursorLockMode.None);
			EventManager.StartListening(GameEvent.GAME_UNPAUSED, () => Cursor.lockState = CursorLockMode.Locked);
			Game.UnPause();
		}

		void Update()
		{
			if(Input.GetKeyUp(KeyCode.Escape))
			{
				Debug.Log("Paused");
				if (!Game.IS_PAUSED)
					Game.Pause();
				else
					Game.UnPause();
			}
		}
	}
}
