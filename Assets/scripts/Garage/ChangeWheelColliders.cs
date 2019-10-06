using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChangeWheelColliders : MonoBehaviour {

	public static void changeWheelCollider(GameObject car) {
		GameObject wheelParent = car.transform.Find("wheels").gameObject;
		for(int i = 0; i < wheelParent.transform.childCount; i++)
		{
			WheelCollider w = wheelParent.transform.GetChild(i).GetComponent<WheelCollider>();

			JointSpring suspensionSpring = new JointSpring ();
			suspensionSpring.spring = 10000;
			suspensionSpring.damper = 200;
			suspensionSpring.targetPosition = 0.45f;
			w.suspensionSpring = suspensionSpring;
		}
	}

}
