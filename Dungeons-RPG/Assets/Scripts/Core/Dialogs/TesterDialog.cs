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

        DialogManager dialogManager = (DialogManager) dialog.GetComponent<DialogManager>();

        dialogManager.addMessage(new DialogMessage("Hola soy brutus", DialogMessage.MODE_NEXT_MESSAGE.AUTOMATIC, brutus));
        dialogManager.addMessage(new DialogMessage("Tengo 2 añitos, crezco muy despacito ", DialogMessage.MODE_NEXT_MESSAGE.AUTOMATIC, brutus));
        dialogManager.addMessage(new DialogMessage("soy brutus", DialogMessage.MODE_NEXT_MESSAGE.AUTOMATIC, brutus));

        dialogManager.startReading();
    }
	
	// Update is called once per frame
	void Update ()
    {
		
	}
}
