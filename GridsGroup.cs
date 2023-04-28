using System.Text;

namespace AutoGenerateLevel
{
    public class GridsGroup
    {
        public int _rowCount,_colCount;
        public List<Grid> _grids = new List<Grid>();
        public GridsGroup(int row,int column) 
        {
            for(int i = 0;i<row;i++)
            {
                for(int j = 0; j < column; j++)
                {
                    _grids.Add(new Grid(i+1,j+1));
                }
            }
            _rowCount = row;
            _colCount = column;

        }

    }

    public class Grid
    {
        public int Row { get;}
        public int Column { get;}
        public bool IsAvailable { set; get; }
        public int UnitID { set; get; }
        public int ZoneID { set; get; }

        public Grid(int row,int column)
        {
            Row = row; Column = column;IsAvailable = true;
            UnitID = 0;ZoneID = 0;
        }
    }

    public class Unit
    {
        public int Color { set; get; }
        public int Unitid { set; get; }
        public int CostX { set; get; }
        public int CostY { set; get; }
        public int Cost { get { return CostX * CostY; } }
        public bool IsUsed = false;
        public string? Name { set; get; }
        public List<Grid> gridList = new List<Grid>();
        public Unit(int unitid, int color, int costX, int costY)
        {
            Color = color;
            Unitid = unitid;
            CostX = costX;
            CostY = costY;
            Name = unitid switch
            {
                1 => "R-01",
                2 => "Boom",
                3 => "Redfin",
                4 => "Stings",
                5 => "Gauntlina",
                6 => "Osci",
                7 => "刺客",
                8 => "Turbo",
                9 => "Pc-100",
                10 => "音符",
                15 => "Alpha",
                16 => "Beta",
                _ => null,
            };
             
        }
    }

    
    
}
