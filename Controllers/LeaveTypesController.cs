using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using Leave_management.Contracts;
using Leave_management.Data;
using Leave_management.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Leave_management.Controllers
{
	[Authorize( Roles = "Administrator")]
	public class LeaveTypesController : Controller
	{
		private readonly IUnitOfWork _repo; 
		private readonly IMapper _mapper;
		public LeaveTypesController(IUnitOfWork repo , IMapper mapper)
		{
			_repo = repo;
			_mapper = mapper;
		}

		public async Task<ActionResult> Index()
		{
			var leaveTypes =  await _repo.LeaveTypes.FindAll();
			var model = _mapper.Map<List<LeaveType>, List<LeaveTypeViewModel>>(leaveTypes.ToList());
			return View(model);
		}

		public async Task<ActionResult> Details(int id)
		{
			var isExists = await _repo.LeaveTypes.IsExists(q=> q.Id == id);
			if (isExists)
			{
				return NotFound(); 
			}
			var leaveType = await _repo.LeaveTypes.Find(q=>q.Id == id);
			var leaveTypeViewModel = _mapper.Map<LeaveTypeViewModel>(leaveType);
			return View(leaveTypeViewModel);
		}


		public ActionResult Create()
		{
			return View();
		}


		[HttpPost]
		[ValidateAntiForgeryToken]
		public async Task<ActionResult> Create(LeaveTypeViewModel leaveTypeViewModel)
		{
			try
			{
				if (!ModelState.IsValid)
				{
					return View();
				}

				var leaveType = _mapper.Map<LeaveType>(leaveTypeViewModel);
				leaveType.DateCreated = DateTime.Now;
				
				await _repo.LeaveTypes.Create(leaveType);
				await _repo.Save();
				return RedirectToAction(nameof(Index));
			}
			catch
			{
				ModelState.AddModelError("", "Something went wrong ... ");
				return View(leaveTypeViewModel);
			}
		}

		public async Task<ActionResult> Edit(int id)
		{
			bool isExists = await _repo.LeaveTypes.IsExists(q=>q.Id == id);
			if (!isExists)
			{
				return NotFound();
			}
			var leaveType = await _repo.LeaveTypes.Find(q => q.Id == id);
			var leaveTypeViewModel = _mapper.Map<LeaveTypeViewModel>(leaveType);

			return View(leaveTypeViewModel);
		}


		[HttpPost]
		[ValidateAntiForgeryToken]
		public async Task<ActionResult> Edit(LeaveTypeViewModel leaveTypeViewModel)
		{
			try
			{
				if (!ModelState.IsValid)
				{
					return View();
				}

				var leaveType = _mapper.Map<LeaveType>(leaveTypeViewModel);

				_repo.LeaveTypes.Update(leaveType);
				await _repo.Save();
				return RedirectToAction(nameof(Index));
			}
			catch
			{
				ModelState.AddModelError("", "Something went wrong ... ");
				return View(leaveTypeViewModel);
			}
		}

		public async Task<ActionResult> Delete(int id)
		{
			var leaveType = await _repo.LeaveTypes.Find(expression : q => q.Id == id);
			if (leaveType == null)
			{
				return NotFound();
			}
			_repo.LeaveTypes.Delete(leaveType);
			await _repo.Save();
			return RedirectToAction(nameof(Index));
		}
		[HttpPost]
		[ValidateAntiForgeryToken]
		public async Task<ActionResult> Delete(int id, LeaveTypeViewModel model)
		{
			try
			{
				var leaveType = await _repo.LeaveTypes.Find(expression: q => q.Id == id);
				if (leaveType == null)
				{
					return NotFound();
				}
				_repo.LeaveTypes.Delete(leaveType);
				await _repo.Save();
				return RedirectToAction(nameof(Index));
			}
			catch (Exception)
			{

				return View(model);
			}
		}
		protected override void Dispose(bool disposing)
		{
			_repo.Dispose();
			base.Dispose(disposing);
		}
	}
}
