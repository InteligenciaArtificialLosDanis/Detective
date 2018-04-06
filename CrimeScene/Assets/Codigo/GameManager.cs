using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour {

	public static GameManager instance;

	//Public GameObject
	public GameObject suelo; 		 
	public GameObject nube;	 		  
	public GameObject hueco; 		
	public GameObject sangre;		
	public GameObject nubeSangre;    
	public GameObject casa;			

	//Agentes de la escena del crimen
	public GameObject agente;		
	public GameObject muerto;		
	public GameObject arma;

	int xArma, yArma;
	int xMuerto, yMuerto;
	int xCasa, yCasa;

	//SUper enumerado de estados de juego :3
	enum TipoCasilla {
		vacio,
		hueco, sueloVacio, sueloArma, 
		nubeVacia, nubeSangre,
		sangre, muerto, casa
	};

	//Constantes útiles
	 const int anchoTablero = 10;
     const int altoTablero = 5;
     const int numHuecos = 4;

	//GameObject [,] tablero;
	TipoCasilla [,] tablero;

	// Use this for initialization
	void Start () {

		tablero = new TipoCasilla [anchoTablero, altoTablero];
		//Creamos el tablero todo a vacio.
		for (int i = 0; i < anchoTablero; i++) {
			for (int j = 0; j < altoTablero; j++) {
				tablero [i, j] = TipoCasilla.vacio;
			}
		}
		creaTablero (tablero);
		instanciaMovidas (tablero);
	}
	
	// Update is called once per frame
	void Update () {
		
	}


    ///////////////////////////////////////
    //__MÉTODOS DE COMUNICACION CON IA___//
    ///////////////////////////////////////
    public TipoCasilla getInfoCasilla(int x, int y)
    {
        return tablero[x, y];
    }

    public void retiraGameObject(string obj)
    {
        if (obj == "muerto")
        {
            //Poner el muerto a invisible
        }
        else if (obj == "arma")
        {
            //Poner el arma a invisible
        }
    }

    void getDimensiones(out int alto, out int ancho) {
        alto = altoTablero;
        ancho = anchoTablero;
    }

    ///////////////////////////////////////
    //__MÉTODOS DE CREACIÓN DE TABLERO___//
    ///////////////////////////////////////
	void instanciaMovidas(TipoCasilla[,]tablero){
	
		for (int i = 0; i < anchoTablero; i++) {
			for (int j = 0; j < altoTablero; j++) {

				switch (tablero [i, j]) {

				case TipoCasilla.sueloVacio: //SUELO
					instanciameEsta(suelo,i,j);
					break;

				case TipoCasilla.sueloArma: //SUELO con ARMA
					instanciameEsta(suelo,i,j);
					instanciameEsta(arma,i,j);
					break;

				case TipoCasilla.hueco: //HUECO
					instanciameEsta (hueco, i, j);
					break;
				//--nubes
				case TipoCasilla.nubeVacia:
					instanciameEsta (nube, i, j);
					break;

				case TipoCasilla.nubeSangre:
					instanciameEsta (nubeSangre, i, j);
					break;

				//--sangre
				case TipoCasilla.sangre:
					instanciameEsta (sangre, i, j);
					break;


				
				//--muerto
				
				case TipoCasilla.muerto:
					instanciameEsta (muerto, i, j);
					break;


				//-casa
				case TipoCasilla.casa:
					instanciameEsta (casa, i, j);
                    instanciameEsta(agente, i, j);
                    break;

				}
			}
		}
	}

	void instanciameEsta (GameObject obj, int x, int y){
		Instantiate (obj, new Vector3 (x, y, 0), Quaternion.identity);
	}


	//CREA EL TABLERO DE JUEGO EN TERMINOS DE INFORMACIÓN
	void creaTablero(TipoCasilla[,] tablero){

		//Tenemos el tablero vacío, de dimensiones ya definidas, y vamos a situar los huecos 
		int x, y;
		//Primero los huecos y las nubes
		for (int i = 0; i < numHuecos; i++) {
			
			do{
				x = Random.Range(0,anchoTablero);
				y = Random.Range(0,altoTablero);

			} while (tablero[x,y] == TipoCasilla.hueco);

			tablero [x, y] = TipoCasilla.hueco;

			creaNubes (x, y);
		}

		//Después metemos el cadaver y la sangre :3
		do{
			x = Random.Range(0,anchoTablero);
			y = Random.Range(0,altoTablero);


		} while (tablero[x,y]== TipoCasilla.hueco || tablero[x,y] == TipoCasilla.nubeVacia);

		tablero [x, y] = TipoCasilla.muerto;
		xMuerto = x;
		yMuerto = y;
		creaSangre (x, y);	//Crea la sangre y cambia la matriz si es necesario
		ponArma (x, y);		//Crea el arma en la zona cercana al cadaver

		//Relleno el resto de huecos con casillas normales
		for (int i = 0; i < anchoTablero; i++) {
			for (int j = 0; j < altoTablero; j++) {
				//Si es null implica que no se ha llenado.
				if (tablero [i, j] == TipoCasilla.vacio) {
					tablero [i, j] = TipoCasilla.sueloVacio;
				}
			}
		}

		//Finalmente, ponemos la casa y el personaje en el mismo sítio
		do{
			x = Random.Range(0,anchoTablero);
			y = Random.Range(0,altoTablero);

		} while (tablero[x,y] != TipoCasilla.sueloVacio);

		tablero [x, y] = TipoCasilla.casa;
		xCasa = x;
		yCasa = y;
		
	}


 	void creaNubes(int x, int y){

		//Norte
		if (y - 1 >= 0 && tablero [x, y - 1] != TipoCasilla.hueco) {

			//No compruebo con la sangre porque esto es primero
			tablero [x, y - 1] = TipoCasilla.nubeVacia;
		}

		//Sur
		if (y + 1 < altoTablero && tablero [x, y + 1] != TipoCasilla.hueco) {
			tablero [x, y + 1] = TipoCasilla.nubeVacia;
		}

		//Izquierda
		if (x - 1 >= 0 && tablero [x - 1, y] != TipoCasilla.hueco) {
			tablero [x-1, y ] = TipoCasilla.nubeVacia;
		}

		//Derecha
		if (x + 1 < anchoTablero  && tablero [x + 1, y] != TipoCasilla.hueco) {
			tablero [x + 1, y] = TipoCasilla.nubeVacia;
		}

	}

	//Se asume que la sangre no va a caer por un hueco
	void creaSangre(int x, int y){

		//Norte
		if (y - 1 >= 0) {
			if(tablero [x, y - 1] != TipoCasilla.nubeVacia)
				tablero [x, y - 1] = TipoCasilla.sangre;
				
			else tablero [x, y - 1] = TipoCasilla.nubeSangre;
		}
	

		//Sur
		if (y + 1 < altoTablero) {
			if(tablero [x, y + 1] != TipoCasilla.nubeVacia)
				tablero [x, y + 1] = TipoCasilla.sangre;
			
			else tablero [x, y + 1] = TipoCasilla.nubeSangre;
		}


		//Izquierda
		if (x - 1 >= 0) {
			if( tablero [x - 1, y] != TipoCasilla.nubeVacia)
				tablero [x-1, y] = TipoCasilla.sangre;

			else tablero [x-1, y] = TipoCasilla.nubeSangre;
		}


		//Derecha
		if (x + 1 < anchoTablero) {
			if(tablero [x + 1, y] != TipoCasilla.nubeVacia)
				tablero [x + 1, y] = TipoCasilla.sangre;
			
			else tablero [x+ 1, y ] = TipoCasilla.nubeSangre;
		}

	}

	void ponArma(int x, int y){
        int caso;
        bool ok = false;
        do {

            caso = Random.Range(0, 9);

            switch (caso)
            {
                case 0: //ARRIBA
                    if ( y-2 >=0 && tablero[x, y-2] != TipoCasilla.hueco && tablero[x, y - 2] != TipoCasilla.nubeVacia && tablero[x, y - 2] != TipoCasilla.nubeSangre)
                    {
                        tablero[x, y - 2] = TipoCasilla.sueloArma;
                        //Set de la posición del arma
                        xArma = x;
                        yArma = y -2;
                        ok = true;
                    }
                    break;

                case 1: //ABAJO
                    if (y + 2 < altoTablero && tablero[x, y + 2] != TipoCasilla.hueco && tablero[x, y + 2] != TipoCasilla.nubeVacia && tablero[x, y + 2] != TipoCasilla.nubeSangre)
                    {
                        tablero[x, y + 2] = TipoCasilla.sueloArma;
                        //Set de la posición del arma
                        xArma = x;
                        yArma = y +2;
                        ok = true;
                    }
                    break;

                case 2: //IZQUIERDA
                    if (x - 2 >= 0 && tablero[x - 2, y] != TipoCasilla.hueco && tablero[x - 2, y] != TipoCasilla.nubeVacia && tablero[x - 2, y] != TipoCasilla.nubeSangre)
                    {
                        tablero[x -2, y] = TipoCasilla.sueloArma;
                        //Set de la posición del arma
                        xArma = x -2;
                        yArma = y;
                        ok = true;
                    }
                    break;

                case 3: //DERECHO
                    if (x + 2 < anchoTablero && tablero[x + 2, y ] != TipoCasilla.hueco && tablero[x + 2, y ] != TipoCasilla.nubeVacia && tablero[x + 2, y] != TipoCasilla.nubeSangre)
                    {
                        tablero[x + 2, y] = TipoCasilla.sueloArma;
                        //Set de la posición del arma
                        xArma = x +2;
                        yArma = y;
                        ok = true;
                    }
                    break;

                case 4: //DIAGONAL ARRIBA-IZQUIERDA
                    if (y - 1 >= 0 && x-1 >=0 && tablero[x -1, y - 1] != TipoCasilla.hueco && tablero[x - 1, y - 1] != TipoCasilla.nubeVacia && tablero[x - 1, y - 1] != TipoCasilla.nubeSangre)
                    {
                        tablero[x - 1, y - 1] = TipoCasilla.sueloArma;
                        //Set de la posición del arma
                        xArma = x -1;
                        yArma = y -1;
                        ok = true;
                    }
                    break;

                case 5: //DIAGONAL ARIBA DERECHA
                    if (y - 1 >= 0 && x + 1 < anchoTablero && tablero[x + 1, y - 1] != TipoCasilla.hueco && tablero[x + 1, y - 1] != TipoCasilla.nubeVacia && tablero[x + 1, y - 1] != TipoCasilla.nubeSangre)
                    {
                        tablero[x + 1, y - 1] = TipoCasilla.sueloArma;
                        //Set de la posición del arma
                        xArma = x +1;
                        yArma = y -1;
                        ok = true;
                    }
                    break;

                case 6: //DIAGONAL ABAJO IZQUIERDA
                    if (y + 1 < altoTablero && x - 1 >= 0 && tablero[x - 1, y + 1] != TipoCasilla.hueco && tablero[x - 1, y + 1] != TipoCasilla.nubeVacia && tablero[x - 1, y + 1] != TipoCasilla.nubeSangre)
                    {
                        tablero[x - 1, y + 1] = TipoCasilla.sueloArma;
                        //Set de la posición del arma
                        xArma = x - 1;
                        yArma = y + 1;
                        ok = true;
                    }
                    break;

                case 7: //DIAGNAL ABAJO DERECHA
                    if (y + 1 < altoTablero && x +1 < anchoTablero && tablero[x + 1, y + 1] != TipoCasilla.hueco && tablero[x + 1, y + 1] != TipoCasilla.nubeVacia && tablero[x + 1, y + 1] != TipoCasilla.nubeSangre)
                    {
                        tablero[x + 1, y + 1] = TipoCasilla.sueloArma;
                        //Set de la posición del arma
                        xArma = x + 1;
                        yArma = y + 1;
                        ok = true;
                    }
                    break;

              
            }




        } while (!ok);


		

	}

}
