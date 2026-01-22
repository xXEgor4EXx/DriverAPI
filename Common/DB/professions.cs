using System.Text.RegularExpressions;
namespace MyAPP.Common;
using System.Globalization;
public class Professions
{

    private const int TextLen = 100;
    private const string TextPattern = @"^[А-ЯЁ][А-ЯЁа-яё\- ]*$";
    private string _text = string.Empty;
    public int ProfessionID { get; set; }
    public string Title
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
}

