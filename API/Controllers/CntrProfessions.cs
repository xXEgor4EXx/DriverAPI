using Microsoft.AspNetCore.Mvc;
using MyAPP.Common;
using MyAPP.Singletons;

[ApiController]
[Route("[controller]")]
public class CntrProfessions : ControllerBase
{
    [HttpGet]
    public ActionResult<List<Professions>> Get()
    {
        return Ok(DAO.Instance.Professions.Get());
    }

    [HttpGet("{id}")]
    public ActionResult<Professions> Get(int id)
    {
        var item = DAO.Instance.Professions.Get(id);
        if (item == null) return NotFound(new { error = $"Профессия с ID {id} не найдена." });
        return Ok(item);
    }

    [HttpDelete]
    public IActionResult Delete(int id)
    {
        var item = DAO.Instance.Professions.Get(id);
        if (item == null) return NotFound(new { error = $"Профессия с ID {id} не найдена." });
        DAO.Instance.Professions.Delete(id);
        return NoContent();
    }

    [HttpPut]
    public IActionResult Put(int id, Professions item)
    {
        var existing = DAO.Instance.Professions.Get(id);
        if (existing == null) return NotFound(new { error = $"Профессия с ID {id} не найдена." });
        DAO.Instance.Professions.Put(id, item);
        return NoContent();
    }

    [HttpPost]
    public IActionResult Post(Professions item)
    {
        DAO.Instance.Professions.Post(item);
        return Created(string.Empty, null);
    }
}
