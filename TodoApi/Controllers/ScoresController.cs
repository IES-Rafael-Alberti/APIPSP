using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Threading.Tasks;
using TodoApi.Models;
using TodoApi.Services;

namespace TodoApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ScoresController : ControllerBase
    {
        private readonly ScoreRepository _scoreRepository;

        public ScoresController(ScoreRepository scoreRepository)
        {
            _scoreRepository = scoreRepository;
        }

        // GET: api/Scores
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Score>>> GetScores()
        {
            var scores = await _scoreRepository.GetAllAsync();
            return Ok(scores);
        }

        // GET: api/Scores/5
        [HttpGet("{id}")]
        public async Task<ActionResult<Score>> GetScore(long id)
        {
            var score = await _scoreRepository.GetByIdAsync(id);

            if (score == null)
            {
                return NotFound();
            }

            return Ok(score);
        }

        // GET: api/Scores/player/John
        [HttpGet("player/{playerName}")]
        public async Task<ActionResult<IEnumerable<Score>>> GetScoresByPlayer(string playerName)
        {
            var scores = await _scoreRepository.GetByPlayerAsync(playerName);
            return Ok(scores);
        }

        // POST: api/Scores
        [HttpPost]
        public async Task<ActionResult<Score>> CreateScore(ScoreDTO scoreDto)
        {
            if (string.IsNullOrEmpty(scoreDto.Player))
            {
                return BadRequest("Player name is required");
            }

            var createdScore = await _scoreRepository.CreateAsync(scoreDto);
            return CreatedAtAction(nameof(GetScore), new { id = createdScore.Id }, createdScore);
        }

        // PUT: api/Scores/5
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateScore(long id, ScoreDTO scoreDto)
        {
            if (string.IsNullOrEmpty(scoreDto.Player))
            {
                return BadRequest("Player name is required");
            }

            var result = await _scoreRepository.UpdateAsync(id, scoreDto);
            
            if (!result)
            {
                return NotFound();
            }

            return NoContent();
        }

        // DELETE: api/Scores/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteScore(long id)
        {
            var result = await _scoreRepository.DeleteAsync(id);
            
            if (!result)
            {
                return NotFound();
            }

            return NoContent();
        }
    }
}