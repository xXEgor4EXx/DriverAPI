using Microsoft.AspNetCore.Mvc;
using MyAPP.Common;
using MyAPP.Singletons;

[ApiController]
[Route("[controller]")]
public class CntrProfessionEmployer : ControllerBase
{
    [HttpGet]
    public ActionResult<List<ProfessionEmployers>> Get()
    {
        return Ok(DAO.Instance.ProfessionEmployers.Get());
    }

    [HttpGet("{id}")]
    public ActionResult<ProfessionEmployers> Get(int id)
    {
        var item = DAO.Instance.ProfessionEmployers.Get(id);
        if (item == null) return NotFound(new { error = $"Профессиональное назначение с ID {id} не найдено." });
        return Ok(item);
    }

    [HttpDelete]
    public IActionResult Delete(int id)
    {
        var item = DAO.Instance.ProfessionEmployers.Get(id);
        if (item == null) return NotFound(new { error = $"Профессиональное назначение с ID {id} не найдено." });
        DAO.Instance.ProfessionEmployers.Delete(id);
        return NoContent();
    }

    [HttpPut]
    public IActionResult Put(int id, ProfessionEmployers item)
    {
        var existing = DAO.Instance.ProfessionEmployers.Get(id);
        if (existing == null) return NotFound(new { error = $"Профессиональное назначение с ID {id} не найдено." });
        DAO.Instance.ProfessionEmployers.Put(id, item);
        return NoContent();
    }

    [HttpPost]
    public IActionResult Post(ProfessionEmployers item)
    {
        DAO.Instance.ProfessionEmployers.Post(item);
        return Created(string.Empty, null);
    }
}
