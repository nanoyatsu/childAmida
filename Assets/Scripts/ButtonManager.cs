using System.Collections;
using System.Collections.Generic;

using UnityEngine;

public class ButtonManager : MonoBehaviour {

	// Use this for initialization
	void Start() { }

	// Update is called once per frame
	void Update() { }

	public void addLineBtnOnClick() {
		GameObject gm = GameObject.Find(CONST.gmObjectName);
		gm.GetComponent<GameManager>().amidaColumnAdd();
	}
	public void reDrawBtnOnClick() {
		GameObject gm = GameObject.Find(CONST.gmObjectName);
		gm.GetComponent<GameManager>().reDraw();
		this.gameObject.SetActive(false);
	}

	public void switchEnabled() {
		this.enabled = this.enabled?false : true;
	}
}
