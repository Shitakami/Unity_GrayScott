// 参考:https://qiita.com/Nekomasu/items/37b2c553a4f46738cbba


#if UNITY_EDITOR
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using System.IO;
using System;
using Random = UnityEngine.Random;

public class NoiseGenerator : MonoBehaviour {
	//ファイル名
	[SerializeField]
	private string _fileName = "Noise";

	[SerializeField]
	private int _textureSize = 128;

	[SerializeField, Tooltip("Trueならタイムスタンプが付きます")]
	private bool _useTimeStamp = false;

	[SerializeField]
	private bool _isPerlinNoise;
	[SerializeField]
	private float _whiteNoisePower;

	[Space(40)]

	//パーリンノイズに渡すxとyの初期値
	[SerializeField]
	private Vector2 _initialSmaple = Vector2.zero;
	//ノイズの大きさの倍率
	[SerializeField]
	private float _noizeScale = 10f;
	//保存先
	private string _filePath = "Assets/Noise/";
	//UIの表示管理
	private bool _uiMessageActiv = false;

	private void CreateTexture(string path, string name) {
		//Pathが無効の場合作成する
		AccessTexturePath(path);

		//タイムスタンプ
		string time = DateTime.Now.ToString("yyyyMMddHHmmss");
		string fullpath;

		//判定
		if (_useTimeStamp) {
			fullpath = path + name + time + ".png";
		}
		else {
			fullpath = path + name + ".png";
		}

		//テクスチャ作成
		Texture2D noise = new Texture2D(_textureSize, _textureSize);

		if (_isPerlinNoise) {

			/*
			float v00 = Random.Range(0, 1.0f);
			float v01 = Random.Range(0, 1.0f);
			float v10 = Random.Range(0, 1.0f);
			float v11 = Random.Range(0, 1.0f);

			for (int y = 0; y < _textureSize; y++) {
				float v0001 = Mathf.Lerp(v00, v01, (float)y / (float)_textureSize);
				float v1011 = Mathf.Lerp(v10, v11, (float)y / (float)_textureSize);
				for (int x = 0; x < _textureSize; x++) {
					float value = Mathf.Lerp(v1011, v0001, (float)x / (float)_textureSize);
					Color color = new Color(value, value, value, 1.0f);
					noise.SetPixel(x, y, color);
				}
			}
			*/
			PerlinNoise(noise);

		}
		else
			WhiteNoise(noise);

		//書き出し処理
		noise.Apply();
		var bytes = noise.EncodeToPNG();
		File.WriteAllBytes(fullpath, bytes);
		Debug.Log("Create : " + fullpath);
		AssetDatabase.Refresh();
		//	StartCoroutine(DisplayUiMessage());
	}

	//パスが無ければ作成
	private void AccessTexturePath(string path) {
		if (Directory.Exists(path)) { return; }

		if (Directory.Exists(path) == false) {
			Directory.CreateDirectory(path);
			Debug.Log("CreatePath : " + path);
		}
	}

	//メッセージの表示管理
	private IEnumerator DisplayUiMessage() {
		if (_uiMessageActiv) { yield break; }

		_uiMessageActiv = true;
		yield return new WaitForSeconds(3.0f);
		_uiMessageActiv = false;
	}


	[ContextMenu("テクスチャ生成")]
	public void MenuCreateTexture() {
		CreateTexture(_filePath, _fileName);
	}

	void WhiteNoise(Texture2D noise) {


		for (int y = 0; y < _textureSize; y++) {
			for (int x = 0; x < _textureSize; x++) {
				float value1 = Mathf.Pow(Random.Range(0, 1.0f), _whiteNoisePower);
				float value2 = Mathf.Pow(Random.Range(0, 1.0f), _whiteNoisePower);
				float value3 = Mathf.Pow(Random.Range(0, 1.0f), _whiteNoisePower);
				Color color = new Color(value1, value2, value3, 1.0f);
				noise.SetPixel(x, y, color);
			}
		}


	}

	private void PerlinNoise(Texture2D noise) {
		if (noise == null) {
			return;
		}

		//全画素に対して更新をかける
		for (int x = 0; x < _textureSize; x++) {
			for (int y = 0; y < _textureSize; y++) {
				//パーリンノイズに入力する値を決定
				float xSample = _initialSmaple.x + x / (float)_textureSize * _noizeScale;
				float ySample = _initialSmaple.y + y / (float)_textureSize * _noizeScale;

				//パーリンノイズで色を決定(0 ~ 1)
				float color = Mathf.PerlinNoise(xSample, ySample);
				noise.SetPixel(x, y, new Color(color, color, color));
			}
		}
	}



}

#endif