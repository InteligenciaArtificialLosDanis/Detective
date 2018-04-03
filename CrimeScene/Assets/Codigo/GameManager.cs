using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour {

	public static GameManager instance;

	//Public GameObject
	public GameObject nube;
	public GameObject hueco;
	public GameObject suelo;
	public GameObject sangre;
	public GameObject nubeSangre;

	//Agentes de la escena del crimen
	public GameObject agente;
	public GameObject muerto;
	public GameObject arma;
	int xArma, yArma;

	//Constantes útiles
	const int anchoTablero = 10;
	const int altoTablero = 5;
	const int numHuecos = 4;

	GameObject [,] tablero;

	// Use this for initialization
	void Start () {

		tablero = new GameObject [anchoTablero, altoTablero];
	}
	
	// Update is called once per frame
	void Update () {
		
	}


	void creaTablero(GameObject[,] tablero){

		//Tenemos el tablero vacío, de dimensiones ya definidas, y vamos a situar los huecos 
		Random rnd = new Random();
		int x, y;
		//Primero los huecos y las nubes
		for (int i = 0; i < numHuecos; i++) {
			
			do{
				x = rnd.Next(0,anchoTablero);
				y = rnd.Next(0,altoTablero);

			} while (tablero[x,y] == hueco);

			tablero [x, y] = hueco;
			creaNubes (x, y);
		}

		//Después metemos el cadaver y la sangre :3
		do{
			x = rnd.Next(0,anchoTablero);
			y = rnd.Next(0,altoTablero);


		} while (tablero[x,y]== hueco || tablero[x,y] == nube);
		tablero [x, y] = muerto;
		creaSangre (x, y);

		//Relleno el resto de huecos con casillas normales

		
	}


	void creaNubes(int x, int y){

		//Norte
		if (y + 1 > 0 && tablero [x, y + 1] != hueco) {

			//No compruebo con la sangre porque esto es primero
			tablero [x, y + 1] = nube;
		}

		//Sur
		if (y - 1 < altoTablero && tablero [x, y - 1] != hueco) {
			tablero [x, y - 1] = nube;
		}

		//Izquierda
		if (x - 1 > 0 && tablero [x - 1, y] != hueco) {
			tablero [x, y - 1] = nube;
		}

		//Derecha
		if (x + 1 < anchoTablero  && tablero [x + 1, y] != hueco) {
			tablero [x + 1, y] = nube;
		}

	}

	//Se asume que la sangre no va a caer por un hueco
	void creaSangre(int x, int y){

		//Norte
		if (y + 1 > 0 && tablero [x, y + 1] != nube) {

			//No compruebo con la sangre porque esto es primero
			tablero [x, y + 1] = sangre;
		}
		else tablero [x, y + 1] = nubeSangre;

		//Sur
		if (y - 1 < altoTablero && tablero [x, y - 1] != nube) {
			tablero [x, y - 1] = sangre;
		}
		else tablero [x, y + 1] = nubeSangre;

		//Izquierda
		if (x - 1 > 0 && tablero [x - 1, y] != nube) {
			tablero [x, y - 1] = sangre;
		}
		else tablero [x, y + 1] = nubeSangre;

		//Derecha
		if (x + 1 < anchoTablero  && tablero [x + 1, y] != nube) {
			tablero [x + 1, y] = sangre;
		}
		else tablero [x, y + 1] = nubeSangre;
	}

	void ponArma(int x, int y){
		int i, j;
		do {

			i = rnd.Next (-1, 2);
			j = rnd.Next (-1, 2);
		} while (tablero [x + i, y + j] == hueco);
	
		//TODO: Set de la posición del arma
		xArma = x + i;
		yArma = y + j;
	}

}
