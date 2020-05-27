using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace OrderService.Controllers
{
    /// <summary>
    /// Created to show everything mapping
    /// </summary>
    [ApiController]
    [ApiVersion("1.0")]
    [Route("api/v{version:apiVersion}/math-helper")]
    public class MathController : ControllerBase
    {
        /// <summary>
        /// Sum
        /// </summary>
        /// <returns></returns>
        [HttpGet("sum/{left}/{right}")]
        [HttpPost("sum/{left}/{right}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public ActionResult<decimal> Sum(decimal left, decimal right)
        {
            return Ok(left + right);
        }

        /// <summary>
        /// Division
        /// </summary>
        /// <returns></returns>
        [HttpGet("division/{left}/{right}")]
        [HttpPost("division/{left}/{right}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public ActionResult<decimal> Divide(decimal left, decimal right)
        {
            if (right == 0)
                return BadRequest("Zero division detected");
            return Ok(left / right);
        }
        
        /// <summary>
        /// Sub
        /// </summary>
        /// <returns></returns>
        [HttpGet("subtraction/{left}/{right}")]
        [HttpPost("subtraction/{left}/{right}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public ActionResult<decimal> Sub(decimal left, decimal right)
        {
            return Ok(left - right);
        }
        
        /// <summary>
        /// Multiplication
        /// </summary>
        /// <returns></returns>
        [HttpGet("multiplication/{left}/{right}")]
        [HttpPost("multiplication/{left}/{right}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public ActionResult<decimal> Multiplication(decimal left, decimal right)
        {
            return Ok(left * right);
        }
    }
}