using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SendAnimationEventToSFX : MonoBehaviour
{
   public PlayerSoundManager playerSoundManager; 

   public void TriggerFootStepSFX()
   {
        playerSoundManager.PlayFootStepSFX();
   }
}