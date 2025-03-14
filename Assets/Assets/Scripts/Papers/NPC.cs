using System;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class NPC : MonoBehaviour
{
    public GameObject dialoguePanel;
    public Text dialogueText;
    public string[] dialogue;
    private int index;

    public float wordSpeed = 0.05f;
    private bool isTyping = false;

    //cuando se haga click en npc se activa el dialogo 
    void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            Vector2 mousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            RaycastHit2D hit = Physics2D.Raycast(mousePosition, Vector2.zero);

            if (hit.collider != null && hit.collider.CompareTag("NPC"))
            {
                ActivateDialogue();
            }
        }
    }

    //si esta vacio, definine inicial y manda a llamar para iniciar 
    void ActivateDialogue()
    {
        if (dialoguePanel != null)
        {
            dialoguePanel.SetActive(true);
            index = 0;
            dialogueText.text = "";
            StartCoroutine(Typing());
        }
    }

    //
    IEnumerator Typing()
    {
        isTyping = true;
        dialogueText.text = ""; //borra antes de escribir nueva linea 

        foreach (char letter in dialogue[index].ToCharArray())
        {
            dialogueText.text += letter;
            yield return new WaitForSeconds(wordSpeed);
        }

        isTyping = false;
    }

    public void NextLine()
    {
        if (!isTyping) // Solo avanza si el texto terminó de escribirse
        {
            if (index < dialogue.Length - 1)
            {
                index++;
                StartCoroutine(Typing());
            }
            else
            {
                zeroText();
            }
        }
    }

    //si se ha terminado escribir regresa a default vacio y desde inicio y quita panel 
    public void zeroText()
    {
        dialogueText.text = "";
        index = 0;
        dialoguePanel.SetActive(false);
    }
}

