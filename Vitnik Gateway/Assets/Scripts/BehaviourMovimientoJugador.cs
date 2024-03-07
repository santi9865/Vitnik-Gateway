using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BehaviourMovimientoJugador : MonoBehaviour
{
    public Eje EjeMovimiento {get; private set;}
    //Arriba o abajo
    public Vector3 Orientacion {get; private set;} 
    [SerializeField] private float velocidad;
    [SerializeField] private float distanciaMaximaSalto;
    public float DistanciaSalto {get => distanciaMaximaSalto;}
    [SerializeField] private float distanciaMaximaDesliz;
    public float DistanciaDesliz {get => distanciaMaximaDesliz;}
    [SerializeField] private float alturaBase;
    public float AlturaBase {get => alturaBase;}
    [SerializeField] private float alturaMaxima = 0;
    public float AlturaMaxima {get => alturaMaxima;}  
    [SerializeField] private bool vivo = true;

    [SerializeField] private bool puedeDoblarIzquierda = false;
    [SerializeField] private bool puedeDoblarDerecha = false;
    private bool doblando = false;
    private Rama ramaDisponible;

    [SerializeField] private List<Carril> carriles;
    [SerializeField] private int carrilActual = 1;

    [SerializeField] private Vector3 centroColliderNormal;
    [SerializeField] private Vector3 tamañoColliderNormal;
    [SerializeField] private Vector3 centroColliderDesliz;
    [SerializeField] private Vector3 tamañoColliderDesliz;
    [SerializeField] private float tiempoMaximoCambioCarril;

    //Distancia que el jugador recorre a lo largo (no en horizontal) de un cambio de carril.
    public float DistanciaCambioCarril {get => tiempoMaximoCambioCarril * velocidad;}

    private float timerDesliz;
    private float tiempoMaximoDesliz;
    private bool deslizando;

    private float timerCambioCarril;
    private bool cambiandoCarril;
    private Vector3 carrilInicial;
    private Vector3 carrilFinal;


    private bool saltando;
    private float timerSalto;
    private float tiempoMaximoSalto;

    private Vector3 posicionAnterior;  

    private BehaviourPlayerCollisionDetector detectorColisiones;

    [SerializeField] private Animator animatorJugador;

    void Start()
    {
        detectorColisiones = gameObject.GetComponentInChildren<BehaviourPlayerCollisionDetector>();

        animatorJugador = gameObject.GetComponentInChildren<Animator>();

        EjeMovimiento = new Eje(EjeDireccion.Z, EjeSentido.Positivo);
        Orientacion = new Vector3(0,1,0);

        posicionAnterior = gameObject.transform.position;
    }

    void Update()
    {
        if(vivo)
        {
            if(Input.GetKeyDown(KeyCode.Space))
            {
                if(!saltando && !cambiandoCarril)
                {
                    Saltar();
                }
            }
            if(Input.GetKeyDown(KeyCode.RightArrow))
            {
                if(!cambiandoCarril && !deslizando)
                {
                    MoverseALaDerecha();
                }
            }
            if(Input.GetKeyDown(KeyCode.LeftArrow))
            {
                if(!cambiandoCarril && !deslizando)
                {
                    MoverseALaIzquierda();
                }
            }
            if(Input.GetKeyDown(KeyCode.DownArrow))
            {
                if(!cambiandoCarril && !saltando)
                {
                    Deslizar();
                }
            }
            if(Input.GetKeyDown(KeyCode.LeftArrow))
            {
                if(!doblando && !cambiandoCarril && !deslizando && !saltando && puedeDoblarIzquierda)
                {
                    DoblarIzquierda();
                }
            }
            if(Input.GetKey(KeyCode.RightArrow))
            {
                if(!doblando && !cambiandoCarril && !deslizando && !saltando && puedeDoblarDerecha)
                {
                    DoblarDerecha();
                }
            }
        }
    }

    void FixedUpdate()
    {
        MoverseHaciaAdelante();

        if(deslizando)
        {
            ContarTimerDesliz();
        }

        if(cambiandoCarril)
        {
            CambiarACarril();
            ContarTimerCambioCarril();
        }

        if(saltando)
        {
            ContarTimerSalto();
            EjecutarTrayectoriaSalto();
        }
    }

    #region Deslizamiento

    private void ContarTimerDesliz()
    {
        timerDesliz += Time.deltaTime;

        if(timerDesliz >= tiempoMaximoDesliz)
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

        tiempoMaximoDesliz = distanciaMaximaDesliz / velocidad;
        timerDesliz = 0;
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
        Vector3 vectorPerpendicular;
        Vector3 vectorParalelo;

        if(EjeMovimiento.Direccion == EjeDireccion.X)
        {
            vectorPerpendicular = Vector3.forward;     
            vectorParalelo = Vector3.right;     
        }
        else
        {
            vectorPerpendicular = Vector3.right;
            vectorParalelo = Vector3.forward; 
        }

        Vector3 vectorAjustador = Vector3.Scale(carrilFinal, vectorPerpendicular);

        gameObject.transform.position = vectorAjustador + Vector3.Scale(gameObject.transform.position, vectorParalelo) + Vector3.Scale(gameObject.transform.position, Vector3.up);
    }

    private void CambiarACarril()
    {
        float interpolador;

        interpolador = Mathf.Clamp(Mathf.Sqrt(timerCambioCarril / tiempoMaximoCambioCarril), 0.1F, 1);

        Vector3 vectorInterpolador = Vector3.Scale(Vector3.Lerp(carrilInicial,carrilFinal,interpolador), EjeMovimiento.VectorAxisPerpendicular);

        gameObject.transform.position = vectorInterpolador + Vector3.Scale(gameObject.transform.position, EjeMovimiento.VectorAxisParalelo) + Vector3.Scale(gameObject.transform.position, Vector3.up);
    }

    private void MoverseALaDerecha()
    {
        if (carrilActual < carriles.Count - 1)
        {
            //Si el carril al que quiere ir no está habilitado, entonces no cambia de carril.
            // if(!carriles[carrilActual + 1].Habilitado)
            // {
            //     return;
            // }

            carrilInicial = carriles[carrilActual].Posicion.transform.position;
            carrilFinal = carriles[carrilActual + 1].Posicion.transform.position;

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
            //Si el carril al que quiere ir no está habilitado, entonces no cambia de carril.
            // if(!carriles[carrilActual - 1].Habilitado)
            // {
            //     return;
            // }

            carrilInicial = carriles[carrilActual].Posicion.transform.position;
            carrilFinal = carriles[carrilActual - 1].Posicion.transform.position;

            carrilActual--;

            animatorJugador.SetTrigger("CambiarCarrilIzquierda");

            timerCambioCarril = 0;
            cambiandoCarril = true;
        }
    }

    #endregion

    #region Salto


    private void ContarTimerSalto()
    {
        timerSalto += Time.fixedDeltaTime;

        if(timerSalto >= tiempoMaximoSalto)
        {
            timerSalto = 0;
            saltando = false;
        }
    }

    private void Saltar()
    {
        Levantar();
        tiempoMaximoSalto = distanciaMaximaSalto / velocidad;
        saltando = true;
        animatorJugador.SetTrigger("salto");
    }
    private void EjecutarTrayectoriaSalto()
    {
        float altura = (-4 * Mathf.Pow(timerSalto / tiempoMaximoSalto, 2) + 4 * (timerSalto / tiempoMaximoSalto)) * alturaMaxima;

        Vector3 vectorAltura = (altura + alturaBase) * Orientacion;

        gameObject.transform.position = Vector3.Scale(gameObject.transform.position, Vector3.one - Orientacion) + vectorAltura;
    }

    #endregion

    #region Doblar

    private void DoblarDerecha()
    {
        if(ramaDisponible == null)
        {
            Debug.Log("Error al doblar, la rama es null.");
            return;
        }
        doblando = true;
        puedeDoblarDerecha = false;

        Carril carrilDestino = EncontrarCarrilMasCercanoEnRamaDisponible();

        gameObject.transform.position = Vector3.Scale(carrilDestino.Posicion.transform.position, ramaDisponible.EjeMovimiento.VectorAxisPerpendicular)
        + Vector3.Scale(gameObject.transform.position, ramaDisponible.EjeMovimiento.VectorAxisParalelo + Orientacion);

        gameObject.transform.Rotate(0, 90, 0, Space.World);

        EjeMovimiento.GirarDerecha();

        GameManager.Instancia.JugadorDoblo(ramaDisponible.PistaPadre, ramaDisponible);

        doblando = false;

        Debug.Log("Jugador doblo derecha");
    }

    private void DoblarIzquierda()
    {
        if(ramaDisponible == null)
        {
            Debug.Log("Error al doblar, la rama es null.");
            return;
        }
        doblando = true;
        puedeDoblarIzquierda = false;

        Carril carrilDestino = EncontrarCarrilMasCercanoEnRamaDisponible();

        gameObject.transform.position = Vector3.Scale(carrilDestino.Posicion.transform.position, ramaDisponible.EjeMovimiento.VectorAxisPerpendicular)
        + Vector3.Scale(gameObject.transform.position, ramaDisponible.EjeMovimiento.VectorAxisParalelo + Orientacion);

        gameObject.transform.Rotate(0, -90, 0, Space.World);

        EjeMovimiento.GirarIzquierda();

        GameManager.Instancia.JugadorDoblo(ramaDisponible.PistaPadre, ramaDisponible);

        doblando = false;

        Debug.Log("Jugador doblo izquierda");
    }

    private Carril EncontrarCarrilMasCercanoEnRamaDisponible()
    {
        List<Carril> carrilesRamaDisponible = ramaDisponible.GetComponentInChildren<BehaviourListaCarriles>().Carriles;

        Carril carrilMasCercano = carrilesRamaDisponible[0];

        float distanciaMinima = Mathf.Abs(Vector3.Dot(carrilesRamaDisponible[0].Posicion.transform.position - gameObject.transform.position, EjeMovimiento.Vectorizado));
        foreach(Carril carril in ramaDisponible.GetComponentInChildren<BehaviourListaCarriles>().Carriles)
        {
            float distancia =  Mathf.Abs(Vector3.Dot(carril.Posicion.transform.position - gameObject.transform.position, EjeMovimiento.Vectorizado));

            if(distancia < distanciaMinima)
            {
                distanciaMinima = distancia;
                carrilMasCercano = carril;
            }
        }

        carriles = ramaDisponible.gameObject.GetComponentInChildren<BehaviourListaCarriles>().Carriles;

        switch(carrilMasCercano.Tipo)
        {
            case TipoCarril.Izquierdo:
                carrilActual = 0;
                break;
            case TipoCarril.Central:
                carrilActual = 1;
                break;
            case TipoCarril.Derecho:
                carrilActual = 2;
                break;
        }

        return carrilMasCercano;
    }

    public void RamaDisponible(Rama nuevaRama)
    {
        switch (nuevaRama.TipoRama)
        {
            case TipoRama.Derecha:
                puedeDoblarDerecha = true;
                break;
            case TipoRama.Izquierda:
                puedeDoblarIzquierda = true;
                break;
        }

        ramaDisponible = nuevaRama;
    }

    public void InhabilitarDoblar()
    {
        puedeDoblarDerecha = false;
        puedeDoblarIzquierda = false;
        ramaDisponible = null;
    }

    #endregion

    private void MoverseHaciaAdelante()
    {
        gameObject.transform.Translate(EjeMovimiento.Vectorizado * velocidad * Time.deltaTime, Space.World);

        GameManager.Instancia.AddDistancia(Vector3.Dot(gameObject.transform.position, EjeMovimiento.Vectorizado) - Vector3.Dot(posicionAnterior, EjeMovimiento.Vectorizado));
        posicionAnterior = gameObject.transform.position;
    }

    //Este metodo es llamado por el script del detector de colisiones del jugador
    public void ColisionObstaculo(ObstacleType tipoObstaculo)
    {
        switch (tipoObstaculo)
        {
            case ObstacleType.Solid:
                vivo = false;
                GameManager.Instancia.InvocarGameOver();
                GameManager.Instancia.IntentoTerminado();
                break;
            case ObstacleType.Moneda:
                GameManager.Instancia.AddMonedas(1);
                break;
            case ObstacleType.FinPista:
                vivo = false;
                GameManager.Instancia.InvocarGameOver();
                GameManager.Instancia.IntentoTerminado();
                break;
            case ObstacleType.Rotura:
                vivo = false;
                GameManager.Instancia.InvocarGameOver();
                GameManager.Instancia.IntentoTerminado();
                break;

        }
    }

    public void NuevaSeccionPista(GameObject seccion)
    {
        carriles = seccion.GetComponentInChildren<BehaviourListaCarriles>().Carriles;
    }
}

