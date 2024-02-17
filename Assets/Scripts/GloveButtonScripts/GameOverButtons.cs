using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameOverButtons : MonoBehaviour
{
    private void OnTriggerEnter(Collider other) {
        SceneManager.LoadScene("HomeScene");
    }
}