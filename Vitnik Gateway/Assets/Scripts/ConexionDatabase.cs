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
        dbURI = "URI=file:" + "Assets/Databases/"+ nombreDatabase;

        conexionDb = new SqliteConnection(dbURI);
    }

    private void AbrirConexion()
    {
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

    public string ObtenerValor(int id, string columna)
    {
        AbrirConexion();

        string resultado = "";

        SqliteDataReader reader = EjecutarComandoConRetorno($"SELECT {columna} FROM {tabla} WHERE ID = {id}");

        if(reader.Read())
        {
            resultado = reader.GetString(0);
        }

        CerrarConexion();

        return resultado;
    }

    public List<string> ObtenerValoresFila(int id, List<string> columnas)
    {
        AbrirConexion();

        List<string> resultados = new List<string>();

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
                resultados.Add(reader.GetString(reader.GetOrdinal(columnas[i])));
            }
        }

        CerrarConexion();

        return resultados;
    }

    public List<string> ObtenerValoresColumna(string columna)
    {
        AbrirConexion();

        List<string> resultados = new List<string>();

        SqliteDataReader reader = EjecutarComandoConRetorno($"SELECT {columna} FROM {tabla}");

        while(reader.Read())
        {
            resultados.Add(reader.GetString(0));
        }

        CerrarConexion();

        return resultados;
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
