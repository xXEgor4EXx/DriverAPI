namespace MyAPP.Common;

public class Payments
{
    public int PaymentID { get; set; }
    public int EmployeeID { get; set; }
    public float AmountToPay { get; set; }
    public DateTime PaymentDate { get; set; }
}