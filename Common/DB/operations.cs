using System.Text.RegularExpressions;
namespace MyAPP.Common;

public class Operations
{
    private const int TextLen = 250;
    private const string TextPattern = @"^[А-ЯЁ][а-яё\- ]+$";
    private string _text = string.Empty;
    public int OperationID { get; set; }
    public string Description
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
    public float RatePerUnit { get; set; }
}