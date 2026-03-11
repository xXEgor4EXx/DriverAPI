using Microsoft.AspNetCore.Mvc;
using MyAPP.Common;
using MyAPP.Singletons;

[ApiController]
[Route("[controller]")]
public class CntrPayments : Controller
{
    [HttpGet]
    public List<Payments> Get()
    {
        List<Payments> result = DAO.Instance.Payments.Get();
        return result;
    }
    [HttpGet("{id}")]
    public Payments? Get(int id)
    {
        Payments? result = DAO.Instance.Payments.Get(id);
        return result;
    }
    [HttpDelete]
    public void Delete(int id)
    {
        DAO.Instance.Payments.Delete(id);
    }
    [HttpPut]
    public void Put(int id, Payments item)
    {
        DAO.Instance.Payments.Put(id, item);
    }
    [HttpPost]
    public void Post(Payments item)
    {   
        DAO.Instance.Payments.Post(item);
    }  
}