using Microsoft.AspNetCore.Mvc;
using PredictionModels.Service;
using PreditionModels.Helpers;

namespace PreditionModels.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class ForecastController : ControllerBase
    {
        private readonly ForecastService _predictService;

        public ForecastController(ForecastService predictService)
        {
            _predictService = predictService;
        }

        [HttpGet("/regression-forecast")]
        public IActionResult GetRegressionForecast([FromQuery] double[] inputFeatures)
        {
            if (inputFeatures == null || inputFeatures.Length != 28)
            {
                return BadRequest("Please provide exactly 28 input features.");
            }

            var results = _predictService.PredictXGBoostModel(inputFeatures);

            return Ok(results);
        }

        [HttpGet("/time-series")]
        public IActionResult GetTimeseriesForcast([FromQuery] float[] labels, [FromQuery] float[] timestamps)
        {
            if (labels == null || timestamps == null || labels.Length != 10 || timestamps.Length != 10)
            {
                return BadRequest("Please provide exactly 10 labels and 10 timestamps.");
            }

            // Use the provided labels and timestamps
            var results = _predictService.PredictTimeseriesModel(timestamps, labels);

            return Ok(results);
        }
    }
}