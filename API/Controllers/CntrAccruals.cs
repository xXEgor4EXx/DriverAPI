using Microsoft.AspNetCore.Mvc;
using MyAPP.Common;
using MyAPP.Singletons;

[ApiController]
[Route("[controller]")]
public class CntrAccruals : ControllerBase
{
    [HttpGet]
    public ActionResult<List<Accruals>> Get()
    {
        return Ok(DAO.Instance.Accruals.Get());
    }

    [HttpGet("{id}")]
    public ActionResult<Accruals> Get(int id)
    {
        var item = DAO.Instance.Accruals.Get(id);
        if (item == null) return NotFound(new { error = $"Начисление с ID {id} не найдено." });
        return Ok(item);
    }

    [HttpDelete]
    public IActionResult Delete(int id)
    {
        var item = DAO.Instance.Accruals.Get(id);
        if (item == null) return NotFound(new { error = $"Начисление с ID {id} не найдено." });
        DAO.Instance.Accruals.Delete(id);
        return NoContent();
    }

    [HttpPut]
    public IActionResult Put(int id, Accruals item)
    {
        var existing = DAO.Instance.Accruals.Get(id);
        if (existing == null) return NotFound(new { error = $"Начисление с ID {id} не найдено." });
        DAO.Instance.Accruals.Put(id, item);
        return NoContent();
    }

    [HttpPost]
    public IActionResult Post(Accruals item)
    {
        DAO.Instance.Accruals.Post(item);
        return Created(string.Empty, null);
    }
}
