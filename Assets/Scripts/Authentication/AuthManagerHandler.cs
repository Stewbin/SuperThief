using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Unity.Services.Core;
using Unity.Services.Authentication;
using System.Threading.Tasks;
using static System.Net.Mime.MediaTypeNames;
using System.Diagnostics;
using System;
using TMPro;

public class AuthManagerHandler : MonoBehaviour
{
    public TextMeshProUGUI loginText;

    async void Start()
    {
        await InitializeUnityServices();
    }

    public async void SignIn()
    {
        await SignInAnonymous();
    }

    async Task InitializeUnityServices()
    {
        try
        {
            await UnityServices.InitializeAsync();
            print("Unity Services Initialized Successfully");
        }
        catch (Exception ex)
        {
            print("Failed to initialize Unity Services");
             print(ex);
        }
    }

    async Task SignInAnonymous()
    {
        try
        {
            if (AuthenticationService.Instance != null)
            {
                await AuthenticationService.Instance.SignInAnonymouslyAsync();
                print("Sign In Success");
                print("Player ID: " + AuthenticationService.Instance.PlayerId);
                loginText.text = "Player ID: " + AuthenticationService.Instance.PlayerId;
            }
            else
            {
                print("AuthenticationService instance is null");
            }
        }
        catch (AuthenticationException ex)
        {
            print("Sign In Failed");
            print(ex);
        }
        catch (Exception ex)
        {
            print("An unexpected error occurred during sign in");
            print(ex);
        }
    }
}
