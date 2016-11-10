using UnityEngine;
using System.Collections.Generic;

namespace LuminousVector
{
	public class TrajectoryMapper
	{
		public static float INTERVAL = 0.05f;

		public static Vector3[] GetTrajectory(Vector3 startPos, float mass,  Vector3[] forces)
		{
			Vector3 netForce = new Vector3();
			foreach(Vector3 f in forces)
			{
				netForce += f;
			}
			return GetTrajectory(startPos, mass, netForce);
		}

		public static Vector3[] GetTrajectory(Vector3 startPos, float mass, Vector3 netForce)
		{
			List<Vector3> traj = new List<Vector3>();
			Vector3 initialVel = new Vector3();
			initialVel.y = netForce.y / mass;
			initialVel.x = netForce.x / mass;
			initialVel.z = netForce.z / mass;
			float travelTime = 2f*((0f- initialVel.y)/-9.8f) + 1f;
			int projectionCount = Mathf.CeilToInt(travelTime / INTERVAL);
			traj.Add(startPos);
			for(int i = 1; i <= projectionCount; i++)
			{
				float t = i * INTERVAL;
				Vector3 point = startPos;
				point.y += initialVel.y * t + (0.5f) * -9.8f * t * t;
				point.x += initialVel.x * t;
				point.z += initialVel.z * t;
				traj.Add(point);
			}
			return traj.ToArray();
		}
	}
}
