using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ParameterChangeSimulator : MonoBehaviour
{

    private RenderTexture m_uvTexture;

    [SerializeField] 
    private ComputeShader m_grayScottCalculator;

	[Header("モデルの各パラメータ")]
	[SerializeField]
	private float m_f = 0.04f;

	[SerializeField]
	private float m_k = 0.06f;

	[SerializeField]
	private float m_simulateSpeed = 1f;

	[SerializeField]
	private int m_step = 8;

	[SerializeField]
	private float m_gridSize = 0.01f;

	[SerializeField]
	private Texture2D m_noiseTexture;

	[Space(10)]
	[SerializeField]
	private float m_Du = 2e-5f;

	[SerializeField]
	private float m_Dv = 1e-5f;
	

    [SerializeField]
    [Header("初期化項目")] 
    private int m_initialQuadSize;
	
    [SerializeField]
    private int m_textureSize;

	public int TextureSize { get { return m_textureSize; } }

	private int m_updateGrayScottKernel;
	private Vector3Int m_groupSize;

	public float F { get { return m_f; } }
	public float K { get { return m_k; } }

	private int m_addcircleKernel;

	[Space(10)]
	[SerializeField]
	private float m_circleRange;

    // Start is called before the first frame update
    void Awake()
    {
		CreateTexture();

        InitializeTexture();

		SetGrayScottParameter();

		m_addcircleKernel = m_grayScottCalculator.FindKernel("AddCircle");

		// 生成したRenderTextureをマテリアルに設定
		GetComponent<Renderer>().sharedMaterial.SetTexture("_MainTex", m_uvTexture);
    }

	void Update() {

		for(int i = 0; i < m_step; ++i)
			UpdateGrayScott();

	}

	private void CreateTexture() {

		m_uvTexture = new RenderTexture(m_textureSize, m_textureSize, 0, RenderTextureFormat.RG32);
		m_uvTexture.wrapMode = TextureWrapMode.Clamp;
		m_uvTexture.enableRandomWrite = true;
		m_uvTexture.Create();

	}

	public void InitializeTexture() {
        
        int initializeTextureKernel = m_grayScottCalculator.FindKernel("InitializeTextureWhite");

		// テクスチャの初期状態を設定
        // m_grayScottCalculator.SetInt("InitialQuadSize", m_initialQuadSize);
        // m_grayScottCalculator.SetInt("minThreadhold", m_textureSize / 2 - m_initialQuadSize / 2);
        // m_grayScottCalculator.SetInt("maxThreadhold", m_textureSize / 2 + m_initialQuadSize / 2);
        m_grayScottCalculator.SetTexture(initializeTextureKernel, "Texture", m_uvTexture);
		m_grayScottCalculator.SetTexture(initializeTextureKernel, "NoiseTexture", m_noiseTexture);

        m_grayScottCalculator.GetKernelThreadGroupSizes(initializeTextureKernel, out uint x, out uint y, out uint z);

		m_grayScottCalculator.Dispatch(initializeTextureKernel,
			m_textureSize / (int)x,
			m_textureSize / (int)y,
			(int) z);

    }

	public void SetGrayScottParameter() {

		m_updateGrayScottKernel = m_grayScottCalculator.FindKernel("UpdateGrayScotte");
		
		// 実行する際のグループサイズを求める
		m_grayScottCalculator.GetKernelThreadGroupSizes(m_updateGrayScottKernel, out uint x, out uint y, out uint z);
		m_groupSize = new Vector3Int(m_textureSize / (int)x, m_textureSize / (int)y, (int)z);

		// GrayScottのパラメータを設定
		m_grayScottCalculator.SetFloat("f", m_f);
		m_grayScottCalculator.SetFloat("k", m_k);
		m_grayScottCalculator.SetFloat("dt", m_simulateSpeed);
		m_grayScottCalculator.SetFloat("dx", m_gridSize);
		m_grayScottCalculator.SetFloat("Du", m_Du);
		m_grayScottCalculator.SetFloat("Dv", m_Dv);

		m_grayScottCalculator.SetTexture(m_updateGrayScottKernel, "Texture", m_uvTexture);
	}


	public void SetGrayScottParameterFK(float f, float k) {

		m_f = f;
		m_k = k;

		m_grayScottCalculator.SetFloat("f", m_f);
		m_grayScottCalculator.SetFloat("k", m_k);
		
	}

	private void UpdateGrayScott() {

		m_grayScottCalculator.Dispatch(m_updateGrayScottKernel, m_groupSize.x, m_groupSize.y, m_groupSize.z);

	}


	public void AddCircle(Vector2 uvPosition) {

		m_grayScottCalculator.SetTexture(m_addcircleKernel, "Texture", m_uvTexture);
		m_grayScottCalculator.SetTexture(m_addcircleKernel, "NoiseTexture", m_noiseTexture);

		m_grayScottCalculator.SetFloat("clickPositionX", uvPosition.x * m_textureSize);
		m_grayScottCalculator.SetFloat("clickPositionY", uvPosition.y * m_textureSize);
		m_grayScottCalculator.SetFloat("Range", m_circleRange);

		m_grayScottCalculator.Dispatch(m_addcircleKernel, m_groupSize.x, m_groupSize.y, m_groupSize.z);

	}

}
