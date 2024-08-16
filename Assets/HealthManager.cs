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
    public float HeadBonus;
    public float BodyBonus;
    public float ExtremityBonus;

    public override void OnEnable()
    {
        base.OnEnable();
        CurrentHealth = MaxHealth;
    }

    [PunRPC]
    private void TakeDamage(string damager, int damageAmount, int actor, Collider collider=null, Action onDealDamage = null)
    {
        if (photonView.IsMine)
        {
            CurrentHealth -= CalculateDamage(damageAmount, collider);

            // Display damage anmount
            UIController.instance.healthSlider.value = CurrentHealth;
            photonView.RPC("UpdateHealthBarRPC", RpcTarget.All, CurrentHealth);

            if (CurrentHealth <= 0)
            {
                CurrentHealth = 0;

                PlayerSpawner.instance.Die(damager);

                MatchManager.instance.UpdateStatsSend(actor, 0, 1);
                MatchManager.instance.UpdateStatsSend(actor, 2, 25);

                string victimName = photonView.Owner.NickName;
                ShowEliminationMessage(damager, victimName);

                // damagerText = photonView.Owner.NickName;
            }
        }
    }

    private int CalculateDamage(int rawDamage, Collider collider=null)
    {
        // Calculate hitbox bonus
        int netDamage = rawDamage;
        if (collider != null)
        {
            switch (collider.tag)
            {
                case "Head":
                    netDamage *= (int)HeadBonus;
                    break;
                case "Body":
                    netDamage *= (int)BodyBonus;
                    break;
                case "Extremity":
                    netDamage *= (int)ExtremityBonus;
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
