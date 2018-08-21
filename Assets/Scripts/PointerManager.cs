using System.Collections;
using System.Collections.Generic;

using UnityEngine;
using UnityEngine.UI;

public class PointerManager : MonoBehaviour {

	private const float moveSpeed = 4;

	// Use this for initialization
	void Start() { }

	// Update is called once per frame
	void Update() { }

	// TODO:進行方向（左右）に回転させる？
	// 一定速度で目的地targetPosに向けて移動
	public void moveToPoint(Vector3 targetPos) {
		var moveDirection = this.returnOneDimensionMoveDirection(targetPos);
		this.transform.Translate(moveDirection * moveSpeed * Time.deltaTime);
	}

	// REVIEW:関数名
	// 「移動方向の内一次元を返す」名が体を表していない・・・
	public Vector3 returnOneDimensionMoveDirection(Vector3 targetPos) {
		// targetPosに十分近い時はzeroを返す
		if(Vector3.Distance(this.transform.position, targetPos) < 0.05f) {
			return Vector3.zero;
		}

		var deltaX = targetPos.x - this.transform.position.x;
		var deltaY = targetPos.y - this.transform.position.y;

		// Y軸優先で移動
		return Mathf.Abs(deltaY) <= 0.05 ?
			new Vector3(deltaX / Mathf.Abs(deltaX), 0f, 0f) :
			new Vector3(0f, deltaY / Mathf.Abs(deltaY), 0f);
	}
}
