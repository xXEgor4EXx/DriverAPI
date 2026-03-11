using Microsoft.AspNetCore.Mvc;
using MyAPP.Common;
using MyAPP.Singletons;

[ApiController]
[Route("[controller]")]
public class CntrWorks : Controller
{
    [HttpGet]
    public List<Works> Get()
    {
        List<Works> result = DAO.Instance.Work.Get();
        return result;
    }

    [HttpGet("{id}")]
    public Works? Get(int id)
    {
        Works? result = DAO.Instance.Work.Get(id);
        return result;
    }

    [HttpDelete]
    public void Delete(int id)
    {
        DAO.Instance.Work.Delete(id);
    }

    [HttpPut]
    public void Put(int id, Works item)
    {
        DAO.Instance.Work.Put(id, item);
    }

    [HttpPost]
    public void Post(Works item)
    {
        DAO.Instance.Work.Post(item);
    }
}