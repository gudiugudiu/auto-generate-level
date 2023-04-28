// See https://aka.ms/new-console-template for more information
using AutoGenerateLevel;

Console.WriteLine("输入行数");

int _row, _column;

_row = Convert.ToInt32(Console.ReadLine());
Console.WriteLine("输入列数");
_column = Convert.ToInt32(Console.ReadLine());
var grids = new GridsGroup(_row,_column);



