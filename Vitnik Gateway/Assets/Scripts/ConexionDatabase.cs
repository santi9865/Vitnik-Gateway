using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mono.Data.Sqlite;
using System.Data;
using System.IO;

public class ConexionDatabase : MonoBehaviour
{

    [SerializeField] private string nombreDatabase;
    [SerializeField] private string tabla;
    private string dbURI;
    private SqliteConnection conexionDb;

    void Start()
    {

    }

    private void AbrirConexion()
    {
        if(conexionDb == null)
        {
            dbURI = "URI=file:" + "Assets/Databases/"+ nombreDatabase;

            conexionDb = new SqliteConnection(dbURI);
        }
        
        conexionDb.Open();

        if(conexionDb.State == ConnectionState.Open)
        {
            Debug.Log("La conexión esta abierta.");
        }
    }

    private void CerrarConexion()
    {
        conexionDb.Close();
        if(conexionDb.State == ConnectionState.Closed)
        {
            Debug.Log("La conexión esta cerrada.");
        }
    }

    //Estos métodos sirven para no tener que repetir la creación de un comando para cada
    //pedido a la base de datos;

    private SqliteDataReader EjecutarComandoConRetorno(string command)
    {
        SqliteCommand newCommand = conexionDb.CreateCommand();
        newCommand.CommandText = command;

        SqliteDataReader reader = newCommand.ExecuteReader();

        return reader;
    }

    private void EjecutarComandoSinRetorno(string command)
    {
        SqliteCommand newCommand = conexionDb.CreateCommand();
        newCommand.CommandText = command;

        newCommand.ExecuteNonQuery();
    }

    public System.Object ObtenerPrimerValor(string columna)
    {
        AbrirConexion();

        System.Object resultado = null;

        SqliteDataReader reader = EjecutarComandoConRetorno($"SELECT {columna} FROM {tabla}");

        if(reader.Read())
        {
            resultado = reader.GetValue(0);
        }

        CerrarConexion();

        return resultado;
    }

    public System.Object ObtenerValorSegunID(int id, string columna)
    {
        AbrirConexion();

        System.Object resultado = null;

        SqliteDataReader reader = EjecutarComandoConRetorno($"SELECT {columna} FROM {tabla} WHERE ID = {id}");

        if(reader.Read())
        {
            resultado = reader.GetValue(0);
        }

        CerrarConexion();

        return resultado;
    }

    public List<object> ObtenerValoresSegunID(int id)
    {
        AbrirConexion();

        List<System.Object> resultados = new List<System.Object>();

        SqliteDataReader reader = EjecutarComandoConRetorno($"SELECT * FROM {tabla} WHERE ID = {id}");

        if(reader.Read())
        {
            for(int i = 0; i < reader.FieldCount; i++)
            {
                resultados.Add(reader.GetValue(i));
            }
        }

        CerrarConexion();

        return resultados;
    }

    public object ObtenerPrimerValorSegunColumna(object valorFiltro, string columnaFiltro, string columna)
    {
        AbrirConexion();

        object resultado = null;

        string valorFiltroAsignado = "";

        if(valorFiltro is System.String)
        {
            valorFiltroAsignado = "\"" + valorFiltro +"\"";
        }
        else
        {
            valorFiltroAsignado = valorFiltro.ToString();
        }


        string comando = $"SELECT {columna} FROM {tabla} WHERE {columnaFiltro} = {valorFiltroAsignado}";

        SqliteDataReader reader = EjecutarComandoConRetorno(comando);

        if(reader.Read())
        {
            resultado = reader.GetValue(0);
        }

        return resultado;
    }

    public List<System.Object> ObtenerValoresColumnasSegunID(int id, List<string> columnas)
    {
        AbrirConexion();

        List<System.Object> resultados = new List<System.Object>();

        string comando = "SELECT ";

        foreach(string columna in columnas)
        {
            comando += columna + ",";
        }

        comando = comando.Remove(comando.Length - 1);

        SqliteDataReader reader = EjecutarComandoConRetorno(comando + $" FROM {tabla} WHERE ID = {id}");

        if(reader.Read())
        {
            for(int i = 0; i < reader.FieldCount; i++)
            {
                resultados.Add(reader.GetValue(reader.GetOrdinal(columnas[i])));
            }
        }

        CerrarConexion();

        return resultados;
    }

    public List<System.Object> ObtenerValoresColumna(string columna)
    {
        AbrirConexion();

        List<System.Object> resultados = new List<System.Object>();

        SqliteDataReader reader = EjecutarComandoConRetorno($"SELECT {columna} FROM {tabla}");

        while(reader.Read())
        {
            resultados.Add(reader.GetValue(0));
        }

        CerrarConexion();

        return resultados;
    }

    public void ModificarValor(string columnaAModificar, System.Object valor, string columnaDeFiltro, System.Object valorFiltro)
    {
        AbrirConexion();

        string valorAsignado;

        if(valor is System.String)
        {
            valorAsignado = "\"" + valor +"\"";
        }
        else
        {
            valorAsignado = valor.ToString();
        }

        string valorFiltroAsignado;

        if(valorFiltro is System.String)
        {
            valorFiltroAsignado = "\"" + valorFiltro +"\"";
        }
        else
        {
            valorFiltroAsignado = valorFiltro.ToString();
        }

        EjecutarComandoSinRetorno($"UPDATE {tabla} SET {columnaAModificar} = {valorAsignado} WHERE {columnaDeFiltro} = {valorFiltroAsignado}");

        CerrarConexion();
    }


    // private void ObtenerNombresColumnas()
    // {
    //     SqliteDataReader reader = EjecutarComandoConRetorno($"PRAGMA table_info({tabla})");
    //     while(reader.Read())
    //     {
    //         string helperString = "";

    //         for(int i = 0; i < reader.FieldCount; i++)
    //         {
    //             helperString += reader.GetValue(i) + " ";
    //         }

    //             Debug.Log(helperString);
    //     }
    // }
}
