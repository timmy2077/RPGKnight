using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneChanger : MonoBehaviour
{

    public string sceneToLoad;
    public Animator fadeAnim;
    public Vector2 newPlayerPos;
    private Transform player;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            player = collision.transform;
            fadeAnim.Play("ToWhite");
            StartCoroutine(FadeIn());
        }
    }

    IEnumerator FadeIn()
    {
        yield return new WaitForSeconds(0.4f);
        if (player != null)
        {
            player.position = newPlayerPos;
        }
        SceneManager.LoadScene(sceneToLoad);
    }

}
