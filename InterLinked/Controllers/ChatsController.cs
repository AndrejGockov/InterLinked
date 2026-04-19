using InterLinked.Models;
using Microsoft.AspNetCore.Mvc;

[Route("[controller]")]
public class ChatsController : Controller
{
    private readonly AIService _aiService;

    public ChatsController(AIService aiService)
    {
        _aiService = aiService;
    }

    [HttpPost("SendMessage")]
    public async Task<IActionResult> SendMessage([FromBody] ChatRequest request)
    {
        if (request == null || string.IsNullOrWhiteSpace(request.Message))
        {
            return BadRequest("Message cannot be empty");
        }

        var reply = await _aiService.GetResponse(request.Message);

        return Json(new ChatResponse
        {
            Reply = reply
        });
    }
    // [HttpPost("SendMessage")]
    // public IActionResult SendMessage([FromBody] ChatRequest request)
    // {
    //     return Json(new { reply = "BACKEND WORKS" });
    // }

    public IActionResult Index()
    {
        return View();
    }
}