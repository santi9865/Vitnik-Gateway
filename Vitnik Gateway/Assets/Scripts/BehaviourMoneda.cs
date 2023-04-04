using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BehaviourMoneda : MonoBehaviour
{
    public BehaviourPista pistaAsociada;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void Destruir()
    {
        pistaAsociada.RemoverMoneda(gameObject);
        gameObject.SetActive(false);
    }
}
