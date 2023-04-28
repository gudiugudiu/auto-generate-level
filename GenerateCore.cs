using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;

namespace AutoGenerateLevel
{
    public static class GenerateCore
    {


        /// <summary>
        /// 获取unit
        /// </summary>
        /// <param name="unitlist">可供选择的unit列表</param>
        /// <param name="totalGridsCount">总可用格子数</param>
        /// <param name="unitidCount">总unit种类数</param>
        /// <returns></returns>
        public static Dictionary<Unit, int> GetUnits(List<Unit> unitlist, int totalGridsCount, int unitidCount = 0, int gridPercent = 0)
        {
            int _count = unitidCount == 0 ? new Random().Next(1, unitlist.Count + 1) : unitidCount;

            float _percent = gridPercent == 0 ? (float)(new Random().Next(20, 41) / 100.0f) : (float)(gridPercent / 100.0f);
            int _gridCount = (int)(totalGridsCount * _percent);

            Dictionary<Unit, int> _dic = new Dictionary<Unit, int>();
            for (int i = 0; i < 9999; i++)
            {
                var _u = unitlist[new Random().Next(0, unitlist.Count)];
                if (_dic.ContainsKey(_u))
                {
                    _dic[_u]++;
                }
                else
                {
                    if (_dic.Count < _count)
                    {
                        _dic.Add(_u, 1);
                    }
                }

                int sum = 0;
                foreach (var item in _dic)
                {
                    sum += item.Key.Cost * item.Value;
                }
                if (sum >= _gridCount)
                {
                    break;
                }
            }
            return _dic;
        }

