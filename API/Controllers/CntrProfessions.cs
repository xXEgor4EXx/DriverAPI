using Microsoft.AspNetCore.Mvc;
using MyAPP.Common;
using MyAPP.Singletons;

[ApiController]
[Route("[controller]")]
public class CntrProfessions : Controller
{
    [HttpGet]
    public List<Professions> Get()
    {
        List<Professions> result = DAO.Instance.Professions.Get();
        return result;
    }
    [HttpGet("{id}")]
    public Professions? Get(int id)
    {
        Professions? result = DAO.Instance.Professions.Get(id);
        return result;
    }
    [HttpDelete]
    public void Delete(int id)
    {
        DAO.Instance.Professions.Delete(id);
    }
    [HttpPut]
    public void Put(int id, Professions item)
    {
        DAO.Instance.Professions.Put(id, item);
    }
    [HttpPost]
    public void Post(Professions item)
    {
        DAO.Instance.Professions.Post(item);
    }
}