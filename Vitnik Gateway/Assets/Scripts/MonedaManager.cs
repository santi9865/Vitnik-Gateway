using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Analytics;

public class MonedaManager : MonoBehaviour
{
    //Variables Comunes

    [SerializeField] private bool debug;
    [SerializeField] private GameObject prefabMoneda;
    [SerializeField] private GameObject contenedorMonedas;
    [SerializeField] private int tamañoPool;
    [SerializeField] private float espaciado;
    //Altura a la que van a estar las monedas cuando indiquen al jugador que debe deslizarse para recogerlas.
    [SerializeField] private float alturaDesliz;
    //Largo máximo que puede tener una tirada de monedas (en monedas).
    [SerializeField] private int largoMaximoTirada;
    //Largo mínimo que puede tener una tirada de monedas (en monedas).
    [SerializeField] private int largoMinimoTirada;
    //Distancia que debe haber entre la última moneda de la tirada anterior y la primera de la siguiente tirada.
    [SerializeField] private float intervaloMinimoTiradas;
    [SerializeField] private float intervaloMaximoTiradas;
    [SerializeField] private BehaviourMovimientoJugador scriptJugador;
    private float distanciaMaxima;
    private List<GameObject> poolMonedas;


    //Variables Senda Principal

    private Vector3 posSiguienteMoneda;
    [SerializeField] private Vector3 posicionUltimaMonedaSpawneada;
    private int carrilUltimaMoneda = 1;
    private LugarObstaculo lugarActual;
    private GrupoObstaculos grupoCercano;
    private float distanciaSiguienteMonedaAlGrupoCercano;
    private bool tiradaEnCurso;
    private int largoTiradaActual; //Cantidad de monedas que se han spawneado para la tirada actual.
    private float proximoIntervalo;
    private int largoAleatorioTirada;

    //Variables Rama Derecha

    private Vector3 posSiguienteMonedaDerecha;
    private Vector3 posicionUltimaMonedaSpawneadaDerecha;
    private int carrilUltimaMonedaDerecha = 1;
    private LugarObstaculo lugarActualDerecha;
    private GrupoObstaculos grupoCercanoDerecha;
    private float distanciaSiguienteMonedaAlGrupoCercanoDerecha;
    private bool tiradaEnCursoDerecha;
    private int largoTiradaActualDerecha;
    private float proximoIntervaloDerecha;
    private int largoAleatorioTiradaDerecha;


    //Variables Rama Izquierda


    private Vector3 posSiguienteMonedaIzquierda;
    private Vector3 posicionUltimaMonedaSpawneadaIzquierda;
    private int carrilUltimaMonedaIzquierda = 1;
    private LugarObstaculo lugarActualIzquierda;
    private GrupoObstaculos grupoCercanoIzquierda;
    private float distanciaSiguienteMonedaAlGrupoCercanoIzquierda;
    private bool tiradaEnCursoIzquierda;
    private int largoTiradaActualIzquierda;
    private float proximoIntervaloIzquierda;
    private int largoAleatorioTiradaIzquierda;

    void Start()
    {
        poolMonedas = new List<GameObject>();

        for(int i = 0 ;i< tamañoPool; i++)
        {
            AgregarMonedaAlPool();
        }

        InicializarVariables();
    }

    private void InicializarVariables()
    {
        lugarActual = null;
        distanciaMaxima = Mathf.Max(scriptJugador.DistanciaDesliz, scriptJugador.DistanciaSalto, scriptJugador.DistanciaCambioCarril);
    }

    private GameObject AgregarMonedaAlPool()
    {
        GameObject moneda = Instantiate(prefabMoneda, Vector3.zero, prefabMoneda.transform.rotation);
        moneda.SetActive(false);
        moneda.transform.SetParent(contenedorMonedas.transform);

        poolMonedas.Add(moneda);

        //Debug.Log("Moneda Creada");

        return moneda;
    }

    private GameObject ObtenerMonedaDesactivada()
    {
        foreach(GameObject moneda in poolMonedas)
        {
            if(!moneda.activeSelf)
            {
                return moneda;
            }
        }

        return AgregarMonedaAlPool();
    }

    private GameObject PosicionarMoneda(GameObject moneda, Vector3 posicion, Quaternion rotacion)
    {
        moneda.transform.position = posicion;
        moneda.transform.rotation = rotacion;
        moneda.SetActive(true);
        return moneda;
    }


    // Calcula la distancia de la siguiente moneda a spawnear spawneada al grupo actual.
    // Si la moneda está antes del grupo la distancia es positiva, si la moneda está después la distancia es negativa.
    // El antes o despues depende del eje de movimiento. El antes es yendo hacia el después en la dirección y sentido del eje de movimiento.
    private float ActualizarDistanciaAlGrupoCercano(GrupoObstaculos grupo, Vector3 posicionMoneda, Eje eje)
    {
        return Vector3.Dot(grupo.Posicion - posicionMoneda, eje.Vectorizado);
    }

    private bool Atravesando(float distancia, Altura altura)
    {
        if(altura == Altura.Arriba && Mathf.Abs(distancia) < (scriptJugador.DistanciaSalto / 2))
        {
            return true;
        }
        else if(altura == Altura.Abajo && Mathf.Abs(distancia) < (scriptJugador.DistanciaDesliz / 2))
        {
            return true;
        }
        else
        {
            return false;
        }
    }

    private bool EstaLibreArriba(GrupoObstaculos grupo, int carril)
    {
        foreach(LugarObstaculo lugar in grupo.Lugares)
        {
            if(lugar.Carril == carril && lugar.Altura == Altura.Arriba)
            {
                return lugar.Libre;
            }
        }

        return false;
    }

    private bool ObstaculoActualAtravesado(float distancia)
    {
        if(distancia < -(distanciaMaxima / 2 + scriptJugador.DistanciaDesliz))
        {
            return true;
        }
        else
        {
            return false;
        }
    }

