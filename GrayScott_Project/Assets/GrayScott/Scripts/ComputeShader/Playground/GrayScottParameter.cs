using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;

[RequireComponent(typeof(ParameterChangeSimulator))]
public class GrayScottParameter : MonoBehaviour
{

    [SerializeField]
    private float m_changeTime; 

    private float m_time;

    private float m_prevF;
    private float m_prevK;

    private float m_nextF;
    private float m_nextK;

    private ParameterChangeSimulator m_simulator;

    // Start is called before the first frame update
    void Start()
    {
        Assert.IsFalse(m_changeTime <= 0, "m_changeTimeが正しく設定されていません。0より大きい値を入れて下さい");        
        m_time = m_changeTime;

        m_simulator = GetComponent<ParameterChangeSimulator>();
    }

    // Update is called once per frame
    void Update()
    {
        
        if(m_time == m_changeTime)
            return;

        m_time = Mathf.Min(m_time + Time.deltaTime, m_changeTime);

        float rate = m_time / m_changeTime;

        m_simulator.SetGrayScottParameterFK(
            Mathf.Lerp(m_prevF, m_nextF, rate),
            Mathf.Lerp(m_prevK, m_nextK, rate)
        );

    }

    private void ChangeGrayScottPattern(float f, float k) {

        m_time = 0;
        
        m_prevF = m_simulator.F;
        m_prevK = m_simulator.K;

        m_nextF = f;
        m_nextK = k;
        
    }

    public void ChangeStripe() {
        ChangeGrayScottPattern(0.022f, 0.051f);
    }

    public void ChageSpot() {
        ChangeGrayScottPattern(0.035f, 0.065f);
    }

    public void ChangeAmorphous() {
        ChangeGrayScottPattern(0.04f, 0.06f);
    }

    public void ChangeBubbles() {
        ChangeGrayScottPattern(0.012f, 0.05f);
    }

    public void ChangeWaves() {
        ChangeGrayScottPattern(0.025f, 0.05f);
    }

}
