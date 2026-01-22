using Microsoft.AspNetCore.Mvc;
using MyAPP.Common;
using MyAPP.Singletons;

[ApiController]
[Route("[controller]")]
public class CntrEmployers : Controller
{ 
    [HttpGet]
    public List<Employers> Get()
    {
        List<Employers> result = DAO.Instance.Employers.Get();
        return result;
    }
    [HttpGet("{id}")]
    public Employers? Get(int id)
    {
        Employers? result = DAO.Instance.Employers.Get(id);
        return result;
    }
    [HttpDelete]
    public void Delete(int id)
    {
        DAO.Instance.Employers.Delete(id);
    }
    [HttpPut]
    public void Put(int id, Employers item)
    {
        DAO.Instance.Employers.Put(id, item);
    }
    [HttpPost]
    public void Post(Employers item)
    {
        DAO.Instance.Employers.Post(item);
    } 
}