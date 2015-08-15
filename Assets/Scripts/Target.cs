using UnityEngine;
using System.Collections;

public class Target : MonoBehaviour
{
	//Public
	public int emissionCount = 100;
	public ParticleSystem particles;

	//Emmit particles to indicate an arrow has hit
	void OnCollisionEnter(Collision col)
	{
		if(col.collider.tag == "Arrow")
		{
			particles.Emit(emissionCount);
		}
	}
}
