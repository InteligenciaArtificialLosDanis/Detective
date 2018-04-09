using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IAManager : MonoBehaviour {

    //La IA consiste en un recorrido a ciegas hasta que llega a la sangre y cambia de modo

    public GameManager gm;

    enum modoAgente {Patrullando, Analizando, Volviendo, Muerto}

    enum percepcionCasilla {ok, desconocido, nube, sangre, riesgo, objetivo}

    enum Movimiento { Arriba, Abajo, Izquierda, Derecha}

    class CasillaIA
    {
       public CasillaIA (percepcionCasilla percepcion)
        {
            infoCasilla = percepcion;
        }

        public percepcionCasilla infoCasilla;
    }

    //Tableros de información de la IA
    CasillaIA[,] tableroIA;
    int anchoTablero, altoTablero;          //Los necesito del GameManager

    Movimiento siguienteMov;
    modoAgente modo;

    //Booleanos de control
    bool muertoEncontrado;
    bool armaEncontrada;
    bool paLaCama;              //Determina si, al volver a casa, la partida debe terminar

	//Stack de movimientos
	Queue <Movimiento> movimientosPlaneados;

	// Use this for initialization
	void Start () {

        gm.getInfoCasilla(anchoTablero, altoTablero);   //Ahora tenemos los valores :3
        tableroIA = new CasillaIA[anchoTablero, altoTablero];

        modo = modoAgente.Patrullando;
	}
	
	// Update is called once per frame
	void Update () {
		
	}

    //MÉTODO PRINCIPAL AAAA//
    /// <summary>
    /// Primero, toma la información de la casilla en la que está actualmente (según el movimiento anterior, o "casa" si es el primnero)
    /// Después. en función del estado en el qe se encuentre llama a un módulo que analiza la información que tenemos hasta este momento
    ///          y que va a devolver un movimiento que quiere hacer acorde a esos parámetros. 
    ///          Por ejemplo el busca() no va a querer alejarse del objetivo, mientras que a la exploracion le da igual.
    ///  Luego, quieres actualizar tu información del tablero con la info que tengas bajo tus pies(incluido si es hueco: te puto mueres), y quizá
    ///         sea bueno hacer estimaciones con diagonales de dónde puede estar un hueco, o un objetivo :3
    /// </summary>
    void MovimientoAgente() {
        //Tomamos e interpretamos la información de la nueva pos :3
		analizaInfoGM();

        switch (modo)
        {
            case modoAgente.Patrullando:
                //Hace un paso de la lógica 
				patrulla();
                break;

            case modoAgente.Analizando:
                //Hace un paso de la lógca de buscar
                break;

            case modoAgente.Volviendo:
                //Ejecuta un paso del A* para volver a casa
                break;

            case modoAgente.Muerto:
                //Pues te has muerto jajja

                break;
        }
    }

    //Módulo para el estado de patrulla
    void patrulla()
    {
        //lA PATRULLA funciona de la sigiente forma:
		// Si encuentras el arma, pones una cola con direcciones fijas que hacer. Las siguientes iteraciones usarán esos movimientos
		//Que prevalecen sobre todo.
		// Si hay una desconocida, se mueve ahí.
		// Si no hay desconocidas, de mueve a las de riesgo. (Necesidad)
		// Si TODAS son ok, seleccionas una aleatoria

		//Si hay desconocida y ok, vas a desconocida. Si hay mas de una desconocida, aleatoria.
		//Si hay riesgo y desconocida, vas a desconocida.

		if (movimientosPlaneados.Count != 0) {
			//Haces un pop y se lo mandas al GM(?)
			//Si sabes que vas hacia un riesgo o una ok, no vayas: pasas a la siguiente.
			
		} 
		else {

			if(




		}
        
    }


    //Módulo para la busqueda perimetral 
    void busca()
    {
		//Podemos establecer una matriz de busqueda.
					//Aqui va la sangre ;3 

    }

    //Para la vuelta a casa
    void vueltaACasa()
    {

    }


    //-------------------------------------------------------
    //METODOS IMPORTANTES (o no)
    //Fragmento que nos permite determinar con lógica guapa guapa dónde están los huecos (y la sangre?)
    //partiendo de las premisas que nos proporcionan los conocimientos que tengamos
    //Premisa: Estas en una casilla con nube.
    void analizaInfoGM()
    {
        //Recibes un int, que representa cierta información del GM que la IA va a interpretar a su manera.
		int tableroX, tableroY;
        int gmInfo = gm.getInfoCasilla(tableroX, tableroY);

		//Ahora con un switch vamos a interpretarlo y en funcion de eso modificar
		//tanto la informacion local del tablero como el modo

		switch (gmInfo) {

		case 0: //Casilla vacia -> ok
			//Realmente no hace nada
			break;

		case 1: //Hueco -> te has muerto :3
			modo = modoAgente.Muerto;
			break;

		case 2: //sueloVacio -> ok
			tableroIA[tableroX, tableroY] = percepcionCasilla.ok;
			break;

		case 3: //sueloArma -> un objetivo completado! :3
			armaEncontrada = true;
			if (armaEncontrada && muertoEncontrado) //Determina si has completado la busqueda
				paLaCama = true;
			
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
			
			if (modo == modoAgente.Analizando && paLaCama)
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

		case 6: //sangre -> cambias a busqueda. Si ya estás en busqueda, supongo que actuaizas mapa y ya.
			tableroIA [tableroX, tableroY] = percepcionCasilla.sangre;

			if (modo != modoAgente.Analizando)
				modo = modoAgente.Analizando;
			break;

		case 7: //muerto -> yay, otro objetivo! 
			muertoEncontrado = true;
			if (armaEncontrada && muertoEncontrado) //Determina si has completado la busqueda
				paLaCama = true;

			tableroIA [tableroX, tableroY] = percepcionCasilla.ok;


			break;

		case 8: //Casa -> si tienes los dos objetivos, has ganado. Si no, no.
			if (paLaCama) {
				//HAS GANADO :333
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
        if (y - 1 >= 0 && (tableroIA[x, y - 1].infoCasilla == percepcionCasilla.desconocido || tableroIA[x, y - 1].infoCasilla == percepcionCasilla.riesgo))
        {
            //Buscamos informacion en las diagonales
            //Diagonal Izq
            if (x - 1 >= 0 && y - 1 >= 0 && tableroIA[x - 1, y - 1].infoCasilla != percepcionCasilla.desconocido && !norteOk)
            {
                //Queremos ver si es: ok
                if (tableroIA[x - 1, y - 1].infoCasilla == percepcionCasilla.ok)
                {
                    tableroIA[x, y - 1].infoCasilla = percepcionCasilla.ok;
                    norteOk = true;
                }
                //Queremos ver si es nube.
                else if (tableroIA[x - 1, y - 1].infoCasilla == percepcionCasilla.nube || tableroIA[x - 1, y - 1].infoCasilla == percepcionCasilla.riesgo)
                {

                    tableroIA[x, y - 1].infoCasilla = percepcionCasilla.riesgo;
                }
            }


            //Diagonal Derecha
            else if (x + 1 < anchoTablero && y - 1 >= 0 && tableroIA[x + 1, y - 1].infoCasilla != percepcionCasilla.desconocido && !norteOk)
            {
                //Queremos ver si es: ok
                if (tableroIA[x + 1, y - 1].infoCasilla == percepcionCasilla.ok)
                {
                    tableroIA[x, y - 1].infoCasilla = percepcionCasilla.ok; //Norte es ok
                    norteOk = true;
                }
                //Queremos ver si es nube.
                else if (tableroIA[x + 1, y - 1].infoCasilla == percepcionCasilla.nube || tableroIA[x + 1, y - 1].infoCasilla == percepcionCasilla.riesgo)
                {

                    tableroIA[x, y - 1].infoCasilla = percepcionCasilla.riesgo; //Hay riesgo en norte
                }

            }

            //Cuando no puedes determinar por los medios dados la información que contiene, la ponemos a riesgo.
            else tableroIA[x, y - 1].infoCasilla = percepcionCasilla.riesgo;

        }

        //--------SUR------------
        //Si no hemos estado en el sur...
        if (y + 1 < altoTablero && (tableroIA[x, y + 1].infoCasilla == percepcionCasilla.desconocido || tableroIA[x, y + 1].infoCasilla == percepcionCasilla.riesgo))
        {
            //Buscamos informacion en las diagonales
            //Diagonal Izq
            if (x - 1 >= 0 && y + 1 < altoTablero && tableroIA[x - 1, y + 1].infoCasilla != percepcionCasilla.desconocido && !surOk)
            {
                //Queremos ver si es: ok
                if (tableroIA[x - 1, y + 1].infoCasilla == percepcionCasilla.ok)
                {
                    tableroIA[x, y + 1].infoCasilla = percepcionCasilla.ok;
                    surOk = true;
                }
                //Queremos ver si es nube.
                else if (tableroIA[x - 1, y + 1].infoCasilla == percepcionCasilla.nube || tableroIA[x - 1, y + 1].infoCasilla == percepcionCasilla.riesgo)
                {

                    tableroIA[x, y + 1].infoCasilla = percepcionCasilla.riesgo;
                }
            }


            //Diagonal Derecha
            else if (x + 1 < anchoTablero && y + 1 < altoTablero && tableroIA[x + 1, y + 1].infoCasilla != percepcionCasilla.desconocido && !surOk)
            {
                //Queremos ver si es: ok
                if (tableroIA[x + 1, y + 1].infoCasilla == percepcionCasilla.ok)
                {
                    tableroIA[x, y + 1].infoCasilla = percepcionCasilla.ok; //Sur es ok
                    surOk = true;
                }
                //Queremos ver si es nube.
                else if (tableroIA[x + 1, y + 1].infoCasilla == percepcionCasilla.nube || tableroIA[x + 1, y + 1].infoCasilla == percepcionCasilla.riesgo)
                {

                    tableroIA[x, y + 1].infoCasilla = percepcionCasilla.riesgo; //Hay riesgo en sur
                }

            }

            //Cuando no puedes determinar por los medios dados la información que contiene, la ponemos a riesgo.
            else tableroIA[x, y + 1].infoCasilla = percepcionCasilla.riesgo;

        }

        //--------ESTE------------
        //Si no hemos estado en el este...
        if (x + 1 < anchoTablero && (tableroIA[x + 1, y].infoCasilla == percepcionCasilla.desconocido || tableroIA[x + 1, y].infoCasilla == percepcionCasilla.riesgo))
        {
            //Buscamos informacion en las diagonales
            //Diagonal Arriba
            if (x + 1 < anchoTablero && y - 1 >= 0 && tableroIA[x + 1, y - 1].infoCasilla != percepcionCasilla.desconocido && !esteOk)
            {
                //Queremos ver si es: ok
                if (tableroIA[x + 1, y - 1].infoCasilla == percepcionCasilla.ok)
                {
                    tableroIA[x + 1, y].infoCasilla = percepcionCasilla.ok;
                    esteOk = true;
                }
                //Queremos ver si es nube.
                else if (tableroIA[x + 1, y - 1].infoCasilla == percepcionCasilla.nube || tableroIA[x + 1, y - 1].infoCasilla == percepcionCasilla.riesgo)
                {

                    tableroIA[x + 1, y].infoCasilla = percepcionCasilla.riesgo;
                }
            }


            //Diagonal Abajo
            else if (x + 1 < anchoTablero && y + 1 < altoTablero && tableroIA[x + 1, y + 1].infoCasilla != percepcionCasilla.desconocido && !esteOk)
            {
                //Queremos ver si es: ok
                if (tableroIA[x + 1, y + 1].infoCasilla == percepcionCasilla.ok)
                {
                    tableroIA[x + 1, y].infoCasilla = percepcionCasilla.ok; //Este es ok
                    esteOk = true;
                }
                //Queremos ver si es nube.
                else if (tableroIA[x + 1, y + 1].infoCasilla == percepcionCasilla.nube || tableroIA[x + 1, y + 1].infoCasilla == percepcionCasilla.riesgo)
                {

                    tableroIA[x + 1, y].infoCasilla = percepcionCasilla.riesgo; //Hay riesgo en este
                }

            }

            //Cuando no puedes determinar por los medios dados la información que contiene, la ponemos a riesgo.
            else tableroIA[x, y - 1].infoCasilla = percepcionCasilla.riesgo;

        }

        //--------OESTE------------
        //Si no hemos estado en el oeste...
        if (x - 1 < anchoTablero && (tableroIA[x - 1, y].infoCasilla == percepcionCasilla.desconocido || tableroIA[x - 1, y].infoCasilla == percepcionCasilla.riesgo))
        {
            //Buscamos informacion en las diagonales
            //Diagonal Arriba
            if (x - 1 >= 0 && y - 1 >= 0 && tableroIA[x - 1, y - 1].infoCasilla != percepcionCasilla.desconocido && !oesteOk)
            {
                //Queremos ver si es: ok
                if (tableroIA[x - 1, y - 1].infoCasilla == percepcionCasilla.ok)
                {
                    tableroIA[x - 1, y].infoCasilla = percepcionCasilla.ok;
                    oesteOk = true;
                }
                //Queremos ver si es nube.
                else if (tableroIA[x - 1, y - 1].infoCasilla == percepcionCasilla.nube || tableroIA[x - 1, y - 1].infoCasilla == percepcionCasilla.riesgo)
                {

                    tableroIA[x - 1, y].infoCasilla = percepcionCasilla.riesgo;
                }
            }


            //Diagonal Abajo
            else if (x - 1 >= 0 && y + 1 < altoTablero && tableroIA[x - 1, y + 1].infoCasilla != percepcionCasilla.desconocido && !oesteOk)
            {
                //Queremos ver si es: ok
                if (tableroIA[x - 1, y + 1].infoCasilla == percepcionCasilla.ok)
                {
                    tableroIA[x - 1, y].infoCasilla = percepcionCasilla.ok; //Oeste es ok
                    oesteOk = true;
                }
                //Queremos ver si es nube.
                else if (tableroIA[x - 1, y + 1].infoCasilla == percepcionCasilla.nube || tableroIA[x - 1, y + 1].infoCasilla == percepcionCasilla.riesgo)
                {

                    tableroIA[x - 1, y].infoCasilla = percepcionCasilla.riesgo; //Hay riesgo en oeste
                }

            }

            //Cuando no puedes determinar por los medios dados la información que contiene, la ponemos a riesgo.
            else tableroIA[x - 1, y].infoCasilla = percepcionCasilla.riesgo;

        }


    }

}