    private int BuscarCarrilHabilitadoAleatorio(List<Carril> carriles)
    {
        List<Carril> carrilesHabilitados = new List<Carril>();

        foreach(Carril carril in carriles)
        {
            if(carril.Habilitado)
            {
                carrilesHabilitados.Add(carril);
            }
        }

        return carriles.IndexOf(carrilesHabilitados[Random.Range(0, carrilesHabilitados.Count)]);
    }

    private int BuscarCarrilHabilitadoAleatorio(List<Carril> carrilesPrincipales, List<Carril> carrilesAConsiderar)
    {
        List<Carril> carrilesHabilitados = new List<Carril>();

        for(int i = 0; i < carrilesPrincipales.Count; i++)
        {
            if(carrilesPrincipales[i].Habilitado && carrilesAConsiderar[i].Habilitado)
            {
                carrilesHabilitados.Add(carrilesPrincipales[i]);
            }
        }

        return carrilesPrincipales.IndexOf(carrilesHabilitados[Random.Range(0, carrilesHabilitados.Count)]);
    }

    public void JugadorDoblo(TipoRama tipoRama)
    {
        switch(tipoRama)
        {
            case TipoRama.Derecha:
                posSiguienteMoneda = posSiguienteMonedaDerecha;
                posicionUltimaMonedaSpawneada = posicionUltimaMonedaSpawneadaDerecha;
                carrilUltimaMoneda = carrilUltimaMonedaDerecha;
                lugarActual = lugarActualDerecha;
                grupoCercano = grupoCercanoDerecha;
                distanciaSiguienteMonedaAlGrupoCercano = distanciaSiguienteMonedaAlGrupoCercanoDerecha;
                tiradaEnCurso = tiradaEnCursoDerecha;
                largoTiradaActual = largoTiradaActualDerecha;
                proximoIntervalo = proximoIntervaloDerecha;
                largoAleatorioTirada = largoAleatorioTiradaDerecha;
                break;
            case TipoRama.Izquierda:
                posSiguienteMoneda = posSiguienteMonedaIzquierda;
                posicionUltimaMonedaSpawneada = posicionUltimaMonedaSpawneadaIzquierda;
                carrilUltimaMoneda = carrilUltimaMonedaIzquierda;
                lugarActual = lugarActualIzquierda;
                grupoCercano = grupoCercanoIzquierda;
                distanciaSiguienteMonedaAlGrupoCercano = distanciaSiguienteMonedaAlGrupoCercanoIzquierda;
                tiradaEnCurso = tiradaEnCursoIzquierda;
                largoTiradaActual = largoTiradaActualIzquierda;
                proximoIntervalo = proximoIntervaloIzquierda;
                largoAleatorioTirada = largoAleatorioTiradaIzquierda;
                break;
        }
    }

    private bool CercaniaObstaculoActual(Altura altura)
    {
        if(altura == Altura.Arriba && Mathf.Abs(distanciaSiguienteMonedaAlGrupoCercano) < (scriptJugador.DistanciaSalto / 2) + scriptJugador.DistanciaCambioCarril)
        {
            return true;
        }
        else if(altura == Altura.Abajo && Mathf.Abs(distanciaSiguienteMonedaAlGrupoCercano) < (scriptJugador.DistanciaDesliz / 2) + scriptJugador.DistanciaCambioCarril)
        {
            return true;
        }
        else
        {
            return false;
        }
    }

    private float CalcularProximoIntervalo()
    {
        float intervalo = Random.Range(intervaloMinimoTiradas,intervaloMaximoTiradas);
        intervalo = intervalo - intervalo % espaciado;
        return intervalo;
    }

    private float PisoEnUnidad(float valor)
    {
        return valor - valor % espaciado;
    }

    private GrupoObstaculos EvaluarCercaniaAGrupos(Vector3 posicion, List<GrupoObstaculos> grupos, Vector3 vectorFiltro)
    {
        float distanciaPosicionAGrupoEvaluado;
        foreach(GrupoObstaculos grupo in grupos)
        {
            distanciaPosicionAGrupoEvaluado = Mathf.Abs(Vector3.Dot(posicion - grupo.Posicion, vectorFiltro));

            if(distanciaPosicionAGrupoEvaluado < distanciaMaxima / 2 + scriptJugador.DistanciaDesliz)
            {
                if(debug)
                {
                    Debug.Log("Grupo cercano encontrado en " + grupo.Posicion + " para la posicion " + posicion);
                }
                return grupo;
            }
        }

        if(debug)
        {
            Debug.Log("Grupo cercano no encontrado para la posicion " + posicion);
        }

        return null;
    }

    private bool EvaluarHabilitacion(int carril, List<Carril> carriles)
    {
        if(carriles[carril].Habilitado)
        {
            return true;
        }

        return false;
    }

