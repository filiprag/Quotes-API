using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;
using QuotesAPI.Data;
using QuotesAPI.Models;
using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace QuotesAPI.Controllers
{
    [Route("api/[controller]")]
    [Authorize]
    public class QuotesController : Controller
    {
        private QuotesDbContext _quotesDbContext;

        public QuotesController(QuotesDbContext quotesDbContext)
        {
            _quotesDbContext = quotesDbContext;
        }

        // GET: api/values
        [HttpGet]
        [ResponseCache(Duration = 60, Location = ResponseCacheLocation.Client)] // Lägger till ResponseCache för förfrågan, kod i startup behövs, med Duration = 60 sekunder, cache:n sparas på klienten & inte proxy serven med client och på båda med any, proxy och client
        [AllowAnonymous] // Ger tillgång till vem som helst att använda, Authoize bortfaller 
        public IActionResult Get(string sort)
        {
            IQueryable<Quote> quotes;
            switch (sort)
            {

                case "desc":
                    quotes = _quotesDbContext.Quotes.OrderByDescending(q => q.CreatedAt);
                    break;
                case "asc":
                    quotes = _quotesDbContext.Quotes.OrderBy(q => q.CreatedAt);
                    break;
                default:
                    quotes = _quotesDbContext.Quotes;
                    break;
            }
  
            return Ok(quotes);
        }


        [HttpGet("[action]")]
        public IActionResult MyQuote()
        {
            string userId = User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier).Value;
            var myQuotes = _quotesDbContext.Quotes.Where(q => q.UserId == userId);
            return Ok(myQuotes);
        }

        // GET api/values/5
        [HttpGet("{id}")]
        public IActionResult Get(int id)
        {
            var quote = _quotesDbContext.Quotes.Find(id);
            if (quote == null)
            {
                return NotFound("No record found against this Id: " + id);
            }
            else
            {
                return Ok(quote);
            }
        }


        [HttpGet("[action]")]
        public IActionResult PagingQuote(int? pageNumber, int? pageSize)
        {

            var quotes = _quotesDbContext.Quotes;
            var currentPageNumber = pageNumber ?? 1;
            var currentPageSize = pageSize ?? 5;


            return Ok(quotes.Skip((currentPageNumber - 1)* currentPageSize).Take(currentPageSize));
        }


        [HttpGet("[action]")]
        public IActionResult SearchQuote(string type)
        {

            //var sQuotes = _quotesDbContext.Quotes.Where(q => q.Type.StartsWith(type)); //Söker efter något som startar med innehållet, typiskt sökruta dropdown material, funkar ej på ASP. net 5.0?
            var sQuotes = _quotesDbContext.Quotes.Where(q => q.Type.Contains(type)); // Söker efter det som användaren matar in och innehåller det, inte bara startar

            if (sQuotes == null)
            {
                return NotFound("Hittades ej, är tom");
            }
            return Ok(sQuotes);
        }


        // POST api/values
        [HttpPost]
        public IActionResult Post([FromBody] Quote quote)
        {

            string userId = User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier).Value; // sparar valuen för användaren, vilket är id och sprar det i databasen under userID
            quote.UserId = userId;

            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            else
            {
                _quotesDbContext.Quotes.Add(quote);
                _quotesDbContext.SaveChanges();
                return StatusCode(StatusCodes.Status201Created);

            }

           
        }

        // PUT api/values/5
        [HttpPut("{id}")]
        public IActionResult Put(int id, [FromBody] Quote quote)
        {
            string userId = User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier).Value;
            quote.UserId = userId;

            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

                var uQuote = _quotesDbContext.Quotes.Find(id);
            if (uQuote == null)
            {
                return NotFound("No record found against this Id: " + id);
            }
            if (userId != uQuote.UserId)
            {
                return BadRequest("Sorry, you cant update this record, wrong user!!");
            }
            else
            {
                uQuote.Title = quote.Title;
                uQuote.Author = quote.Author;
                uQuote.Description = quote.Description;
                uQuote.Type = quote.Type;
                uQuote.CreatedAt = quote.CreatedAt;

                _quotesDbContext.SaveChanges();

                return Ok("Record Updated sucessfully!");
            }
            
        }

        // DELETE api/values/5
        [HttpDelete("{id}")]
        public IActionResult Delete(int id)
        {
            string userId = User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier).Value;

            var quote = _quotesDbContext.Quotes.Find(id);
            if (quote == null)
            {
                return NotFound("No record found against this Id: " + id);
            }

            if (userId != quote.UserId)
            {
                return BadRequest("Sorry, you cant delete this record, wrong user!!");
            }
            else
            {
                _quotesDbContext.Quotes.Remove(quote);
                _quotesDbContext.SaveChanges();
                return Ok("Record deleted sucessfully!");
            }

        }
    }
}
