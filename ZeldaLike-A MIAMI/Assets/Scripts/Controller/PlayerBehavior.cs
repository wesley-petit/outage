﻿/* Author : Raphaël Marczak - 2018/2020, for MIAMI Teaching (IUT Tarbes)
 * 
 * This work is licensed under the CC0 License. 
 * 
 */

using UnityEngine;

// Direction du personnage
public enum CardinalDirections { CARDINAL_S, CARDINAL_N, CARDINAL_W, CARDINAL_E};

// Comportement du joueur
public class PlayerBehavior : MonoBehaviour
{
	public float speed = 1f;                                  // Vitesse de déplacement
	private CardinalDirections direction;                     // Actuel direction du joueur

	//animation du personnage
	Animator animationSens;

	public GameObject map = null;                           // Carte à afficher
	public DialogManager dialogDisplayer;                   // Dialogue Manager / S'occupe d'afficher les dialogues
	[SerializeField] private GameObject _uiInteract = null;	// UI d'interaction à afficher / cacher

	public bool BlockByNPC { get; set; }                    // Bloquer par un NPC ou non
	public CardinalDirections BlockDirection { get; set; }	// Bloque le joueur dans une direction précise

	private Dialog closestNPCDialog;                        // Dialogue du NPC le plus proche

	private interact interact;								// Interagir avec les items d'interaction

	Rigidbody2D rb2D;										// Composant permettant d'appliquer de la physique
	
	void Awake()
	{
		// Initialise les variables en utilisant les composants du GameObject
		rb2D = GetComponent<Rigidbody2D>();
		animationSens = GetComponent<Animator>();

		closestNPCDialog = null;
	}

	/// <summary>
	/// FixedUpdate est appelée chaque frame à un intervalle constant / fixe
	/// Elle est utilisé pour tout ce qui touche à la physique (ici le RigidBody)
	/// </summary>
	void FixedUpdate()
	{
		// Si un dialogue ou une carte est affiché,
		// le joueur ne doit pas faire d'action
		if (dialogDisplayer.IsOnScreen || map.activeSelf)
		{
			return;
		}

		// Déplace le joueur selon les touches utilisées
		Move();
	}

	// Déplace le joueur
	private void Move()
	{
		// Valeur de déplacement sur l'axe X
		float horizontalOffset = Input.GetAxis("Horizontal");
		// Valeur de déplacement sur l'axe Y
		float verticalOffset = Input.GetAxis("Vertical");

		// Si le joueur est bloqué par un NPC et 
		// si la direction choisi et celle bloqué
		if (BlockByNPC)
		{
			horizontalOffset = InputWithBlockDirection(horizontalOffset, CardinalDirections.CARDINAL_E, CardinalDirections.CARDINAL_W);
			verticalOffset = InputWithBlockDirection(verticalOffset, CardinalDirections.CARDINAL_N, CardinalDirections.CARDINAL_S);
		}

		// Donne la direction principal du joueur
		if (Mathf.Abs(horizontalOffset) > Mathf.Abs(verticalOffset))
		{
			animationSens.SetBool("marche", true);
			if (horizontalOffset > 0)
			{
				direction = CardinalDirections.CARDINAL_E;
			}
			else
			{
				direction = CardinalDirections.CARDINAL_W;
			}
		}
		else if (Mathf.Abs(horizontalOffset) < Mathf.Abs(verticalOffset))
		{
			animationSens.SetBool("marche", true);
			if (verticalOffset > 0)
			{
				direction = CardinalDirections.CARDINAL_N;
			}
			else
			{
				direction = CardinalDirections.CARDINAL_S;
			}
		}
		else if (0 < Mathf.Abs(horizontalOffset) && (Mathf.Abs(horizontalOffset) == Mathf.Abs(verticalOffset)))
		{
			animationSens.SetBool("marche", true);
		}
		else { animationSens.SetBool("marche", false); }
		

		// Utilise les inputs de l'utilisateur pour les multiplier par la vitesse de déplacement
		Vector2 newPos = new Vector2(transform.position.x + horizontalOffset * speed,
									 transform.position.y + verticalOffset * speed);
		rb2D.MovePosition(newPos);
	}