    public void SpawnearMonedasRamaDerecha(GameObject pistaAnterior, GameObject pista, GameObject pistaSiguiente, bool reiniciar = false)
    {
        if(debug)
        {
            Debug.Log("Moneda Manager Debug, Spawnear Monedas Rama Derecha: ");
            Debug.Log("Pista anterior: " + pistaAnterior.name);
            Debug.Log("Pista actual: " + pista.name);
            Debug.Log("Pista siguiente: " + pistaSiguiente.name);
            Debug.Log("Reiniciar? " + reiniciar);
        }

        List<GameObject> nuevasMonedas = new List<GameObject>();

        BehaviourPista scriptPista = pista.GetComponent<BehaviourPista>();
        List<Carril> carrilesPista = pista.GetComponentInChildren<BehaviourListaCarriles>().Carriles;

        BehaviourPista scriptPistaAnterior = pistaAnterior.GetComponent<BehaviourPista>();
        List<Carril> carrilesPistaAnterior = pistaAnterior.GetComponentInChildren<BehaviourListaCarriles>().Carriles;

        BehaviourPista scriptPistaSiguiente = pistaSiguiente.GetComponent<BehaviourPista>();
        List<Carril> carrilesPistaSiguiente= pistaSiguiente.GetComponentInChildren<BehaviourListaCarriles>().Carriles;

        List<GrupoObstaculos> gruposRelevantes = new List<GrupoObstaculos>();

        if(reiniciar)
        {
            posSiguienteMonedaDerecha = pista.transform.position - (scriptPista.Longitud / 2) * scriptPista.EjeMovimiento.Vectorizado;
            largoTiradaActualDerecha = 0;
            tiradaEnCursoDerecha = false;
            grupoCercanoDerecha = null;
            lugarActualDerecha = null;
            carrilUltimaMonedaDerecha = BuscarCarrilHabilitadoAleatorio(carrilesPista);         
        }

        if(posSiguienteMonedaDerecha == null)
        {
            posSiguienteMonedaDerecha = pista.transform.position - (scriptPista.Longitud / 2) * scriptPista.EjeMovimiento.Vectorizado;
        }

        if(scriptPistaAnterior.gruposObstaculos != null && !reiniciar)
        {
            gruposRelevantes.Add(scriptPistaAnterior.gruposObstaculos[scriptPistaAnterior.gruposObstaculos.Count - 1]);
        }

        if(scriptPista.gruposObstaculos != null)
        {
            foreach(GrupoObstaculos grupo in scriptPista.gruposObstaculos)
            {
                gruposRelevantes.Add(grupo);
            }
        }

        if(scriptPistaSiguiente.gruposObstaculos != null)
        {
            gruposRelevantes.Add(scriptPistaSiguiente.gruposObstaculos[0]);
        }

        //Posición del final y comienzo de la pista.
        Vector3 posFinalPista = pista.transform.position + (scriptPista.Longitud / 2) * scriptPista.EjeMovimiento.Vectorizado;
        Vector3 posComienzoPista = pista.transform.position - (scriptPista.Longitud / 2) * scriptPista.EjeMovimiento.Vectorizado;

        int carrilMonedaSiguiente = carrilUltimaMonedaDerecha;
        float alturaSiguienteMoneda;

        float distanciaSiguienteMonedaAComienzoPista = Vector3.Dot(scriptPista.EjeMovimiento.Vectorizado, posSiguienteMonedaDerecha - posComienzoPista);

        float distanciaSiguienteMonedaAFinalPista = Vector3.Dot(scriptPista.EjeMovimiento.Vectorizado, posFinalPista - posSiguienteMonedaDerecha);

        if(distanciaSiguienteMonedaAComienzoPista < 0)
        {
            posSiguienteMonedaDerecha = posComienzoPista;
        }

        GameObject nuevaMoneda;

        bool obstaculoAtravesado;

        while(distanciaSiguienteMonedaAFinalPista > 0)
        {
            nuevaMoneda = ObtenerMonedaDesactivada();

            obstaculoAtravesado = true;

            if(!tiradaEnCursoDerecha)
            {
                if(gruposRelevantes.Count > 0)
                {
                    grupoCercanoDerecha = EvaluarCercaniaAGrupos(posSiguienteMonedaDerecha, gruposRelevantes, scriptPista.EjeMovimiento.Vectorizado);
                }

                if(grupoCercanoDerecha != null)
                {
                    //Si el comienzo del área de influencia del grupo de obstáculos está fuera de la pista entonces pone
                    //la siguiente moneda al final del área de influencia del grupo. En caso contrario pone la moneda al
                    //comienzo del área de influencia.

                    Vector3 comienzoGrupoCercano = grupoCercanoDerecha.Posicion - (distanciaMaxima / 2) * scriptPista.EjeMovimiento.Vectorizado;
                    float disComienzoPistaAComienzoGrupoCercano = Vector3.Dot(scriptPista.EjeMovimiento.Vectorizado, comienzoGrupoCercano - posComienzoPista);
                    if(disComienzoPistaAComienzoGrupoCercano < 0)
                    {
                        //Coloca la siguiente moneda en el final del grupo cercano.
                        posSiguienteMonedaDerecha = grupoCercanoDerecha.Posicion + (distanciaMaxima / 2) * scriptPista.EjeMovimiento.Vectorizado;
                    }
                    else
                    {
                        posSiguienteMonedaDerecha = comienzoGrupoCercano;
                    }
                }
                lugarActualDerecha = null;
                tiradaEnCursoDerecha = true;
                largoAleatorioTiradaDerecha = Random.Range(largoMinimoTirada, largoMaximoTirada);

                if(debug)
                {
                    Debug.Log("Largo siguiente tirada: " + largoAleatorioTiradaDerecha);
                }
            }

            if(grupoCercanoDerecha != null)
            {
                distanciaSiguienteMonedaAlGrupoCercanoDerecha = ActualizarDistanciaAlGrupoCercano(grupoCercanoDerecha, posSiguienteMonedaDerecha, scriptPista.EjeMovimiento);

                obstaculoAtravesado = ObstaculoActualAtravesado(distanciaSiguienteMonedaAlGrupoCercanoDerecha);

                if(obstaculoAtravesado)
                {
                    lugarActualDerecha = null;

                    if(gruposRelevantes.Count > 0)
                    {
                        grupoCercanoDerecha = EvaluarCercaniaAGrupos(posSiguienteMonedaDerecha, gruposRelevantes, scriptPista.EjeMovimiento.Vectorizado);
                    }
                }
            }
            else
            {
                lugarActualDerecha = null;

                if(gruposRelevantes.Count > 0)
                {
                    grupoCercanoDerecha = EvaluarCercaniaAGrupos(posSiguienteMonedaDerecha, gruposRelevantes, scriptPista.EjeMovimiento.Vectorizado);
                }
            }

            if(grupoCercanoDerecha != null)
            {
                distanciaSiguienteMonedaAlGrupoCercanoDerecha = ActualizarDistanciaAlGrupoCercano(grupoCercanoDerecha, posSiguienteMonedaDerecha, scriptPista.EjeMovimiento);

                if(lugarActualDerecha == null)
                {
                    lugarActualDerecha = grupoCercanoDerecha.DevolverLugarLibreAleatorioEnCarrilHabilitado(carrilUltimaMonedaDerecha);

                    if(lugarActualDerecha == null)
                    {
                        lugarActualDerecha = grupoCercanoDerecha.DevolverLugarLibreAleatorioHabilitado();
                    }
                }

                carrilMonedaSiguiente = lugarActualDerecha.Carril;
            }
            else
            {
                float disFinalPistaAnteriorSiguienteMoneda = Vector3.Dot(scriptPista.EjeMovimiento.Vectorizado, posSiguienteMonedaDerecha - posComienzoPista);
                float disFinalPistaSiguienteSiguienteMoneda = Vector3.Dot(scriptPista.EjeMovimiento.Vectorizado, posFinalPista - posSiguienteMonedaDerecha);

                if(disFinalPistaAnteriorSiguienteMoneda < distanciaMaxima / 2)
                {
                    if(!EvaluarHabilitacion(carrilMonedaSiguiente, carrilesPistaAnterior))
                    {
                        carrilMonedaSiguiente = BuscarCarrilHabilitadoAleatorio(carrilesPista, carrilesPistaAnterior);
                    }
                }
                else if(disFinalPistaSiguienteSiguienteMoneda < distanciaMaxima / 2)
                {
                    if(!EvaluarHabilitacion(carrilMonedaSiguiente, carrilesPistaSiguiente))
                    {
                        carrilMonedaSiguiente = BuscarCarrilHabilitadoAleatorio(carrilesPista, carrilesPistaSiguiente);
                    }
                }
                else if(!EvaluarHabilitacion(carrilMonedaSiguiente, carrilesPista))
                {
                    carrilMonedaSiguiente = BuscarCarrilHabilitadoAleatorio(carrilesPista);
                }
            }

            alturaSiguienteMoneda = scriptJugador.AlturaBase + 0.2f;

            if(lugarActualDerecha != null)
            {
                if(Atravesando(distanciaSiguienteMonedaAlGrupoCercanoDerecha, lugarActualDerecha.Altura))
                {
                    if(lugarActualDerecha.Altura == Altura.Arriba)
                    {
                        float distanciaRelativa = (scriptJugador.DistanciaSalto / 2 - distanciaSiguienteMonedaAlGrupoCercanoDerecha) / scriptJugador.DistanciaSalto;

                        alturaSiguienteMoneda = (-4 * Mathf.Pow(distanciaRelativa, 2) + 4 * (distanciaRelativa)) * scriptJugador.AlturaMaxima + scriptJugador.AlturaBase; 
                    }
                    else
                    {
                        if(!EstaLibreArriba(grupoCercanoDerecha, lugarActualDerecha.Carril))
                        {
                            alturaSiguienteMoneda = alturaDesliz;
                        }
                    }
                }
            }

            PosicionarMoneda(nuevaMoneda, Vector3.Scale(carrilesPista[carrilMonedaSiguiente].Posicion.transform.position
            , scriptPista.EjeMovimiento.VectorAxisPerpendicular) + Vector3.Scale(posSiguienteMonedaDerecha, scriptPista.EjeMovimiento.VectorAxisParalelo)
            + alturaSiguienteMoneda * Vector3.up , prefabMoneda.transform.rotation);

            posicionUltimaMonedaSpawneadaDerecha = nuevaMoneda.transform.position;

            nuevasMonedas.Add(nuevaMoneda);
            
            nuevaMoneda.GetComponent<BehaviourMoneda>().pistaAsociada = scriptPista;

            nuevaMoneda.transform.Rotate(0,Eje.EjeZPositivo.AngulosA(scriptPista.EjeMovimiento) ,0, Space.World);

            carrilUltimaMonedaDerecha = carrilMonedaSiguiente;

            largoTiradaActualDerecha++;

            if(largoTiradaActualDerecha > largoAleatorioTiradaDerecha && obstaculoAtravesado)
            {
                tiradaEnCursoDerecha = false;
                proximoIntervaloDerecha = CalcularProximoIntervalo();

                if(debug)
                {
                    Debug.Log("Tirada finalizada. Largo: " + largoTiradaActualDerecha + " , próximo intervalo: " + proximoIntervaloDerecha);
                }

                largoTiradaActualDerecha = 0;
            }

            if(tiradaEnCursoDerecha)
            {
                //La posición de la moneda sobre el eje de movimiento será la posición de la moneda anterior más la distancia establecida en "espaciado", pero en la
                //dirección del movimiento.
                posSiguienteMonedaDerecha = posicionUltimaMonedaSpawneadaDerecha + espaciado * scriptPista.EjeMovimiento.Vectorizado;
            }
            else
            {
                posSiguienteMonedaDerecha = posicionUltimaMonedaSpawneadaDerecha + proximoIntervaloDerecha * scriptPista.EjeMovimiento.Vectorizado;
            }

            distanciaSiguienteMonedaAFinalPista = Vector3.Dot(scriptPista.EjeMovimiento.Vectorizado, posFinalPista - posSiguienteMonedaDerecha);
        }

        scriptPista.monedas = nuevasMonedas;
    }

