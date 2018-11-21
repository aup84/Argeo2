using UnityEngine.UI;
using UnityEngine;
using System;

public class Helps :MonoBehaviour {
    public void GetAyuda()
    {
        Text txtAyuda = GameObject.Find("txtAyuda").GetComponent<Text>();
        Text txtNAyudas = GameObject.Find("txtNAyudas").GetComponent<Text>();
        int nAyudas = Convert.ToInt32(txtNAyudas.text);
        switch (GameObject.Find("txtFind").GetComponent<Text>().text) {           
            case "Cilindro":
                txtAyuda.text = "El cilindro es una superficie cilíndrica que se forma cuando una recta, llamada generatriz gira alrededor de otra recta paralela, eje.";
                break;
            case "Cilindro Oblicuo":
                txtAyuda.text = "El Cilindro cuyas bases son oblicuas a las generatrices de la superficie cilíndrica.";
                break;
            case "Cono":
                txtAyuda.text = "Un Cono es un sólido de revolución generado por el giro de un triángulo rectángulo alrededor de uno de sus catetos. Al círculo conformado por el otro cateto se denomina base y al punto donde confluyen las generatrices se llama vértice.";
                break;
            case "Cubo":
                txtAyuda.text = "El Cubo es una figura con seis lados iguales ";
                break;
            case "Dodecaedro":
                txtAyuda.text = "El Dodecaedro es un sólido que dispone de doce caras. La suma del número de vértices y el número de caras de un dodecaedro regular es igual a 2 más el número de aristas";
                break;
            case "Octaedro":
                txtAyuda.text = "El Octaedro es un poliedro de ocho caras. Sus caras han de ser polígonos de siete lados o menos. Si las ocho caras del octaedro son triángulos equiláteros, iguales entre sí, el octaedro es convexo y se denomina regular.";
                break;
            case "Piramide Trunca":
                txtAyuda.text = "Una Pirámide Truncada es un poliedro comprendido entre la base de la pirámide y un plano que corta a todas las aristas laterales";
                break;
            case "Prisma Pentagonal":
                txtAyuda.text = "El Prisma Pentagonal es un prisma recto que tiene como bases dos pentágonos.";
                break;
            case "Tetraedro":
                txtAyuda.text = "El Tetraedro es un poliedro de cuatro caras. Si las cuatro caras del tetraedro son triángulos equiláteros, iguales entre sí, el tetraedro se denomina regular.";
                break;
            case "Cono Trunco":
                txtAyuda.text = "Un Cono Truncado es un cuerpo geométrico que resulta al cortar un cono por un plano paralelo a la base y separar la parte que contiene al vértice.";
                break;
            case "Prisma Triangular":
                txtAyuda.text = "El Prisma Triangular es un prisma recto que tiene como bases dos triángulos equiláteros.";
                break;
            case "Prisma Rectangular":
                txtAyuda.text = "El Prisma Rectangular u Ortoedro es un poliedro cuya superficie está formada por dos rectángulos iguales y paralelos llamados bases y por cuatro caras laterales que son rectángulos paralelos";
                break;            
            case "Salir":
                txtAyuda.text = "Fin de la Partidas";
                break;
        }
        nAyudas++;
        txtNAyudas.text = nAyudas.ToString();
    }
}
