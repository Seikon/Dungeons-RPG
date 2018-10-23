using Assets.Scripts.Core.Dialogs;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TesterDialog : MonoBehaviour {

	// Use this for initialization
	void Start ()
    {
        GameObject dialog = (GameObject)Instantiate(Resources.Load("Prefabs/Dialogs/DialogManager"));
        dialog.transform.SetParent(this.gameObject.transform);

        GameObject brutus = GameObject.Find("BrutusElPutus");
        GameObject skeleton = GameObject.Find("Skeleton");

        DialogManager dialogManager = (DialogManager) dialog.GetComponent<DialogManager>();

        dialogManager.addMessage(new DialogMessage("Hola soy brutus", DialogMessage.MODE_NEXT_MESSAGE.AUTOMATIC, brutus));
        dialogManager.addMessage(new DialogMessage("No te voy a poner el Lorem Impsum, pero por favor no leas mis chorradas.", DialogMessage.MODE_NEXT_MESSAGE.AUTOMATIC, brutus));
        dialogManager.addMessage(new DialogMessage("Centrate en el funcionamiento del diálogo... vale?.", DialogMessage.MODE_NEXT_MESSAGE.AUTOMATIC, brutus));
        dialogManager.addMessage(new DialogMessage("Mierda no tengo el símbolo de cerrar exclamación en este Tastateur alemán.", DialogMessage.MODE_NEXT_MESSAGE.AUTOMATIC, brutus));

        dialogManager.addMessage(new DialogMessage("Haber si me dibujan ya, estos sprites los han sacado de la super nes", DialogMessage.MODE_NEXT_MESSAGE.AUTOMATIC, skeleton));

        dialogManager.addMessage(new DialogMessage("No te quejes que yo en el juego ni existo...", DialogMessage.MODE_NEXT_MESSAGE.AUTOMATIC, brutus));

        dialogManager.addMessage(new DialogMessage("Vaya...", DialogMessage.MODE_NEXT_MESSAGE.AUTOMATIC, skeleton));

        dialogManager.startReading();
    }
	
	// Update is called once per frame
	void Update ()
    {
		
	}
}
