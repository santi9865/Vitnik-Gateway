using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BehaviourMovimientoJugador : MonoBehaviour
{
    [SerializeField] private float speed;
    [SerializeField] private float jumpForce;
    private Rigidbody rb;
    [SerializeField] private bool vivo = true;
    [SerializeField] private List<Transform> carriles;
    [SerializeField] private int carrilActual = 1;
    [SerializeField] private BehaviourMiniPantallas scriptMiniPantallas;

    [SerializeField] private Vector3 centroColliderNormal;
    [SerializeField] private Vector3 tamañoColliderNormal;
    [SerializeField] private Vector3 centroColliderDesliz;
    [SerializeField] private Vector3 tamañoColliderDesliz;
    [SerializeField] private float tiempoMaximoDesliz;
    [SerializeField] private float tiempoMaximoCambioCarril;
    private float timerDesliz;
    private bool deslizando;

    private float timerCambioCarril;
    private bool cambiandoCarril;
    private Vector3 carrilInicial;
    private Vector3 carrilFinal;

    private BehaviourPlayerCollisionDetector detectorColisiones;

    [SerializeField] private Animator animatorJugador;

    //Debug Variables

    [SerializeField] private float alturaMaxima = 0;

    //End of debug variables

    void Start()
    {
        detectorColisiones = gameObject.GetComponentInChildren<BehaviourPlayerCollisionDetector>();

        animatorJugador = gameObject.GetComponentInChildren<Animator>();

        rb = gameObject.GetComponent<Rigidbody>();

        if(rb == null)
        {
            Debug.Log("Rigidbody not found.");
        }
    }

    // Update is called once per frame
    void Update()
    {
        if(vivo)
        {
            MoverseHaciaAdelante();

            if (Input.GetKeyDown(KeyCode.Space))
            {
                Saltar();
            }

            if(Input.GetKeyDown(KeyCode.RightArrow))
            {
                MoverseALaDerecha();
            }
            if(Input.GetKeyDown(KeyCode.LeftArrow))
            {
                MoverseALaIzquierda();
            }
            if(Input.GetKeyDown(KeyCode.DownArrow))
            {
                Deslizar();
            }


        }
    }

    void FixedUpdate()
    {
            if(deslizando)
            {
                ContarTimerDesliz();
            }

            if(cambiandoCarril)
            {
                CambiarACarril();
                ContarTimerCambioCarril();
            }
    }


    #region Deslizamiento

    private void ContarTimerDesliz()
    {
        timerDesliz -= Time.deltaTime;

        if(timerDesliz <= 0)
        {
            Levantar();
        }
    }
        private void Deslizar()
    {
        //Solo se actualiza el collider si no se está deslizando actualmente.
        if(!deslizando)
        {
            detectorColisiones.ActualizarCollider(centroColliderDesliz, tamañoColliderDesliz);
        }

        timerDesliz = tiempoMaximoDesliz;

        deslizando = true;
        animatorJugador.SetBool("deslizando", true);
    }

    private void Levantar()
    {
        detectorColisiones.ActualizarCollider(centroColliderNormal, tamañoColliderNormal);

        deslizando = false;
        animatorJugador.SetBool("deslizando", false);
    }

    #endregion

    #region Cambio de carril

    private void ContarTimerCambioCarril()
    {
        timerCambioCarril += Time.fixedDeltaTime;

        if(timerCambioCarril >= tiempoMaximoCambioCarril)
        {
            cambiandoCarril = false;
            AjustarCarril();
        }
    }

    private void AjustarCarril()
    {
        gameObject.transform.position = new Vector3(carrilFinal.x, gameObject.transform.position.y, gameObject.transform.position.z);
    }

    private void CambiarACarril()
    {
        float interpolador;

        interpolador = Mathf.Clamp(Mathf.Sqrt(timerCambioCarril / tiempoMaximoCambioCarril), 0, 1);

        // float valorMedio = ((2 / tiempoMaximoCambioCarril) * Mathf.Pow(0.5F,3)) * (timerCambioCarril) - Mathf.Pow(0.5F,3);
        // if(valorMedio < 0)
        // {
        //     interpolador = Mathf.Clamp((-Mathf.Pow(-valorMedio, 1F/3F) + 0.5F), 0, 1);
        // }
        // else
        // {
        //     interpolador = Mathf.Clamp((Mathf.Pow(valorMedio, 1F/3F) + 0.5F), 0, 1);
        // }

        //Produce error porque no se puede usar Mathf.Pow para sacar raíces con exponentes impares de números negativos
        //Mathf.Clamp((Mathf.Pow(((2 / tiempoMaximoCambioCarril) * Mathf.Pow(0.5F,3)) * (timerCambioCarril) - Mathf.Pow(0.5F,3), 1F/3F) + 0.5F), 0, 1);

        Debug.Log("timer: " + timerCambioCarril);
        //Debug.Log("timer por 0.5f^3: " + Mathf.Pow(0.5F,3) * (timerCambioCarril));
        Debug.Log("interpolador: " + interpolador);

        gameObject.transform.position = new Vector3(Vector3.Lerp(carrilInicial,carrilFinal,interpolador).x, gameObject.transform.position.y, gameObject.transform.position.z);
    }

    private void MoverseALaDerecha()
    {
        if (carrilActual < carriles.Count - 1)
        {
            carrilInicial = carriles[carrilActual].position;
            carrilFinal = carriles[carrilActual + 1].position;

            carrilActual++;

            animatorJugador.SetTrigger("CambiarCarrilDerecha");

            timerCambioCarril = 0;
            cambiandoCarril = true;
        }
    }

    private void MoverseALaIzquierda()
    {
        if (carrilActual > 0)
        {
            carrilInicial = carriles[carrilActual].position;
            carrilFinal = carriles[carrilActual - 1].position;

            carrilActual--;

            animatorJugador.SetTrigger("CambiarCarrilIzquierda");

            timerCambioCarril = 0;
            cambiandoCarril = true;
        }
    }

    #endregion


    private void MoverseHaciaAdelante()
    {
        gameObject.transform.Translate(Vector3.forward * speed * Time.deltaTime);
    }

    private void Saltar()
    {
        Levantar();
        rb.AddForce(Vector3.up * jumpForce, ForceMode.Impulse);
        animatorJugador.SetTrigger("salto");
    }

    //Este m�todo es llamado por el script del detector de colisiones del jugador
    public void ColisionObstaculo(ObstacleType tipoObstaculo)
    {
        switch (tipoObstaculo)
        {
            case ObstacleType.Solid:
                vivo = false;
                scriptMiniPantallas.PantallaGameOverSetActive(true);
                break;
            case ObstacleType.Moneda:
                GameManager.Instancia.AddMonedas(1);
                break;
        }
    }

    public void NuevaSeccionPista(GameObject seccion)
    {
        carriles = seccion.GetComponent<BehaviourListaCarriles>().Carriles;
    }

    //Start of debug methods

    private void DebugAltura(bool outputToConsole)
    {
        if(gameObject.transform.position.y > alturaMaxima)
        {
            alturaMaxima = gameObject.transform.position.y;

            if(outputToConsole)
            {
                Debug.Log(alturaMaxima);
            }
        }
    }

    //End of debug methods
}

