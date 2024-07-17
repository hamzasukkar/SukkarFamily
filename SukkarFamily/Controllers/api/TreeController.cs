using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using SukkarFamily.Models;

namespace SukkarFamily.Controllers.api
{
    
    [Route("api/[controller]")]
    [ApiController]
    public class TreeController : ControllerBase
    {
        private DB db;
        public TreeController(DB db)
        {
            this.db = db;
        }
        // GET: api/Tree
        [HttpGet]
        public IEnumerable<string> Get()
        {
            return new string[] { "value1", "value2" };
        }

        // GET: api/Tree/5
        //[HttpGet("{id}", Name = "Get")]
        //public Persone Get(int id)
        //{

        //    var GetPersone = db.persones.Include("Chlidren").ToList();
        //    var Root = GetPersone.FirstOrDefault(x => x.Id == 3);
        //    //return db.persones.Include(x => x.Chlidren).FirstOrDefault();
        //    return Root;
        //}

        

        // POST: api/Tree
        [HttpPost]
        public void Post([FromBody] string value)
        {
        }

        // PUT: api/Tree/5
        [HttpPut("{id}")]
        public void Put(int id, [FromBody] string value)
        {
        }

        // DELETE: api/ApiWithActions/5
        [HttpDelete("{id}")]
        public void Delete(int id)
        {
        }
    }
}
