using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DestroyOverTime : MonoBehaviour
{
   public float lifeTime = 1.5f;

   //Destroy(gameObject, lifeTime);d

   void Start(){
       Destroy(gameObject, lifeTime);
   }

}
