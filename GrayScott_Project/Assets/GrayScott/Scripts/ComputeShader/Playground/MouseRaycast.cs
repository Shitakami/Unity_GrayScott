using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MouseRaycast : MonoBehaviour
{
    [SerializeField]
    private ParameterChangeSimulator m_simulator;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
        if(!Input.GetMouseButton(0))
            return;

        var ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        RaycastHit hit = new RaycastHit();

        if(!Physics.Raycast(ray, out hit) || !hit.collider.CompareTag("Plane"))
            return;
        
        
        m_simulator.AddCircle(hit.textureCoord);

    }


}
