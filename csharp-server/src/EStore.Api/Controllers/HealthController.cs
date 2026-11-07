using EStore.Api.Contracts;
using Microsoft.AspNetCore.Mvc;

namespace EStore.Api.Controllers;


[ApiController]
[Route("Health")]
public class HealthCOntroller : ControllerBase
{
    [HttpGet]
    public IActionResult Get() => Ok(new HealthResponse("UP"));
}