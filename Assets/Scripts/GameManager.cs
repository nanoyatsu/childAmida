using System.Collections;
using System.Collections.Generic;

using UnityEngine;

// 定数定義用
public static class CONST {
	public const int rowCnt = 5;

	public const float minXpos = -6f;
	public const float maxXpos = 6f;
	public const float maxYpos = 4f;
	public const float minYpos = -4f;

	public const string gmObjectName = "GameManager";
	public const string pointerObjectName = "ひつじ";
	public const string goalObjectName = "saku";
	public const string selectedGoalTagName = "saku";
	public const string selectorGoalTagName = "sakuButton";
	public const string amidaLineTagName = "amidaLine";
	public const string resultTagName = "result";
}

public class GameManager : MonoBehaviour {

	// Inspectorで設定
	public GameObject linePrefab;
	public GameObject sakuPrefab;
	public GameObject resultPrefab;
	public Sprite[] resultSprite;

	// 処理用
	private Amida amida;
	private GameObject pointer;
	private Vector3 nextArrivalPos;
	private Vector3 goalPos;
	private int columnCnt = 2;
	private bool moving = false;
	private int pointerColumn = 0;
	private int pointerRow = -1;

	// Use this for initialization
	void Start() {
		pointer = GameObject.Find(CONST.pointerObjectName);
		newAmida();
	}
	// 縦線増加関数 columnCnt = 2～5を循環
	public void amidaColumnAdd() {
		if(columnCnt == 5) {
			columnCnt = 1;
		}
		columnCnt++;
		newAmida();
	}

	private void newAmida() {
		// HACK:再描画・初期化まわり混ざり気味
		moving = false;
		amida = new Amida(CONST.rowCnt, columnCnt);
		var a = amida.rowMap;
		for(int n = 0; n < columnCnt - 1; n++) {
			Debug.Log("amida " + n.ToString() + " : " + a[0, n] + "," + a[1, n] + "," + a[2, n] + "," + a[3, n] + "," + a[4, n] + ",");
		}
		AmidaDraw();
	}

	// 設問表示時の描画関数 
	public void AmidaDraw() {
		// Debug.Log("amidadraw");
		var rowMap = amida.rowMap;

		// pointer（ひつじ）を先に置く
		pointerRow = -1;
		pointerColumn =(Random.Range(0, columnCnt));
		pointer.transform.position = rowMapIndexToPosition(columnCnt, -1, pointerColumn);

		// 前のAmidaのlineを消す
		// HACK:Drawっぽい処理じゃない 別に抜くべき
		ClonesDestroyWithTag(CONST.amidaLineTagName);
		ClonesDestroyWithTag(CONST.selectorGoalTagName);
		ClonesDestroyWithTag(CONST.selectedGoalTagName);
		ClonesDestroyWithTag(CONST.resultTagName);

		// 横線の配置
		for(int m = 0; m < CONST.rowCnt; m++) {
			for(int n = 0; n < columnCnt - 1; n++) {
				if(rowMap[m, n]) {
					drawOneLineRenderer(rowMapIndexToPosition(columnCnt, m, n), rowMapIndexToPosition(columnCnt, m, n + 1));
				}
			}
		}

		// 縦要素(線とゴール)の配置
		for(int n = 0; n < columnCnt; n++) {
			// 縦線を引く
			drawOneLineRenderer(rowMapIndexToPosition(columnCnt, -1, n), rowMapIndexToPosition(columnCnt, CONST.rowCnt, n));

			// 半透明のゴールを置く
			var sakuButton =(GameObject) Instantiate(sakuPrefab);
			sakuButton.tag = CONST.selectorGoalTagName;
			sakuButton.transform.position = rowMapIndexToPosition(columnCnt, CONST.rowCnt, n);
			sakuButton.GetComponent<SpriteRenderer>().color = new Color(1, 1, 1, 0.5f);
		}
	}

	// Goal選択時の呼び出し関数 ここからアニメーション開始
	public void moveStart(Vector3 _goalPos) {
		goalPos = _goalPos;
		moving = true;
		nextArrivalPos = searchNextArrivalPos(amida.rowMap, ref pointerRow, ref pointerColumn);
	}

	// Update is called once per frame
	void Update() {
		if(moving) {
			var moveDirection = pointer.GetComponent<PointerManager>().returnOneDimensionMoveDirection(nextArrivalPos);
			if(moveDirection == Vector3.zero) {
				pointer.transform.position = nextArrivalPos;

				var isMoveEnd = pointerRow >= CONST.rowCnt;
				//TODO:Rowの比較らへんで代替？
				if(isMoveEnd) {
					moving = false;
					resultShow();
					return;
				}
				nextArrivalPos = searchNextArrivalPos(amida.rowMap, ref pointerRow, ref pointerColumn);
			} else {
				pointer.GetComponent<PointerManager>().moveToPoint(nextArrivalPos);
			}
		}
	}

