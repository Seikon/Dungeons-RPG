using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Assets.Scripts.Core.Dialogs
{

    public class DialogManager : MonoBehaviour
    {
        private DialogMessage currentMessage;
        private string buffer;
        private Text txtPlacer;
        private DIALOG_MANAGER_STATE state = DIALOG_MANAGER_STATE.PAUSE;
        private int letterIndex = 0;
        private Queue<DialogMessage> messages = new Queue<DialogMessage>();
        private float speed = 0.05f; //segundos por letra
        private float time = 0.0f; 

        public enum DIALOG_MANAGER_STATE
        {
            PAUSE = 0,
            READING = 1
        }

        // Use this for initialization
        void Start ()
        {
	    }
	
	    // Update is called once per frame
	    void Update ()
        {

            if (state == DIALOG_MANAGER_STATE.READING)
            {
                //Muestra la letra en función de la velocidad
                //Cuando se contabilice un tiempo igual a la velocidad de caracter por letra,
                // se muestra la letra
                if (this.time <= this.speed)
                {
                    this.time += Time.deltaTime;
                }
                else
                {
                    this.time = 0;

                    if (letterIndex == this.currentMessage.message.Length)
                    {
                        this.nextMessage();
                    }
                    else
                    {
                        this.readCurrentMessage();
                    }
                }
            }
	    }

        private void readCurrentMessage()
        {

            buffer += this.currentMessage.message[letterIndex];

            letterIndex++;
            txtPlacer.text = buffer;
        }

        private void nextMessage()
        {
            this.buffer = "";
            this.letterIndex = 0;

            if(this.messages.Count > 0)
            {
                this.currentMessage = messages.Dequeue();
                Vector2 position = this.currentMessage.emitter.transform.position;
                position.y += this.currentMessage.emitter.GetComponent<RectTransform>().rect.width;
                this.gameObject.transform.SetPositionAndRotation(position,
                                                                 this.currentMessage.emitter.transform.rotation);
            }
            else
            {
                this.stopReading();
            }
        }

        public void addMessage(DialogMessage newMessage)
        {
            this.messages.Enqueue(newMessage);
        }

        public void startReading()
        {
            this.state = DIALOG_MANAGER_STATE.READING;
            this.txtPlacer = gameObject.GetComponentsInChildren<Text>()[0];
            this.nextMessage();
        }

        public void stopReading()
        {
            this.state = DIALOG_MANAGER_STATE.PAUSE;
        }
    }

}
