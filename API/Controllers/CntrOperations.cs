using Microsoft.AspNetCore.Mvc;
using MyAPP.Common;
using MyAPP.Singletons;

[ApiController]
[Route("[controller]")]
public class CntrOperations : Controller
{
    [HttpGet]
    public List<Operations> Get()
    {
        List<Operations> result = DAO.Instance.Operations.Get();
        return result;
    }
    [HttpGet("{id}")]
    public Operations? Get(int id)
    {
        Operations? result = DAO.Instance.Operations.Get(id);
        return result;
    }
    [HttpDelete]
    public void Delete(int id)
    {   
        DAO.Instance.Operations.Delete(id);
    }
    [HttpPut]
    public void Put(int id, Operations item)
    {
        DAO.Instance.Operations.Put(id, item);
    }
    [HttpPost]
    public void Post(Operations item)
    {
        DAO.Instance.Operations.Post(item);
    }
}