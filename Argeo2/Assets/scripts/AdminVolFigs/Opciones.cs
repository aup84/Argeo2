using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Text;
using Mono.Data.Sqlite;
using UnityEngine;
using UnityEngine.UI;

public class Opciones : MonoBehaviour
{
    private string connection;
    private IDbConnection dbCon;
    private IDbCommand dbCmd;
    private IDataReader reader;
    private string pass = "";
    Text txtAcciones;
    InputField InputIdFigura, InputMedida1, InputMedida2, InputAltura, InputArea, InputVolumen;
    Dropdown dropFigura;
    Dropdown dropMarcador;
    Dropdown dropNivel;
    void Start() {
        InputIdFigura = GameObject.Find("InputIdFigura").GetComponent<InputField>();
        InputMedida1 = GameObject.Find("InputMedida1").GetComponent<InputField>();
        InputMedida2 = GameObject.Find("InputMedida2").GetComponent<InputField>();
        InputAltura = GameObject.Find("InputAltura").GetComponent<InputField>();
        InputArea = GameObject.Find("InputArea").GetComponent<InputField>();
        InputVolumen = GameObject.Find("InputVolumen").GetComponent<InputField>();

        txtAcciones = GameObject.Find("txtAcciones").GetComponent<Text>();
        dropFigura = GameObject.Find("dropFigura").GetComponent<Dropdown>();
        dropMarcador = GameObject.Find("dropMarcador").GetComponent<Dropdown>();
        dropNivel = GameObject.Find("dropNivel").GetComponent<Dropdown>();
    }

    public void AbrirConexion(){
        string p = "DataBase.db";
        string filePath;
        if (Application.isMobilePlatform)
        {
            filePath = Application.persistentDataPath + "/" + p;
        }
        else
        {
            filePath = Application.streamingAssetsPath + "/" + p;
        }
        connection = "URI=file:" + filePath;
        Debug.Log("Conectando a la base de datos  " + connection);
        txtAcciones.text += "Conectando a la base de datos  " + connection + "\n";
        dbCon = new SqliteConnection(connection);
        dbCon.Open();
    }

    public void CerrarConexion() {
        reader.Close();
        reader = null;
        dbCmd.Dispose();
        dbCmd = null;
        dbCon.Close();
        dbCon = null;
        Debug.Log("Cerrando Conexion");
    }

    public int GetIndexDropDown(Dropdown drop, string marcador)
    {
        List<Dropdown.OptionData> lista = drop.options;

        return lista.FindIndex(item => item.text == marcador);
    }

    public void Consultar()
    {

        if (InputIdFigura.text.Equals("")) {
            Debug.Log("NO SE PUEDEN CONSULTAR CLAVES VACIOS");
            return;
        }
        AbrirConexion();
        dbCmd = dbCon.CreateCommand();
        dbCmd.CommandText = "Select * from VolFiguras where idFigura = " + InputIdFigura.text;    // Es la consulta en codigo SQl
        reader = dbCmd.ExecuteReader();     //Ejecuta la consulta
    

        while (reader.Read()) { //Recupera los datos del DataSet
            int id = reader.GetInt32(0);
            string nombre = reader.GetString(1);
            string marcador = reader.GetString(2);
            float medida1 = reader.GetFloat(3);
            float medida2 = reader.GetFloat(4);
            float medida3 = reader.GetFloat(5);
            float area = reader.GetFloat(6);
            float volumen = reader.GetFloat(7);
            int nivel = reader.GetInt32(8);
            
            InputMedida1.text = medida1.ToString();
            InputMedida2.text = medida2.ToString();
            InputAltura.text = medida3.ToString();
            InputArea.text = area.ToString();
            InputVolumen.text = volumen.ToString();
            dropFigura.value = GetIndexDropDown(dropFigura, nombre);
            dropMarcador.value = GetIndexDropDown(dropMarcador, marcador);
            dropNivel.value = GetIndexDropDown(dropNivel, nivel.ToString() );            
        }
        CerrarConexion();
        txtAcciones.text += "Cerrando Conexion \n";
    }

    public void Select()
    {
        Consultar();
    }

    

    public bool RevisarInputs()
    {
        bool resultado = false;

        if (InputIdFigura.text.Equals("") || InputVolumen.text.Equals("") ||
            InputArea.text.Equals("") || InputMedida1.text.Equals("") || InputMedida1.text.Equals("")) {
            resultado = true;
        }
        Debug.Log("Resultado" + resultado);
        return resultado;
    }

    public void Insert()
    {
        Debug.Log("Validando espaciados");
        if (RevisarInputs() == true)
        {
            return;
        }
        AbrirConexion();
        dbCmd = dbCon.CreateCommand(); //   La consulta de la tabla en la base de datos
        Debug.Log("Antes de Agregar");
        dbCmd.CommandText = "INSERT INTO VolFiguras (idFigura, nombre, marcador, medida1, medida2, medida3, area, volumen, nivel) " +
            " VALUES (" + InputIdFigura.text + ",'"+ dropFigura.captionText.text + "','" + dropMarcador.captionText.text + "'," +
            Convert.ToDouble(InputMedida1.text) + "," + Convert.ToDouble(InputMedida2.text) + "," + Convert.ToDouble(InputAltura.text) + "," +
            Convert.ToDouble(InputArea.text) + "," + Convert.ToDouble(InputVolumen.text) + "," + Convert.ToInt32(dropNivel.captionText.text) + ")";
        Debug.Log(dbCmd.CommandText);
        dbCmd.ExecuteNonQuery();
        Debug.Log("Se ha AGREGADO el registro");
        txtAcciones.text += "Se ha agregado el registro \n";
        dbCmd.Dispose();
        dbCmd = null;
        dbCon.Close();
        dbCon = null;
        txtAcciones.text += "Cerrando Conexion \n";
        ClearInputs();
    }