	// Donne les valeurs inputs quand le joueur est bloqué par le NPC
	private float InputWithBlockDirection(float input, CardinalDirections max, CardinalDirections minus)
	{
		// Si on veut aller dans le même sens que l'une des directions
		if (BlockDirection == max && 0 < input || BlockDirection == minus && input < 0)
		{
			input = 0;
		}

		return input;
	}

	/// <summary>
	/// Update appelée à chaque frame, mais à un intervalle variant / changeant
	/// Utilisé principalement pour toutes autres activités
	/// ne demandant pas le calcul de physique (pas le RigidBody)
	/// </summary>
	private void Update()
	{        
		// If the player presses M, the map will be activated if not on screen
		// or desactivated if already on screen
		if (Input.GetKeyDown(KeyCode.M))
		{
			map.SetActive(!map.activeSelf);
		}

		// Quitte le jeu sur la touche Echap
		if (Input.GetKeyDown(KeyCode.Escape))
		{
			Application.Quit();
		}

		// Si un dialogue ou une carte est affiché,
		// le joueur ne doit pas faire d'action
		if (dialogDisplayer.IsOnScreen || map.activeSelf)
		{
			return;
		}

		// Change l'animation du personnage selon la direction du joueur
		ChangeSpriteToMatchDirection();

		///<summary>
		/// Si le joueur appuie sur la barre d'espace :
		///   - Si un dialogue doit se lancer (joueur proche d'un NPC),
		///     alors il l'affiche
		///   - Sinon, il fait une autre action comme tirée une boule de feu
		/// </summary>
		if (Input.GetKeyDown(KeyCode.Space))
		{
			if (closestNPCDialog != null)
			{
				dialogDisplayer.SetDialog(closestNPCDialog.GetDialog());
			}
			else
			{
				if (interact)
				{
					interact.tilesexchanger();
					_uiInteract.SetActive(false);
				}
			}
		}
	}

	// Change le sprite du joueur selon la direction
	// (back when going North, front when going south, right when going east, left when going west)
	private void ChangeSpriteToMatchDirection() => animationSens.SetInteger("direction_animation", (int)direction);

	/// <summary>
	/// Appeler automatiquement par Unity quand un élément RENTRE dans une zone "trigger"
	/// (Zone de collision permettant de détecter la collision sans appliquer de physique comme bloquer un objet)
	///      - le joueur est proche de la zone d'un NPC, il va alors récupérer les informations de dialogues
	///        afin de les utiliser par la suite en utilisant espace 
	///      - le joueur est dans une zone d'instantDialog ou de réflexion, il va afficher directement le dialogue
	/// </summary>
	private void OnTriggerEnter2D(Collider2D collision)
	{
		if (collision.tag == "NPC")
		{
			closestNPCDialog = collision.GetComponent<Dialog>();
		}
		else if (collision.tag == "InstantDialog")
		{
			Dialog instantDialog = collision.GetComponent<Dialog>();
			if (instantDialog != null)
			{
				dialogDisplayer.SetDialog(instantDialog.GetDialog());
			}
		}
		else if (collision.tag == "Interaction")
        {
			interact = collision.GetComponent<interact>();

			if (!interact.IsOff)
			{
				_uiInteract.SetActive(true);
			}
        }
	}

	/// <summary>
	/// Appeler automatiquement par Unity quand un élément SORT dans une zone "trigger"
	/// (Zone de collision permettant de détecter la collision sans appliquer de physique comme bloquer un objet)
	///      - le joueur était dans la zone d'un NPC, l'information stockée servant au dialogue est alors détruite
	///      - le joueur était dans une zone d'instantDialog ou de réflexion, l'instantDialogue est détruit
	///        (déjà afficher, et il ne doit s'afficher qu'une seul fois)
	/// </summary>
	private void OnTriggerExit2D(Collider2D collision)
	{
		if (collision.tag == "NPC")
		{
			closestNPCDialog = null;
		}
		else if (collision.tag == "InstantDialog")
		{
			Destroy(collision.gameObject);
		}
		else if (collision.tag == "Interaction")
        {
			interact = null;
			_uiInteract.SetActive(false);
		}
	}
}
