using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BehaviourCamera : MonoBehaviour
{
    //Offset cuando el ejeMovimiento está en su estado inicial
    [SerializeField] private Vector3 positionOffset;
    [SerializeField] private Transform playerTransform;
    [SerializeField] private float tiempoTransicion;

    private bool acomodando = false;
    private float timerAcomodo = 0;

    private Eje ejeMovimiento;
    private Vector3 viejaRotacion;
    private Vector3 nuevaRotacion;
    private Vector3 viejaPosicionRelativa;
    private Vector3 nuevaPosicionRelativa;
    private Vector3 posicionRelativa;

    void Start()
    {
        ejeMovimiento = new Eje(EjeDireccion.Z, EjeSentido.Positivo);

        positionOffset = - playerTransform.position + gameObject.transform.position;

        posicionRelativa = positionOffset;
    }
    
    void FixedUpdate()
    {
        if(acomodando)
        {
            Acomodar();
            ContarTimerAcomodo();
        }
        MoveCamera();
    }

    private void MoveCamera()
    {
        gameObject.transform.position = playerTransform.position + posicionRelativa;
    }

    public void AcomodarCamara(Eje nuevoEje)
    {
        viejaRotacion = transform.rotation.eulerAngles;
        nuevaRotacion = viejaRotacion + new Vector3(0, ejeMovimiento.AngulosA(nuevoEje), 0);

        viejaPosicionRelativa = - playerTransform.position + gameObject.transform.position;
        nuevaPosicionRelativa = nuevoEje.Vectorizado * positionOffset.z + positionOffset.y * Vector3.up;

        ejeMovimiento = new Eje(nuevoEje.Direccion, nuevoEje.Sentido);
        acomodando = true;
    }

    private void ContarTimerAcomodo()
    {
        if(timerAcomodo < tiempoTransicion)
        {
            timerAcomodo += Time.deltaTime;
        }
        else
        {
            acomodando = false;
            timerAcomodo = 0;
            AjustarCamara();
        }
    }

    private void AjustarCamara()
    {
        posicionRelativa = nuevaPosicionRelativa;
        transform.rotation = Quaternion.Euler(nuevaRotacion);
    }

    private void Acomodar()
    {
        float interpolador = Mathf.Clamp(Mathf.Pow(timerAcomodo / tiempoTransicion, 1F), 0.00001F ,1F);

        posicionRelativa = Vector3.Slerp(viejaPosicionRelativa, nuevaPosicionRelativa, interpolador);

        transform.rotation = Quaternion.Euler(Vector3.Lerp(viejaRotacion, nuevaRotacion, interpolador));
    }
}
