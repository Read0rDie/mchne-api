using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using mchne_api.Data;
using mchne_api.Models;

namespace mchne_api.Controllers
{    
    [Route("api/[controller]")]
    public class NovelsController : ControllerBase
    {
        private readonly MchneContext _context;

        public NovelsController(MchneContext context)
        {
            _context = context;
        }

        [HttpGet]
        public IActionResult GetAll()
        {
            return new OkObjectResult(_context.Novels.ToList());
        }

        [HttpGet("{id}", Name = "GetNovel")]
        public IActionResult GetById(long id)
        {
            var item = _context.Novels.Find(id);
            if (item == null)
            {
                return NotFound();
            }
            return Ok(item);
        }        

        [HttpPost]
        public IActionResult Create([FromBody] Novel item)
        {
            if (item == null)
            {
                return BadRequest();
            }

            _context.Novels.Add(item);
            _context.SaveChanges();

            return CreatedAtRoute("GetNovel", new { id = item.NovelID }, item);
        }

        [HttpPut("{id}")]
        public IActionResult Update(long id, [FromBody] Novel item)
        {
            if (item == null || item.NovelID != id)
            {
                return BadRequest();
            }

            var todo = _context.Novels.Find(id);
            if (todo == null)
            {
                return NotFound();
            }

            todo.CoverImage = item.CoverImage;
            todo.Title = item.Title;

            _context.Novels.Update(todo);
            _context.SaveChanges();
            return NoContent();
        }

        [HttpDelete("{id}")]
        public IActionResult Delete(long id)
        {
            var todo = _context.Novels.Find(id);
            if (todo == null)
            {
                return NotFound();
            }

            _context.Novels.Remove(todo);
            _context.SaveChanges();
            return NoContent();
        }
    }
}