using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TennesseeStockings.Models;
using TennesseeStockings;

public class StockingsController : Controller
{
    private DataContext _context;

    public StockingsController(DataContext context)
    {
        _context = context;
    }

    public async Task<IActionResult> Index(string searchStream)
    {
        ViewData["CurrentFilter"] = searchStream;

        //var stockings = await _context.Stockings.ToListAsync();
        var stockings = from s in _context.Stockings select s;

        if(!String.IsNullOrEmpty(searchStream))
        {
            stockings=stockings.Where(s => s.Stream.Contains(searchStream));            
        }       

        return View(await stockings.AsNoTracking().ToListAsync());
    }

    [HttpGet]
    public IActionResult Create()
    {
        return View();
    }

    [HttpPost]
    public async Task<IActionResult> Create(Stock stocks)
    {
        // validate that our model meets the requirement
        if (ModelState.IsValid)
        {
            try
            {
                // update the ef core context in memory 
                _context.Add(stocks);

                // sync the changes of ef code in memory with the database
                await _context.SaveChangesAsync();

                return RedirectToAction("Index");
            }
            catch(Exception ex)
            {
                ModelState.AddModelError(string.Empty, $"Something went wrong {ex.Message}");
            }
        }

        ModelState.AddModelError(string.Empty, $"Something went wrong, invalid model");

        // We return the object back to view
        return View(stocks);
    }

    [HttpGet]
    public async Task<IActionResult> Edit(int id)
    {
        var exist = await _context.Stockings.Where(x => x.Id == id).FirstOrDefaultAsync();

        return View(exist);
    }

    [HttpPost]
    public async Task<IActionResult> Edit(Stock stocks)
    {
        // validate that our model meets the requirement
        if (ModelState.IsValid)
        {
            try
            {
                // Check if the stocking exist based on the id
                var exist = _context.Stockings.Where(x => x.Id == stocks.Id).FirstOrDefault();

                // if the stocking is not null we update the information
                if(exist != null)
                {
                    exist.Stream = stocks.Stream;
                    exist.Stocking = stocks.Stocking;
                    
                    // we save the changes into the db
                    await _context.SaveChangesAsync();

                    return RedirectToAction("Index");
                }
            }
            catch(Exception ex)
            {
                ModelState.AddModelError(string.Empty, $"Something went wrong {ex.Message}");
            }
        }

        ModelState.AddModelError(string.Empty, $"Something went wrong, invalid model");

        return View(stocks);
    }

    [HttpGet]
    public async Task<IActionResult> Delete(int id)
    {
        var exist = await _context.Stockings.Where(x => x.Id == id).FirstOrDefaultAsync();

        return View(exist);
    }

    [HttpPost]
    public async Task<IActionResult> Delete(Stock stocks)
    {
        if (ModelState.IsValid)
        {
            try
            {
                var exist = _context.Stockings.Where(x => x.Id == stocks.Id).FirstOrDefault();

                if(exist != null)
                {
                    _context.Remove(exist);
                    await _context.SaveChangesAsync();

                    return RedirectToAction("Index");
                } 
            }
            catch(Exception ex)
            {
                ModelState.AddModelError(string.Empty, $"Something went wrong {ex.Message}");
            }
        }

        ModelState.AddModelError(string.Empty, $"Something went wrong, invalid model");

        return View();
    }
}