    public void Guardar() {
        int nreg = 0;
        if (RevisarInputs() == true)
        {
            return;
        }
        AbrirConexion();
        dbCmd = dbCon.CreateCommand(); //   La consulta de la tabla en la base de datos
        dbCmd.CommandText = "Select count(*) from VolFiguras where idFigura = " + InputIdFigura.text;    // Es la consulta en codigo SQl
        reader = dbCmd.ExecuteReader();     //Ejecuta la consulta
        while (reader.Read())
        {
            nreg = reader.GetInt32(0);            
        }
        reader.Close();
        CerrarConexion();
        if (nreg == 1)
        {
            Debug.Log("Se invoca al metodo actualizar");
            Actualizar();
        }
        else if (nreg == 0)
        {
            Debug.Log("Se invoca al metodo Insertar");
            Insert();
        }
    }

    public void Actualizar()
    {
        if (RevisarInputs() == true)
        {
            return;
        }
        AbrirConexion();

        dbCmd = dbCon.CreateCommand(); //   La consulta de la tabla en la base de datos
        Debug.Log("Antes de Agregar");
        dbCmd.CommandText = "UPDATE VolFiguras SET Nombre = '" + dropFigura.captionText.text+ "', marcador = '" + 
            dropMarcador.captionText.text + "', medida1 = " + Convert.ToDouble(InputMedida1.text) + ", medida2 = " +
            Convert.ToDouble(InputMedida2.text) + ", medida3 = " + Convert.ToDouble(InputAltura.text) + ", area = " +
             Convert.ToDouble(InputArea.text) + ", volumen = " + Convert.ToDouble(InputVolumen.text) + ", nivel = " +
             Convert.ToInt32(dropNivel.captionText.text) + " WHERE idFigura =  " + InputIdFigura.text;
        dbCmd.ExecuteNonQuery();
        Debug.Log("Se ha Actualizado el registro");
        txtAcciones.text += "Se ha Actualizado el registro \n";
        dbCmd.Dispose();
        dbCmd = null;
        dbCon.Close();
        dbCon = null;
        txtAcciones.text += "Cerrando Conexion \n";
        ClearInputs();
    }

    public void LogIn()
    {

        InputField txtUsuario = GameObject.Find("txtUsuario").GetComponent<InputField>();
        InputField txtPass = GameObject.Find("txtPass").GetComponent<InputField>();

        if (txtUsuario.text.Equals("") || txtPass.text.Equals(""))
        {
            Debug.Log("Favor de escribir usuario y contraseña");
            GameObject.Find("txtAcciones").GetComponent<Text>().text = "Falta usuario y contraseña";
            return;
        }

        string filePath;
        if (Application.isMobilePlatform)
        {
            filePath = Application.persistentDataPath + "/DataBase.db";

        }
        else
        {
            filePath = Application.streamingAssetsPath + "/DataBase.db";
        }

       
        connection = "URI=file:" + filePath;
        Debug.Log("Conectando a la base de datos  " + connection);
        txtAcciones.text += "Conectando a la base de datos  " + connection + "\n";
        dbCon = new SqliteConnection(connection);

        dbCmd = dbCon.CreateCommand(); //   La consulta de la tabla en la base de datos
        Debug.Log("Select * from Usuarios where idUsuario = '" + txtUsuario.text + "' and status = 1");    // Es la consulta en codigo SQl)
        dbCon.Open();
        dbCmd.CommandText = "Select * from Usuarios where idUsuario = '" + txtUsuario.text + "' and status = 1";
        reader = dbCmd.ExecuteReader();     //Ejecuta la consulta

        while (reader.Read())
        { //Recupera los datos del DataSet
          //  string id = reader.GetString(0);
            string nombre = reader.GetString(1);
            pass = reader.GetString(2);
            //    status = reader.GetInt32(3);
            txtAcciones.text = "Conexion establecida. \n Usuario: " + nombre;
            GameObject.Find("lblNombre").GetComponent<Text>().text = nombre;
            GameObject.Find("txtUsuario").GetComponent<InputField>().text = nombre;
            GameObject.Find("txtPass").GetComponent<InputField>().text = pass.ToString();
        }
        reader.Close();
        reader = null;

        dbCmd.CommandText = "INSERT INTO 'Sesion' ('IdUsuario', 'Nombre', 'LastSesion') VALUES ('" +
                  GameObject.Find("txtUsuario").GetComponent<InputField>().text + "', '" +
                  GameObject.Find("lblNombre").GetComponent<Text>().text + "' , '" +
                  System.DateTime.Now.Date.ToShortDateString().ToString() + "')";
        dbCmd.ExecuteNonQuery();
        Debug.Log("Login registrado en la tabla sesion");
        PlayerPrefs.SetString("id", txtUsuario.text);
        PlayerPrefs.SetString("nombre", GameObject.Find("txtUsuario").GetComponent<InputField>().text);


        dbCmd.Dispose();
        dbCmd = null;
        dbCon.Close();
        dbCon = null;

        Debug.Log("Cerrando Conexion");
        GameObject.Find("MainPanel").GetComponent<RectTransform>().localScale = new Vector3(0.6f, 0.6f, 0.6f);
        GameObject.Find("LoginPanel").GetComponent<RectTransform>().localScale = Vector3.zero;
    }
    public void ClearInputs() {
        InputIdFigura.text = "";
        InputMedida1.text = "";
        InputMedida2.text = "";
        InputAltura.text = "";
        InputArea.text = "";
        InputVolumen.text = "";
        dropFigura.value = 0;
        dropMarcador.value = 0;
        dropNivel.value = 0;
    }
}