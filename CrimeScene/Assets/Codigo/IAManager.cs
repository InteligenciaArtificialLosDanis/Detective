using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IAManager : MonoBehaviour {

    //La IA consiste en un recorrido a ciegas hasta que llega a la sangre y cambia de modo

    public GameManager gm;

    enum modoAgente {Patrullando, Analizando, Volviendo, Muerto}

    enum percepcionCasilla {ok, desconocido, nube, riesgo, objetivo}

    enum MovimientoAgente { Arriba, Abajo, Izquierda, Derecha}

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

    MovimientoAgente sigMov;

    //Booleanos de control
    bool muertoEncontrado;
    bool armaEncontrada;
    bool paLaCama;              //Determina si, al volver a casa, la partida debe terminar

	// Use this for initialization
	void Start () {

        gm.getInfoCasilla(anchoTablero, altoTablero);   //Ahora tenemos los valores :3
        tableroIA = new CasillaIA[anchoTablero, altoTablero];
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
    void MovimientoAgente()
    {

    }

    //Módulo para el estado de patrulla
    void patrulla()
    {
        //Te dan la info. Si ya tienes algo en plan has asumido una nube y la pones a ok, cuando te llegue dicha nube la pondrás a lo que es: una nube.
    }

    //Fragmento que nos permite determinar con lógica guapa guapa dónde están los huecos (y la sangre?)
    //partiendo de las premisas que nos proporcionan los conocimientos que tengamos
    //Premisa: Estas en una casilla con nube.
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
                else if (tableroIA[x + 1, y - 1].infoCasilla == percepcionCasilla.nube || tableroIA[x+1,y-1].infoCasilla == percepcionCasilla.riesgo)
                {

                    tableroIA[x, y - 1].infoCasilla = percepcionCasilla.riesgo; //Hay riesgo en norte
                }

            }

            //Cuando no puedes determinar por los medios dados la información que contiene, la ponemos a riesgo.
            else tableroIA[x,y-1].infoCasilla = percepcionCasilla.riesgo;

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
        if (x + 1 < anchoTablero && (tableroIA[x + 1, y].infoCasilla == percepcionCasilla.desconocido || tableroIA[x + 1, y ].infoCasilla == percepcionCasilla.riesgo))
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

    //Módulo para la busqueda perimetral 
    void busca()
    {

    }

    //Para la vuelta a casa
    void vueltaACasa()
    {

    }
}
