﻿using CapstoneWine.Data;
using CapstoneWine.Infrastructure;
using CapstoneWine.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SendGrid;
using SendGrid.Helpers.Mail;
using System.Diagnostics;
using CapstoneWine.Models.ViewModels;
using Microsoft.Identity.Client;
using Microsoft.AspNetCore.Identity.UI.Services;
using CapstoneWine.Areas.Services;

namespace CapstoneWine.Controllers
{
	public class HomeController : Controller
	{
		private readonly ApplicationDbContext _context;
		private readonly IEmailSender _emailSender;
		public HomeController(ApplicationDbContext context, IEmailSender emailSender)
		{
			_context = context;
			_emailSender = emailSender;
		}
		public IActionResult Index()
		{
			var allWine = _context.Wines.ToList();
			var categories = allWine.Select(w => w.Category).Distinct().ToArray();
			ViewData["Categories"] = categories;

			if (!Request.Cookies.ContainsKey("AgeVerified"))
			{
				return RedirectToAction("VerifyAge");
			}
			return View();
		}

		public IActionResult VerifyAge()
		{
			return View();
		}
		/*AgeCheck*/
		[HttpPost]
		public IActionResult VerifyAge(DateTime dateOfBirth)
		{
			DateTime currentDate = DateTime.Now;
			int age = currentDate.Year - dateOfBirth.Year;

			//check if user is above min age
			int minimumAge = 18;
			if (age >= minimumAge)
			{
				HttpContext.Response.Cookies.Append("AgeVerified", "true");
				return RedirectToAction("Index", "Home");
			}
			else
			{
				return Redirect("https://www.alcohol.org.nz");
			}
		}

		public IActionResult Options()
		{
			return View();
		}


		public IActionResult Privacy()
		{
			return View();
		}

		public IActionResult Help()
		{
			return View();
		}

		public IActionResult TermsAndConditions()
		{
			return View();
		}

		public IActionResult LiquorLicense()
		{
			return View();
		}

		[HttpGet]
		public IActionResult About()
		{
			return View();
		}
		//Gets the values from the about form
		[HttpPost]
		public async Task<IActionResult> AboutAsync(string AboutEmail, string AboutName)
		{

			await _emailSender.SendEmailAsync(email: AboutEmail, $"Thank You for the feedback {AboutName}", htmlMessage: $"We will contact you for anything further");


			return RedirectToAction("About");
		}

