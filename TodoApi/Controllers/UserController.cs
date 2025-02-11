using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TodoApi.Models;

namespace UserApi.Controllers;

[Route("api/Users")]
[ApiController]
public class UserController : ControllerBase
{
    private readonly UserContext _context;

    public UserController(UserContext context)
    {
        _context = context;
    }

    // GET: api/Users
    [HttpGet]
    public async Task<ActionResult<IEnumerable<UserDTO>>> GetUsers()
    {
        return await _context.User
            .Select(x => UserToDTO(x))
            .ToListAsync();
    }

    // GET: api/Users/5
    [HttpGet("{id}")]
    public async Task<ActionResult<UserDTO>> GetUser(long id)
    {
        var user = await _context.User.FindAsync(id);
        if (user == null)
        {
            return NotFound();
        }
        return UserToDTO(user);
    }

    // PUT: api/Users/5
    [HttpPut("{id}")]
    public async Task<IActionResult> PutUser(long id, UserDTO userDTO)
    {
        var user = await _context.User.FindAsync(id);
        if (user == null)
        {
            return NotFound();
        }

        user.Name = userDTO.Name;
        // Add other properties as needed

        try
        {
            await _context.SaveChangesAsync();
        }
        catch (DbUpdateConcurrencyException) when (!UserExists(id))
        {
            return NotFound();
        }

        return NoContent();
    }

    // POST: api/Users
    [HttpPost]
    public async Task<ActionResult<UserDTO>> PostUser(UserDTO userDTO)
    {
        var user = new User
        {
            Name = userDTO.Name,
            Password = userDTO.Password // Be careful with password handling
        };

        _context.User.Add(user);
        await _context.SaveChangesAsync();

        return CreatedAtAction(
            nameof(GetUser),
            new { id = user.Id },
            UserToDTO(user));
    }

    // DELETE: api/Users/5
    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteUser(long id)
    {
        var user = await _context.User.FindAsync(id);
        if (user == null)
        {
            return NotFound();
        }

        _context.User.Remove(user);
        await _context.SaveChangesAsync();

        return NoContent();
    }

    private bool UserExists(long id)
    {
        return _context.User.Any(e => e.Id == id);
    }

    private static UserDTO UserToDTO(User user) =>
        new UserDTO
        {
            Name = user.Name,
            Password = user.Password
        };
}