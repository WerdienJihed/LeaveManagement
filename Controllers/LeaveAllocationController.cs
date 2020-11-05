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
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace Leave_management.Controllers
{
	[Authorize(Roles = "Administrator")]
	public class LeaveAllocationController : Controller
	{
		private readonly ILeaveTypeRepository _leaveTypeRepo;
		private readonly ILeaveAllocationRepository _leaveAllocationRepo;
		private readonly IMapper _mapper;
		private readonly UserManager<Employee> _userManager;

		public LeaveAllocationController(
			ILeaveTypeRepository leaveTypeRepo, 
			ILeaveAllocationRepository leaveAllocationRepository, 
			IMapper mapper,
			UserManager<Employee> userManager)
		{
			_leaveTypeRepo = leaveTypeRepo;
			_leaveAllocationRepo = leaveAllocationRepository;
			_mapper = mapper;
			_userManager = userManager;
		}

		// GET: LeaveAllocationController
		public async Task<ActionResult> Index()
		{
			var leaveTypes = await _leaveTypeRepo.FindAll();
			var mappedLeaveTypes = _mapper.Map<List<LeaveType>, List<LeaveTypeViewModel>>(leaveTypes.ToList());
			var model = new CreateLeaveAllocationViewModel
			{
				LeaveTypeViewModels = mappedLeaveTypes,
				NumberUpdated = 0
			};
			return View(model);
		}

		public async Task<ActionResult> SetLeave(int id)
		{
			var leaveType = await _leaveTypeRepo.FindById(id);
			var employees = await _userManager.GetUsersInRoleAsync("Employee");
			foreach (var emplee in employees)
			{
				bool isExists = await _leaveAllocationRepo.CheckAllocation(leaveType.Id, emplee.Id);
				if (isExists)
					continue;
					
				var allocation = new LeaveAllocationViewModel
					{
						DateCreated = DateTime.Now,
						EmployeeId = emplee.Id,
						LeaveTypeId = id,
						NumberOfDays = leaveType.DefaultDays,
						Period = DateTime.Now.Year
					};
				var leaveAllocation = _mapper.Map<LeaveAllocation>(allocation);
				await _leaveAllocationRepo.Create(leaveAllocation);
			}
			return RedirectToAction(nameof(Index));
		}

		public async Task<ActionResult> ListEmployees()
		{
			var employees = await _userManager.GetUsersInRoleAsync("Employee");
			var model = _mapper.Map<List<EmployeeViewModel>>(employees);
			return View(model);
		}

		// GET: LeaveAllocationController/Details/5
		public async Task<ActionResult> Details(string id)
		{
			var employee = await _userManager.FindByIdAsync(id);
			var leaveAllocations = await _leaveAllocationRepo.GetLeaveAllocationByEmployee(id);
			
			var mappedEmployee = _mapper.Map<EmployeeViewModel>(employee);
			var mappedLeaveAllocations = _mapper.Map<List<LeaveAllocationViewModel>>(leaveAllocations);

			var model = new ViewAllocationViewModel
			{
				Employee = mappedEmployee,
				leaveAllocations = mappedLeaveAllocations
			};
			return View(model);
		}

		// GET: LeaveAllocationController/Create
		public ActionResult Create()
		{
			return View();
		}

		// POST: LeaveAllocationController/Create
		[HttpPost]
		[ValidateAntiForgeryToken]
		public ActionResult Create(IFormCollection collection)
		{
			try
			{
				return RedirectToAction(nameof(Index));
			}
			catch
			{
				return View();
			}
		}

		// GET: LeaveAllocationController/Edit/5
		public async Task<ActionResult> Edit(int id)
		{
			var leaveAllocation = await _leaveAllocationRepo.FindById(id);
			var model = _mapper.Map<EditLeaveAllocationViewModel>(leaveAllocation);
			return View(model);
		}

		// POST: LeaveAllocationController/Edit/5
		[HttpPost]
		[ValidateAntiForgeryToken]
		public async Task<ActionResult> Edit(EditLeaveAllocationViewModel model)
		{
			try
			{
				if (!ModelState.IsValid)
				{
					return View(model);
				}

				var record = await _leaveAllocationRepo.FindById(model.Id);
				record.NumberOfDays = model.NumberOfDays;

				var isSuccess =await _leaveAllocationRepo.Update(record);
				if (!isSuccess)
				{
					ModelState.AddModelError("", "Error while saving");
					return View(model);
				}
				return RedirectToAction(nameof(Details),new {id = model.EmployeeId});
			}
			catch
			{
				return View(model);
			}
		}

		// GET: LeaveAllocationController/Delete/5
		public ActionResult Delete(int id)
		{
			return View();
		}

		// POST: LeaveAllocationController/Delete/5
		[HttpPost]
		[ValidateAntiForgeryToken]
		public ActionResult Delete(int id, IFormCollection collection)
		{
			try
			{
				return RedirectToAction(nameof(Index));
			}
			catch
			{
				return View();
			}
		}
	}
}
