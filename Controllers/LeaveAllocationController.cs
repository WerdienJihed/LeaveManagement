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
		private readonly IUnitOfWork _repo;
		private readonly IMapper _mapper;
		private readonly UserManager<Employee> _userManager;

		public LeaveAllocationController(IUnitOfWork repo,IMapper mapper, UserManager<Employee> userManager)
		{
			_repo = repo;
			_mapper = mapper;
			_userManager = userManager;
		}

		public async Task<ActionResult> Index()
		{
			var leaveTypes = await _repo.LeaveTypes.FindAll();
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
			var leaveType = await _repo.LeaveTypes.Find(q=>q.Id == id);
			var employees = await _userManager.GetUsersInRoleAsync("Employee");
			foreach (var emplee in employees)
			{
				bool isExists = await _repo.LeaveAllocations.IsExists
					(
						q=> q.EmployeeId == emplee.Id 
						&& q.LeaveTypeId == leaveType.Id 
						&& q.Period == DateTime.Now.Year
					);
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
				await _repo.LeaveAllocations.Create(leaveAllocation);
				await _repo.Save();
			}
			return RedirectToAction(nameof(Index));
		}

		public async Task<ActionResult> ListEmployees()
		{
			var employees = await _userManager.GetUsersInRoleAsync("Employee");
			var model = _mapper.Map<List<EmployeeViewModel>>(employees);
			return View(model);
		}

		public async Task<ActionResult> Details(string id)
		{
			var employee = await _userManager.FindByIdAsync(id);
			var leaveAllocations = await _repo.LeaveAllocations.FindAll
				(
					expression : q=>q.EmployeeId == id
					&& q.Period == DateTime.Now.Year,
					includes : new List<string> {"LeaveType"}
				);
			
			var mappedEmployee = _mapper.Map<EmployeeViewModel>(employee);
			var mappedLeaveAllocations = _mapper.Map<List<LeaveAllocationViewModel>>(leaveAllocations);

			var model = new ViewAllocationViewModel
			{
				Employee = mappedEmployee,
				leaveAllocations = mappedLeaveAllocations
			};
			return View(model);
		}

		public ActionResult Create()
		{
			return View();
		}

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

		public async Task<ActionResult> Edit(int id)
		{
			var leaveAllocation = await _repo.LeaveAllocations.Find
				(
					q=>q.Id == id,
					includes : new List<string> {"Employee","LeaveType"}
				);
			var model = _mapper.Map<EditLeaveAllocationViewModel>(leaveAllocation);
			return View(model);
		}

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

				var record = await _repo.LeaveAllocations.Find(q=>q.Id == model.Id);
				record.NumberOfDays = model.NumberOfDays;
				_repo.LeaveAllocations.Update(record);
				await _repo.Save();

				return RedirectToAction(nameof(Details),new {id = model.EmployeeId});
			}
			catch
			{
				return View(model);
			}
		}

		public ActionResult Delete(int id)
		{
			return View();
		}

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

		protected override void Dispose(bool disposing)
		{
			_repo.Dispose();
			base.Dispose(disposing);
		}
	}
}
