using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIManager : MonoBehaviour {

	public void DisableBoolAnimator(Animator anim) {
		anim.SetBool("IsDisplayed", false);
	}
	public void EnableBoolAnimator(Animator anim) {
		anim.SetBool("IsDisplayed", true);
	}
}
