using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace Assets.Scripts.Core.Dialogs
{
    public class DialogMessage
    {
        public enum  MODE_NEXT_MESSAGE //Indica el modo en el que el mensaje desaparecé y salta el siguiente
        {
            AUTOMATIC = 0, //Automáticamente cuando el mensaje se ha mostrado
            USER_INTERACTIVE = 1,  //Cuando el usuario pulsa una tecla
            LOGIC_INTERACTIVE = 2 // Cuando se llama a la función next message

        }
        public char[] message;
        public MODE_NEXT_MESSAGE mode;
        public GameObject emitter; //El objeto que emite el mensaje y del cual obtiene su posición

        public DialogMessage(string message, MODE_NEXT_MESSAGE mode, GameObject emitter)
        {
            this.message = message.ToArray<Char>();
            this.mode = mode;
            this.emitter = emitter;
        }
    }
}
