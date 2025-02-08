using MongoDB.Driver;
using WebApplication1.Data;
using WebApplication1.Models;

namespace WebApplication1.Repositories
{
    public class UserRepository : IUserRepository
    {
        private readonly IMongoCollection<User> _users;

        public UserRepository(MongoDbContext context)
        {
            _users = context.Users;
        }

        public async Task<User> GetByEmailAsync(string email)
        {
            //var projection = Builders<User>.Projection.Exclude("_id");
            var current_user = await _users.Find(u => u.Email == email).FirstOrDefaultAsync();
            return current_user;
        }

        public async Task<bool> CheckPassword(string email, string password)
        {
            var user = await GetByEmailAsync(email);

            if (user == null || user.Password != password)
            {
                return false;
            }

            return true;
        }
        public async Task<bool> CreateAsync(User user)
        {
            var current_user = await GetByEmailAsync(user.Email);
            if (current_user != null)
            {
                return false; // User already exists
            }
            try
            {

                await _users.InsertOneAsync(user);
                return true; // Insertion successful
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Insert failed: {ex.Message}");
                return false; // Insertion failed
            }
        }
        public async Task UpdateAsync(User user) => await _users.ReplaceOneAsync(u => u.Email == user.Email, user);
        public async Task DeleteAsync(string email) => await _users.DeleteOneAsync(u => u.Email == email);

        public async Task<List<Dictionary<string, List<Video>>>> GetAllProjects(string email)
        {
            var current_user = await GetByEmailAsync(email);
            return current_user.Projects;
        }

        public async Task<bool> SaveProject(string email, string project_name, List<Video> data)
        {
            var current_user = await GetByEmailAsync(email);
            bool found = false;

            for (int i = 0; i < current_user.Projects.Count; i++)
            {
                if (current_user.Projects[i].ContainsKey(project_name))
                {
                    current_user.Projects[i][project_name] = data;
                    await UpdateAsync(current_user);
                    return true;
                }
            }

            if (!found)
            {
                current_user.Projects.Add(new Dictionary<string, List<Video>> { { project_name, data } });
                await UpdateAsync(current_user);
                return true;
            }
            return false;
        }

        public async Task<List<Video>> GetProject(string email, string project_name)
        {
            var current_user = await GetByEmailAsync(email);
            foreach (var project in current_user.Projects)
            {
                if (project.ContainsKey(project_name))
                {
                    return project[project_name];
                }
            }
            return null;
        }
    }
}
