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
    public class ChaptersController : ControllerBase
    {
        private readonly MchneContext _context;

        public ChaptersController(MchneContext context)
        {
            _context = context;
        }

        [HttpGet]
        public List<Chapter> GetAll()
        {
            return _context.Chapters.ToList();
        }

        [HttpGet("{id}", Name = "GetChapter")]
        public IActionResult GetById(long id)
        {
            var item = _context.Chapters.Find(id);
            if (item == null)
            {
                return NotFound();
            }
            return Ok(item);
        }        

        [HttpPost]
        public IActionResult Create([FromBody] Chapter item)
        {
            if (item == null)
            {
                return BadRequest();
            }

            _context.Chapters.Add(item);
            _context.SaveChanges();

            return CreatedAtRoute("GetNovel", new { id = item.ChapterID }, item);
        }

        [HttpPut("{id}")]
        public IActionResult Update(long id, [FromBody] Chapter item)
        {
            if (item == null || item.ChapterID != id)
            {
                return BadRequest();
            }

            var todo = _context.Chapters.Find(id);
            if (todo == null)
            {
                return NotFound();
            }

            todo.ContentUrl = item.ContentUrl;
            todo.ChapterNumber = item.ChapterNumber;

            _context.Chapters.Update(todo);
            _context.SaveChanges();
            return NoContent();
        }

        [HttpDelete("{id}")]
        public IActionResult Delete(long id)
        {
            var todo = _context.Chapters.Find(id);
            if (todo == null)
            {
                return NotFound();
            }

            _context.Chapters.Remove(todo);
            _context.SaveChanges();
            return NoContent();
        }
    }
}