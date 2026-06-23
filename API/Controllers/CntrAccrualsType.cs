using Microsoft.AspNetCore.Mvc;
using MyAPP.Common;
using MyAPP.Singletons;

[ApiController]
[Route("[controller]")]
public class CntrAccrualsType : ControllerBase
{
    [HttpGet]
    public ActionResult<List<AccrualsType>> Get()
    {
        return Ok(DAO.Instance.AccrualsType.Get());
    }

    [HttpGet("{id}")]
    public ActionResult<AccrualsType> Get(int id)
    {
        var item = DAO.Instance.AccrualsType.Get(id);
        if (item == null) return NotFound(new { error = $"Тип начисления с ID {id} не найден." });
        return Ok(item);
    }

    [HttpDelete]
    public IActionResult Delete(int id)
    {
        var item = DAO.Instance.AccrualsType.Get(id);
        if (item == null) return NotFound(new { error = $"Тип начисления с ID {id} не найден." });
        DAO.Instance.AccrualsType.Delete(id);
        return NoContent();
    }

    [HttpPut]
    public IActionResult Put(int id, AccrualsType item)
    {
        var existing = DAO.Instance.AccrualsType.Get(id);
        if (existing == null) return NotFound(new { error = $"Тип начисления с ID {id} не найден." });
        DAO.Instance.AccrualsType.Put(id, item);
        return NoContent();
    }

    [HttpPost]
    public IActionResult Post(AccrualsType item)
    {
        DAO.Instance.AccrualsType.Post(item);
        return Created(string.Empty, null);
    }
}
