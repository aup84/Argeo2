﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Threading;
using AForge.Fuzzy;
using System;

public class FuzzyConstruirFig {
    private InferenceSystem IS;
    private Thread thMovement;
    // private bool FirstInference;    
    double ntiempo, naciertos, nerrores, nayuda;
    LinguisticVariable lvDificultad;
    //Constructor
    public FuzzyConstruirFig (double naciertos, double nerrores, double ntiempo, double nayuda)
    {
        this.naciertos = naciertos;
        this.nerrores = nerrores;
        this.ntiempo = ntiempo;
        this.nayuda = nayuda;
        InitFuzzyEngine();
        //  FirstInference = true;
    }

    void InitFuzzyEngine()
    {
        /*
        // Linguistic labels (fuzzy sets) para aciertos
        FuzzySet fsLow = new FuzzySet("Low", new TrapezoidalFunction(0.0, 0.33, TrapezoidalFunction.EdgeType.Right));
        FuzzySet fsNeutral = new FuzzySet("Normal", new TrapezoidalFunction(0.0, 0.33, 0.66, 1.00));
        FuzzySet fsHigh = new FuzzySet("High", new TrapezoidalFunction(0.66, 1.00, TrapezoidalFunction.EdgeType.Left));

        // Linguistic labels (fuzzy sets) para errores y ayuda
        FuzzySet fsLowInv = new FuzzySet("Low", new TrapezoidalFunction(0.0, 0.33, TrapezoidalFunction.EdgeType.Right));        
        FuzzySet fsHighInv = new FuzzySet("High", new TrapezoidalFunction(0.66, 1.00, TrapezoidalFunction.EdgeType.Left));
        */

        // Linguistic labels (fuzzy sets) para aciertos, errores y ayudas
        FuzzySet fsLow = new FuzzySet("Low", new TrapezoidalFunction(0.0, 0.40, TrapezoidalFunction.EdgeType.Right));
        FuzzySet fsNeutral = new FuzzySet("Normal", new TrapezoidalFunction(0.0, 0.40, 0.65, 1.00));
        FuzzySet fsHigh = new FuzzySet("High", new TrapezoidalFunction(0.65, 1.00, TrapezoidalFunction.EdgeType.Left));

        // Linguistic labels (fuzzy sets) para el tiempo
        FuzzySet fsFast = new FuzzySet("Fast", new TrapezoidalFunction(0.0, 0.33, TrapezoidalFunction.EdgeType.Right));
        FuzzySet fsNormal = new FuzzySet("Normal", new TrapezoidalFunction(0, 33, 0.66, 1.00));
        FuzzySet fsSlow = new FuzzySet("Slow", new TrapezoidalFunction(0.66, 1.00, TrapezoidalFunction.EdgeType.Left));

        // Right Distance (Input)
        LinguisticVariable numAciertos = new LinguisticVariable("Aciertos", 0, 1);
        numAciertos.AddLabel(fsLow);
        numAciertos.AddLabel(fsNeutral);
        numAciertos.AddLabel(fsHigh);


        // Left Distance (Input)
        LinguisticVariable numErrores = new LinguisticVariable("Errores", 0, 1);
        numErrores.AddLabel(fsLow);
        numErrores.AddLabel(fsNeutral);
        numErrores.AddLabel(fsHigh);

        LinguisticVariable ayudas = new LinguisticVariable("Ayuda", 0, 1);
        ayudas.AddLabel(fsLow);
        ayudas.AddLabel(fsNeutral);
        ayudas.AddLabel(fsHigh);

        // Time (Input)
        LinguisticVariable tiempoSeg = new LinguisticVariable("Tiempo", 0, 1);
        tiempoSeg.AddLabel(fsSlow);
        tiempoSeg.AddLabel(fsNormal);
        tiempoSeg.AddLabel(fsFast);

        // Linguistic labels (fuzzy sets) that compose Dificultad
        FuzzySet fsVeryWeak = new FuzzySet("VeryWeak", new TrapezoidalFunction(0.0, 0.30, TrapezoidalFunction.EdgeType.Right));
        FuzzySet fsWeak = new FuzzySet("Weak", new TrapezoidalFunction(0.0, 0.30, 0.50, 0.55));
        FuzzySet fsBasico = new FuzzySet("Normal", new TrapezoidalFunction(0.50, 0.55, 0.70, 0.75));
        FuzzySet fsGood = new FuzzySet("Good", new TrapezoidalFunction(0.70, 0.75, 0.90, 1.0));
        FuzzySet fsGreat = new FuzzySet("Great", new TrapezoidalFunction(0.90, 1.0, TrapezoidalFunction.EdgeType.Left));

        // AnglefsWonderFull
        lvDificultad = new LinguisticVariable("Dificultad", 0, 1.00);
        lvDificultad.AddLabel(fsVeryWeak);
        lvDificultad.AddLabel(fsWeak);
        lvDificultad.AddLabel(fsBasico);
        lvDificultad.AddLabel(fsGood);
        lvDificultad.AddLabel(fsGreat);

        // The database
        Database fuzzyDB = new Database();
        fuzzyDB.AddVariable(numAciertos);
        fuzzyDB.AddVariable(numErrores);
        fuzzyDB.AddVariable(tiempoSeg);
        fuzzyDB.AddVariable(ayudas);
        fuzzyDB.AddVariable(lvDificultad);

        // Creating the inference system
        IS = new InferenceSystem(fuzzyDB, new CentroidDefuzzifier(1000));

        IS.NewRule("RULE 1", "IF Aciertos IS High AND Tiempo IS Fast AND Errores IS High AND Ayuda IS Low THEN Dificultad IS Normal");
        IS.NewRule("RULE 2", "IF Aciertos IS High AND Tiempo IS Fast AND Errores IS High AND Ayuda IS Normal THEN Dificultad IS Normal");
        IS.NewRule("RULE 3", "IF Aciertos IS High AND Tiempo IS Fast AND Errores IS High AND Ayuda IS High THEN Dificultad IS Normal");
        IS.NewRule("RULE 4", "IF Aciertos IS High AND Tiempo IS Fast AND Errores IS Low AND Ayuda IS Low THEN Dificultad IS Great");
        IS.NewRule("RULE 5", "IF Aciertos IS High AND Tiempo IS Fast AND Errores IS Low AND Ayuda IS Normal THEN Dificultad IS Great");
        IS.NewRule("RULE 6", "IF Aciertos IS High AND Tiempo IS Fast AND Errores IS Low AND Ayuda IS High THEN Dificultad IS Good");
        IS.NewRule("RULE 7", "IF Aciertos IS High AND Tiempo IS Fast AND Errores IS Normal AND Ayuda IS Low THEN Dificultad IS Good");
        IS.NewRule("RULE 8", "IF Aciertos IS High AND Tiempo IS Fast AND Errores IS Normal AND Ayuda IS Normal THEN Dificultad IS Good");
        IS.NewRule("RULE 9", "IF Aciertos IS High AND Tiempo IS Fast AND Errores IS Normal AND Ayuda IS High THEN Dificultad IS Normal");
        IS.NewRule("RULE 10", "IF Aciertos IS High AND Tiempo IS Normal AND Errores IS High AND Ayuda IS Low THEN Dificultad IS Normal");
        IS.NewRule("RULE 11", "IF Aciertos IS High AND Tiempo IS Normal AND Errores IS High AND Ayuda IS Normal THEN Dificultad IS Normal");
        IS.NewRule("RULE 12", "IF Aciertos IS High AND Tiempo IS Normal AND Errores IS High AND Ayuda IS High THEN Dificultad IS Normal");
        IS.NewRule("RULE 13", "IF Aciertos IS High AND Tiempo IS Normal AND Errores IS Low AND Ayuda IS Low THEN Dificultad IS Great");
        IS.NewRule("RULE 14", "IF Aciertos IS High AND Tiempo IS Normal AND Errores IS Low AND Ayuda IS Normal THEN Dificultad IS Good");
        IS.NewRule("RULE 15", "IF Aciertos IS High AND Tiempo IS Normal AND Errores IS Low AND Ayuda IS High THEN Dificultad IS Good");
        IS.NewRule("RULE 16", "IF Aciertos IS High AND Tiempo IS Normal AND Errores IS Normal AND Ayuda IS Low THEN Dificultad IS Good");
        IS.NewRule("RULE 17", "IF Aciertos IS High AND Tiempo IS Normal AND Errores IS Normal AND Ayuda IS Normal THEN Dificultad IS Good");
        IS.NewRule("RULE 18", "IF Aciertos IS High AND Tiempo IS Normal AND Errores IS Normal AND Ayuda IS High THEN Dificultad IS Normal");
        IS.NewRule("RULE 19", "IF Aciertos IS High AND Tiempo IS Slow AND Errores IS High AND Ayuda IS Low THEN Dificultad IS Normal");
        IS.NewRule("RULE 20", "IF Aciertos IS High AND Tiempo IS Slow AND Errores IS High AND Ayuda IS Normal THEN Dificultad IS Normal");
        IS.NewRule("RULE 21", "IF Aciertos IS High AND Tiempo IS Slow AND Errores IS High AND Ayuda IS High THEN Dificultad IS Weak");
        IS.NewRule("RULE 22", "IF Aciertos IS High AND Tiempo IS Slow AND Errores IS Low AND Ayuda IS Low THEN Dificultad IS Good");
        IS.NewRule("RULE 23", "IF Aciertos IS High AND Tiempo IS Slow AND Errores IS Low AND Ayuda IS Normal THEN Dificultad IS Normal");
        IS.NewRule("RULE 24", "IF Aciertos IS High AND Tiempo IS Slow AND Errores IS Low AND Ayuda IS High THEN Dificultad IS Normal");
        IS.NewRule("RULE 25", "IF Aciertos IS High AND Tiempo IS Slow AND Errores IS Normal AND Ayuda IS Low THEN Dificultad IS Normal");
        IS.NewRule("RULE 26", "IF Aciertos IS High AND Tiempo IS Slow AND Errores IS Normal AND Ayuda IS Normal THEN Dificultad IS Normal");
        IS.NewRule("RULE 27", "IF Aciertos IS High AND Tiempo IS Slow AND Errores IS Normal AND Ayuda IS High THEN Dificultad IS Normal");
        IS.NewRule("RULE 28", "IF Aciertos IS Low AND Tiempo IS Fast AND Errores IS High AND Ayuda IS Low THEN Dificultad IS Weak");
        IS.NewRule("RULE 29", "IF Aciertos IS Low AND Tiempo IS Fast AND Errores IS High AND Ayuda IS Normal THEN Dificultad IS Weak");
        IS.NewRule("RULE 30", "IF Aciertos IS Low AND Tiempo IS Fast AND Errores IS High AND Ayuda IS High THEN Dificultad IS Weak");
        IS.NewRule("RULE 31", "IF Aciertos IS Low AND Tiempo IS Fast AND Errores IS Low AND Ayuda IS Low THEN Dificultad IS Normal");
        IS.NewRule("RULE 32", "IF Aciertos IS Low AND Tiempo IS Fast AND Errores IS Low AND Ayuda IS Normal THEN Dificultad IS Weak");
        IS.NewRule("RULE 33", "IF Aciertos IS Low AND Tiempo IS Fast AND Errores IS Low AND Ayuda IS High THEN Dificultad IS Weak");
        IS.NewRule("RULE 34", "IF Aciertos IS Low AND Tiempo IS Fast AND Errores IS Normal AND Ayuda IS Low THEN Dificultad IS Normal");
        IS.NewRule("RULE 35", "IF Aciertos IS Low AND Tiempo IS Fast AND Errores IS Normal AND Ayuda IS Normal THEN Dificultad IS Weak");
        IS.NewRule("RULE 36", "IF Aciertos IS Low AND Tiempo IS Fast AND Errores IS Normal AND Ayuda IS High THEN Dificultad IS Weak");
        IS.NewRule("RULE 37", "IF Aciertos IS Low AND Tiempo IS Normal AND Errores IS High AND Ayuda IS Low THEN Dificultad IS Weak");
        IS.NewRule("RULE 38", "IF Aciertos IS Low AND Tiempo IS Normal AND Errores IS High AND Ayuda IS Normal THEN Dificultad IS VeryWeak");
        IS.NewRule("RULE 39", "IF Aciertos IS Low AND Tiempo IS Normal AND Errores IS High AND Ayuda IS High THEN Dificultad IS VeryWeak");
        IS.NewRule("RULE 40", "IF Aciertos IS Low AND Tiempo IS Normal AND Errores IS Low AND Ayuda IS Low THEN Dificultad IS Normal");
        IS.NewRule("RULE 41", "IF Aciertos IS Low AND Tiempo IS Normal AND Errores IS Low AND Ayuda IS Normal THEN Dificultad IS Weak");
        IS.NewRule("RULE 42", "IF Aciertos IS Low AND Tiempo IS Normal AND Errores IS Low AND Ayuda IS High THEN Dificultad IS Weak");
        IS.NewRule("RULE 43", "IF Aciertos IS Low AND Tiempo IS Normal AND Errores IS Normal AND Ayuda IS Low THEN Dificultad IS Normal");
        IS.NewRule("RULE 44", "IF Aciertos IS Low AND Tiempo IS Normal AND Errores IS Normal AND Ayuda IS Normal THEN Dificultad IS Weak");
        IS.NewRule("RULE 45", "IF Aciertos IS Low AND Tiempo IS Normal AND Errores IS Normal AND Ayuda IS High THEN Dificultad IS Weak");
        IS.NewRule("RULE 46", "IF Aciertos IS Low AND Tiempo IS Slow AND Errores IS High AND Ayuda IS Low THEN Dificultad IS Weak");
        IS.NewRule("RULE 47", "IF Aciertos IS Low AND Tiempo IS Slow AND Errores IS High AND Ayuda IS Normal THEN Dificultad IS VeryWeak");
        IS.NewRule("RULE 48", "IF Aciertos IS Low AND Tiempo IS Slow AND Errores IS High AND Ayuda IS High THEN Dificultad IS VeryWeak");
        IS.NewRule("RULE 49", "IF Aciertos IS Low AND Tiempo IS Slow AND Errores IS Low AND Ayuda IS Low THEN Dificultad IS Normal");
        IS.NewRule("RULE 50", "IF Aciertos IS Low AND Tiempo IS Slow AND Errores IS Low AND Ayuda IS Normal THEN Dificultad IS Weak");
        IS.NewRule("RULE 51", "IF Aciertos IS Low AND Tiempo IS Slow AND Errores IS Low AND Ayuda IS High THEN Dificultad IS Weak");
        IS.NewRule("RULE 52", "IF Aciertos IS Low AND Tiempo IS Slow AND Errores IS Normal AND Ayuda IS Low THEN Dificultad IS Weak");
        IS.NewRule("RULE 53", "IF Aciertos IS Low AND Tiempo IS Slow AND Errores IS Normal AND Ayuda IS Normal THEN Dificultad IS Weak");
        IS.NewRule("RULE 54", "IF Aciertos IS Low AND Tiempo IS Slow AND Errores IS Normal AND Ayuda IS High THEN Dificultad IS VeryWeak");
        IS.NewRule("RULE 55", "IF Aciertos IS Normal AND Tiempo IS Fast AND Errores IS High AND Ayuda IS Low THEN Dificultad IS Normal");
        IS.NewRule("RULE 56", "IF Aciertos IS Normal AND Tiempo IS Fast AND Errores IS High AND Ayuda IS Normal THEN Dificultad IS Weak");
        IS.NewRule("RULE 57", "IF Aciertos IS Normal AND Tiempo IS Fast AND Errores IS High AND Ayuda IS High THEN Dificultad IS Weak");
        IS.NewRule("RULE 58", "IF Aciertos IS Normal AND Tiempo IS Fast AND Errores IS Low AND Ayuda IS Low THEN Dificultad IS Good");
        IS.NewRule("RULE 59", "IF Aciertos IS Normal AND Tiempo IS Fast AND Errores IS Low AND Ayuda IS Normal THEN Dificultad IS Normal");
        IS.NewRule("RULE 60", "IF Aciertos IS Normal AND Tiempo IS Fast AND Errores IS Low AND Ayuda IS High THEN Dificultad IS Normal");
        IS.NewRule("RULE 61", "IF Aciertos IS Normal AND Tiempo IS Fast AND Errores IS Normal AND Ayuda IS Low THEN Dificultad IS Normal");
        IS.NewRule("RULE 62", "IF Aciertos IS Normal AND Tiempo IS Fast AND Errores IS Normal AND Ayuda IS Normal THEN Dificultad IS Normal");
        IS.NewRule("RULE 63", "IF Aciertos IS Normal AND Tiempo IS Fast AND Errores IS Normal AND Ayuda IS High THEN Dificultad IS Weak");
        IS.NewRule("RULE 64", "IF Aciertos IS Normal AND Tiempo IS Normal AND Errores IS High AND Ayuda IS Low THEN Dificultad IS Weak");
        IS.NewRule("RULE 65", "IF Aciertos IS Normal AND Tiempo IS Normal AND Errores IS High AND Ayuda IS Normal THEN Dificultad IS Weak");
        IS.NewRule("RULE 66", "IF Aciertos IS Normal AND Tiempo IS Normal AND Errores IS High AND Ayuda IS High THEN Dificultad IS Weak");
        IS.NewRule("RULE 67", "IF Aciertos IS Normal AND Tiempo IS Normal AND Errores IS Low AND Ayuda IS Low THEN Dificultad IS Good");
        IS.NewRule("RULE 68", "IF Aciertos IS Normal AND Tiempo IS Normal AND Errores IS Low AND Ayuda IS Normal THEN Dificultad IS Normal");
        IS.NewRule("RULE 69", "IF Aciertos IS Normal AND Tiempo IS Normal AND Errores IS Low AND Ayuda IS High THEN Dificultad IS Normal");
        IS.NewRule("RULE 70", "IF Aciertos IS Normal AND Tiempo IS Normal AND Errores IS Normal AND Ayuda IS Low THEN Dificultad IS Normal");
        IS.NewRule("RULE 71", "IF Aciertos IS Normal AND Tiempo IS Normal AND Errores IS Normal AND Ayuda IS Normal THEN Dificultad IS Normal");
        IS.NewRule("RULE 72", "IF Aciertos IS Normal AND Tiempo IS Normal AND Errores IS Normal AND Ayuda IS High THEN Dificultad IS Weak");
        IS.NewRule("RULE 73", "IF Aciertos IS Normal AND Tiempo IS Slow AND Errores IS High AND Ayuda IS Low THEN Dificultad IS Weak");
        IS.NewRule("RULE 74", "IF Aciertos IS Normal AND Tiempo IS Slow AND Errores IS High AND Ayuda IS Normal THEN Dificultad IS Weak");
        IS.NewRule("RULE 75", "IF Aciertos IS Normal AND Tiempo IS Slow AND Errores IS High AND Ayuda IS High THEN Dificultad IS VeryWeak");
        IS.NewRule("RULE 76", "IF Aciertos IS Normal AND Tiempo IS Slow AND Errores IS Low AND Ayuda IS Low THEN Dificultad IS Normal");
        IS.NewRule("RULE 77", "IF Aciertos IS Normal AND Tiempo IS Slow AND Errores IS Low AND Ayuda IS Normal THEN Dificultad IS Normal");
        IS.NewRule("RULE 78", "IF Aciertos IS Normal AND Tiempo IS Slow AND Errores IS Low AND Ayuda IS High THEN Dificultad IS Weak");
        IS.NewRule("RULE 79", "IF Aciertos IS Normal AND Tiempo IS Slow AND Errores IS Normal AND Ayuda IS Low THEN Dificultad IS Normal");
        IS.NewRule("RULE 80", "IF Aciertos IS Normal AND Tiempo IS Slow AND Errores IS Normal AND Ayuda IS Normal THEN Dificultad IS Normal");
        IS.NewRule("RULE 81", "IF Aciertos IS Normal AND Tiempo IS Slow AND Errores IS Normal AND Ayuda IS High THEN Dificultad IS Weak");

    }
    /*
    public static void Main(string[] args) {
        FuzzyLogic f = new FuzzyLogic(0.75, 0.25, 0.1, 0.0);
        Console.Write(f.DoInference().ToString());

    }
    */
    public double Inferencias()
    {
        double d = DoInference();
        //   Debug.LogFormat("Nivel actual : " +FuzzyToCrisp(d));
        return d;
    }

    private double DoInference()
    {
        /*  Normalizar datos) */
        double dificultad = 0;

        IS.SetInput("Aciertos", naciertos);
        IS.SetInput("Errores", nerrores);
        IS.SetInput("Tiempo", ntiempo);
        IS.SetInput("Ayuda", nayuda);
        Debug.Log("v1: " + naciertos + "   v2: " + nerrores + "      v3: " + ntiempo + "      v4 " + nayuda);

        // Setting outputs
        try
        {
            dificultad = IS.Evaluate("Dificultad");
        }
        catch (Exception)
        {
        }

        return dificultad;
    }
    public int FuzzyToCrisp(double valorIS)
    {

        int level = 0;
        if (valorIS > 0 && valorIS <= 0.46)
        {
            level = 1;
        }
        else if (valorIS > 0.46 && valorIS <= 0.70)
        {
            level = 2;
        }       
        else if (valorIS >= 0.70 && valorIS <= 1.00)
        {
            level = 3;
        }
        Debug.Log("Valor IS:  " + valorIS + "   Nuevo nivel:  " + level);
        return level;
    }
}
