using System;
using System.Data;
using System.IO;
using System.Text;
using Mono.Data.Sqlite;
using UnityEngine;
using UnityEngine.UI;
using System.Linq;
using System.Collections.Generic;

public class LoadingData  {
    List<CuerpoG> datos;    
    List<CuerpoG> lista;
    private static LoadingData instance;
    private string connection;
    private IDbConnection dbCon1;
    private IDbCommand dbCmd1;
    private IDataReader reader1;

    private LoadingData() { }

    public List<CuerpoG> Datos {
        get {  return datos; }
    }

    public static LoadingData Instance {
        get {
            if (instance == null) {
                LoadingData load = new LoadingData();
                instance = new LoadingData();
                Debug.Log("Creando la lista de datos");
                instance.datos = load.CrearLista();              
            }
            return instance;
        }
    }

    public void AbrirConexion(){
        string p = "DataBase.db";
        string filePath;
        if (Application.isMobilePlatform){
            filePath = Application.persistentDataPath + "/" + p;
        }
        else{
            filePath = Application.streamingAssetsPath + "/" + p;
        }

        connection = "URI=file:" + filePath;
        Debug.Log("Conectando a la base de datos  " + connection);
        dbCon1 = new SqliteConnection(connection);
        dbCon1.Open();
    }

    public void CerrarConexion(){
        dbCmd1.Dispose();
        dbCmd1 = null;
        dbCon1.Close();
        dbCon1 = null;

        Debug.Log("Cerrando Conexion");
    }

    public List<CuerpoG> CrearLista(){
        lista = new List<CuerpoG>();
        AbrirConexion();
        dbCmd1 = dbCon1.CreateCommand(); //   La consulta de la tabla en la base de datos

        dbCmd1.CommandText = "select * from FigInc order by Nivel";
        reader1 = dbCmd1.ExecuteReader();     //Ejecuta la consulta
        while (reader1.Read()){
            CuerpoG r3 = new CuerpoG {
                IdFigura = reader1.GetInt32(0),
                IdImagen = reader1.GetString(1),
                Descripcion = reader1.GetString(2),
                Marcador = reader1.GetString(3),
                Nivel = reader1.GetInt32(4),
                Status = false
            };
            lista.Add(r3);
        }
        reader1.Close();
        reader1 = null;
        CerrarConexion();
      //   string stri = "";
      //  lista.ForEach(item => stri += item.Descripcion + ", ");
      //  Debug.Log("Figuras disponibles: " + stri);
        return lista;
    }
    public List<CuerpoG> RecuperarLista(){
        return lista;
    }

    public List<CuerpoG> RemoverElemento(string elemento){
        var cuerpo = datos.Where(y => y.Descripcion == elemento && y.Status == false );
        if (cuerpo.FirstOrDefault()!= null) {
            Datos.FirstOrDefault().Status = true;
        }        
        return lista;
    }

    public void ActualizarNivel(int nivel) {
        if (Instance.Datos.Where(x => x.Nivel == nivel && x.Status == false).Count() < 1){
            Instance.Datos.ForEach(x => { if (x.Nivel == nivel) x.Status = false; });
            Debug.Log("Restableciendo el nivel " + nivel);
        }          
    }

    public CuerpoG SetObjetivo(int nivel)
    {
        var fig = LoadingData.Instance.Datos.Where(x => x.Nivel == nivel && x.Status == false);
        Debug.Log("Nivel Actual Elelemtnos:  " + fig.Count());
        if (fig.Count() == 0){
            ActualizarNivel(nivel);
            fig = LoadingData.Instance.Datos.Where(x => x.Nivel == nivel && x.Status == false);
        }        
        System.Random rnd = new System.Random();
        fig = fig.OrderBy(item => rnd.Next()).ToList();
        int indice = rnd.Next(0, fig.Count());
        Debug.Log("Fig Count " + fig.Count() + " Indice generado " + indice);
        Debug.Log(fig.ElementAt(indice).Descripcion);
        return fig.ElementAt(indice);
    }

    public void ElementosTotales() {
        int r = LoadingData.Instance.Datos.Where(x => x.Status == false).Count();
        Debug.Log("Elementos Restantes: " + r);
    }

    public CuerpoG GetDescripciones(string descripcion) {
        var fig = LoadingData.Instance.Datos.Where(x => x.Descripcion == descripcion && x.Status == false);
        Debug.Log("Nivel Actual Elelemtnos:  " + fig.Count());
        if (fig.Count() == 0){
            fig = LoadingData.Instance.Datos.Where(x => x.Descripcion == descripcion);
            ActualizarNivel(fig.First().Nivel);
            fig = LoadingData.Instance.Datos.Where(x => x.Descripcion == descripcion && x.Status == false);
        }
        System.Random rnd = new System.Random();
        int indice = rnd.Next(0, fig.Count());
        return fig.ElementAt(indice);      
    }

    public CuerpoG GetDescripciones(string marcador, int folio){
        var fig = LoadingData.Instance.Datos.Where(x => x.Marcador == marcador && x.Status == false);        
        if (fig.Count() == 0) {            
            fig = LoadingData.Instance.Datos.Where(x => x.Marcador == marcador);
            ActualizarNivel(fig.First().Nivel);
            fig = LoadingData.Instance.Datos.Where(x => x.Marcador == marcador && x.Status == false);
        }
        System.Random rnd = new System.Random();
        int indice = rnd.Next(0, fig.Count());
        return fig.ElementAt(indice);       
    }

    public CuerpoG GetDescripciones(int nivel, int nivel2)
    {
        var fig = LoadingData.Instance.Datos.Where(x => x.Nivel == nivel && x.Status == false);
        Debug.Log("Elementos actuale del nivel :  " + nivel + "  = "  + fig.Count());
        if (fig.Count() == 0) {
            ActualizarNivel(nivel);
            fig = LoadingData.Instance.Datos.Where(x => x.Nivel == nivel && x.Status == false);
        }
        System.Random rnd = new System.Random();
        int indice = rnd.Next(0, fig.Count());
        return fig.ElementAt(indice);           
    }
}