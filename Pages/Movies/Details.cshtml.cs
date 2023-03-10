﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using SecurityForAssessmentStudent.Data;
using SecurityForAssessmentStudent.Model;

namespace SecurityForAssessmentStudent.Pages.Movies
{
    public class DetailsModel : PageModel
    {
        private readonly SecurityForAssessmentStudent.Data.ApplicationDbContext _context;

        public DetailsModel(SecurityForAssessmentStudent.Data.ApplicationDbContext context)
        {
            _context = context;
        }

      public Movie Movie { get; set; }

        public async Task<IActionResult> OnGetAsync(Guid? id)
        {
            if (id == null || _context.Movie == null)
            {
                return NotFound();
            }

            var movie = await _context.Movie.FirstOrDefaultAsync(m => m.Id == id);
            if (movie == null)
            {
                return NotFound();
            }
            else 
            {
                Movie = movie;
            }
            return Page();
        }
    }
}