        public static string[] DeployUnits(GridsGroup gridsGroup, List<Unit> unitlist, out List<Unit> _finalList, int unitidCount = 0, int enemyLevel = 1, int gridPercent = 0)
        {
            List<Unit> finalList = new List<Unit>();
            int totalUnitNum = 0,
                totalGrids = gridsGroup._rowCount * gridsGroup._colCount;
            Dictionary<Unit, int> _dic = GetUnits(unitlist, totalGrids, unitidCount, gridPercent);
            List<int> unitIDs = new List<int>();
            foreach (var item in _dic)
            {
                totalUnitNum += item.Value;
                unitIDs.Add(item.Key.Unitid);
            }


            bool isRowOk = true, isColOk = true;
            int rowOkNum = 0, colOkNum = 0;

            //开始给每个单位找格子
            for (int i = 0; i < totalUnitNum; i++)
            {
                List<int> rows = new List<int>();
                List<int> cols = new List<int>();
                for (int _i = 1; _i <= gridsGroup._rowCount; _i++)
                {
                    rows.Add(_i);
                }
                for (int _i = 1; _i <= gridsGroup._colCount; _i++)
                {
                    cols.Add(_i);
                }

                int _index = new Random().Next(0, unitIDs.Count);
                var _u = _dic.First(e => e.Key.Unitid == unitIDs[_index] && e.Value != 0).Key;
                Unit _unit = new Unit(_u.Unitid, _u.Color, _u.CostX, _u.CostY);
                for (int j = 1; j <= 9999; j++)
                {
                    isColOk = true;
                    isRowOk = true;
                    int _startRow = rows[new Random().Next(0, rows.Count)];
                    int _startCol = cols[new Random().Next(0, cols.Count)];

                    if (gridsGroup._grids.Find(e => e.Row == _startRow && e.Column == _startCol && e.IsAvailable) != null)
                    {
                        for (int _y = 0; _y < _unit.CostX; _y++)
                        {
                            if (gridsGroup._grids.Find(e => e.Row == _startRow && e.Column == _startCol + _y && e.IsAvailable) == null)
                            {
                                isColOk = false;
                            }
                        }
                        for (int _x = 0; _x < _unit.CostY; _x++)
                        {
                            if (gridsGroup._grids.Find(e => e.Row == _startRow + _x && e.Column == _startCol && e.IsAvailable) == null)
                            {
                                isRowOk = false;
                            }
                        }
                        if (isColOk && isRowOk)
                        {
                            rowOkNum = _startRow;
                            colOkNum = _startCol;
                            _unit.gridList.Clear();
                            for (int a = rowOkNum; a < rowOkNum + _unit.CostY; a++)
                            {
                                for (int b = colOkNum; b < colOkNum + _unit.CostX; b++)
                                {
                                    var g = gridsGroup._grids.Find(e => e.Row == a && e.Column == b);
                                    g.IsAvailable = false;
                                    g.UnitID = _unit.Unitid;
                                    _unit.gridList.Add(g);
                                }
                            }
                            if (!finalList.Contains(_unit))
                            {
                                finalList.Add(_unit);
                            }

                            _dic[_u]--;
                            if (_dic[_u] == 0)
                            {
                                unitIDs.Remove(_unit.Unitid);
                            }
                            break;
                        }
                    }
                    else
                    {
                        continue;
                    }

                }

            }
            _finalList = finalList;

            StringBuilder sb_red = new StringBuilder();
            StringBuilder sb_yellow = new StringBuilder();
            StringBuilder sb_blue = new StringBuilder();
            foreach (var item in finalList)
            {
                item.gridList.Sort((a, b) => a.Row.CompareTo(b.Row));
                item.gridList.Sort((a, b) => a.Column.CompareTo(b.Column));
                switch (item.Color)
                {
                    case 1:
                        sb_red.Append(item.Unitid).Append(',').Append(enemyLevel).Append(',').Append(item.gridList[0].Row).Append(',').Append(item.gridList[0].Column).Append('|');
                        break;
                    case 2:
                        sb_yellow.Append(item.Unitid).Append(',').Append(enemyLevel).Append(',').Append(item.gridList[0].Row).Append(',').Append(item.gridList[0].Column).Append('|');
                        break;
                    case 3:
                        sb_blue.Append(item.Unitid).Append(',').Append(enemyLevel).Append(',').Append(item.gridList[0].Row).Append(',').Append(item.gridList[0].Column).Append('|');
                        break;
                }
            }
            string red = sb_red.ToString();
            string yellow = sb_yellow.ToString();
            string blue = sb_blue.ToString();
            red = string.IsNullOrEmpty(red) ? "kong" : red.Substring(0, red.Length - 1);
            yellow = string.IsNullOrEmpty(yellow) ? "kong" : yellow.Substring(0, yellow.Length - 1);
            blue = string.IsNullOrEmpty(blue) ? "kong" : blue.Substring(0, blue.Length - 1);
            return new string[] { red, yellow, blue };
        }
        public static string[] DeployUnits_new(GridsGroup gridsGroup, List<Unit> unitlist, out List<Unit> _finalList, int unitidCount = 0, int enemyLevel = 1, int gridPercent = 0,DeployTactic tactic = default)
        {
            List<Unit> finalList = new List<Unit>();
            int totalUnitNum = 0,
                totalGrids = gridsGroup._rowCount * gridsGroup._colCount;
            Dictionary<Unit, int> _dic = GetUnits(unitlist, totalGrids, unitidCount, gridPercent);
            List<int> unitIDs = new List<int>();
            foreach (var item in _dic)
            {
                totalUnitNum += item.Value;
                unitIDs.Add(item.Key.Unitid);
            }


            bool isRowOk = true, isColOk = true;
            int rowOkNum = 0, colOkNum = 0;

            //开始给每个单位找格子
            for (int i = 0; i < totalUnitNum; i++)
            {
                List<int> rows = new List<int>();
                List<int> cols = new List<int>();
                for (int _i = 1; _i <= gridsGroup._rowCount; _i++)
                {
                    rows.Add(_i);
                }
                for (int _i = 1; _i <= gridsGroup._colCount; _i++)
                {
                    cols.Add(_i);
                }

                int _index = new Random().Next(0, unitIDs.Count);
                var _u = _dic.First(e => e.Key.Unitid == unitIDs[_index] && e.Value != 0).Key;
                Unit _unit = new Unit(_u.Unitid, _u.Color, _u.CostX, _u.CostY);
                for (int j = 1; j <= 9999; j++)
                {
                    isColOk = true;
                    isRowOk = true;
                    int _startRow = rows[new Random().Next(0, rows.Count)];
                    int _startCol = cols[new Random().Next(0, cols.Count)];

                    if (gridsGroup._grids.Find(e => e.Row == _startRow && e.Column == _startCol && e.IsAvailable) != null)
                    {
                        for (int _y = 0; _y < _unit.CostX; _y++)
                        {
                            if (gridsGroup._grids.Find(e => e.Row == _startRow && e.Column == _startCol + _y && e.IsAvailable) == null)
                            {
                                isColOk = false;
                            }
                        }
                        for (int _x = 0; _x < _unit.CostY; _x++)
                        {
                            if (gridsGroup._grids.Find(e => e.Row == _startRow + _x && e.Column == _startCol && e.IsAvailable) == null)
                            {
                                isRowOk = false;
                            }
                        }
                        if (isColOk && isRowOk)
                        {
                            rowOkNum = _startRow;
                            colOkNum = _startCol;
                            _unit.gridList.Clear();
                            for (int a = rowOkNum; a < rowOkNum + _unit.CostY; a++)
                            {
                                for (int b = colOkNum; b < colOkNum + _unit.CostX; b++)
                                {
                                    var g = gridsGroup._grids.Find(e => e.Row == a && e.Column == b);
                                    g.IsAvailable = false;
                                    g.UnitID = _unit.Unitid;
                                    _unit.gridList.Add(g);
                                }
                            }
                            if (!finalList.Contains(_unit))
                            {
                                finalList.Add(_unit);
                            }

                            _dic[_u]--;
                            if (_dic[_u] == 0)
                            {
                                unitIDs.Remove(_unit.Unitid);
                            }
                            break;
                        }
                    }
                    else
                    {
                        continue;
                    }

                }

            }
            _finalList = finalList;

            
            return OutputData(finalList, enemyLevel);
        }

