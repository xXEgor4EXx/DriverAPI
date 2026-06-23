using Microsoft.AspNetCore.Mvc;
using MyAPP.Common;
using MyAPP.Singletons;

[ApiController]
[Route("[controller]")]
public class CntrPayments : ControllerBase
{
    [HttpGet]
    public ActionResult<List<Payments>> Get()
    {
        return Ok(DAO.Instance.Payments.Get());
    }

    [HttpGet("{id}")]
    public ActionResult<Payments> Get(int id)
    {
        var item = DAO.Instance.Payments.Get(id);
        if (item == null) return NotFound(new { error = $"Выплата с ID {id} не найдена." });
        return Ok(item);
    }

    [HttpDelete]
    public IActionResult Delete(int id)
    {
        var item = DAO.Instance.Payments.Get(id);
        if (item == null) return NotFound(new { error = $"Выплата с ID {id} не найдена." });
        DAO.Instance.Payments.Delete(id);
        return NoContent();
    }

    [HttpPut]
    public IActionResult Put(int id, Payments item)
    {
        var existing = DAO.Instance.Payments.Get(id);
        if (existing == null) return NotFound(new { error = $"Выплата с ID {id} не найдена." });
        DAO.Instance.Payments.Put(id, item);
        return NoContent();
    }

    [HttpPost]
    public IActionResult Post(Payments item)
    {
        DAO.Instance.Payments.Post(item);
        return Created(string.Empty, null);
    }
}
