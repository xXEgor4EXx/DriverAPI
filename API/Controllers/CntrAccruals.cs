using Microsoft.AspNetCore.Mvc;
using MyAPP.Common;
using MyAPP.Singletons;

[ApiController]
[Route("[controller]")]
public class CntrAccruals : Controller
{
    [HttpGet]
    public List<Accruals> Get()
    {
        List<Accruals> result = DAO.Instance.Accruals.Get();
        return result;
    }
    [HttpGet("{id}")]
    public Accruals? Get(int id)
    {
        Accruals? result = DAO.Instance.Accruals.Get(id);
        return result;
    }
    [HttpDelete]
    public void Delete(int id)
    {
        DAO.Instance.Accruals.Delete(id);
    }
    [HttpPut]
    public void Put(int id, Accruals item)
    {
        DAO.Instance.Accruals.Put(id, item);
    }
    [HttpPost]
    public void Post(Accruals item)
    {
        DAO.Instance.Accruals.Post(item);
    } 
}