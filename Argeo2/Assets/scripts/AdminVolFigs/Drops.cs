using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Drops :MonoBehaviour {
    Dropdown dr1, dr2;
    InputField txt1, txt2, txt3, txtArea, txtVol;
    private void Start(){
        dr1 = GameObject.Find("dropFigura").GetComponent<Dropdown>();
        dr2 = GameObject.Find("dropMarcador").GetComponent<Dropdown>();
        txt1 = GameObject.Find("InputMedida1").GetComponent<InputField>();
        txt2 = GameObject.Find("InputMedida2").GetComponent<InputField>();
        txt3 = GameObject.Find("InputAltura").GetComponent<InputField>();
        txtArea = GameObject.Find("InputArea").GetComponent<InputField>();
        txtVol = GameObject.Find("InputVolumen").GetComponent<InputField>();
    }
    public void Actualizar() {      
        dr2.value = dr1.value;
    }

    public void CalcularAV() {
           if (!txt1.text.Equals("") && !txt2.text.Equals("") && !txt3.text.Equals("")){
            Debug.Log("Entrando a los calculos");
            double medida1 = Convert.ToDouble(txt1.text);
            double medida2 = Convert.ToDouble(txt2.text);
            double medida3 = Convert.ToDouble(txt3.text);
            double area=0; 
            
            switch (dr1.value) {
                case 0: area = (medida1 * medida2) / 2; break;
                case 1: area = (medida1 * medida2); break;
                case 2: area = (5 * medida1 * medida2) / 2; break;
                case 3: area = (6 * medida1 * medida2) / 2; break;
                case 4: area = (10 * medida1 * medida2) / 2; break;
                case 5: area = (Math.PI * Math.Pow(medida1,2)); break;
            }
            double volumen = area * medida3;
            txtArea.text = area.ToString();
            txtVol.text = volumen.ToString();
            Debug.Log("Actualizando Calculos");
        }
    }
}
