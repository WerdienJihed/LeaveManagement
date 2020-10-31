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
		public ActionResult Index()
		{
			var leaveTypes = _leaveTypeRepo.FindAll().ToList();
			var mappedLeaveTypes = _mapper.Map<List<LeaveType>, List<LeaveTypeViewModel>>(leaveTypes);
			var model = new CreateLeaveAllocationViewModel
			{
				LeaveTypeViewModels = mappedLeaveTypes,
				NumberUpdated = 0
			};
			return View(model);
		}

		public ActionResult SetLeave(int id)
		{
			var leaveType = _leaveTypeRepo.FindById(id);
			var employees = _userManager.GetUsersInRoleAsync("Employee").Result;
			foreach (var emplee in employees)
			{
				if (_leaveAllocationRepo.CheckAllocation(leaveType.Id, emplee.Id))
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
				_leaveAllocationRepo.Create(leaveAllocation);
			}
			return RedirectToAction(nameof(Index));
		}

		public ActionResult ListEmployees()
		{
			var employees = _userManager.GetUsersInRoleAsync("Employee").Result;
			var model = _mapper.Map<List<EmployeeViewModel>>(employees);
			return View(model);
		}

		// GET: LeaveAllocationController/Details/5
		public ActionResult Details(string id)
		{
			var employee = _userManager.FindByIdAsync(id).Result;
			var leaveAllocations = _leaveAllocationRepo.GetLeaveAllocationByEmployee(id);
			
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
		public ActionResult Edit(int id)
		{
			var leaveAllocation = _leaveAllocationRepo.FindById(id);
			var model = _mapper.Map<EditLeaveAllocationViewModel>(leaveAllocation);
			return View(model);
		}

		// POST: LeaveAllocationController/Edit/5
		[HttpPost]
		[ValidateAntiForgeryToken]
		public ActionResult Edit(EditLeaveAllocationViewModel model)
		{
			try
			{
				if (!ModelState.IsValid)
				{
					return View(model);
				}

				var record = _leaveAllocationRepo.FindById(model.Id);
				record.NumberOfDays = model.NumberOfDays;

				var isSuccess =_leaveAllocationRepo.Update(record);
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