		public IActionResult Home()
		{
			return View();
		}
		[Authorize]
		public async Task<IActionResult> Subscription()
		{
			// Return an error message if the Wine entity set is null
			if (_context.Subscriptions == null)
			{
				return Problem("Entity set 'ApplicationDbContext.Wines' is null.");
			}
			// Otherwise, return the Wines entity set as a view
			return View(await _context.Subscriptions.ToListAsync());
		}//View for Subscription
		public async Task<IActionResult> Shop()
		{
			// Return an error message if the Wine entity set is null
			if (_context.Wines == null)
			{
				return Problem("Entity set 'ApplicationDbContext.Wines' is null.");
			}
			// Otherwise, return the Wines entity set as a view
			return View(await _context.Wines.ToListAsync());
		}
		public IActionResult SubCart()
		{
			List<SubItem> cart = HttpContext.Session.GetJson<List<SubItem>>("Sub") ?? new List<SubItem>();

			SubViewModel cartVM = new()
			{
				SubItems = cart,

			};

			return View(cartVM);
		}//View for SubCart
		public async Task<IActionResult> Add(int id)
		{
			SubscriptionsModel subscriptions = await _context.Subscriptions.FindAsync(id);

			List<SubItem> cart = HttpContext.Session.GetJson<List<SubItem>>("Sub") ?? new List<SubItem>();

			SubItem cartItem = cart.Where(c => c.ProductId == id).FirstOrDefault();

			if (cartItem == null)
			{
				cart.Add(new SubItem(subscriptions));
			}


			HttpContext.Session.SetJson("Sub", cart);

			TempData["Success"] = "The product has been added!";

			return RedirectToAction("SubCart");
		}//To add the subscription to the subcart
		public async Task<IActionResult> Red(int id)
		{
			SubscriptionsModel subscriptions = await _context.Subscriptions.FindAsync(id);

			List<SubItem> cart = HttpContext.Session.GetJson<List<SubItem>>("Sub") ?? new List<SubItem>();

			SubItem cartItem = cart.Where(c => c.ProductId == id).FirstOrDefault();

			if (cartItem == null)
			{
				cart.Add(new SubItem(subscriptions));
			}
			else
			{
				cartItem.Type = "Red";
			}

			HttpContext.Session.SetJson("Sub", cart);

			TempData["Success"] = "The product has been added!";

			return Redirect(Request.Headers["Referer"].ToString());
		}//Changes SubItem.type to Red	
		public async Task<IActionResult> White(int id)
		{
			SubscriptionsModel subscriptions = await _context.Subscriptions.FindAsync(id);

			List<SubItem> cart = HttpContext.Session.GetJson<List<SubItem>>("Sub") ?? new List<SubItem>();

			SubItem cartItem = cart.Where(c => c.ProductId == id).FirstOrDefault();

			if (cartItem == null)
			{
				cart.Add(new SubItem(subscriptions));
			}
			else
			{
				cartItem.Type = "White";
			}

			HttpContext.Session.SetJson("Sub", cart);

			TempData["Success"] = "The product has been added!";

			return Redirect(Request.Headers["Referer"].ToString());
		}//Changes SubItem.type to White
		public async Task<IActionResult> Mixed(int id)
		{
			SubscriptionsModel subscriptions = await _context.Subscriptions.FindAsync(id);

			List<SubItem> cart = HttpContext.Session.GetJson<List<SubItem>>("Sub") ?? new List<SubItem>();

			SubItem cartItem = cart.Where(c => c.ProductId == id).FirstOrDefault();

			if (cartItem == null)
			{
				cart.Add(new SubItem(subscriptions));
			}
			else
			{
				cartItem.Type = "Mixed";
			}

			HttpContext.Session.SetJson("Sub", cart);

			TempData["Success"] = "The product has been added!";

			return Redirect(Request.Headers["Referer"].ToString());
		}//Changes SubItem.type to Mixed
		public async Task<IActionResult> Bottles3(int id)
		{
			SubscriptionsModel subscriptions = await _context.Subscriptions.FindAsync(id);

			List<SubItem> cart = HttpContext.Session.GetJson<List<SubItem>>("Sub") ?? new List<SubItem>();

			SubItem cartItem = cart.Where(c => c.ProductId == id).FirstOrDefault();

			if (cartItem == null)
			{
				cart.Add(new SubItem(subscriptions));
			}
			else
			{
				cartItem.NumOfBottles = 3;
			}

			HttpContext.Session.SetJson("Sub", cart);

			TempData["Success"] = "The product has been added!";

			return Redirect(Request.Headers["Referer"].ToString());
		}//Chagnes SubItem.NumOfBottles to 3
		public async Task<IActionResult> Bottles6(int id)
		{
			SubscriptionsModel subscriptions = await _context.Subscriptions.FindAsync(id);

			List<SubItem> cart = HttpContext.Session.GetJson<List<SubItem>>("Sub") ?? new List<SubItem>();

			SubItem cartItem = cart.Where(c => c.ProductId == id).FirstOrDefault();

			if (cartItem == null)
			{
				cart.Add(new SubItem(subscriptions));
			}
			else
			{
				cartItem.NumOfBottles = 6;
			}

			HttpContext.Session.SetJson("Sub", cart);

			TempData["Success"] = "The product has been added!";

			return Redirect(Request.Headers["Referer"].ToString());
		}//Chagnes SubItem.NumOfBottles to 6
		public async Task<IActionResult> Bottles12(int id)
		{
			SubscriptionsModel subscriptions = await _context.Subscriptions.FindAsync(id);

			List<SubItem> cart = HttpContext.Session.GetJson<List<SubItem>>("Sub") ?? new List<SubItem>();

			SubItem cartItem = cart.Where(c => c.ProductId == id).FirstOrDefault();

			if (cartItem == null)
			{
				cart.Add(new SubItem(subscriptions));
			}
			else
			{
				cartItem.NumOfBottles = 12;
			}

			HttpContext.Session.SetJson("Sub", cart);

			TempData["Success"] = "The product has been added!";

			return Redirect(Request.Headers["Referer"].ToString());
		}//Changes SubItem.NumOfBottles to 12
		public async Task<IActionResult> Freq3(int id)
		{
			SubscriptionsModel subscriptions = await _context.Subscriptions.FindAsync(id);

			List<SubItem> cart = HttpContext.Session.GetJson<List<SubItem>>("Sub") ?? new List<SubItem>();

			SubItem cartItem = cart.Where(c => c.ProductId == id).FirstOrDefault();

			if (cartItem == null)
			{
				cart.Add(new SubItem(subscriptions));
			}
			else
			{
				cartItem.Frequency = 3;
			}

			HttpContext.Session.SetJson("Sub", cart);

			TempData["Success"] = "The product has been added!";

			return Redirect(Request.Headers["Referer"].ToString());
		}//Changes SubItem.Frequency to 3
		public async Task<IActionResult> Freq6(int id)
		{
			SubscriptionsModel subscriptions = await _context.Subscriptions.FindAsync(id);

			List<SubItem> cart = HttpContext.Session.GetJson<List<SubItem>>("Sub") ?? new List<SubItem>();

			SubItem cartItem = cart.Where(c => c.ProductId == id).FirstOrDefault();

			if (cartItem == null)
			{
				cart.Add(new SubItem(subscriptions));
			}
			else
			{
				cartItem.Frequency = 6;
			}

			HttpContext.Session.SetJson("Sub", cart);

			TempData["Success"] = "The product has been added!";

			return Redirect(Request.Headers["Referer"].ToString());
		}//Chagnes SubItem.Frequency to 6
		public async Task<IActionResult> Freq12(int id)
		{
			SubscriptionsModel subscriptions = await _context.Subscriptions.FindAsync(id);

			List<SubItem> cart = HttpContext.Session.GetJson<List<SubItem>>("Sub") ?? new List<SubItem>();

			SubItem cartItem = cart.Where(c => c.ProductId == id).FirstOrDefault();

			if (cartItem == null)
			{
				cart.Add(new SubItem(subscriptions));
			}
			else
			{
				cartItem.Frequency = 12;
			}

			HttpContext.Session.SetJson("Sub", cart);

			TempData["Success"] = "The product has been added!";

			return Redirect(Request.Headers["Referer"].ToString());
		}//Changes SubItem.Frequency to 12
		public IActionResult Clear()
		{
			HttpContext.Session.Remove("Sub");

			return RedirectToAction("Subscription");
		}
		public string Email { get; set; }
		[HttpGet]
		public IActionResult Checkout()
		{
			List<SubItem> cart = HttpContext.Session.GetJson<List<SubItem>>("Sub") ?? new List<SubItem>();

			SubViewModel cartVM = new()
			{
				SubItems = cart,
				GrandTotal = cart.Sum(x => x.Total + x.Shipping)//creates variable GrandTotal = Total (from CartViewModel) * Frequency (from SubModel)
			};

			return View(cartVM);
		}//View for Checkout