    public void SpawnearMonedasRamaIzquierda(GameObject pistaAnterior, GameObject pista, GameObject pistaSiguiente, bool reiniciar = false)
    {
        if(debug)
        {
            Debug.Log("Moneda Manager Debug, Spawnear Monedas Rama Izquierda: ");
            if(pistaAnterior != null)
                Debug.Log("Pista anterior: " + pistaAnterior.name);
            Debug.Log("Pista actual: " + pista.name);
            if(pistaSiguiente != null)
                Debug.Log("Pista siguiente: " + pistaSiguiente.name);
            Debug.Log("Reiniciar? " + reiniciar);
        }

        List<GameObject> nuevasMonedas = new List<GameObject>();

        BehaviourPista scriptPista = pista.GetComponent<BehaviourPista>();
        List<Carril> carrilesPista = pista.GetComponentInChildren<BehaviourListaCarriles>().Carriles;

        BehaviourPista scriptPistaAnterior = pistaAnterior.GetComponent<BehaviourPista>();
        List<Carril> carrilesPistaAnterior = pistaAnterior.GetComponentInChildren<BehaviourListaCarriles>().Carriles;

        BehaviourPista scriptPistaSiguiente = pistaSiguiente.GetComponent<BehaviourPista>();
        List<Carril> carrilesPistaSiguiente= pistaSiguiente.GetComponentInChildren<BehaviourListaCarriles>().Carriles;

        List<GrupoObstaculos> gruposRelevantes = new List<GrupoObstaculos>();

        if(reiniciar)
        {
            posSiguienteMonedaIzquierda = pista.transform.position - (scriptPista.Longitud / 2) * scriptPista.EjeMovimiento.Vectorizado;
            largoTiradaActualIzquierda = 0;
            tiradaEnCursoIzquierda = false;
            grupoCercanoIzquierda = null;
            lugarActualIzquierda = null;
            carrilUltimaMonedaIzquierda = BuscarCarrilHabilitadoAleatorio(carrilesPista);         
        }

        if(posSiguienteMonedaIzquierda == null)
        {
            posSiguienteMonedaIzquierda = pista.transform.position - (scriptPista.Longitud / 2) * scriptPista.EjeMovimiento.Vectorizado;
        }

        if(scriptPistaAnterior.gruposObstaculos != null && !reiniciar)
        {
            gruposRelevantes.Add(scriptPistaAnterior.gruposObstaculos[scriptPistaAnterior.gruposObstaculos.Count - 1]);
        }

        if(scriptPista.gruposObstaculos != null)
        {
            foreach(GrupoObstaculos grupo in scriptPista.gruposObstaculos)
            {
                gruposRelevantes.Add(grupo);
            }
        }

        if(scriptPistaSiguiente.gruposObstaculos != null)
        {
            gruposRelevantes.Add(scriptPistaSiguiente.gruposObstaculos[0]);
        }

        //Posición del final y comienzo de la pista.
        Vector3 posFinalPista = pista.transform.position + (scriptPista.Longitud / 2) * scriptPista.EjeMovimiento.Vectorizado;
        Vector3 posComienzoPista = pista.transform.position - (scriptPista.Longitud / 2) * scriptPista.EjeMovimiento.Vectorizado;

        int carrilMonedaSiguiente = carrilUltimaMonedaIzquierda;
        float alturaSiguienteMoneda;

        float distanciaSiguienteMonedaAComienzoPista = Vector3.Dot(scriptPista.EjeMovimiento.Vectorizado, posSiguienteMonedaIzquierda - posComienzoPista);

        float distanciaSiguienteMonedaAFinalPista = Vector3.Dot(scriptPista.EjeMovimiento.Vectorizado, posFinalPista - posSiguienteMonedaIzquierda);

        if(distanciaSiguienteMonedaAComienzoPista < 0)
        {
            posSiguienteMonedaIzquierda = posComienzoPista;
        }

        GameObject nuevaMoneda;

        bool obstaculoAtravesado;

        while(distanciaSiguienteMonedaAFinalPista > 0)
        {
            nuevaMoneda = ObtenerMonedaDesactivada();

            obstaculoAtravesado = true;

            if(!tiradaEnCursoIzquierda)
            {
                if(gruposRelevantes.Count > 0)
                {
                    grupoCercanoIzquierda = EvaluarCercaniaAGrupos(posSiguienteMonedaIzquierda, gruposRelevantes, scriptPista.EjeMovimiento.Vectorizado);
                }

                if(grupoCercanoIzquierda != null)
                {
                    //Si el comienzo del área de influencia del grupo de obstáculos está fuera de la pista entonces pone
                    //la siguiente moneda al final del área de influencia del grupo. En caso contrario pone la moneda al
                    //comienzo del área de influencia.

                    Vector3 comienzoGrupoCercano = grupoCercanoIzquierda.Posicion - (distanciaMaxima / 2) * scriptPista.EjeMovimiento.Vectorizado;
                    float disComienzoPistaAComienzoGrupoCercano = Vector3.Dot(scriptPista.EjeMovimiento.Vectorizado, comienzoGrupoCercano - posComienzoPista);
                    if(disComienzoPistaAComienzoGrupoCercano < 0)
                    {
                        //Coloca la siguiente moneda en el final del grupo cercano.
                        posSiguienteMonedaIzquierda = grupoCercanoIzquierda.Posicion + (distanciaMaxima / 2) * scriptPista.EjeMovimiento.Vectorizado;
                    }
                    else
                    {
                        posSiguienteMonedaIzquierda = comienzoGrupoCercano;
                    }
                }
                lugarActualIzquierda = null;
                tiradaEnCursoIzquierda = true;
                largoAleatorioTiradaIzquierda = Random.Range(largoMinimoTirada, largoMaximoTirada);

                if(debug)
                {
                    Debug.Log("Largo siguiente tirada: " + largoAleatorioTiradaIzquierda);
                }
            }

            if(grupoCercanoIzquierda != null)
            {
                distanciaSiguienteMonedaAlGrupoCercanoIzquierda = ActualizarDistanciaAlGrupoCercano(grupoCercanoIzquierda, posSiguienteMonedaIzquierda, scriptPista.EjeMovimiento);

                obstaculoAtravesado = ObstaculoActualAtravesado(distanciaSiguienteMonedaAlGrupoCercanoIzquierda);

                if(obstaculoAtravesado)
                {
                    lugarActualIzquierda = null;

                    if(gruposRelevantes.Count > 0)
                    {
                        grupoCercanoIzquierda = EvaluarCercaniaAGrupos(posSiguienteMonedaIzquierda, gruposRelevantes, scriptPista.EjeMovimiento.Vectorizado);
                    }
                }
            }
            else
            {
                lugarActualIzquierda = null;

                if(gruposRelevantes.Count > 0)
                {
                    grupoCercanoIzquierda = EvaluarCercaniaAGrupos(posSiguienteMonedaIzquierda, gruposRelevantes, scriptPista.EjeMovimiento.Vectorizado);
                }
            }

            if(grupoCercanoIzquierda != null)
            {
                distanciaSiguienteMonedaAlGrupoCercanoIzquierda = ActualizarDistanciaAlGrupoCercano(grupoCercanoIzquierda, posSiguienteMonedaIzquierda, scriptPista.EjeMovimiento);

                if(lugarActualIzquierda == null)
                {
                    lugarActualIzquierda = grupoCercanoIzquierda.DevolverLugarLibreAleatorioEnCarrilHabilitado(carrilUltimaMonedaIzquierda);

                    if(lugarActualIzquierda == null)
                    {
                        lugarActualIzquierda = grupoCercanoIzquierda.DevolverLugarLibreAleatorioHabilitado();
                    }
                }

                carrilMonedaSiguiente = lugarActualIzquierda.Carril;
            }
            else
            {
                float disFinalPistaAnteriorSiguienteMoneda = Vector3.Dot(scriptPista.EjeMovimiento.Vectorizado, posSiguienteMonedaIzquierda - posComienzoPista);
                float disFinalPistaSiguienteSiguienteMoneda = Vector3.Dot(scriptPista.EjeMovimiento.Vectorizado, posFinalPista - posSiguienteMonedaIzquierda);

                if(disFinalPistaAnteriorSiguienteMoneda < distanciaMaxima / 2)
                {
                    if(!EvaluarHabilitacion(carrilMonedaSiguiente, carrilesPistaAnterior))
                    {
                        carrilMonedaSiguiente = BuscarCarrilHabilitadoAleatorio(carrilesPista, carrilesPistaAnterior);
                    }
                }
                else if(disFinalPistaSiguienteSiguienteMoneda < distanciaMaxima / 2)
                {
                    if(!EvaluarHabilitacion(carrilMonedaSiguiente, carrilesPistaSiguiente))
                    {
                        carrilMonedaSiguiente = BuscarCarrilHabilitadoAleatorio(carrilesPista, carrilesPistaSiguiente);
                    }
                }
                else if(!EvaluarHabilitacion(carrilMonedaSiguiente, carrilesPista))
                {
                    carrilMonedaSiguiente = BuscarCarrilHabilitadoAleatorio(carrilesPista);
                }
            }

            alturaSiguienteMoneda = scriptJugador.AlturaBase + 0.2f;

            if(lugarActualIzquierda != null)
            {
                if(Atravesando(distanciaSiguienteMonedaAlGrupoCercanoIzquierda, lugarActualIzquierda.Altura))
                {
                    if(lugarActualIzquierda.Altura == Altura.Arriba)
                    {
                        float distanciaRelativa = (scriptJugador.DistanciaSalto / 2 - distanciaSiguienteMonedaAlGrupoCercanoIzquierda) / scriptJugador.DistanciaSalto;

                        alturaSiguienteMoneda = (-4 * Mathf.Pow(distanciaRelativa, 2) + 4 * (distanciaRelativa)) * scriptJugador.AlturaMaxima + scriptJugador.AlturaBase; 
                    }
                    else
                    {
                        if(!EstaLibreArriba(grupoCercanoIzquierda, lugarActualIzquierda.Carril))
                        {
                            alturaSiguienteMoneda = alturaDesliz;
                        }
                    }
                }
            }

            PosicionarMoneda(nuevaMoneda, Vector3.Scale(carrilesPista[carrilMonedaSiguiente].Posicion.transform.position
            , scriptPista.EjeMovimiento.VectorAxisPerpendicular) + Vector3.Scale(posSiguienteMonedaIzquierda, scriptPista.EjeMovimiento.VectorAxisParalelo)
            + alturaSiguienteMoneda * Vector3.up , prefabMoneda.transform.rotation);

            posicionUltimaMonedaSpawneadaIzquierda = nuevaMoneda.transform.position;

            nuevasMonedas.Add(nuevaMoneda);
            
            nuevaMoneda.GetComponent<BehaviourMoneda>().pistaAsociada = scriptPista;

            nuevaMoneda.transform.Rotate(0,Eje.EjeZPositivo.AngulosA(scriptPista.EjeMovimiento) ,0, Space.World);

            carrilUltimaMonedaIzquierda = carrilMonedaSiguiente;

            largoTiradaActualIzquierda++;

            if(largoTiradaActualIzquierda > largoAleatorioTiradaIzquierda && obstaculoAtravesado)
            {
                tiradaEnCursoIzquierda = false;
                proximoIntervaloIzquierda = CalcularProximoIntervalo();

                if(debug)
                {
                    Debug.Log("Tirada finalizada. Largo: " + largoTiradaActualIzquierda + " , próximo intervalo: " + proximoIntervaloIzquierda);
                }

                largoTiradaActualIzquierda = 0;
            }

            if(tiradaEnCursoIzquierda)
            {
                //La posición de la moneda sobre el eje de movimiento será la posición de la moneda anterior más la distancia establecida en "espaciado", pero en la
                //dirección del movimiento.
                posSiguienteMonedaIzquierda = posicionUltimaMonedaSpawneadaIzquierda + espaciado * scriptPista.EjeMovimiento.Vectorizado;
            }
            else
            {
                posSiguienteMonedaIzquierda = posicionUltimaMonedaSpawneadaIzquierda + proximoIntervaloIzquierda * scriptPista.EjeMovimiento.Vectorizado;
            }

            distanciaSiguienteMonedaAFinalPista = Vector3.Dot(scriptPista.EjeMovimiento.Vectorizado, posFinalPista - posSiguienteMonedaIzquierda);
        }

        scriptPista.monedas = nuevasMonedas;
    }

