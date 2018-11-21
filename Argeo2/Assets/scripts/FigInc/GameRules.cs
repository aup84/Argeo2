using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Vuforia;
using UnityEngine.UI;
using System.IO;
using System.Linq;
using System;

public class GameRules {
    Text txtFind, txtPuntos;
    int nivel = 1;
    Texture2D textura2;
    public bool pausar = false;
    int puntos = 0, aciertos = 0, errores = 0, nAyudas = 0, arch2 = 0;
    double tiempo, tiempo2, tempTiempo;
    public GameObject pnlPausa;
   
    public CuerpoG GetObjetivo(int nFigura, int nivel) {
        CuerpoG fig = (CuerpoG)LoadingData.Instance.Datos.Where(x => x.IdFigura == nFigura && x.Nivel == nivel);
        return fig;
    }

    public string getFiguraPrefab(string tb){
        string resultado = "figura" + tb.Substring(5, 2);
        return resultado;
    }

    public void SetImgObjetivo()
    { //Permite repetir elementos
        RawImage imgObjetivo = GameObject.Find("imgObjetivo").GetComponent<RawImage>();
        string tarFind = "";
        nivel = Convert.ToInt32(GameObject.Find("txtNivel").GetComponent<Text>().text);
        try
        {
            CuerpoG cg = LoadingData.Instance.GetDescripciones(nivel, nivel);
            tarFind = cg.Descripcion;

           // Debug.Log("TarFInd " + tarFind);
            GameObject.Find("txtFind").GetComponent<Text>().text = tarFind;
            //GameObject.Find("lblTiempo2").GetComponent<Text>().text = tarFind;
            textura2 = new Texture2D(150, 150, TextureFormat.RGBA32, false);

            if (Application.isMobilePlatform)
            {
                string ruta = Application.streamingAssetsPath + "/images/plano/P" + cg.Marcador + ".png";

                WWW www = new WWW(ruta);
                while (!www.isDone)
                {
                }
                www.LoadImageIntoTexture(textura2);
            }
            else
            {
                textura2.LoadImage(File.ReadAllBytes(Application.dataPath + "/images/plano/P" + cg.Marcador + ".png"));
            }
            imgObjetivo.texture = textura2;
        }
        catch (Exception e)
        {
            Debug.Log(e.Message);
        }
    }

    public void MostrarPanel(bool result){		
		GameObject.Find ("pnlPausa").gameObject.GetComponent<RectTransform> ().localScale.Set (0.3f, 0.5f, 1f);
		RawImage raw2 = GameObject.Find ("RawPausa").GetComponent<RawImage> ();
		Text txtAviso = GameObject.Find("txtAviso").GetComponent<Text>();
		txtAviso.text = txtFind.text + " encontrado. Presiona el botón continuar para seguir al siguiente objetivo";
		string arch = "incorrecto.png";
		if (result) {
			arch = "correcto.jpg";
		}
		Texture2D textura = new Texture2D (150, 150, TextureFormat.RGBA32, false);
		if (Application.isMobilePlatform) {
			string ruta = Application.streamingAssetsPath + "/images/" + arch; 

			WWW www = new WWW (ruta);
			while (!www.isDone) {
			}
			www.LoadImageIntoTexture (textura);
		}
		else {
			textura.LoadImage(File.ReadAllBytes(Application.dataPath + "/images/" + arch));
		}
		raw2.texture = textura;
		Text txtPausar = GameObject.Find("txtPausar").GetComponent<Text>();
		txtPausar.text = "Si";
	}

    public void ActualizarEstrellas() {
        arch2 = (aciertos / 3) + 1;
        RawImage imgStar = GameObject.Find("imgStar").GetComponent<RawImage>();
        Texture2D textura = new Texture2D(150, 150, TextureFormat.RGBA32, false);
        if (Application.isMobilePlatform){
            string ruta = Application.dataPath + "/icons/" + arch2 + ".png";
            WWW www = new WWW(ruta);
            while (!www.isDone) {}
            www.LoadImageIntoTexture(textura);
        }
        else{
            textura.LoadImage(File.ReadAllBytes(Application.dataPath + "/icons/" + arch2 + ".png"));
        }
        imgStar.texture = textura;
        GameObject.Find("txtAyuda").GetComponent<Text>().text = "";
    }

