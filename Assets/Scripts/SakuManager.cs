using System.Collections;
using System.Collections.Generic;

using UnityEngine;

public class SakuManager : MonoBehaviour {
	// Inspectorで設定
	public GameObject sakuPrefab;

	// Use this for initialization
	void Start() {

	}

	// Update is called once per frame
	void Update() {

	}

	public void onPointerEnter() {
		// Debug.Log("sakuenter");
		if(this.tag == CONST.selectorGoalTagName) {
			this.GetComponent<SpriteRenderer>().color = new Color(1, 1, 1, 1);
		}
	}
	public void onPointerExit() {
		// Debug.Log("sakuexit");
		if(this.tag == CONST.selectorGoalTagName) {
			this.GetComponent<SpriteRenderer>().color = new Color(1, 1, 1, 0.5f);
		}
	}
	public void onClick() {
		if(this.tag == CONST.selectorGoalTagName) {
			this.tag = CONST.selectedGoalTagName;

			GameManager gm = GameObject.Find(CONST.gmObjectName).GetComponent<GameManager>();
			gm.moveStart(this.transform.position);

			GameManager.clonesDestroyWithTag(CONST.selectorGoalTagName);
		}
	}
}
