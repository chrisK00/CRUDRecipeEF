﻿using CRUDRecipeEF.BL.DTOs;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace CRUDRecipeEF.DAL.DTOs
{
    public class MenuAddDTO
    {
        public string RestaurantName { get; set; }
        [StringLength(200, MinimumLength = 3)]
        public string Name { get; set; }

        public List<RecipeAddDTO> Recipes { get; set; } = new();
    }
}