        /// <summary>
        /// 计算完成后，根据计算得到的unit的列表和设置的敌人等级，输出结果字符串
        /// </summary>
        static string[] OutputData(List<Unit> finalList,int enemyLevel)
        {
            StringBuilder sb_red = new StringBuilder();
            StringBuilder sb_yellow = new StringBuilder();
            StringBuilder sb_blue = new StringBuilder();
            foreach (var item in finalList)
            {
                item.gridList.Sort((a, b) => a.Row.CompareTo(b.Row));
                item.gridList.Sort((a, b) => a.Column.CompareTo(b.Column));
                switch (item.Color)
                {
                    case 1:
                        sb_red.Append(item.Unitid).Append(',').Append(enemyLevel).Append(',').Append(item.gridList[0].Row).Append(',').Append(item.gridList[0].Column).Append('|');
                        break;
                    case 2:
                        sb_yellow.Append(item.Unitid).Append(',').Append(enemyLevel).Append(',').Append(item.gridList[0].Row).Append(',').Append(item.gridList[0].Column).Append('|');
                        break;
                    case 3:
                        sb_blue.Append(item.Unitid).Append(',').Append(enemyLevel).Append(',').Append(item.gridList[0].Row).Append(',').Append(item.gridList[0].Column).Append('|');
                        break;
                }
            }
            string red = sb_red.ToString();
            string yellow = sb_yellow.ToString();
            string blue = sb_blue.ToString();
            red = string.IsNullOrEmpty(red) ? "kong" : red.Substring(0, red.Length - 1);
            yellow = string.IsNullOrEmpty(yellow) ? "kong" : yellow.Substring(0, yellow.Length - 1);
            blue = string.IsNullOrEmpty(blue) ? "kong" : blue.Substring(0, blue.Length - 1);
            return new string[] { red, yellow, blue };
        }

    }
}
public enum DeployTactic
{
    Default,//全随机
    OneLegion,//中央集中
    TwoLegion,//上下分散、左右分散随机
    ThreeLegion,//上下分散3团、左右分散3团随机
    Scatter,//尽量每一个都不互相挨着
    Follow//两个兵种配对，尽量一个跟一个固定组合
}