		//Gets the values from the Checkout form
		[HttpPost]
		public async Task<IActionResult> CheckoutAsync(string ShippingEmail)
		{
			List<SubItem> cart = HttpContext.Session.GetJson<List<SubItem>>("Sub") ?? new List<SubItem>();

			SubViewModel cartVM = new()
			{
				SubItems = cart,
				GrandTotal = cart.Sum(x => x.Total + x.Shipping),
				NumOfItems = cart.Count.ToString()
			};
			Email = ShippingEmail;
			await _emailSender.SendEmailAsync(Email, "Order Complete", htmlMessage: "For " + cartVM.NumOfItems.ToString() + " Subscriptions, You have been charged: " + cartVM.GrandTotal.ToString("C2"));
			return Redirect(Request.Headers["Referer"].ToString());
		}
		public async Task<IActionResult> Remove(int id)
		{
			List<SubItem> cart = HttpContext.Session.GetJson<List<SubItem>>("Sub");

			cart.RemoveAll(p => p.ProductId == id);

			if (cart.Count == 0)
			{
				HttpContext.Session.Remove("Sub");
			}
			else
			{
				HttpContext.Session.SetJson("Sub", cart);
			}

			TempData["Success"] = "The product has been removed!";

			return RedirectToAction("SubCart");
		}//Button to remove item from subcart


		[ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
		public IActionResult Error()
		{
			return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
		}
	}
}