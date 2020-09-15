﻿/* Author : Raphaël Marczak - 2018/2020, for MIAMI Teaching (IUT Tarbes)
 * 
 * This work is licensed under the CC0 License. 
 * 
 */

using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

<<<<<<< Updated upstream
// Struct représentant la page d'un dialogue
=======
// Structure représentant la page d'un dialogue
>>>>>>> Stashed changes
// (texte et la couleur)
[System.Serializable]
public struct DialogPage
{
	public string text;
	public Color color;
}

// S'occupe d'afficher un dialogue
public class DialogManager : MonoBehaviour
{
<<<<<<< Updated upstream
	public Text renderText;                                             // Texte dans lequel le dialogue est écrit
	private List<DialogPage> dialogToDisplay;                           // Dialogues à écrire
=======
	public Text renderText;                         // Texte dans lequel le dialogue est écrit
	private List<DialogPage> dialogToDisplay;       // Dialogues à écrire
>>>>>>> Stashed changes

	// Définie le dialogue à afficher
	public void SetDialog(List<DialogPage> dialogToAdd)
	{
		dialogToDisplay = new List<DialogPage>(dialogToAdd);

		if (dialogToDisplay.Count > 0)
		{
			if (renderText != null)
			{
				renderText.text = "";
			}

			gameObject.SetActive(true);
		}
	}

	// Update is called once per frame
	void Update()
	{
		if (renderText == null)
		{
			gameObject.SetActive(false);
		}

		// Affiche un dialogue tant qu'il en reste dans la liste
		if (dialogToDisplay.Count > 0)
		{
<<<<<<< Updated upstream
			renderText.text = dialogToDisplay[0].text;
			renderText.color = dialogToDisplay[0].color;
=======
			renderText.text = dialogToDisplay[0].text;		//RenderText.text récupère le texte qu'il y a dans la liste
			renderText.color = dialogToDisplay[0].color;	//RenderText.color récupère la couleur qu'il y a dans la liste
>>>>>>> Stashed changes
		}
		else
		{
			gameObject.SetActive(false);
		}

		// Supprime la premiére page quand le joueur appuie sur la barre espace
		if (Input.GetKeyDown(KeyCode.Space))
		{
<<<<<<< Updated upstream
			dialogToDisplay.RemoveAt(0);
=======
			dialogToDisplay.RemoveAt(0);	//On suprrime un emplacement de la liste avec sont contenu
>>>>>>> Stashed changes
		}
	}

	// Si un dialogue est actuellement à l'écran
	public bool IsOnScreen()
	{
		return gameObject.activeSelf;
	}
}