    public void SpawnearMonedasComienzoRamaDerecha(GameObject pista, GameObject pistaSiguiente)
    {
        BehaviourPista scriptPista = pista.GetComponent<BehaviourPista>();

        carrilUltimaMonedaDerecha = BuscarCarrilHabilitadoAleatorio(scriptPista.GetComponentInChildren<BehaviourListaCarriles>().Carriles);

        posSiguienteMonedaDerecha = pista.transform.position - (scriptPista.Longitud / 2) * scriptPista.EjeMovimiento.Vectorizado;

        Vector3 posFinalPista = pista.transform.position + (scriptPista.Longitud / 2) * scriptPista.EjeMovimiento.Vectorizado;

        float distanciaSiguienteMonedaAFinalPista = Vector3.Dot(posFinalPista - posSiguienteMonedaDerecha, scriptPista.EjeMovimiento.Vectorizado);

        while(distanciaSiguienteMonedaAFinalPista > distanciaMaxima / 2)
        {
            // posSiguienteMoneda = Vector3.Scale(posSiguienteMonedaDerecha + espaciado * scriptPista.EjeMovimiento.Vectorizado, scriptPista.EjeMovimiento.VectorAxisParalelo)
            // + scriptJugador.AlturaBase * Vector3.up + Vector3.Scale(carrilAleatorio.Posicion.transform.position, scriptPista.EjeMovimiento.VectorAxisPerpendicular);

            // posUltimaMoneda = posSiguienteMoneda;

            // PosicionarMoneda(ObtenerMonedaDesactivada(), posSiguienteMoneda, prefabMoneda.transform.rotation);
        }
    }

