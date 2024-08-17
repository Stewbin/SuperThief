using System;
using System.Collections;
using System.Collections.Generic;
using Photon.Pun;
using UnityEngine;

public class HealthManager : MonoBehaviourPunCallbacks
{
    [Header("Health")]
    public float MaxHealth = 100f;
    public float CurrentHealth { get; private set; }
    [Header("Damage Multipliers")]
    public float HeadMultiplier;
    public float BodyMultiplier;
    public float ExtremityMultiplier;

    public override void OnEnable()
    {
        base.OnEnable();
        CurrentHealth = MaxHealth;
    }

    [PunRPC]
    private void TakeDamage(string damager, int damageAmount, int actor)
    {
        if (photonView.IsMine)
        {
            // Deduct damage
            CurrentHealth -= damageAmount;

            // Display damage anmount
            UIController.instance.healthSlider.value = CurrentHealth;
            photonView.RPC("UpdateHealthBarRPC", RpcTarget.All, CurrentHealth);

            if (CurrentHealth <= 0)
            {
                CurrentHealth = 0;
                Debug.Log("I'm dead! x<");

                PlayerSpawner.instance.Die(damager);

                MatchManager.instance.UpdateStatsSend(actor, 0, 1);
                MatchManager.instance.UpdateStatsSend(actor, 2, 25);

                string victimName = photonView.Owner.NickName;
                ShowEliminationMessage(damager, victimName);

                // damagerText = photonView.Owner.NickName;
            }

            Debug.Log("Ouch! Current health: " + CurrentHealth);
        }
    }

    public int CalculateDamage(int rawDamage, Collider collider = null)
    {
        // Calculate hitbox bonus
        int netDamage = rawDamage;
        if (collider != null)
        {
            switch (collider.tag)
            {
                case "Head":
                    netDamage *= (int)HeadMultiplier;
                    break;
                case "Body":
                    netDamage *= (int)BodyMultiplier;
                    break;
                case "Extremity":
                    netDamage *= (int)ExtremityMultiplier;
                    break;
            }
        }

        return netDamage;
    }

    [PunRPC]
    private void ShowEliminationMessage(string killer, string victim)
    {
        if (photonView.IsMine && killer == PhotonNetwork.LocalPlayer.NickName)
        {
            StartCoroutine(DisplayEliminationMessage(victim));
            UnityEngine.Debug.Log("Eliminated " + victim);
        }
    }

    private IEnumerator DisplayEliminationMessage(string victim)
    {
        UIController.instance.eliminationMessage.gameObject.SetActive(true);
        UIController.instance.eliminationMessage.text = "Eliminated " + victim;
        UnityEngine.Debug.Log("Eliminated " + victim);

        yield return new WaitForSeconds(5f);

        UIController.instance.eliminationMessage.gameObject.SetActive(false);
    }
}
