using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Windows.Threading;

namespace GameOfLife
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }
        #region Variables <----
        const int BoardSize = 82;
        const int RectSize = 10;
        const int CreationDelay = 100;
        bool IsGameStarted = false;
        DispatcherTimer dTimer;
        Rectangle[,] BoardRef;
        int LiveCells = 0;
        #endregion
        #region Methods <----
        void CreateBoard()
        {
            cBoard.Children.Clear();
            BoardRef = new Rectangle[BoardSize, BoardSize];
            for (int i = 0; i < BoardSize; i++)
            {
                for (int j = 0; j < BoardSize; j++)
                {
                    if (i == 0 || i == BoardSize - 1 || j == 0 || j == BoardSize - 1)
                    {
                        Rectangle r = new Rectangle
                        {
                            Width = RectSize,
                            Height = RectSize,
                            Stroke = Brushes.Blue,
                            StrokeThickness = 0.5,
                            Fill = Brushes.Blue,
                        };
                        BoardRef[i, j] = r;
                        Canvas.SetLeft(r, j * RectSize);
                        Canvas.SetTop(r, i * RectSize);
                        cBoard.Children.Add(r);
                    }
                    else
                    {
                        CellModel Cell = new CellModel { State = false, Col = i, Ren = j };
                        Rectangle r = new Rectangle
                        {
                            Width = RectSize,
                            Height = RectSize,
                            Stroke = Brushes.Black,
                            StrokeThickness = 0.5,
                            Fill = Brushes.Black,
                            Tag = Cell
                        };
                        r.MouseDown += R_MouseDown;
                        BoardRef[i, j] = r;
                        Canvas.SetLeft(r, j * RectSize);
                        Canvas.SetTop(r, i * RectSize);
                        cBoard.Children.Add(r);
                    }
                }
            }
        }
        void StartGame()
        {
            dTimer = new DispatcherTimer { Interval = new TimeSpan(0, 0, 0, 0, CreationDelay) };
            dTimer.Tick += DispatcherTimer_Tick;
            dTimer.Start();
            IsGameStarted = true;
            btnStart.IsEnabled = btnReset.IsEnabled = false;
            btnStop.IsEnabled = true;
        }
        void StopGame()
        {
            dTimer.Stop();
            IsGameStarted = false;
            btnStart.IsEnabled = btnReset.IsEnabled = true;
            btnStop.IsEnabled = false;
        }
        void ResetGame()
        {
            foreach (var cuadrito in BoardRef)
            {
                if (cuadrito.Tag != null)
                {
                    var celula = (CellModel)cuadrito.Tag;
                    if (celula.State)
                        ChangeCellState(celula);
                }
            }
        }
        void ChangeCellState(CellModel cell)
        {
            if (!cell.State)
            {
                cell.State = true;
                BoardRef[cell.Col, cell.Ren].Fill = Brushes.White;
                LiveCells++;
            }
            else
            {
                cell.State = false;
                BoardRef[cell.Col, cell.Ren].Fill = Brushes.Black;
                LiveCells--;
                if (LiveCells == 0 && IsGameStarted)
                    StopGame();
            }
            lblLiveCells.Text = LiveCells.ToString();
        }
        void ApplyRules()
        {
            List<CellModel> CellsToChange = new List<CellModel>();
            foreach (var cellRectref in BoardRef)
            {
                List<CellModel> neighbors;
                if (cellRectref.Tag != null)
                {
                    var tempCell = (CellModel)cellRectref.Tag;
                    neighbors = GetNeighbors(tempCell);
                    int neighborsCount = neighbors.Count(x => x.State == true);

                    if (tempCell.State) //Regla #1 Para que una celula siga viva,tiene que tener 2 o 3 vecinas vivas.
                    {
                        if (neighborsCount < 2 || neighborsCount > 3)
                            CellsToChange.Add(tempCell);
                    }

                    else //Regla #2 Si una celula esta muerta y tiene 3 vecinas vivas, revive.
                    {
                        if (neighborsCount == 3)
                            CellsToChange.Add(tempCell);
                    }

                }
            }
            if (CellsToChange.Count != 0)
                ChangeCells(CellsToChange);
        }
        void ChangeCells(List<CellModel> cells)
        {
            foreach (var cell in cells)
            {
                ChangeCellState(cell);
            }
        }
        List<CellModel> GetNeighbors(CellModel cell)
        {
            List<CellModel> NeighborsList = new List<CellModel>();
            for (int i = -1; i <= 1; i++)
            {
                for (int j = -1; j <= 1; j++)
                {
                    var neighbour = BoardRef[cell.Col + i, cell.Ren + j];
                    if (neighbour.Tag != null)
                    {
                        var temp = (CellModel)neighbour.Tag;
                        if (temp.Col != cell.Col || temp.Ren != cell.Ren)
                            NeighborsList.Add(temp);
                    }
                }
            }
            return NeighborsList;
        }
        #endregion
        #region Eventos <----
        private void btnStart_Click(object sender, RoutedEventArgs e)
        {
            if (!IsGameStarted)
                StartGame();
        }
        private void DispatcherTimer_Tick(object sender, EventArgs e)
        {
            if (LiveCells > 0)
                ApplyRules();
            else
                dTimer.Stop();
        }
        private void R_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (!IsGameStarted)
            {
                var cell = (CellModel)(sender as Rectangle).Tag;
                ChangeCellState(cell);
            }
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            CreateBoard();
        }


        private void btnStop_Click(object sender, RoutedEventArgs e)
        {
            StopGame();
        }
        private void btnReset_Click(object sender, RoutedEventArgs e)
        {
            if (!IsGameStarted)
                ResetGame();
        }
        #endregion
    }
}
