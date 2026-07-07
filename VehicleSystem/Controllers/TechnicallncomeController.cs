using Microsoft.AspNetCore.Mvc;
using VehicleSystem.Data;
using VehicleSystem.models;
//using VehicleSystem.Models;

namespace VehicleSystem.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class TechnicalIncomeController : ControllerBase
    {
        private readonly TechnicalIncomeHelper _technicalHelper = new TechnicalIncomeHelper();

        // POST: api/TechnicalIncome/Add
        [HttpPost("Add")]
        public IActionResult Add([FromBody] TechnicalIncome data)
        {
            var result = _technicalHelper.AddTechnicalIncome(data);
            return result.Status ? Ok(result) : BadRequest(result);
        }

        // PUT: api/TechnicalIncome/Update
        [HttpPut("Update")]
        public IActionResult Update([FromBody] TechnicalIncome data)
        {
            var result = _technicalHelper.UpdateTechnicalIncome(data);
            return result.Status ? Ok(result) : BadRequest(result);
        }

        // GET: api/TechnicalIncome/Get?id=0&vehicleType=A
        [HttpGet("Get")]
        public IActionResult Get([FromQuery] int id = 0, [FromQuery] string vehicleType = null)
        {
            var result = _technicalHelper.GetTechnicalIncome(id, vehicleType);
            return result.Status ? Ok(result) : NotFound(result);
        }

        // GET: api/TechnicalIncome/AllMonthlyTotals
        [HttpGet("AllMonthlyTotals")]
        public IActionResult AllMonthlyTotals()
        {
            var result = _technicalHelper.GetAllMonthlyTotals();
            return result.Status ? Ok(result) : BadRequest(result);
        }

        // DELETE: api/TechnicalIncome/Delete/5
        [HttpDelete("Delete/{id}")]
        public IActionResult Delete(int id)
        {
            var result = _technicalHelper.DeleteTechnicalIncome(id);
            return result.Status ? Ok(result) : BadRequest(result);
        }
    }
}
