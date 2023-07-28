using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MonedaManager : MonoBehaviour
{

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
    [SerializeField] private float intervaloTiradas;

    [SerializeField] private BehaviourMovimientoJugador scriptJugador;

    [SerializeField] private Vector3 posicionUltimaMonedaSpawneada;

    private int carrilUltimaMoneda;
    private float distanciaMaxima;

    private List<GrupoObstaculos> grupos;
    private LugarObstaculo lugarActual;
    private List<GameObject> poolMonedas;

    private float distanciaSiguienteMonedaAlGrupoActual;
    private bool grupoActualAtravesado;
    private bool atravesandoObstaculo;
    private bool continuarTirada;

    private bool colocadaPrimerMoneda;

    //Cantidad de monedas que se han spawneado para la tirada actual.
    private int largoTiradaActual;


    // Start is called before the first frame update
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
        grupos = new List<GrupoObstaculos>();
        lugarActual = null;
        colocadaPrimerMoneda = false;
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
    private void ActualizarDistanciaAlGrupoActual(Eje eje)
    {
        distanciaSiguienteMonedaAlGrupoActual = Vector3.Dot(grupos[0].Posicion - (posicionUltimaMonedaSpawneada + espaciado * eje.Vectorizado), eje.Vectorizado);
    }

    private void AgregarGrupos(List<GrupoObstaculos> gruposNuevos)
    {
        GrupoObstaculos ultimoGrupo;

        if(gruposNuevos == null)
        {
            return;
        }

        if(gruposNuevos.Count == 0)
        {
            return;
        }

        if(grupos.Count > 0)
        {
            ultimoGrupo = grupos[0];
            grupos.Clear();
            grupos.Add(ultimoGrupo);
        }

        foreach(GrupoObstaculos grupo in gruposNuevos)
        {
            grupos.Add(grupo);
        }
    }

    private void Atravesando(Altura altura)
    {
        if(altura == Altura.Arriba && Mathf.Abs(distanciaSiguienteMonedaAlGrupoActual) < (scriptJugador.DistanciaSalto / 2))
        {
            atravesandoObstaculo = true;
        }
        else if(altura == Altura.Abajo && Mathf.Abs(distanciaSiguienteMonedaAlGrupoActual) < (scriptJugador.DistanciaDesliz / 2))
        {
            atravesandoObstaculo = true;
        }
        else
        {
            atravesandoObstaculo = false;
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

    private bool ObstaculoActualAtravesado(Altura altura)
    {
        if(altura == Altura.Arriba && distanciaSiguienteMonedaAlGrupoActual < -(scriptJugador.DistanciaSalto / 2))
        {
            return true;
        }
        else if(altura == Altura.Abajo && distanciaSiguienteMonedaAlGrupoActual < -(scriptJugador.DistanciaDesliz / 2))
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

        return Random.Range(0, carrilesHabilitados.Count);
    }

    private void CortarGruposHasta(int hasta)
    {
        for(int i = 0; i <= hasta; i++)
        {
            grupos.RemoveAt(0);
        }
    }

    public void JugadorDoblo()
    {
        colocadaPrimerMoneda = false;
        continuarTirada = false;
        largoTiradaActual = 0;
    }

    private LugarObstaculo BuscarLugarLibreEnCarril(int carril, List<LugarObstaculo> lugares)
    {
        foreach(LugarObstaculo lugar in lugares)
        {
            if(lugar.Carril == carril && lugar.Libre)
            {
                return lugar;
            }
        }

        return null;
    }

    //Las monedas solamente pueden spawnearse sobre la pista que se provee.
    public void SpawnearMonedas(GameObject pista, Vector3 posicionPrimeraMoneda, bool posicionValida)
    {
        BehaviourPista scriptPista = pista.GetComponent<BehaviourPista>();
        List<Carril> carrilesPista = pista.GetComponentInChildren<BehaviourListaCarriles>().Carriles;

        AgregarGrupos(scriptPista.gruposObstaculos);

        List<GameObject> nuevasMonedas = new List<GameObject>();

        Vector3 posicionSiguienteMoneda;

        Vector3 finalPista = pista.transform.position + (scriptPista.Longitud / 2) * scriptPista.EjeMovimiento.Vectorizado;  

        if(!colocadaPrimerMoneda)
        {
            posicionUltimaMonedaSpawneada = pista.transform.position - (scriptPista.Longitud / 2) * scriptPista.EjeMovimiento.Vectorizado;
            carrilUltimaMoneda = BuscarCarrilHabilitadoAleatorio(carrilesPista);
            colocadaPrimerMoneda = true;
        }

        if(posicionPrimeraMoneda != null && posicionValida)
        {
            posicionSiguienteMoneda = posicionPrimeraMoneda;
            continuarTirada = true;
            atravesandoObstaculo = false;
            largoTiradaActual = 0;

            if(grupos != null && grupos.Count > 0)
            {
                //Si el último grupo de obstáculos está a una distancia menor a la mitad de la acción más larga del jugador más dos cambios de carril del final
                // de la pista entonces se considera el final de la pista como la posición del grupo de obstáculos más la mitad de la acción más larga del jugador.
                //Si no el final de la pista considerado será el final real de la pista menos la mitad de la acción más larga del jugador más 2 cambios de carril.

                if(Vector3.Dot(finalPista - grupos[grupos.Count - 1].Posicion, scriptPista.EjeMovimiento.Vectorizado) < (distanciaMaxima / 2 + scriptJugador.DistanciaCambioCarril * 2))
                {
                    finalPista = grupos[grupos.Count - 1].Posicion + (distanciaMaxima / 2) * scriptPista.EjeMovimiento.Vectorizado;
                }
                else
                {
                    finalPista -= (distanciaMaxima / 2 + scriptJugador.DistanciaCambioCarril * 2) * scriptPista.EjeMovimiento.Vectorizado;
                }
            }
            else
            {

                //Si la pista no tiene ningún grupo de obstáculos entonces se considera el final de la pista como el final real de la pista
                // menos la mitad de la acción más larga del jugador más 2 cambios de carril.
                finalPista -= (distanciaMaxima / 2 + scriptJugador.DistanciaCambioCarril * 2) * scriptPista.EjeMovimiento.Vectorizado;
            }
        }
        else
        {
            posicionSiguienteMoneda = posicionUltimaMonedaSpawneada + espaciado * scriptPista.EjeMovimiento.Vectorizado;

            if(!continuarTirada)
            {
                float distanciaASiguienteTirada = intervaloTiradas;

                //Como no sabemos la posición del último grupo de obstáculos de la pista anterior entonces se comienza a una distancia igual a la mitad
                // de la acción mas larga del jugador más dos cambios de carril.

                float distanciaComienzoTiradaAComienzoPista = Vector3.Dot((pista.transform.position - (scriptPista.Longitud / 2) * scriptPista.EjeMovimiento.Vectorizado) 
                    - (posicionUltimaMonedaSpawneada + distanciaASiguienteTirada * scriptPista.EjeMovimiento.Vectorizado), scriptPista.EjeMovimiento.Vectorizado);

                if(distanciaComienzoTiradaAComienzoPista > -(distanciaMaxima / 2 + scriptJugador.DistanciaCambioCarril))
                {
                    distanciaASiguienteTirada = Vector3.Dot((pista.transform.position - (scriptPista.Longitud / 2) * scriptPista.EjeMovimiento.Vectorizado
                    + (distanciaMaxima / 2 + scriptJugador.DistanciaCambioCarril) * scriptPista.EjeMovimiento.Vectorizado) 
                    - posicionUltimaMonedaSpawneada, scriptPista.EjeMovimiento.Vectorizado);
                }

                //Si la tirada fuese a comenzar en el medio del rango de acción de un grupo de obstáculos entonces se corre la
                //distanciaASiguienteTirada hasta el final de dicho grupo de obstáculos más la distancia necesaria para hacer dos cambios de carril
                // y la mitad de la acción más larga del jugador.
                if(grupos != null && grupos.Count > 0)
                {
                    int indiceUltimoGrupoEvaluado = -1;
                    for(int i = 0; i < grupos.Count; i++)
                    {
                        if(Mathf.Abs(Vector3.Dot(grupos[i].Posicion - (posicionUltimaMonedaSpawneada + scriptPista.EjeMovimiento.Vectorizado * distanciaASiguienteTirada)
                        , scriptPista.EjeMovimiento.Vectorizado)) < distanciaMaxima / 2)
                        {
                            distanciaASiguienteTirada = Vector3.Dot((grupos[i].Posicion + (distanciaMaxima / 2 
                            + scriptJugador.DistanciaCambioCarril * 2) * scriptPista.EjeMovimiento.Vectorizado) 
                            - posicionUltimaMonedaSpawneada , scriptPista.EjeMovimiento.Vectorizado);

                            indiceUltimoGrupoEvaluado = i;
                        }
                    }

                    CortarGruposHasta(indiceUltimoGrupoEvaluado);

                    //Si el último grupo de obstáculos está a una distancia menor a la mitad de la acción más larga del jugador más dos cambios de carril del final
                    // de la pista entonces se considera el final de la pista como la posición del grupo de obstáculos más la mitad de la acción más larga del jugador.
                    //Si no el final de la pista considerado será el final real de la pista menos la mitad de la acción más larga del jugador más 1 cambios de carril.

                    if(grupos.Count > 0)
                    {
                        if(Vector3.Dot(finalPista - grupos[grupos.Count - 1].Posicion, scriptPista.EjeMovimiento.Vectorizado) < (distanciaMaxima / 2 + scriptJugador.DistanciaCambioCarril))
                        {
                            finalPista = grupos[grupos.Count - 1].Posicion + (distanciaMaxima / 2) * scriptPista.EjeMovimiento.Vectorizado;
                        }
                        else
                        {
                            finalPista -= (distanciaMaxima / 2 + scriptJugador.DistanciaCambioCarril) * scriptPista.EjeMovimiento.Vectorizado;
                        }
                    }
                    else
                    {
                        finalPista -= (distanciaMaxima / 2 + scriptJugador.DistanciaCambioCarril) * scriptPista.EjeMovimiento.Vectorizado;
                    }

                }
                else
                {

                    //Si la pista no tiene ningún grupo de obstáculos entonces se considera el final de la pista como el final real de la pista
                    // menos la mitad de la acción más larga del jugador más 2 cambios de carril.
                    finalPista -= (distanciaMaxima / 2 + scriptJugador.DistanciaCambioCarril * 2) * scriptPista.EjeMovimiento.Vectorizado;
                }

                //Si el espacio es suficiente desde el inicio de la tirada hasta el final de la pista
                //para admitir la tirada mínima de monedas entonces se permite que inicie una tirada.

                if(Vector3.Dot(finalPista - (posicionUltimaMonedaSpawneada + scriptPista.EjeMovimiento.Vectorizado * distanciaASiguienteTirada)
                , scriptPista.EjeMovimiento.Vectorizado) >= largoMinimoTirada * espaciado)
                {
                    continuarTirada = true;
                    largoTiradaActual = 0;
                    posicionSiguienteMoneda = posicionUltimaMonedaSpawneada + distanciaASiguienteTirada * scriptPista.EjeMovimiento.Vectorizado;
                }
            }
        }

        //Las monedas se spawnearan solamente si la siguiente moneda que se fuera a spawnear lo hiciera antes del final de la pista brindada
        //y respetando el máximo de tirada posible.
        while(Vector3.Dot(finalPista - posicionSiguienteMoneda, scriptPista.EjeMovimiento.Vectorizado) >= 0 
        && continuarTirada)
        {
            int carrilNuevaMoneda;
            float alturaNuevaMoneda;

            GameObject nuevaMoneda = ObtenerMonedaDesactivada();

            if(grupos.Count > 0)
            {
                ActualizarDistanciaAlGrupoActual(scriptPista.EjeMovimiento);

                if(lugarActual == null)
                {
                    LugarObstaculo probarLugarContinuo = BuscarLugarLibreEnCarril(carrilUltimaMoneda, grupos[0].Lugares);
                    if(probarLugarContinuo != null)
                    {
                        lugarActual = probarLugarContinuo;
                    }
                    else
                    {
                        lugarActual = grupos[0].DevolverLugarLibreAleatorioHabilitado();
                    }
                }

                grupoActualAtravesado = ObstaculoActualAtravesado(lugarActual.Altura);

                if(grupoActualAtravesado)
                {
                    grupos.RemoveAt(0);
                    grupoActualAtravesado = false;

                    if(grupos.Count > 0)
                    {
                        LugarObstaculo probarLugarContinuo = BuscarLugarLibreEnCarril(carrilUltimaMoneda, grupos[0].Lugares);
                        if(probarLugarContinuo != null)
                        {
                            lugarActual = probarLugarContinuo;
                        }
                        else
                        {
                            lugarActual = grupos[0].DevolverLugarLibreAleatorioHabilitado();
                        }

                        carrilNuevaMoneda = lugarActual.Carril;
                    }
                    else
                    {
                        lugarActual = null;
                        carrilNuevaMoneda = carrilUltimaMoneda;
                        atravesandoObstaculo = false;
                    }
                }

                if(lugarActual != null)
                {
                    Atravesando(lugarActual.Altura);
                }

                if(atravesandoObstaculo)
                {
                    carrilNuevaMoneda = lugarActual.Carril;
                    carrilUltimaMoneda = carrilNuevaMoneda;

                    if(lugarActual.Altura == Altura.Arriba)
                    {
                        float distanciaRelativa = (scriptJugador.DistanciaSalto / 2 - distanciaSiguienteMonedaAlGrupoActual) / scriptJugador.DistanciaSalto;

                        alturaNuevaMoneda = (-4 * Mathf.Pow(distanciaRelativa, 2) + 4 * (distanciaRelativa)) * scriptJugador.AlturaMaxima + scriptJugador.AlturaBase;                
                    }
                    else
                    {
                        if(EstaLibreArriba(grupos[0], lugarActual.Carril))
                        {
                            alturaNuevaMoneda = scriptJugador.AlturaBase;
                        }
                        else
                        {
                            alturaNuevaMoneda = alturaDesliz;
                        }
                    }
                }
                else
                {
                    if(!carrilesPista[carrilUltimaMoneda].Habilitado)
                    {
                        carrilNuevaMoneda = BuscarCarrilHabilitadoAleatorio(carrilesPista);
                        carrilUltimaMoneda = carrilNuevaMoneda;
                    }
                    else
                    {
                        carrilNuevaMoneda = carrilUltimaMoneda;
                    }

                    alturaNuevaMoneda = scriptJugador.AlturaBase;
                }
                
            }
            else
            {
                //Si el carril de la última moneda spawneada está inhabilitado entonces elige otro carril al azar.
                if(!carrilesPista[carrilUltimaMoneda].Habilitado)
                {
                    carrilNuevaMoneda = BuscarCarrilHabilitadoAleatorio(carrilesPista);
                }
                else
                {
                    carrilNuevaMoneda = carrilUltimaMoneda;
                }

                alturaNuevaMoneda = scriptJugador.AlturaBase;
            }

            PosicionarMoneda(nuevaMoneda, Vector3.Scale(carrilesPista[carrilNuevaMoneda].Posicion.transform.position
            , scriptPista.EjeMovimiento.VectorAxisPerpendicular) + Vector3.Scale(posicionSiguienteMoneda, scriptPista.EjeMovimiento.VectorAxisParalelo)
            + alturaNuevaMoneda * Vector3.up , prefabMoneda.transform.rotation);

            posicionUltimaMonedaSpawneada = nuevaMoneda.transform.position;

            nuevasMonedas.Add(nuevaMoneda);
            
            nuevaMoneda.GetComponent<BehaviourMoneda>().pistaAsociada = scriptPista;

            nuevaMoneda.transform.Rotate(0,Eje.EjeZPositivo.AngulosA(scriptPista.EjeMovimiento) ,0, Space.World);

            carrilUltimaMoneda = carrilNuevaMoneda;

            largoTiradaActual++;

            if(largoTiradaActual >= largoMaximoTirada && !atravesandoObstaculo)
            {
                continuarTirada = false;
            }

            posicionSiguienteMoneda = posicionUltimaMonedaSpawneada + espaciado * scriptPista.EjeMovimiento.Vectorizado;
        }

        scriptPista.monedas = nuevasMonedas;
    }

    // public void SpawnearMonedas(GameObject pista, Vector3 posicionPrimeraMoneda)
    // {
    //     if(posicionPrimeraMoneda != null)
    //     {
    //         posicionUltimaMonedaSpawneada = posicionPrimeraMoneda;
    //     }

    //     BehaviourPista scriptPista = pista.GetComponent<BehaviourPista>();
    //     scriptPista.monedas = new List<GameObject>();

    //     AgregarGrupos(scriptPista.gruposObstaculos);

    //     while(Vector3.Dot(pista.transform.position + ((scriptPista.Longitud / 2) - (distanciaMaxima / 2)) * scriptPista.EjeMovimiento.Vectorizado - 
    //     posicionUltimaMonedaSpawneada, scriptPista.EjeMovimiento.Vectorizado) > espaciado)
    //     {
    //         GameObject nuevaMoneda = ObtenerMonedaDesactivada();

    //         float distanciaGrupoActualAlSiguienteGrupo;

    //         ActualizarDistanciaAlGrupoActual(scriptPista.EjeMovimiento);

    //         if(grupoActualAtravesado)
    //         {
    //             if(grupoActual + 1 < grupos.Count)
    //             {
    //                 distanciaGrupoActualAlSiguienteGrupo = Vector3.Dot(grupos[grupoActual].Posicion - grupos[grupoActual + 1].Posicion, scriptPista.EjeMovimiento.Vectorizado);

    //                 lugarActual = grupos[grupoActual + 1].DevolverLugarLibreAleatorioHabilitado();

    //                 grupoActual++;
    //                 grupoActualAtravesado = false;
    //             }

    //             ActualizarDistanciaAlGrupoActual(scriptPista.EjeMovimiento);
    //         }
    //         else
    //         {
    //             CercaniaObstaculo();
    //         }

    //         if(cercaAObstaculo)
    //         {
    //             Atravesando(lugarActual.Altura);
    //         }
    //         else
    //         {
    //             if(distanciaUltimaMonedaAlGrupoActual - espaciado < -(distanciaMaxima / 2))
    //             {
    //                 grupoActualAtravesado = true;
    //             }
    //         }

    //         float altura;

    //         if(atravesandoObstaculo)
    //         {
    //             if(lugarActual.Altura == Altura.Arriba)
    //             {
    //                 float distanciaRelativa = (scriptJugador.DistanciaSalto / 2 - (distanciaUltimaMonedaAlGrupoActual - espaciado)) / scriptJugador.DistanciaSalto;

    //                 altura = (-4 * Mathf.Pow(distanciaRelativa, 2) + 4 * (distanciaRelativa)) * scriptJugador.AlturaMaxima + scriptJugador.AlturaBase;                
    //             }
    //             else
    //             {
    //                 if(EstaLibreArriba(grupos[grupoActual], lugarActual.Carril))
    //                 {
    //                     altura = scriptJugador.AlturaBase;
    //                 }
    //                 else
    //                 {
    //                     altura = alturaDesliz;
    //                 }
    //             }
    //         }
    //         else
    //         {
    //             altura = scriptJugador.AlturaBase;
    //         }

    //         PosicionarMoneda(nuevaMoneda, Vector3.Scale(grupos[grupoActual].Carriles[lugarActual.Carril].Posicion.transform.position, scriptJugador.EjeMovimiento.VectorAxisPerpendicular)
    //         + altura * Vector3.up + Vector3.Scale(posicionUltimaMonedaSpawneada, scriptPista.EjeMovimiento.VectorAxisParalelo)
    //         + espaciado * scriptPista.EjeMovimiento.Vectorizado, prefabMoneda.transform.rotation);

    //         scriptPista.monedas.Add(nuevaMoneda);

    //         posicionUltimaMonedaSpawneada = nuevaMoneda.transform.position;
            
    //         nuevaMoneda.GetComponent<BehaviourMoneda>().pistaAsociada = scriptPista;

    //         nuevaMoneda.transform.Rotate(0,Eje.EjeZPositivo.AngulosA(scriptJugador.EjeMovimiento) ,0);
    //     }
    //}
}
