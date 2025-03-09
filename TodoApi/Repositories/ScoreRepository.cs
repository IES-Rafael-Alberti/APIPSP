using MongoDB.Driver;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using TodoApi.Models;
using Microsoft.Extensions.Configuration;

namespace TodoApi.Services
{
    public class ScoreRepository
    {
        private readonly IMongoCollection<Score> _scores;
        
        public ScoreRepository(IConfiguration configuration)
        { 
            var connectionString = configuration["MONGO_URI"];
            
            if (string.IsNullOrEmpty(connectionString))
            {
                throw new ArgumentException("MongoDB connection string not found in configuration");
            }
            
            // Create MongoDB client and connect to database
            var client = new MongoClient(connectionString);
            var database = client.GetDatabase("ScoreDatabase");
            _scores = database.GetCollection<Score>("Puntuaciones");
        }
        
        public async Task<List<Score>> GetAllAsync()
        {
            return await _scores.Find(_ => true).ToListAsync();
        }
        
        public async Task<Score> GetByIdAsync(long id)
        {
            return await _scores.Find(score => score.Id == id).FirstOrDefaultAsync();
        }
        
        public async Task<List<Score>> GetByPlayerAsync(string playerName)
        {
            return await _scores.Find(score => score.Player == playerName).ToListAsync();
        }
        
        public async Task<Score> CreateAsync(ScoreDTO scoreDto)
        {
            var lastScore = await _scores.Find(_ => true).SortByDescending(s => s.Id).FirstOrDefaultAsync();
            long newId = (lastScore?.Id ?? 0) + 1;
            
            var score = new Score
            {
                Id = newId,
                Player = scoreDto.Player,
                Points = scoreDto.Points
            };
            
            await _scores.InsertOneAsync(score);
            return score;
        }
        
        public async Task<bool> UpdateAsync(long id, ScoreDTO scoreDto)
        {
            var update = Builders<Score>.Update
                .Set(s => s.Player, scoreDto.Player)
                .Set(s => s.Points, scoreDto.Points);
                
            var result = await _scores.UpdateOneAsync(score => score.Id == id, update);
            return result.ModifiedCount > 0;
        }
        
        public async Task<bool> DeleteAsync(long id)
        {
            var result = await _scores.DeleteOneAsync(score => score.Id == id);
            return result.DeletedCount > 0;
        }
    }
}