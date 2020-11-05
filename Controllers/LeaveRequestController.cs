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
using Microsoft.AspNetCore.Mvc.Rendering;

namespace Leave_management.Controllers
{
	[Authorize]
	public class LeaveRequestController : Controller
	{
		private readonly ILeaveRequestRepository _leaveRequestRepository;
		private readonly ILeaveTypeRepository  _leaveTypeRepository;
		private readonly ILeaveAllocationRepository _leaveAllocationRepository;
		private readonly IMapper _mapper;
		private readonly UserManager<Employee> _userManager;

		public LeaveRequestController(
			ILeaveRequestRepository leaveRequestRepository,
			ILeaveTypeRepository  leaveTypeRepository,
			ILeaveAllocationRepository leaveAllocationRepository,
			IMapper mapper,
			UserManager<Employee> userManager)
		{
			_leaveRequestRepository = leaveRequestRepository;
			_leaveTypeRepository = leaveTypeRepository;
			_leaveAllocationRepository = leaveAllocationRepository;
			_mapper = mapper;
			_userManager = userManager;

		}
		[Authorize(Roles ="Administrator")]
		public async Task<ActionResult> Index()
		{
			var leaveRequests = await _leaveRequestRepository.FindAll();
			var leaveRequestModel = _mapper.Map<List<LeaveRequestViewModel>>(leaveRequests);
			var model = new AdminLeaveRequestViewViewModel
			{
				TotalRequests = leaveRequestModel.Count,
				PendingRequests = leaveRequestModel.Where(q => q.Approved == null).Count(),
				ApprovedRequests = leaveRequestModel.Where(q => q.Approved == true).Count(),
				RejectedRequests = leaveRequestModel.Where(q => q.Approved == false).Count(),
				leaveRequests = leaveRequestModel
			};
			return View(model);
		}

		public async Task<ActionResult> MyLeave()
		{
			var employee =  await _userManager.GetUserAsync(User);
			var employeeId = employee.Id;
			var employeeAllocation = await _leaveAllocationRepository.GetLeaveAllocationByEmployee(employeeId);
			var employeeRequests = await _leaveRequestRepository.GetLeaveRequestsByEmployee(employeeId);

			var employeeAllocationsModel = _mapper.Map<List<LeaveAllocationViewModel>>(employeeAllocation);
			var employeeRequestsModel = _mapper.Map<List<LeaveRequestViewModel>>(employeeRequests);

			var model = new EmployeeLeaveRequestViewModel
			{
				LeaveAllocations = employeeAllocationsModel,
				LeaveRequests = employeeRequestsModel
			};
			return View(model);
		}
		public async Task<ActionResult> Details(int id)
		{
			var leaveRequest = await _leaveRequestRepository.FindById(id);
			var model = _mapper.Map<LeaveRequestViewModel>(leaveRequest);
			return View(model);
		}

		public async Task<ActionResult> ApproveRequest(int id)
		{
			try
			{
				var user = await _userManager.GetUserAsync(User);
				var leaveRequest = await _leaveRequestRepository.FindById(id);
				string employeeId =  leaveRequest.RequestingEmployeeId;
				int leaveTypeId = leaveRequest.LeaveTypeId;
				var allocation = await _leaveAllocationRepository.GetLeaveAllocationByEmployeeAndType(employeeId, leaveTypeId);

				int daysRequested = (int)(leaveRequest.EndDate - leaveRequest.StartDate).TotalDays;
				allocation.NumberOfDays -= daysRequested;

				leaveRequest.Approved = true;
				leaveRequest.ApprovedBy = user;
				leaveRequest.DateActioned = DateTime.Now;
				await _leaveRequestRepository.Update(leaveRequest);
				await _leaveAllocationRepository.Update(allocation);
				return RedirectToAction(nameof(Index));
			}
			catch (Exception ex)
			{
				return RedirectToAction(nameof(Index));
			}
		}
	
		public async Task<ActionResult> RejectRequest(int id)
		{
			try
			{
				var user = await _userManager.GetUserAsync(User);
				var leaveRequest =await _leaveRequestRepository.FindById(id);
				leaveRequest.Approved = false;
				leaveRequest.ApprovedBy = user;
				leaveRequest.DateActioned = DateTime.Now;
				await _leaveRequestRepository.Update(leaveRequest);
				return RedirectToAction(nameof(Index));

			}
			catch (Exception ex)
			{
				return RedirectToAction(nameof(Index));
			}
			
		}

		public async Task<ActionResult> Create()
		{
			var leaveTypes = await _leaveTypeRepository.FindAll();
			var leaveTypeItems = leaveTypes.Select(q => new SelectListItem
			{
				Text = q.Name,
				Value = q.Id.ToString()
			});
			var model = new CreateLeaveRequestViewViewModel
			{
				LeaveTypes = leaveTypeItems
			};
			return View(model);
		}

		[HttpPost]
		[ValidateAntiForgeryToken]
		public async Task<ActionResult> Create(CreateLeaveRequestViewViewModel model)
		{
			
			try
			{
				var startDate = Convert.ToDateTime(model.StartDate);
				var endDate = Convert.ToDateTime(model.EndDate);
				var leaveTypes = await _leaveTypeRepository.FindAll();
				var leaveTypeItems = leaveTypes.Select(q => new SelectListItem
				{
					Text = q.Name,
					Value = q.Id.ToString()
				});
				model.LeaveTypes = leaveTypeItems;

				if (!ModelState.IsValid)
				{
					return View(model);
				}
				if (DateTime.Compare(startDate,endDate) > 1 )
				{
					ModelState.AddModelError("", "Start date cannot be further in the future than the end date");
					return View(model);
				}

				var employee = await _userManager.GetUserAsync(User);
				var allocation = await _leaveAllocationRepository.GetLeaveAllocationByEmployeeAndType(employee.Id,model.LeaveTypeId);
				int daysRequested = (int)(endDate - startDate).TotalDays;

				if (daysRequested > allocation.NumberOfDays)
				{
					ModelState.AddModelError("", "You do not have sufficient days for this request");
					return View(model);
				}

				var leaveRequestModel = new LeaveRequestViewModel
				{
					RequestingEmployeeId = employee.Id,
					StartDate = startDate,
					EndDate = endDate,
					Approved = null,
					DateRequested = DateTime.Now,
					DateActioned = DateTime.Now,
					LeaveTypeId = model.LeaveTypeId,
					RequestComments = model.RequestComments
				};

				var leaveRequest = _mapper.Map<LeaveRequest>(leaveRequestModel);
				var isSuccess = await _leaveRequestRepository.Create(leaveRequest);
				
				if (!isSuccess)
				{
					ModelState.AddModelError("", "Something went wrong with submitting your record");
					return View(model);
				}

				return RedirectToAction(nameof(Index),"Home");
			}
			catch (Exception ex)
			{
				ModelState.AddModelError("", "Something went wrong");
				return View(model);
			}
		}

		public async Task<ActionResult> CancelRequest(int id)
		{
			var leaveRequest = await _leaveRequestRepository.FindById(id);
			leaveRequest.Cancelled = true;
			await _leaveRequestRepository.Update(leaveRequest);
			return RedirectToAction("MyLeave");
		}
		public ActionResult Edit(int id)
		{
			return View();
		}

		[HttpPost]
		[ValidateAntiForgeryToken]
		public ActionResult Edit(int id, IFormCollection collection)
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
	}
}
