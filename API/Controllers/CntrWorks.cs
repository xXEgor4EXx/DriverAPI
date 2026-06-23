using Microsoft.AspNetCore.Mvc;
using MyAPP.Common;
using MyAPP.Singletons;

[ApiController]
[Route("[controller]")]
public class CntrWorks : ControllerBase
{
    [HttpGet]
    public ActionResult<List<Works>> Get()
    {
        return Ok(DAO.Instance.Work.Get());
    }

    [HttpGet("{id}")]
    public ActionResult<Works> Get(int id)
    {
        var item = DAO.Instance.Work.Get(id);
        if (item == null) return NotFound(new { error = $"Работа с ID {id} не найдена." });
        return Ok(item);
    }

    [HttpDelete]
    public IActionResult Delete(int id)
    {
        var item = DAO.Instance.Work.Get(id);
        if (item == null) return NotFound(new { error = $"Работа с ID {id} не найдена." });
        DAO.Instance.Work.Delete(id);
        return NoContent();
    }

    [HttpPut]
    public IActionResult Put(int id, Works item)
    {
        var existing = DAO.Instance.Work.Get(id);
        if (existing == null) return NotFound(new { error = $"Работа с ID {id} не найдена." });
        DAO.Instance.Work.Put(id, item);
        return NoContent();
    }

    [HttpPost]
    public IActionResult Post(Works item)
    {
        DAO.Instance.Work.Post(item);
        return Created(string.Empty, null);
    }
}
