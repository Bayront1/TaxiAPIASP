using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TaxiAPI.Models;

namespace TaxiAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class MessageController : ControllerBase
    {
        private readonly TaxiContext _context;

        public MessageController(TaxiContext context)
        {
            _context = context;
        }

/*        [HttpGet("{senderId}/{receiverId}")]
        public async Task<ActionResult<IEnumerable<Message>>> GetMessages(int senderId, int receiverId)
        {
            var messages = await _context.Messages
                .Where(m => (m.SenderId == senderId && m.ReceiverId == receiverId) || (m.SenderId == receiverId && m.ReceiverId == senderId))
                .ToListAsync();

            if (messages == null)
            {
                return NotFound();
            }
            return Ok(messages);
        }

        [HttpPost]
        public async Task<ActionResult<Message>> SendMessage(Message message)
        {
            _context.Messages.Add(message);
            await _context.SaveChangesAsync();
            return CreatedAtAction(nameof(GetMessages), new { senderId = message.SenderId, receiverId = message.ReceiverId }, message);
        }*/

    }
}
