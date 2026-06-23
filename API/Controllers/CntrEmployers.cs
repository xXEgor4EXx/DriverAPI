using Microsoft.AspNetCore.Mvc;
using MyAPP.Common;
using MyAPP.Singletons;

[ApiController]
[Route("[controller]")]
public class CntrEmployers : ControllerBase
{
    [HttpGet]
    public ActionResult<List<Employers>> Get()
    {
        return Ok(DAO.Instance.Employers.Get());
    }

    [HttpGet("{id}")]
    public ActionResult<Employers> Get(int id)
    {
        var item = DAO.Instance.Employers.Get(id);
        if (item == null) return NotFound(new { error = $"Сотрудник с ID {id} не найден." });
        return Ok(item);
    }

    [HttpDelete]
    public IActionResult Delete(int id)
    {
        var item = DAO.Instance.Employers.Get(id);
        if (item == null) return NotFound(new { error = $"Сотрудник с ID {id} не найден." });
        DAO.Instance.Employers.Delete(id);
        return NoContent();
    }

    [HttpPut]
    public IActionResult Put(int id, Employers item)
    {
        var existing = DAO.Instance.Employers.Get(id);
        if (existing == null) return NotFound(new { error = $"Сотрудник с ID {id} не найден." });
        DAO.Instance.Employers.Put(id, item);
        return NoContent();
    }

    [HttpPost]
    public IActionResult Post(Employers item)
    {
        DAO.Instance.Employers.Post(item);
        return Created(string.Empty, null);
    }
}
