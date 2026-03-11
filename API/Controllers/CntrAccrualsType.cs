using Microsoft.AspNetCore.Mvc;
using MyAPP.Common;
using MyAPP.Singletons;

[ApiController]
[Route("[controller]")]
public class CntrAccrualsType : Controller
{
     
    [HttpGet]
    public List<AccrualsType> Get()
    {
        List<AccrualsType> result = DAO.Instance.AccrualsType.Get();
        return result;
    }
    [HttpGet("{id}")]
    public AccrualsType? Get(int id)
    {
        AccrualsType? result = DAO.Instance.AccrualsType.Get(id);
        return result;
    }
    [HttpDelete]
    public void Delete(int id)
    {
        DAO.Instance.AccrualsType.Delete(id);
    }
    [HttpPut]
    public void Put(int id, AccrualsType item)
    {
        DAO.Instance.AccrualsType.Put(id, item);
    }
    [HttpPost]
    public void Post(AccrualsType item)
    {
        DAO.Instance.AccrualsType.Post(item);
    } 
}