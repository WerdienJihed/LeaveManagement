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
		private readonly ILeaveTypeRepository _repo; 
		private readonly IMapper _mapper;
		public LeaveTypesController(ILeaveTypeRepository ropo , IMapper mapper)
		{
			_repo = ropo;
			_mapper = mapper;
		}

		public async Task<ActionResult> Index()
		{
			var leaveTypes =  await _repo.FindAll();
			var model = _mapper.Map<List<LeaveType>, List<LeaveTypeViewModel>>(leaveTypes.ToList());
			return View(model);
		}

		public async Task<ActionResult> Details(int id)
		{
			var isExists = await _repo.IsExists(id);
			if (isExists)
			{
				return NotFound(); 
			}
			var leaveType = await _repo.FindById(id);
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
				
				var isSucess = await _repo.Create(leaveType);
				if (!isSucess)
				{
					ModelState.AddModelError("", "Something went wrong ... ");
					return View(leaveTypeViewModel);
				}
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
			bool isExists = await _repo.IsExists(id);
			if (!isExists)
			{
				return NotFound();
			}
			var leaveType = await _repo.FindById(id);
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

				var isSucess = await _repo.Update(leaveType);
				if (!isSucess)
				{
					ModelState.AddModelError("", "Something went wrong ... ");
					return View(leaveTypeViewModel);
				}
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
			bool isExists = await _repo.IsExists(id);
			if (!isExists)
			{
				return NotFound();
			}
			var leaveType = await _repo.FindById(id);
			var isSuccess = await _repo.Delete(leaveType);
			if (!isSuccess)
			{
				return BadRequest();
			}
			return RedirectToAction(nameof(Index));
		}
	}
}
