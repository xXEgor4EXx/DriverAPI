using System.Text.RegularExpressions;

namespace MyAPP.Common;

public class ProfessionEmployers
{
    private const int TextLen = 250;
    private const string TextPattern = @"^[А-ЯЁ][а-яё\- ]+$";
    private string _text = string.Empty;
    public int ProfessionEmployeeID { get; set; }
    public int EmployeeID { get; set; }
    public int ProfessionID { get; set; }
    public string Name
    {
        get
        {
            return _text;
        }
        set
        {
            if (value.Length > TextLen)
            {
                throw new ArgumentException($"Text must be shorten than {TextLen}!");
            }
            else
            {
                if (Regex.IsMatch(value, TextPattern))
                {
                    _text = value;
                }
                else
                {
                    throw new ArgumentException($"Text format error!");
                }
            }
        }
    }
    public DateTime DateOfStart { get; set; }
    public DateTime? DateOfEnd{ get; set; }
}