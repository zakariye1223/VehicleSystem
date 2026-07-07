using Microsoft.AspNetCore.Mvc;
using VehicleSystem.Data;
using VehicleSystem.models;
//using VehicleSystem.Models;

namespace VehicleSystem.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class VehicleDataController : ControllerBase
    {
        private readonly VehicleDataHelper _vehicleHelper = new VehicleDataHelper();

        // POST: api/VehicleData/Add
        [HttpPost("Add")]
        public IActionResult Add([FromBody] VehicleData data)
        {
            var result = _vehicleHelper.AddVehicleData(data);
            return result.Status ? Ok(result) : BadRequest(result);
        }

        // PUT: api/VehicleData/Update
        [HttpPut("Update")]
        public IActionResult Update([FromBody] VehicleData data)
        {
            var result = _vehicleHelper.UpdateVehicleData(data);
            return result.Status ? Ok(result) : BadRequest(result);
        }

        // GET: api/VehicleData/Get?id=0&userId=0&vehicleType=A
        [HttpGet("Get")]
        public IActionResult Get([FromQuery] int id = 0, [FromQuery] int userId = 0, [FromQuery] string vehicleType = null)
        {
            var result = _vehicleHelper.GetVehicleData(id, userId, vehicleType);
            return result.Status ? Ok(result) : NotFound(result);
        }

        // GET: api/VehicleData/Search?vehicleType=A&fromDate=2026-07-01&toDate=2026-07-31&keyword=fuel
        [HttpGet("Search")]
        public IActionResult Search(
            [FromQuery] string vehicleType = null,
            [FromQuery] DateTime? fromDate = null,
            [FromQuery] DateTime? toDate = null,
            [FromQuery] string keyword = null)
        {
            var result = _vehicleHelper.SearchVehicleData(vehicleType, fromDate, toDate, keyword);
            return result.Status ? Ok(result) : NotFound(result);
        }

        // DELETE: api/VehicleData/Delete/5
        [HttpDelete("Delete/{id}")]
        public IActionResult Delete(int id)
        {
            var result = _vehicleHelper.DeleteVehicleData(id);
            return result.Status ? Ok(result) : BadRequest(result);
        }

        // GET: api/VehicleData/MonthlyReport?year=2026&month=7
        [HttpGet("MonthlyReport")]
        public IActionResult MonthlyReport([FromQuery] int year, [FromQuery] int month)
        {
            var result = _vehicleHelper.GetMonthlyReport(year, month);
            return result.Status ? Ok(result) : BadRequest(result);
        }

        // GET: api/VehicleData/AllMonthlyTotals
        [HttpGet("AllMonthlyTotals")]
        public IActionResult AllMonthlyTotals()
        {
            var result = _vehicleHelper.GetAllMonthlyTotals();
            return result.Status ? Ok(result) : BadRequest(result);
        }
    }
}
