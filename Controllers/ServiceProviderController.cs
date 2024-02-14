using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Memory;
using WorkWise.Models;

namespace WorkWise.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ServiceProviderController : ControllerBase
    {
        private readonly IMemoryCache _memoryCache;

        public ServiceProviderController(IMemoryCache memoryCache)
        {
            _memoryCache = memoryCache;
        }

        [HttpGet("All")]
        public IActionResult GetAllServiceProviders()
        {
            return Ok(_memoryCache.Get("ServiceProviders"));
        }

        [HttpPost]
        public IActionResult CreateServiceProvider([FromBody] User user)
        {
            var serviceProviers = new List<User>();

            var users = _memoryCache.GetOrCreate<List<User>>("ServiceProviders", (entry) =>
            {
                entry.AbsoluteExpiration = DateTime.Now.AddMinutes(10);
                return serviceProviers;
            });

            users.Add(user);

            _memoryCache.Set<List<User>>("ServiceProviders", users);
            return Ok(users);
        }

        [HttpPut("{id}", Name = "UpdateServiceProvider")]
        public IActionResult UpdateServiceProvider(int id, [FromBody] User user)
        {
            var users = _memoryCache.Get<List<User>>("ServiceProviders");

            if (users != null && users.Count > 0)
            {
                var updateuser = users.FirstOrDefault(a => a.Id == id);

                if (updateuser != null)
                {
                    updateuser.FirstName = user.FirstName;
                    updateuser.LastName = user.LastName;
                    updateuser.DateOfBirth = user.DateOfBirth;

                    _memoryCache.Set<List<User>>("ServiceProviders", users);
                    return CreatedAtRoute("UpdateServiceProvider", new { Id = updateuser.Id }, updateuser );
                }
                return NotFound();

            }
            return NotFound();
        }

        [HttpDelete("{id}")]
        public IActionResult DeleteServiceProvider(int id)
        {
            var users = _memoryCache.Get<List<User>>("ServiceProviders");
            if (users != null && users.Count > 0)
            {
                var updateuser = users.FirstOrDefault(a => a.Id == id);

                if (updateuser != null)
                {
                    var filteredUsers = users.Where(a => a.Id != updateuser.Id).ToList();
                    _memoryCache.Set<List<User>>("ServiceProviders", filteredUsers);
                    return NoContent();
                }
                return NotFound();

            }
            return NotFound();
        }
    }
}
