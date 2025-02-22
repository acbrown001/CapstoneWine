﻿using CapstoneWine.Data;
using CapstoneWine.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace CapstoneWine.Controllers
{
	public class AdminController : Controller
	{
		private readonly ApplicationDbContext _context;

		public AdminController(ApplicationDbContext context)
		{
			_context = context;
		}

		// GET: Admin
		public async Task<IActionResult> Index()
		{
			var subscriptions = await _context.Subscriptions.ToListAsync();
			var wines = await _context.Wines.ToListAsync();
			var orders = await _context.Orders
				.Include(o => o.subscription)
				.Include(o => o.wine)
				.ToListAsync();

			var model = new Tuple<List<SubscriptionsModel>, List<WinesModel>, List<OrdersModel>>(subscriptions, wines, orders);
			return View(model);
		}
	
	}
}
