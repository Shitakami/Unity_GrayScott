using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RenderTextureUpdater : MonoBehaviour
{
    [SerializeField]
    private CustomRenderTexture m_texture;

    [SerializeField]
    private int m_updateStep = 1;

    void Start()
    {
        m_texture.Initialize();
    }

    void Update()
    {
        m_texture.Update(m_updateStep);
    }
}
