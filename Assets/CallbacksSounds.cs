using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using FMODUnity;
using FMOD.Studio;

public class CallbacksSounds : MonoBehaviour
{
    private EventInstance throwInst;
    private EventInstance grabInst;
    private void Awake()
    {
        

        


    }
    public void SoundsList(string soundType)
    {
        switch (soundType)
        {
           case "Throw":
               throwInst = RuntimeManager.CreateInstance(
                   "event:/Player/Manipulation Objet,Ennemi/Lancer Ennemi (ok)");
               throwInst.set3DAttributes(RuntimeUtils.To3DAttributes(this.transform));
               RuntimeManager.AttachInstanceToGameObject (throwInst, this.transform,this.GetComponent<Rigidbody> ());
               throwInst.start();
               break;
           case "Grab":
               grabInst = RuntimeManager.CreateInstance(
                   "event:/Player/Manipulation Objet,Ennemi/Attraper Ennemi (ok)");
               grabInst.set3DAttributes(RuntimeUtils.To3DAttributes(this.transform));
               RuntimeManager.AttachInstanceToGameObject (grabInst, this.transform,this.GetComponent<Rigidbody> ());
               grabInst.start();
               break;
        }
    }
}
