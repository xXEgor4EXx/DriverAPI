using Microsoft.AspNetCore.Mvc;
using MyAPP.Common;
using MyAPP.Singletons;

[ApiController]
[Route("[controller]")]
public class CntrProfessionEmployer : Controller
{

    [HttpGet]
    public List<ProfessionEmployers> Get()
    {
        List<ProfessionEmployers> result = DAO.Instance.ProfessionEmployers.Get();
        return result;
    }
    [HttpGet("{id}")]
    public ProfessionEmployers? Get(int id)
    {
        ProfessionEmployers? result = DAO.Instance.ProfessionEmployers.Get(id);
        return result;
    }
    [HttpDelete]
    public void Delete(int id)
    {
        DAO.Instance.ProfessionEmployers.Delete(id);
    }
    [HttpPut]
    public void Put(int id, ProfessionEmployers item)
    {
        DAO.Instance.ProfessionEmployers.Put(id, item);
    }
    [HttpPost]
    public void Post(ProfessionEmployers item)
    {
        DAO.Instance.ProfessionEmployers.Post(item);
    }
}