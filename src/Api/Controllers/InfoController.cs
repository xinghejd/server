﻿using Bit.Core.Utilities;
using Microsoft.AspNetCore.Mvc;

namespace Bit.Api.Controllers;

public class InfoController : Controller
{
    [HttpGet("~/alive")]
    [HttpGet("~/now")]
    public DateTime GetAlive()
    {
        return DateTime.UtcNow;
    }

    [HttpGet("~/version")]
    public JsonResult GetVersion()
    {
        return Json(AssemblyHelpers.GetVersion());
    }

    [HttpGet("~/ip")]
    public JsonResult Ip()
    {
        var headerSet = new HashSet<string> { "x-forwarded-for", "x-connecting-ip", "cf-connecting-ip", "client-ip", "true-client-ip" };
        var headers = HttpContext.Request?.Headers
            .Where(h => headerSet.Contains(h.Key.ToLower()))
            .ToDictionary(h => h.Key);
        return new JsonResult(new
        {
            Ip = HttpContext.Connection?.RemoteIpAddress?.ToString(),
            Headers = headers,
        });
    }

    [HttpGet("~/opendoors")]
    public IActionResult Hello()
    {
        return Content("I'm sorry Dave, I'm afraid I can't do that.");
    }
}
