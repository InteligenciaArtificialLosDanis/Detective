﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IAManager : MonoBehaviour {

    //La IA consiste en un recorrido a ciegas hasta que llega a la sangre y cambia de modo

    enum modoAgente {Patrullando, Analizando, Volviendo, Muerto}

    enum percepcionCasilla {ok, desconocido, nube, sangre, riesgo, objetivo}

    enum Movimiento { Arriba, Abajo, Izquierda, Derecha}

    

    //Tableros de información de la IA
    percepcionCasilla [,] tableroIA;
    int anchoTablero, altoTablero;          //Los necesito del GameManager

	int tableroX, tableroY;

    Movimiento siguienteMov;
    modoAgente modo;

    //Booleanos de control
    bool muertoEncontrado;
    bool armaEncontrada;
    bool aLaCama;              //Determina si, al volver a casa, la partida debe terminar

	//Stack de movimientos
	Queue <Movimiento> movimientosPlaneados;

	percepcionCasilla [] alrededores = new percepcionCasilla[4]; //Array con la información de lo que rodea al personaje (del tablero de la IA)

	// Use this for initialization
	void Start () {

        anchoTablero = 10;
        altoTablero = 5;
        tableroIA = new percepcionCasilla[anchoTablero, altoTablero];

        //Para la patrulla
        movimientosPlaneados = new Queue<Movimiento>(8);

        modo = modoAgente.Patrullando;
	}
	
	// Update is called once per frame
	void Update () {
		
	}

    //MÉTODO PRINCIPAL//
    /// <summary>
    /// Primero, toma la información de la casilla en la que está actualmente (según el movimiento anterior, o "casa" si es el primnero)
    /// Después. en función del estado en el qe se encuentre llama a un módulo que analiza la información que tenemos hasta este momento
    ///          y que va a devolver un movimiento que quiere hacer acorde a esos parámetros. 
    ///          Por ejemplo el busca() no va a querer alejarse del objetivo, mientras que a la exploracion le da igual.
    ///  Luego, quieres actualizar tu información del tablero con la info que tengas bajo tus pies(incluido si es hueco: te puto mueres), y quizá
    ///         sea bueno hacer estimaciones con diagonales de dónde puede estar un hueco, o un objetivo :3
    /// </summary>
    public int MovimientoAgente() {
        //Tomamos e interpretamos la información de la nueva pos :3
		analizaInfoGM();

        switch (modo)
        {
            case modoAgente.Patrullando:
                //Hace un paso de la lógica 
				return patrulla();
                break;

            case modoAgente.Analizando:
                //Hace un paso de la lógca de buscar
				return busca();
                break;

            case modoAgente.Volviendo:
                //Ejecuta un paso del A* para volver a casa
				return vueltaACasa();
                break;

			case modoAgente.Muerto:
			//El personaje muere y vulve a la casilla de salida (casa)
			//Además se reinicia el tablero, la IA vuelve a no conocer nada
			//¿Vuelve a generarse de forma aleatoria?

			return -1;

                break;
        }
		return 666; //;)
    }

    //Módulo para el estado de patrulla
    int patrulla()
    {
        //lA PATRULLA funciona de la sigiente forma:
		// Si encuentras el arma, pones una cola con direcciones fijas que hacer. Las siguientes iteraciones usarán esos movimientos
		//Que prevalecen sobre todo.
		// Si hay una desconocida, se mueve ahí.
		// Si no hay desconocidas, se mueve a las de riesgo. (Necesidad)
		// Si TODAS son ok, seleccionas una aleatoria

		//Si hay desconocida y ok, vas a desconocida. Si hay mas de una desconocida, aleatoria.
		//Si hay riesgo y desconocida, vas a desconocida.

		if (movimientosPlaneados.Count != 0) {
			//Haces un pop y se lo mandas al GM(?)
			//Si sabes que vas hacia un riesgo o una ok, no vayas: pasas a la siguiente.
			Movimiento sig = movimientosPlaneados.Peek();
			movimientosPlaneados.Dequeue ();
			return (int)sig;	
		} 
		else {

			//Buscamos lo que hay alrededor del personaje para decidir donde se mueve a continuación (tableroIA)
			for (int i = 0; i < 4; i++) {
				switch (i) {
				case 0: //norte
					if (tableroY - 1 >= 0)
						alrededores[i] = tableroIA [tableroX, tableroY - 1];
					break;

				case 1: //sur
					if (tableroY + 1 < altoTablero)
						alrededores[i] = tableroIA [tableroX, tableroY + 1];
					break;

				case 2: //este
					if (tableroX + 1 < anchoTablero)
						alrededores[i] = tableroIA [tableroX + 1, tableroY];
					break;

				case 3: //oeste
					if (tableroX - 1 >= 0)
						alrededores[i] = tableroIA [tableroX -1, tableroY];
					break;

				}
			}

			int k = 0;
			//Buscamos si hay alguna casilla desconocida
			while (k < 4 && alrededores [k] != percepcionCasilla.desconocido) k++;

			if (k != 4)
				return k;
			else {

				k = 0;
				while (k < 4 && alrededores [k] != percepcionCasilla.riesgo) k++;
				if (k != 4)
					return k;
				else
					return Random.Range (0, 3);

			}
		}
        
    }


    //Módulo para la busqueda perimetral 
    int busca()
    {
        //Podemos establecer una matriz de busqueda.
        //Aqui va la sangre
        GameManager.instance.IACompletedPuzzle(); //TEST
		return 666;

    }

    //Para la vuelta a casa
    int vueltaACasa()
    {
		return 666;
    }


    //-------------------------------------------------------
    //METODOS IMPORTANTES (o no)
    //Fragmento que nos permite determinar con lógica guapa guapa dónde están los huecos (y la sangre?)
    //partiendo de las premisas que nos proporcionan los conocimientos que tengamos
    //Premisa: Estas en una casilla con nube.
    void analizaInfoGM()
    {
        //Recibes un int, que representa cierta información del GM que la IA va a interpretar a su manera.
		//int tableroX, tableroY;
		int gmInfo = GameManager.instance.getInfoCasilla( out tableroX, out tableroY);

		//Ahora con un switch vamos a interpretarlo y en funcion de eso modificar
		//tanto la informacion local del tablero como el modo

		switch (gmInfo) {

		    case 0: //Casilla vacia -> ok
			    //Realmente no hace nada
			    break;

		    case 1: //Hueco -> te has muerto
			    modo = modoAgente.Muerto;
			    break;

		    case 2: //sueloVacio -> ok
			
			    tableroIA[tableroX, tableroY] = percepcionCasilla.ok;
			    break;

		    case 3: //sueloArma -> un objetivo completado!
			    armaEncontrada = true;
			    if (armaEncontrada && muertoEncontrado) //Determina si has completado la busqueda
				    aLaCama = true;
			
			    tableroIA [tableroX, tableroY] = percepcionCasilla.ok;
			    //Metes las cuatro direcciones 
			    for (int i = 0; i < 4; i++) {
				    switch (i) {
					    case 0: //norte: ida y vuelta
								    movimientosPlaneados.Enqueue(Movimiento.Arriba);
								    movimientosPlaneados.Enqueue(Movimiento.Abajo);
						    break;
					    case 1: //sur: ida y vuelta
								    movimientosPlaneados.Enqueue(Movimiento.Abajo);
								    movimientosPlaneados.Enqueue(Movimiento.Arriba);
				  	 	    break;
					    case 2: //este: ida y vuelta
								    movimientosPlaneados.Enqueue(Movimiento.Derecha);
								    movimientosPlaneados.Enqueue(Movimiento.Izquierda);
						    break;
					    case 3: //oeste: ida y vuelta
								    movimientosPlaneados.Enqueue(Movimiento.Izquierda);
								    movimientosPlaneados.Enqueue(Movimiento.Derecha);
						    break;
				
				    }
			    }
		
	
			    //Si no estabas en busca, te pones a ello.
			
			    if (modo == modoAgente.Analizando && aLaCama)
				    modo = modoAgente.Volviendo;
			
			    break;
		
		    case 4: //NubeVacia -> pones a nube y miras alrededores
			    tableroIA [tableroX, tableroY] = percepcionCasilla.nube;
			    analizaTerreno (tableroX, tableroY);
			
			    break;

		    case 5: //NubeSangre -> Cambias a busqueda (y miras alrededores antes?)
			    tableroIA [tableroX, tableroY] = percepcionCasilla.nube;
			    analizaTerreno (tableroX, tableroY);

			    //tableroIA [tableroX, tableroY] = percepcionCasilla.sangre; //Tambien es sangre :3

			    if (modo != modoAgente.Analizando)
				    modo = modoAgente.Analizando;
			    break;

              case 6: //NubeArma
                    armaEncontrada = true;
                    if (armaEncontrada && muertoEncontrado) //Determina si has completado la busqueda
                        aLaCama = true;

                    tableroIA[tableroX, tableroY] = percepcionCasilla.nube;
                    analizaTerreno(tableroX, tableroY);

                    break;
             case 7:
                    muertoEncontrado = true;
                    if (armaEncontrada && muertoEncontrado) //Determina si has completado la busqueda
                        aLaCama = true;

                    tableroIA[tableroX, tableroY] = percepcionCasilla.nube;
                    analizaTerreno(tableroX, tableroY);

                    break;
         

		    case 8: //sangre -> cambias a busqueda. Si ya estás en busqueda, supongo que actuaizas mapa y ya.
			    tableroIA [tableroX, tableroY] = percepcionCasilla.sangre;

			    if (modo != modoAgente.Analizando)
				    modo = modoAgente.Analizando;
			    break;

		    case 9: //muerto -> yay, otro objetivo! 
			    muertoEncontrado = true;
			    if (armaEncontrada && muertoEncontrado) //Determina si has completado la busqueda
				    aLaCama = true;

			    tableroIA [tableroX, tableroY] = percepcionCasilla.ok;


			    break;

		    case 10: //Casa -> si tienes los dos objetivos, has ganado. Si no, no.
			    if (aLaCama) {
                        //HAS GANADO :333
                        GameManager.instance.IACompletedPuzzle();
			    }
				
			    tableroIA [tableroX, tableroY] = percepcionCasilla.ok;
			    break;


		}
    }
    void analizaTerreno(int x, int y)
    {
        //Booleanos de control: para evitar modificar un punto cardinal dos veces
        bool norteOk = false;
        bool surOk = false;
        bool esteOk = false;
        bool oesteOk = false;
        //Intento averiguar:

        //--------NORTE------------
        //Si no hemos estado en el norte...
        if (y - 1 >= 0 && (tableroIA[x, y - 1] == percepcionCasilla.desconocido || tableroIA[x, y - 1] == percepcionCasilla.riesgo))
        {
            //Buscamos informacion en las diagonales
            //Diagonal Izq
            if (x - 1 >= 0 && y - 1 >= 0 && tableroIA[x - 1, y - 1] != percepcionCasilla.desconocido && !norteOk)
            {
                //Queremos ver si es: ok
                if (tableroIA[x - 1, y - 1] == percepcionCasilla.ok)
                {
                    tableroIA[x, y - 1] = percepcionCasilla.ok;
                    norteOk = true;
                }
                //Queremos ver si es nube.
                else if (tableroIA[x - 1, y - 1] == percepcionCasilla.nube || tableroIA[x - 1, y - 1] == percepcionCasilla.riesgo)
                {

                    tableroIA[x, y - 1] = percepcionCasilla.riesgo;
                }
            }


            //Diagonal Derecha
            else if (x + 1 < anchoTablero && y - 1 >= 0 && tableroIA[x + 1, y - 1] != percepcionCasilla.desconocido && !norteOk)
            {
                //Queremos ver si es: ok
                if (tableroIA[x + 1, y - 1] == percepcionCasilla.ok)
                {
                    tableroIA[x, y - 1] = percepcionCasilla.ok; //Norte es ok
                    norteOk = true;
                }
                //Queremos ver si es nube.
                else if (tableroIA[x + 1, y - 1] == percepcionCasilla.nube || tableroIA[x + 1, y - 1] == percepcionCasilla.riesgo)
                {

                    tableroIA[x, y - 1] = percepcionCasilla.riesgo; //Hay riesgo en norte
                }

            }

            //Cuando no puedes determinar por los medios dados la información que contiene, la ponemos a riesgo.
            else tableroIA[x, y - 1] = percepcionCasilla.riesgo;

        }

        //--------SUR------------
        //Si no hemos estado en el sur...
        if (y + 1 < altoTablero && (tableroIA[x, y + 1] == percepcionCasilla.desconocido || tableroIA[x, y + 1] == percepcionCasilla.riesgo))
        {
            //Buscamos informacion en las diagonales
            //Diagonal Izq
            if (x - 1 >= 0 && y + 1 < altoTablero && tableroIA[x - 1, y + 1] != percepcionCasilla.desconocido && !surOk)
            {
                //Queremos ver si es: ok
                if (tableroIA[x - 1, y + 1] == percepcionCasilla.ok)
                {
                    tableroIA[x, y + 1] = percepcionCasilla.ok;
                    surOk = true;
                }
                //Queremos ver si es nube.
                else if (tableroIA[x - 1, y + 1] == percepcionCasilla.nube || tableroIA[x - 1, y + 1] == percepcionCasilla.riesgo)
                {

                    tableroIA[x, y + 1] = percepcionCasilla.riesgo;
                }
            }


            //Diagonal Derecha
            else if (x + 1 < anchoTablero && y + 1 < altoTablero && tableroIA[x + 1, y + 1] != percepcionCasilla.desconocido && !surOk)
            {
                //Queremos ver si es: ok
                if (tableroIA[x + 1, y + 1] == percepcionCasilla.ok)
                {
                    tableroIA[x, y + 1] = percepcionCasilla.ok; //Sur es ok
                    surOk = true;
                }
                //Queremos ver si es nube.
                else if (tableroIA[x + 1, y + 1] == percepcionCasilla.nube || tableroIA[x + 1, y + 1] == percepcionCasilla.riesgo)
                {

                    tableroIA[x, y + 1] = percepcionCasilla.riesgo; //Hay riesgo en sur
                }

            }

            //Cuando no puedes determinar por los medios dados la información que contiene, la ponemos a riesgo.
            else tableroIA[x, y + 1] = percepcionCasilla.riesgo;

        }

        //--------ESTE------------
        //Si no hemos estado en el este...
        if (x + 1 < anchoTablero && (tableroIA[x + 1, y] == percepcionCasilla.desconocido || tableroIA[x + 1, y] == percepcionCasilla.riesgo))
        {
            //Buscamos informacion en las diagonales
            //Diagonal Arriba
            if (x + 1 < anchoTablero && y - 1 >= 0 && tableroIA[x + 1, y - 1] != percepcionCasilla.desconocido && !esteOk)
            {
                //Queremos ver si es: ok
                if (tableroIA[x + 1, y - 1] == percepcionCasilla.ok)
                {
                    tableroIA[x + 1, y] = percepcionCasilla.ok;
                    esteOk = true;
                }
                //Queremos ver si es nube.
                else if (tableroIA[x + 1, y - 1] == percepcionCasilla.nube || tableroIA[x + 1, y - 1] == percepcionCasilla.riesgo)
                {

                    tableroIA[x + 1, y] = percepcionCasilla.riesgo;
                }
            }


            //Diagonal Abajo
            else if (x + 1 < anchoTablero && y + 1 < altoTablero && tableroIA[x + 1, y + 1] != percepcionCasilla.desconocido && !esteOk)
            {
                //Queremos ver si es: ok
                if (tableroIA[x + 1, y + 1] == percepcionCasilla.ok)
                {
                    tableroIA[x + 1, y] = percepcionCasilla.ok; //Este es ok
                    esteOk = true;
                }
                //Queremos ver si es nube.
                else if (tableroIA[x + 1, y + 1] == percepcionCasilla.nube || tableroIA[x + 1, y + 1] == percepcionCasilla.riesgo)
                {

                    tableroIA[x + 1, y] = percepcionCasilla.riesgo; //Hay riesgo en este
                }

            }

            //Cuando no puedes determinar por los medios dados la información que contiene, la ponemos a riesgo.
            else tableroIA[x, y - 1] = percepcionCasilla.riesgo;

        }

        //--------OESTE------------
        //Si no hemos estado en el oeste...
        if (x - 1 < anchoTablero && (tableroIA[x - 1, y] == percepcionCasilla.desconocido || tableroIA[x - 1, y] == percepcionCasilla.riesgo))
        {
            //Buscamos informacion en las diagonales
            //Diagonal Arriba
            if (x - 1 >= 0 && y - 1 >= 0 && tableroIA[x - 1, y - 1] != percepcionCasilla.desconocido && !oesteOk)
            {
                //Queremos ver si es: ok
                if (tableroIA[x - 1, y - 1] == percepcionCasilla.ok)
                {
                    tableroIA[x - 1, y] = percepcionCasilla.ok;
                    oesteOk = true;
                }
                //Queremos ver si es nube.
                else if (tableroIA[x - 1, y - 1] == percepcionCasilla.nube || tableroIA[x - 1, y - 1] == percepcionCasilla.riesgo)
                {

                    tableroIA[x - 1, y] = percepcionCasilla.riesgo;
                }
            }


            //Diagonal Abajo
            else if (x - 1 >= 0 && y + 1 < altoTablero && tableroIA[x - 1, y + 1] != percepcionCasilla.desconocido && !oesteOk)
            {
                //Queremos ver si es: ok
                if (tableroIA[x - 1, y + 1] == percepcionCasilla.ok)
                {
                    tableroIA[x - 1, y] = percepcionCasilla.ok; //Oeste es ok
                    oesteOk = true;
                }
                //Queremos ver si es nube.
                else if (tableroIA[x - 1, y + 1] == percepcionCasilla.nube || tableroIA[x - 1, y + 1] == percepcionCasilla.riesgo)
                {

                    tableroIA[x - 1, y] = percepcionCasilla.riesgo; //Hay riesgo en oeste
                }

            }

            //Cuando no puedes determinar por los medios dados la información que contiene, la ponemos a riesgo.
            else tableroIA[x - 1, y] = percepcionCasilla.riesgo;

        }


    }

}
