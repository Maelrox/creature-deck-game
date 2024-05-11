using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenuController : MonoBehaviour
{
    public string battleSelectScene;
    public TMP_InputField userId;
    public TMP_InputField password;
    public TMP_InputField loginUserId;
    public TMP_InputField loginPassword;
    public GameObject registerModal;
    public GameObject loginModal;
    public GameObject spinner;

    public static MainMenuController instance;

    private void Awake()
    {
        instance = this;
    }

    public void StartGame() {
        SceneManager.LoadScene(battleSelectScene);
        AudioManager.instance.PlaySFX(0);
    }

    public void QuitGame() {
        Application.Quit();
        AudioManager.instance.PlaySFX(0);

    }

    public void Login()
    {
        StartCoroutine(NetworkManager.instance.LoginUser(loginUserId.text, loginPassword.text));
    }

    public void Register()
    {
        spinner.SetActive(true);
        StartCoroutine(NetworkManager.instance.RegisterUser(userId.text, password.text));
    }

    public void OpenRegisterModal()
    {
        registerModal.SetActive(true);
    }

    public void CloseRegisterModal()
    {
        registerModal.SetActive(false);
    }

    public void OpenLoginModal()
    {
        loginModal.SetActive(true);
    }

    public void CloseLoginModal()
    {
        loginModal.SetActive(false);
    }

    public void ShowRegisterResponse(string message)
    {
        if (message.Contains("User registered"))
        {
            CloseRegisterModal();
            //TODO: Show success
        } else
        {
            CloseRegisterModal();
            //TODO: Show error message
            Debug.Log("Register error " + message);
        }
        spinner.SetActive(false);
    }

}
