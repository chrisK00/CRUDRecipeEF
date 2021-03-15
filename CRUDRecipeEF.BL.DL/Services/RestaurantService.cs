﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AutoMapper;
using CRUDRecipeEF.BL.DL.Data;
using CRUDRecipeEF.BL.DL.Entities;
using Microsoft.EntityFrameworkCore;

namespace CRUDRecipeEF.BL.DL.Services
{
    public class RestaurantService : IRestaurantService
    {
        private readonly RecipeContext _context;
        private readonly IMapper _mapper;

        public RestaurantService(RecipeContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }

        /// <summary>
        /// Commits any changes to the db that are tracked by EF
        /// </summary>
        /// <returns></returns>
        private async Task Save()
        {
            await _context.SaveChangesAsync();
        }

        /// <summary>
        /// Checks if a restaurant with the specified name already exists
        /// </summary>
        /// <param name="name"></param>
        /// <returns>If the restaurant exists or not</returns>
        private async Task<bool> RestaurantExists(string name) =>
            await _context.Restaurants.AnyAsync(r => r.Name.ToLower() == name.ToLower().Trim());

        /// <summary>
        /// Finds a restaurant with the specified name. Throws an exception if it doesnt exist
        /// </summary>
        /// <param name="name"></param>
        /// <returns>A recipe if found</returns>
        /// <exception cref="KeyNotFoundException"></exception>
        private async Task<Restaurant> GetRestaurantByNameIfExists(string name)
        {
            var restaurant = await _context.Restaurants.Include(m => m.Menus)
                .FirstOrDefaultAsync(r => r.Name.ToLower() == name.ToLower().Trim());
            return restaurant ?? throw new KeyNotFoundException("Restaurant doesnt exist");
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="name"></param>
        /// <returns>Name of the restaurant</returns>
        /// <exception cref="KeyNotFoundException"></exception>
        public async Task<string> AddMenuToRestaurant(Menu menuAdd, string restaurantName)
        {
            var restaurant = await GetRestaurantByNameIfExists(restaurantName);
            var menu = await _context.Menus
                .FirstOrDefaultAsync(m => m.Name.ToLower() == menuAdd.Name.ToLower().Trim());

            if (menu == null)
            {
                restaurant.Menus.Add(menuAdd);
            }
            else
            {
                restaurant.Menus.Add(menu);
            }

            await Save();
            return restaurantName;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="restaurant"></param>
        /// <returns>name of the restaurant</returns>
        ///  /// <exception cref="ArgumentException"></exception>
        public async Task<string> AddRestaurant(Restaurant restaurant)
        {
            if (await RestaurantExists(restaurant.Name))
            {
                throw new ArgumentException("Restaurant exists");
            }
            await _context.AddAsync(restaurant);
            await Save();

            return restaurant.Name;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="menuName"></param>
        /// <param name="restaurantName"></param>
        /// <returns></returns>
        /// <exception cref="KeyNotFoundException"></exception>
        public async Task RemoveMenuFromRestaurant(string menuName, string restaurantName)
        {
            var restaurant = await GetRestaurantByNameIfExists(restaurantName);
            var menu = restaurant.Menus
                .FirstOrDefault(m => m.Name.ToLower() == menuName.ToLower().Trim());

            if (menu == null)
            {
                throw new KeyNotFoundException("Restaurant doesnt exist");
            }

            restaurant.Menus.Remove(menu);
            await Save();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        /// <exception cref="KeyNotFoundException"></exception>
        public async Task DeleteRestaurant(string name)
        {
            var restaurant = await GetRestaurantByNameIfExists(name);

            _context.Remove(restaurant);
            await Save();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        /// <exception cref="KeyNotFoundException"></exception>
        public async Task<Restaurant> GetRestaurantByName(string name) =>
            await GetRestaurantByNameIfExists(name);
    }
}