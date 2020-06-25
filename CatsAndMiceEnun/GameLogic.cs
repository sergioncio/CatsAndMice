using CatsAndMiceEnun;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CatsAndMice
{

    public class GameLogic:IGameLogic
    {

        public static readonly int MaxFila = 25;
        public static readonly int MaxColumna = 50;

        // Los elementos del juego se gestionan en varios objetos contenedores de datos 
        // que apuntan hacia datos comunes.
        // Ventaja: se facilita la impelemntación de la lógica del juego.
        // Inconveniente: Hay que mantener la coherencia entre los diferents contenedores, duplicando
        // las operaciones de inserción y borrado.

        // Panel del juego: contiene referencia a todos los datos.
        // Cada casilla o bien está vacía (contiene referencia a null) o bien
        // contiene una referencia al elemento del juego que está sobre ella.
        private IGameItem[,] board;

        // Lista de elementos del juego. Si se elimina o añade un elemento en
        // esta lista también hay que elimanarlo/añadirlo en el panel de juego (board).
        private List<IGameItem> gameItems = new List<IGameItem>();

        // Personajes mouse 
        private Mouse myMouse = new Mouse(new ItemCoordinates(20, 1), 0);
        //private Mouse myMouse = new SmartMouse(new ItemCoordinates(20, 1), 0);

        IGameObserver gameObserver;

        // Gatos
        private List<Cat> myCats = new List<Cat>();
        private List<IGameItem> myFruits = new List<IGameItem>();
        private List<IGameItem> myPoissons = new List<IGameItem>();

        private int stepCounter = 0;
        Boolean gameOver = false;

        // eventos del juego
        public event EventHandler<GameEventArgs> NewGameStepEventHandlers;

        
        /// <summary>
        /// Construye un tablero de filas x columnas
        /// </summary>
        /// <param name="filas"></param>
        /// <param name="columnas"></param>
        public GameLogic()
        {
            gameOver = false;
            board =new IGameItem[MaxFila, MaxColumna];
            FillBoard(10, 10);
            Mouse raton = myMouse;
            gameItems.Add(raton);

            /*if (raton is SmartMouse)
            {
                NewGameStepEventHandlers += SmartMouse;
            }*/
            
        }

        /// <summary>
        /// Devuelve true si en la (fila, columna) especificada no hay
        /// ningún elemento de juego.
        /// </summary>
        /// <param name="fila"></param>
        /// <param name="columna"></param>
        /// <returns></returns>
        public Boolean IsCellAvailable(int fila, int columna)
        {
            return board[fila, columna] == null;
        }
        

        /// <summary>
        /// Añade un elemento al juego en la celda especificada en las coordenadas del
        /// argumento (item), siempre que (1) item != null, (2) la posición no esté ya
        /// ocupada por otro elemntoe y (3) el elemento no esté ya en el juego.
        /// </summary>
        /// <param name="item"></param>
        public void AddItem(IGameItem item)
        {   
            if (item != null  & board[item.Coords.Fila, item.Coords.Columna]== null)
            {
                if (item is Fruit)
                {
                    board[item.Coords.Fila, item.Coords.Columna] = item;
                    gameItems.Add(item);
                    myFruits.Add(item);
                }
                if (item is Poisson)
                {
                    board[item.Coords.Fila, item.Coords.Columna] = item;
                    gameItems.Add(item);
                    myPoissons.Add(item);
                }

            }
        }

        /// <summary>
        /// Elimina un elemento del juego.
        /// </summary>
        /// <param name="item"></param>        
        public void RemoveItem(IGameItem item)
        {
            if (item != null & board[item.Coords.Fila, item.Coords.Columna] != null)
            {
                board[item.Coords.Fila, item.Coords.Columna] = null;
                gameItems.Remove(item);
                if (item is Fruit)
                {
                    myFruits.Remove(item);
                }
                if (item is Poisson)
                {
                    myPoissons.Remove(item);
                }
            }

        }
        
        /// <summary>
        /// Rellena el juego con un número determinado de frutas y venenos colocados en 
        /// posiciones aleatorias.
        /// </summary>
        public void FillBoard(int nFruits, int nPoissons)
        {
            Random rnd1 = new Random();
            for (int i = 0; i < nFruits; i++)
                {
                int fila = rnd1.Next(1, MaxFila);
                int columna = rnd1.Next(1, MaxColumna);
                ItemCoordinates coord = new ItemCoordinates(fila,columna);
                IGameItem fruit = new Fruit(coord,rnd1.Next(1,10));
                AddItem(fruit);
                }
            for (int i = 0; i < nPoissons; i++)
            {
                int fila = rnd1.Next(1, MaxFila);
                int columna = rnd1.Next(1, MaxColumna);
                ItemCoordinates coord = new ItemCoordinates(fila, columna);
                IGameItem veneno = new Poisson(coord, -rnd1.Next(1,10));
                AddItem(veneno);
            }


        }

        /// <summary>
        /// Fija la dirección de movimiento del ratón.
        /// </summary>
        /// <param name="next"></param>
        public void SetMouseDirection(Direction next)
        {
            myMouse.CurrentDirection = next;
        }

        /// <summary>
        /// Devuelve un enumerador de los elementos del juego.
        /// </summary>
        /// <returns></returns>
        public IEnumerator<IGameItem> GetEnumerator()
        {
            foreach (var item in gameItems)
            {
                yield return item;
            }
        }

        /// <summary>
        /// Implementa la lógica del juego que se ejecuta en cada Tick de un temporizador.
        /// </summary>
        /// <returns>>= 0 si el juego puede continuar, un valor negativo si no se puede continuar</returns>

        int valor = 20;
        public int ExecuteStep(GameMode mode)
        {
            // Actualiza posición del ratón de acuerdo con su dirección.
            if (gameOver== false & myMouse.MyValue>=0)
            {
                if (mode == GameMode.AutonomousMouse)
                {
                    myMouse.ChangeDirection();
                }
                ItemCoordinates coordRaton = myMouse.Coords;
                updateMovablePosition(myMouse);
                processCell();
                board[coordRaton.Fila, coordRaton.Columna] = null;
                board[myMouse.Coords.Fila, myMouse.Coords.Columna] = myMouse;

                Random random = new Random();
                if (myFruits.Count < 2)
                {
                    for (int i=0; i<10; i++)
                    {
                        int f = random.Next(1, MaxFila);
                        int c = random.Next(1, MaxColumna);
                        ItemCoordinates coord = new ItemCoordinates(f, c);
                        Fruit fruta = new Fruit(coord, random.Next(1, 10));
                        AddItem(fruta);
                    }
                }
                if (myPoissons.Count < 2)
                {
                    for (int i = 0; i < 10; i++)
                    {
                        int f = random.Next(1, MaxFila);
                        int c = random.Next(1, MaxColumna);
                        ItemCoordinates coord = new ItemCoordinates(f, c);
                        Poisson veneno = new Poisson(coord, random.Next(1, 10));
                        AddItem(veneno);
                    }
                }

                Random rnd = new Random();
                if (stepCounter<= 240 && stepCounter==valor)
                {
                    int f = rnd.Next(1, MaxFila);
                    int c = rnd.Next(1, MaxColumna);
                    ItemCoordinates coord = new ItemCoordinates(f, c);
                    Cat gato = new Cat(coord, 0);
                    myCats.Add(gato);
                    gameItems.Add(gato);
                    valor = valor + 20;
                }
                stepCounter++;

                foreach (var gato in myCats)
                {
                    ItemCoordinates cGatoAntes = gato.Coords;
                    if (cGatoAntes.Fila == coordRaton.Fila & cGatoAntes.Columna == coordRaton.Columna)
                    {
                        gameOver = true;
                        return -1;
                    }
                    gato.ChangeDirection();
                    updateMovablePosition(gato);
                    ItemCoordinates cGato = gato.Coords;
                    if (cGato.Fila == coordRaton.Fila & cGato.Columna == coordRaton.Columna)
                    {
                        gameOver = true;
                        return -1;
                    }
                }
                //NewGameStepEventHandlers(this, new GameEventArgs(gameItems));
                
                return 0;
            }
            else return -1;
        }

        /// <summary>
        /// Actualiza posición del elemento moviéndolo una fila o columna dependiendo
        /// de la dirección de su movimiento.
        /// Cuando el elemento llega a un límite del tablero aparece por el lado contrario.
        /// </summary>
        /// <param name="item"></param>
        private void updateMovablePosition(IMovableItem item)
        {
            if (item.CurrentDirection == Direction.Up)
            {
                item.Coords.Fila = (item.Coords.Fila - 1);
                if (item.Coords.Fila <= 0) item.Coords.Fila = MaxFila - 1;
            }
            else if (item.CurrentDirection == Direction.Down)
            {
                item.Coords.Fila = (item.Coords.Fila + 1) % MaxFila;
            }
            else if (item.CurrentDirection == Direction.Right)
            {
                item.Coords.Columna = (item.Coords.Columna + 1) % MaxColumna;
            }
            else if (item.CurrentDirection == Direction.Left)
            {
                item.Coords.Columna = (item.Coords.Columna - 1);
                if (item.Coords.Columna <= 0) item.Coords.Columna = MaxColumna - 1;
            }
        }

        /// <summary>
        /// Actualiza el juego en función de lo que hay en la celda donde está el ratón.
        /// Si no hay nada, no hace nada.
        /// Si hay fruta o veneno, suma al "valor" del ratón el valor de la fruta (positivo) o del veneno (negativo) y
        /// elimina la fruta o el veneno.
        /// Si hay un gato pone el valor del ratón en -1.
        /// </summary>
        /// <returns> El valor almacenado en el ratón. </returns>
        
        private int processCell()
        {
            int f = myMouse.Coords.Fila;
            int c = myMouse.Coords.Columna;
            IGameItem item = board[f, c];

            if (item is Fruit || item is Poisson)
            {
                myMouse.MyValue += item.MyValue;
                RemoveItem(item);
                if (myMouse.MyValue < 0)
                {
                    gameOver = true;
                }
                return myMouse.MyValue;
            }
            else if (item is Cat)
            {
                gameOver = true;
                return myMouse.MyValue=-1;
            }
            else return myMouse.MyValue;
            
        }
    }
}