    public void SpawnearMonedasSendaPrincipal(GameObject pistaAnterior, GameObject pista, GameObject pistaSiguiente)
    {
        if(debug)
        {
            Debug.Log("Moneda Manager Debug: ");
            Debug.Log("Pista anterior: " + pistaAnterior.name);
            Debug.Log("Pista actual: " + pista.name);
            Debug.Log("Pista siguiente: " + pistaSiguiente.name);
        }

        List<GameObject> nuevasMonedas = new List<GameObject>();

        BehaviourPista scriptPista = pista.GetComponent<BehaviourPista>();
        List<Carril> carrilesPista = pista.GetComponentInChildren<BehaviourListaCarriles>().Carriles;

        BehaviourPista scriptPistaAnterior = pistaAnterior.GetComponent<BehaviourPista>();
        List<Carril> carrilesPistaAnterior = pistaAnterior.GetComponentInChildren<BehaviourListaCarriles>().Carriles;

        BehaviourPista scriptPistaSiguiente = pistaSiguiente.GetComponent<BehaviourPista>();
        List<Carril> carrilesPistaSiguiente= pistaSiguiente.GetComponentInChildren<BehaviourListaCarriles>().Carriles;

        List<GrupoObstaculos> gruposRelevantes = new List<GrupoObstaculos>();

        if(posSiguienteMoneda == null)
        {
            posSiguienteMoneda = pista.transform.position - (scriptPista.Longitud / 2) * scriptPista.EjeMovimiento.Vectorizado;
        }

        if(scriptPistaAnterior.gruposObstaculos != null)
        {
            gruposRelevantes.Add(scriptPistaAnterior.gruposObstaculos[scriptPistaAnterior.gruposObstaculos.Count - 1]);
        }

        if(scriptPista.gruposObstaculos != null)
        {
            foreach(GrupoObstaculos grupo in scriptPista.gruposObstaculos)
            {
                gruposRelevantes.Add(grupo);
            }
        }

        if(scriptPistaSiguiente.gruposObstaculos != null)
        {
            gruposRelevantes.Add(scriptPistaSiguiente.gruposObstaculos[0]);
        }

        //Posición del final y comienzo de la pista.
        Vector3 posFinalPista = pista.transform.position + (scriptPista.Longitud / 2) * scriptPista.EjeMovimiento.Vectorizado;
        Vector3 posComienzoPista = pista.transform.position - (scriptPista.Longitud / 2) * scriptPista.EjeMovimiento.Vectorizado;

        int carrilMonedaSiguiente = carrilUltimaMoneda;
        float alturaSiguienteMoneda;

        float distanciaSiguienteMonedaAComienzoPista = Vector3.Dot(scriptPista.EjeMovimiento.Vectorizado, posSiguienteMoneda - posComienzoPista);

        float distanciaSiguienteMonedaAFinalPista = Vector3.Dot(scriptPista.EjeMovimiento.Vectorizado, posFinalPista - posSiguienteMoneda);

        if(distanciaSiguienteMonedaAComienzoPista < 0)
        {
            posSiguienteMoneda = posComienzoPista;
        }

        GameObject nuevaMoneda;

        bool obstaculoAtravesado;

        while(distanciaSiguienteMonedaAFinalPista > 0)
        {
            nuevaMoneda = ObtenerMonedaDesactivada();

            obstaculoAtravesado = true;

            if(!tiradaEnCurso)
            {
                if(gruposRelevantes.Count > 0)
                {
                    grupoCercano = EvaluarCercaniaAGrupos(posSiguienteMoneda, gruposRelevantes, scriptPista.EjeMovimiento.Vectorizado);
                }

                if(grupoCercano != null)
                {
                    //Si el comienzo del área de influencia del grupo de obstáculos está fuera de la pista entonces pone
                    //la siguiente moneda al final del área de influencia del grupo. En caso contrario pone la moneda al
                    //comienzo del área de influencia.

                    Vector3 comienzoGrupoCercano = grupoCercano.Posicion - (distanciaMaxima / 2) * scriptPista.EjeMovimiento.Vectorizado;
                    float disComienzoPistaAComienzoGrupoCercano = Vector3.Dot(scriptPista.EjeMovimiento.Vectorizado, comienzoGrupoCercano - posComienzoPista);
                    if(disComienzoPistaAComienzoGrupoCercano < 0)
                    {
                        //Coloca la siguiente moneda en el final del grupo cercano.
                        posSiguienteMoneda = grupoCercano.Posicion + (distanciaMaxima / 2) * scriptPista.EjeMovimiento.Vectorizado;
                    }
                    else
                    {
                        posSiguienteMoneda = comienzoGrupoCercano;
                    }
                }
                lugarActual = null;
                tiradaEnCurso = true;
                largoAleatorioTirada = Random.Range(largoMinimoTirada, largoMaximoTirada);

                if(debug)
                {
                    Debug.Log("Largo siguiente tirada: " + largoAleatorioTirada);
                }
            }

            if(grupoCercano != null)
            {
                distanciaSiguienteMonedaAlGrupoCercano = ActualizarDistanciaAlGrupoCercano(grupoCercano, posSiguienteMoneda, scriptPista.EjeMovimiento);

                obstaculoAtravesado = ObstaculoActualAtravesado(distanciaSiguienteMonedaAlGrupoCercano);

                if(obstaculoAtravesado)
                {
                    lugarActual = null;

                    if(gruposRelevantes.Count > 0)
                    {
                        grupoCercano = EvaluarCercaniaAGrupos(posSiguienteMoneda, gruposRelevantes, scriptPista.EjeMovimiento.Vectorizado);
                    }
                }
            }
            else
            {
                lugarActual = null;

                if(gruposRelevantes.Count > 0)
                {
                    grupoCercano = EvaluarCercaniaAGrupos(posSiguienteMoneda, gruposRelevantes, scriptPista.EjeMovimiento.Vectorizado);
                }
            }

            if(grupoCercano != null)
            {
                distanciaSiguienteMonedaAlGrupoCercano = ActualizarDistanciaAlGrupoCercano(grupoCercano, posSiguienteMoneda, scriptPista.EjeMovimiento);

                if(lugarActual == null)
                {
                    lugarActual = grupoCercano.DevolverLugarLibreAleatorioEnCarrilHabilitado(carrilUltimaMoneda);

                    if(lugarActual == null)
                    {
                        lugarActual = grupoCercano.DevolverLugarLibreAleatorioHabilitado();
                    }
                }

                carrilMonedaSiguiente = lugarActual.Carril;
            }
            else
            {
                float disFinalPistaAnteriorSiguienteMoneda = Vector3.Dot(scriptPista.EjeMovimiento.Vectorizado, posSiguienteMoneda - posComienzoPista);
                float disFinalPistaSiguienteSiguienteMoneda = Vector3.Dot(scriptPista.EjeMovimiento.Vectorizado, posFinalPista - posSiguienteMoneda);

                if(disFinalPistaAnteriorSiguienteMoneda < distanciaMaxima / 2)
                {
                    if(!EvaluarHabilitacion(carrilMonedaSiguiente, carrilesPistaAnterior))
                    {
                        carrilMonedaSiguiente = BuscarCarrilHabilitadoAleatorio(carrilesPista, carrilesPistaAnterior);
                    }
                }
                else if(disFinalPistaSiguienteSiguienteMoneda < distanciaMaxima / 2)
                {
                    if(!EvaluarHabilitacion(carrilMonedaSiguiente, carrilesPistaSiguiente))
                    {
                        carrilMonedaSiguiente = BuscarCarrilHabilitadoAleatorio(carrilesPista, carrilesPistaSiguiente);
                    }
                }
                else if(!EvaluarHabilitacion(carrilMonedaSiguiente, carrilesPista))
                {
                    carrilMonedaSiguiente = BuscarCarrilHabilitadoAleatorio(carrilesPista);
                }
            }

            alturaSiguienteMoneda = scriptJugador.AlturaBase + 0.2f;

            if(lugarActual != null)
            {
                if(Atravesando(distanciaSiguienteMonedaAlGrupoCercano, lugarActual.Altura))
                {
                    if(lugarActual.Altura == Altura.Arriba)
                    {
                        float distanciaRelativa = (scriptJugador.DistanciaSalto / 2 - distanciaSiguienteMonedaAlGrupoCercano) / scriptJugador.DistanciaSalto;

                        alturaSiguienteMoneda = (-4 * Mathf.Pow(distanciaRelativa, 2) + 4 * (distanciaRelativa)) * scriptJugador.AlturaMaxima + scriptJugador.AlturaBase; 
                    }
                    else
                    {
                        if(!EstaLibreArriba(grupoCercano, lugarActual.Carril))
                        {
                            alturaSiguienteMoneda = alturaDesliz;
                        }
                    }
                }
            }

            PosicionarMoneda(nuevaMoneda, Vector3.Scale(carrilesPista[carrilMonedaSiguiente].Posicion.transform.position
            , scriptPista.EjeMovimiento.VectorAxisPerpendicular) + Vector3.Scale(posSiguienteMoneda, scriptPista.EjeMovimiento.VectorAxisParalelo)
            + alturaSiguienteMoneda * Vector3.up , prefabMoneda.transform.rotation);

            posicionUltimaMonedaSpawneada = nuevaMoneda.transform.position;

            nuevasMonedas.Add(nuevaMoneda);
            
            nuevaMoneda.GetComponent<BehaviourMoneda>().pistaAsociada = scriptPista;

            nuevaMoneda.transform.Rotate(0,Eje.EjeZPositivo.AngulosA(scriptPista.EjeMovimiento) ,0, Space.World);

            carrilUltimaMoneda = carrilMonedaSiguiente;

            largoTiradaActual++;

            if(largoTiradaActual > largoAleatorioTirada && obstaculoAtravesado)
            {
                tiradaEnCurso = false;
                proximoIntervalo = CalcularProximoIntervalo();

                if(debug)
                {
                    Debug.Log("Tirada finalizada. Largo: " + largoTiradaActual + " , próximo intervalo: " + proximoIntervalo);
                }

                largoTiradaActual = 0;
            }

            if(tiradaEnCurso)
            {
                //La posición de la moneda sobre el eje de movimiento será la posición de la moneda anterior más la distancia establecida en "espaciado", pero en la
                //dirección del movimiento.
                posSiguienteMoneda = posicionUltimaMonedaSpawneada + espaciado * scriptPista.EjeMovimiento.Vectorizado;
            }
            else
            {
                posSiguienteMoneda = posicionUltimaMonedaSpawneada + proximoIntervalo * scriptPista.EjeMovimiento.Vectorizado;
            }

            distanciaSiguienteMonedaAFinalPista = Vector3.Dot(scriptPista.EjeMovimiento.Vectorizado, posFinalPista - posSiguienteMoneda);
        }

        scriptPista.monedas = nuevasMonedas;
    }
}
