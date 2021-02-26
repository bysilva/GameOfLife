using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
public enum CellState { Live = 1, Dead = 2 }
namespace GameOfLife
{
    public class CellModel
    {
        public int Ren { get; set; }
        public int Col { get; set; }
        public bool State { get; set; }
    }
}
