using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RotacionConstante : MonoBehaviour
{
    //Velocidad angular expresada en grados por segundo.
    [SerializeField] float velocidadAngular;
    [SerializeField] float rotacionY;

    // Update is called once per frame
    void Update()
    {
        rotacionY += (Time.deltaTime * velocidadAngular);

        rotacionY %= 360;
        
        for(int i = 0; i< transform.childCount; i++)
        {
            Transform transformHijo = transform.GetChild(i);

            if(transformHijo.gameObject.activeSelf)
            {
                Vector3 rotacionHijo = transformHijo.rotation.eulerAngles;
                transformHijo.rotation = Quaternion.Euler(rotacionHijo.x, rotacionY, rotacionHijo.z);
            }
        }
    }
}
