using UnityEngine;
using Vuforia;
using UnityEngine.UI;
using System.Linq;
using System.IO;
using System;

/// <summary>
///     A custom handler that implements the ITrackableEventHandler interface.
/// </summary>
public class TrackableEj2 : MonoBehaviour, ITrackableEventHandler {
    protected TrackableBehaviour mTrackableBehaviour;
    GameRules reglas;
    public string anterior ="";

    protected virtual void Start(){
        reglas = new GameRules();
		mTrackableBehaviour = GetComponent<TrackableBehaviour>();
        if (mTrackableBehaviour)
            mTrackableBehaviour.RegisterTrackableEventHandler(this);
    }

    public void OnTrackableStateChanged(TrackableBehaviour.Status previousStatus, TrackableBehaviour.Status newStatus) {
		Text txtTarget = GameObject.Find("txtTarget").GetComponent<Text>();
        if (newStatus == TrackableBehaviour.Status.DETECTED || newStatus == TrackableBehaviour.Status.TRACKED ||
            newStatus == TrackableBehaviour.Status.EXTENDED_TRACKED){
			int ac = Convert.ToInt32 (GameObject.Find("txtAciertos").GetComponent<Text>().text);
            int err = Convert.ToInt32(GameObject.Find("txtErrores").GetComponent<Text>().text);

            if (ac <15 && err < 15 && GameObject.Find ("pnlPausa").gameObject.GetComponent<RectTransform> ().localScale.Equals(Vector3.zero)){
                Debug.Log("Trackable " + mTrackableBehaviour.TrackableName + " found");

                OnTrackingFound();
                
                try
                {
                    CuerpoG cg = LoadingData.Instance.GetDescripciones(mTrackableBehaviour.TrackableName, 1);
                    txtTarget.text = cg.Descripcion;
                }
                catch (Exception e) {
                    Debug.Log("Entro en excepcion con el marcador # "+ mTrackableBehaviour.TrackableName + " " + e.Message);
                }

                bool result = reglas.Validar(txtTarget.text);
				 if (result) {
					GameObject.Find ("pnlPausa").gameObject.SetActive (true);
					GameObject.Find ("pnlPausa").gameObject.GetComponent<RectTransform> ().localScale = new Vector3(0.3f, 0.5f, 1f);					
					reglas.MostrarPanel (result);
					Text txtPausar = GameObject.Find("txtPausar").GetComponent<Text>();
					txtPausar.text = "Si";
				}
            }
        }
        else if (previousStatus == TrackableBehaviour.Status.TRACKED && newStatus == TrackableBehaviour.Status.NO_POSE){
            Debug.Log("Trackable " + mTrackableBehaviour.TrackableName + " lost");
			txtTarget.text = "";
            OnTrackingLost();
        }
        else{
            // For combo of previousStatus=UNKNOWN + newStatus=UNKNOWN|NOT_FOUND Vuforia is starting, but tracking has not been lost or found yet
            // Call OnTrackingLost() to hide the augmentations
            OnTrackingLost();
			txtTarget.text = "";
        }
    }

    protected virtual void OnTrackingFound(){
        var rendererComponents = GetComponentsInChildren<Renderer>(true);
        var colliderComponents = GetComponentsInChildren<Collider>(true);
        var canvasComponents = GetComponentsInChildren<Canvas>(true);


		foreach (var component in rendererComponents)      	// Enable rendering:
            component.enabled = true;

		foreach (var component in colliderComponents)		// Enable colliders:
            component.enabled = true;

		foreach (var component in canvasComponents)			 // Enable canvas':
            component.enabled = true;
    }

    protected virtual void OnTrackingLost(){
        var rendererComponents = GetComponentsInChildren<Renderer>(true);
        var colliderComponents = GetComponentsInChildren<Collider>(true);
        var canvasComponents = GetComponentsInChildren<Canvas>(true);

		foreach (var component in rendererComponents)			   // Disable rendering:
            component.enabled = false;

		foreach (var component in colliderComponents)			   // Disable colliders:
            component.enabled = false;

		foreach (var component in canvasComponents)				   // Disable canvas':
            component.enabled = false;
    }
}