    public bool Validar(string tb)
    {
        bool result = false;
        txtFind = GameObject.Find("txtFind").GetComponent<Text>();
        Text txtAciertos = GameObject.Find("txtAciertos").GetComponent<Text>();
        Text txtNAyudas = GameObject.Find("txtNAyudas").GetComponent<Text>();
        txtPuntos = GameObject.Find("txtPuntos").GetComponent<Text>();

        puntos = Convert.ToInt32(txtPuntos.text);
        Text txtErrores = GameObject.Find("txtErrores").GetComponent<Text>();
        nAyudas = Convert.ToInt32(txtNAyudas.text);
        errores = Convert.ToInt32(txtErrores.text);
        aciertos = Convert.ToInt32(txtAciertos.text);   
        if (txtFind.text.Equals(tb)){
            GameObject.Find("txtAyuda").GetComponent<Text>().text = "";
            result = true;
            puntos += 100;
            
            if (errores < 15 && aciertos < 15){
                aciertos++;
                if (aciertos % 3 == 1)                {
                    ActualizarEstrellas();
                }
                if (aciertos % 2 == 0){
                    //Fuzzy Logic
                    tiempo2 = Convert.ToDouble(GameObject.Find("txtTiempo2").GetComponent<Text>().text);
                    tempTiempo = Math.Round(tiempo2 / 30, 3);
                    tempTiempo = (tempTiempo < 1) ? tempTiempo : 1d;

                    double denominador = aciertos + errores + nAyudas;
                    Debug.Log("v1: " + aciertos / denominador + "   v2: " + errores / denominador + "      v3: " + tempTiempo + "      v4 " + nAyudas);
                    FuzzyConstruirFig fuzzy = new FuzzyConstruirFig(aciertos / denominador, errores / denominador, tempTiempo, nAyudas / denominador);
                    double d = fuzzy.Inferencias();
                    nivel = fuzzy.FuzzyToCrisp(d);

                    int ant = Convert.ToInt32(GameObject.Find("txtNivel").GetComponent<Text>().text);
                    if ((ant - nivel) == 0 || (ant - nivel) == 1 || (ant - nivel) == -1){
                        Debug.Log("Se queda en el nivel " + nivel);
                    }
                    else if ((ant - nivel) < 0){
                        nivel = ant + 1;
                    }
                    else{
                        nivel = ant - 1;
                    }

                    GameObject.Find("txtNivel").GetComponent<Text>().text = nivel.ToString(); // + "    " + Math.Round(d,4);
                    Debug.Log("txtNivel se ha actualizado a : " + nivel);

                    GameObject.Find("txtTiempo2").GetComponent<Text>().text = "0.01";
                    tiempo2 = 0;
                    //Termina Fuzzy Logic
                }
                
                pausar = true;
                GameObject.Find("Visor").GetComponent<Videos>().AbrirVideo();

                RemoverActual(tb);
                LoadingData.Instance.ElementosTotales();
            }
            else {
                Debug.Log("Se han alcanzado los 15 aciertos o 15 errores");
                Text txtAyuda = GameObject.Find("txtAyuda").GetComponent<Text>();
                txtAyuda.text = "Fin del juego porque has alncanzado los 15 aciertos o 15 errores";
            }
        }
        else {
            result = false;
            errores++;
           
            puntos -= 100;
            Helps help = new Helps();
            help.GetAyuda();
            
        }

        txtPuntos = GameObject.Find("txtPuntos").GetComponent<Text>();
        txtPuntos.text = puntos.ToString();
        txtAciertos.text = aciertos.ToString();
        txtErrores.text = errores.ToString();
        pausar = true;
        return result;
    }

    public void RemoverActual(string tb) {
        var c = LoadingData.Instance.Datos.Where(y => y.Descripcion == tb && y.Status == false);
        if (c.Count() > 0) {
            LoadingData.Instance.Datos.ForEach(x => { if (x.Descripcion == tb && x.Status == false) x.Status = true; });
        }
        

        /*
        var c = LoadingData.Instance.Datos.Where(y => y.Descripcion == tb);
        Debug.Log("Elementos a quitar  " + c.Count() + "  Marcador " + tb);
        temporal.Add((CuerpoG)c.First());
        datos = LoadingData.Instance.RemoverElemento(tb);
        Debug.Log("Elemenros restantes   " + LoadingData.Instance.Datos.Count());
        */


    }

   
	public bool Ccontinuar(){
		Debug.Log ("Proceso de pausado es: " + pausar);
		return pausar;
	}

}