using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RenderTextureUpdater : MonoBehaviour
{
    [SerializeField]
    private CustomRenderTexture m_texture;

    [SerializeField]
    private int m_updateStep = 1;

    // Start is called before the first frame update
    void Start()
    {
        m_texture.Initialize();
    }

    // Update is called once per frame
    void Update()
    {
        m_texture.Update(m_updateStep);
    }
}
