using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;

namespace ProductApp_API.Controllers;
[ApiController]
[Route("[Controller]")]
public class ConfigController : ControllerBase
{
    private readonly IConfiguration _Config;
    private readonly IOptionsMonitor<AttachmentOptions> _attachmentOptions;

    public ConfigController(IConfiguration Config, IOptionsMonitor<AttachmentOptions> attachmentOptions)
    {
        _Config=Config;
        _attachmentOptions=attachmentOptions;
        var value=_attachmentOptions.CurrentValue;
    }
    [HttpGet]
    [Route("")]
    public ActionResult GetConfig()
    {
        Thread.Sleep(10000);
        var config = new
        {
            EnvName = _Config["ASPNETCORE_ENVIRONMENT"],
            AllowedHosts = _Config["AllowedHosts"],
            DefaultLogLevel = _Config["Logging:LogLevel:Default"],
            DefaultConnection = _Config.GetConnectionString("DefaultConnection"),
            TestingKey = _Config["TestingKey"],
            SigningKey = _Config["SigningKey"],
            AttachmentOptions = _attachmentOptions.CurrentValue,
        };
        return Ok(config);
    }

}

