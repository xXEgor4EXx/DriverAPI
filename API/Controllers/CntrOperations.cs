using Microsoft.AspNetCore.Mvc;
using MyAPP.Common;
using MyAPP.Singletons;

[ApiController]
[Route("[controller]")]
public class CntrOperations : ControllerBase
{
    [HttpGet]
    public ActionResult<List<Operations>> Get()
    {
        return Ok(DAO.Instance.Operations.Get());
    }

    [HttpGet("{id}")]
    public ActionResult<Operations> Get(int id)
    {
        var item = DAO.Instance.Operations.Get(id);
        if (item == null) return NotFound(new { error = $"Операция с ID {id} не найдена." });
        return Ok(item);
    }

    [HttpDelete]
    public IActionResult Delete(int id)
    {
        var item = DAO.Instance.Operations.Get(id);
        if (item == null) return NotFound(new { error = $"Операция с ID {id} не найдена." });
        DAO.Instance.Operations.Delete(id);
        return NoContent();
    }

    [HttpPut]
    public IActionResult Put(int id, Operations item)
    {
        var existing = DAO.Instance.Operations.Get(id);
        if (existing == null) return NotFound(new { error = $"Операция с ID {id} не найдена." });
        DAO.Instance.Operations.Put(id, item);
        return NoContent();
    }

    [HttpPost]
    public IActionResult Post(Operations item)
    {
        DAO.Instance.Operations.Post(item);
        return Created(string.Empty, null);
    }
}
