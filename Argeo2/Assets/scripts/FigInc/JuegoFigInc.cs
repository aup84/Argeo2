using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.IO;
using System;

public class JuegoFigInc : MonoBehaviour {
    int aciertos = 0, errores = 0;
    double tiempo, tiempo2, tempTiempo;
    Text txtErrores, txtAciertos, txtPausar, txtTiempo, txtTiempo2;
    GameObject boton;
	GameObject BtnReset;
	public GameObject pnlPausa;
    GameRules regla;
	public bool pause;
    public bool pausar = false;

    void Start () {
        txtTiempo = GameObject.Find("txtTiempo").GetComponent<Text>();
        txtTiempo2 = GameObject.Find("txtTiempo2").GetComponent<Text>();
        txtErrores = GameObject.Find("txtErrores").GetComponent<Text>();
        txtAciertos = GameObject.Find("txtAciertos").GetComponent<Text>();
        txtPausar = GameObject.Find("txtPausar").GetComponent<Text>();
        boton = GameObject.Find("BtnMenu");
        BtnReset = GameObject.Find("BtnReset");
        pnlPausa = GameObject.Find("pnlPausa");
        pnlPausa.GetComponent<RectTransform>().localScale = Vector3.zero;
        Time.timeScale = 1;
        txtPausar.text = "No";       
        regla = new GameRules();       
        regla.SetImgObjetivo();
       
	}

	// Update is called once per frame
	void Update () {
		errores = Convert.ToInt32 (txtErrores.text);
		aciertos = int.Parse(txtAciertos.text);

        if (errores <15 && aciertos <15){
			if (txtPausar.text.Equals("No")) {
				StartCoroutine ("ControlTiempo");
			}
		}
		else {
			Time.timeScale = 0;
			PausarTiempo ();
			Text txtAyuda = GameObject.Find("txtAyuda").GetComponent<Text>();
			if (errores >= 15) {
				txtAyuda.text = "Fin del juego. Has acumulado 15 errores";
			} 
			else {
				txtAyuda.text = "Felicidades, has alcanzado los 15 objetivos";		
			}
			BtnReset.SetActive (true);
			boton.gameObject.SetActive (true);
		}		
	}
    IEnumerator ControlTiempo()
    {
        try
        {
            tiempo = double.Parse(txtTiempo.text, System.Globalization.NumberStyles.Any);
            tiempo2 = double.Parse(txtTiempo2.text, System.Globalization.NumberStyles.Any);
            tiempo += Time.deltaTime;
            tiempo2 += Time.deltaTime;
            txtTiempo.text = "" + tiempo.ToString("F");
            txtTiempo2.text = "" + tiempo2.ToString("F");
        }
        catch (Exception e)
        {
            Debug.Log(e.Message);
        }
        yield return new WaitForSeconds(0);
    }

    public void PausarTiempo()
    {
        StopCoroutine("ControlTiempo");
    }

    public void SigObj(){
		pnlPausa.SetActive(true);
		pnlPausa.GetComponent<RectTransform> ().localScale = Vector3.zero;
		txtPausar.text = "No";
        StartCoroutine("ControlTiempo");
        regla.SetImgObjetivo();
	}


}