	// 次の目的座標を探す
	public Vector3 searchNextArrivalPos(bool[, ] rowMap, ref int pointerRow, ref int pointerColumn) {
		// REVIEW:この説明変数要るかな
		var rightRowIndex = pointerColumn;
		var leftRowIndex = rightRowIndex - 1;

		if(rightRowIndex >= rowMap.GetLength(1)) {
			rightRowIndex = -1;
		}
		if(leftRowIndex < 0) {
			leftRowIndex = -1;
		}
		// Debug.Log("L:" + LeftRowIndex.ToString() + " R:" + RightRowIndex.ToString());

		// 1つ次の点から探す
		pointerRow++;
		// 1つ次の横線の有無を確認
		if(pointerRow < rowMap.GetLength(0)) {
			if(rightRowIndex != -1 && rowMap[pointerRow, rightRowIndex]) {
				Debug.Log("sita migi");
				pointerColumn++;
			} else
			if(leftRowIndex != -1 && rowMap[pointerRow, leftRowIndex]) {
				Debug.Log("sita hidari");
				pointerColumn--;
			}
		}

		// Debug.Log("seach result -> " + rowMapIndexToPosition(rowMap.GetLength(1) + 1, pointerRow, pointerColumn).ToString());
		return rowMapIndexToPosition(rowMap.GetLength(1) + 1, pointerRow, pointerColumn);
	}

	// REVIEW: 名前にAMIDAつける？(prefab渡してAmidaクラスに入れてstaticにする？)
	private void drawOneLineRenderer(Vector3 startPos, Vector3 endPos) {
		var line =(GameObject) Instantiate(linePrefab);
		line.tag = CONST.amidaLineTagName;
		line.GetComponent<LineRenderer>().SetPosition(0, startPos);
		line.GetComponent<LineRenderer>().SetPosition(1, endPos);

	}

	private void resultShow() {
		//Debug.Log("resultshow()");
		var result =(GameObject) Instantiate(resultPrefab);
		result.tag = CONST.resultTagName;

		// 正解のsprite、間違いのspriteを設定
		result.GetComponent<SpriteRenderer>().sprite =
			(pointer.transform.position == goalPos) ? resultSprite[0] : resultSprite[1];

		return;
	}

	public void checkGoal() {
		var pointerPos = GameObject.Find("Pointer").transform.position;
		var goalPos = GameObject.Find("Goal").transform.position;
	}

	// static
	public static void ClonesDestroyWithTag(string tagname) {
		var clones = GameObject.FindGameObjectsWithTag(tagname);
		foreach(var clone in clones) {
			Destroy(clone);
		}
	}

	public static Vector3 rowMapIndexToPosition(int _colmnCnt, int m_R, int n_C) {
		var rowInterval =(Mathf.Abs(CONST.maxYpos - CONST.minYpos) - 2f) /(CONST.rowCnt + 1f);
		var columnInterval =(Mathf.Abs(CONST.minXpos - CONST.maxXpos) - 2f) /(_colmnCnt + 1f);

		if(m_R <= -1) {
			return new Vector3((CONST.minXpos + 1) +(columnInterval *(n_C + 1)), CONST.maxYpos, 0f);
		} else
		if(m_R >= CONST.rowCnt) {
			return new Vector3((CONST.minXpos + 1) +(columnInterval *(n_C + 1)), CONST.minYpos, 0f);
		} else {
			return new Vector3(
				(CONST.minXpos + 1) +(columnInterval *(n_C + 1)),
				(CONST.maxYpos - 1) -(rowInterval *(m_R + 1)),
				0f
			);
		}
	}
}

public class Amida {
	public bool[, ] rowMap;

	public Amida(int rowCnt, int columnCnt) {
		rowMap = new bool[rowCnt, columnCnt - 1];

		// 横線の配置図を作成
		for(int m = 0; m < rowCnt; m++) {
			for(int n = 0; n < columnCnt - 1; n++) {
				// 乱数0以外なら横線あり
				rowMap[m, n] = Random.Range(0, 2) != 0;

				// 1つ左とTrueが衝突したらランダムに分配
				if(n != 0 && rowMap[m, n - 1] && rowMap[m, n]) {
					rowMap[m, n] = Random.Range(0, 2) == 0;
					rowMap[m, n - 1] = !rowMap[m, n];
				}
			}
		}
	}
}
