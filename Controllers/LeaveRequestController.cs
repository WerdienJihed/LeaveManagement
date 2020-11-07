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
		private readonly IUnitOfWork _repo;
		private readonly IMapper _mapper;
		private readonly UserManager<Employee> _userManager;

		public LeaveRequestController(IUnitOfWork repo ,IMapper mapper,UserManager<Employee> userManager)
		{
			_repo = repo;
			_mapper = mapper;
			_userManager = userManager;
		}

		[Authorize(Roles ="Administrator")]
		public async Task<ActionResult> Index()
		{
			var leaveRequests = await _repo.LeaveRequests.FindAll
				(
					includes : new List<string> {"RequestingEmployee","LeaveType"}
				);
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
			var employeeAllocation = await _repo.LeaveAllocations.FindAll(q=> q.EmployeeId == employeeId);
			var employeeRequests = await _repo.LeaveRequests.FindAll
				(
					q => q.RequestingEmployeeId == employeeId,
					includes: new List<string> { "LeaveType" }
				);

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
			var leaveRequest = await _repo.LeaveRequests.Find
				(
					q=> q.Id == id,
					includes: new List<string> {"ApprovedBy","RequestingEmployee","LeaveType" }
				);
			var model = _mapper.Map<LeaveRequestViewModel>(leaveRequest);
			return View(model);
		}

		public async Task<ActionResult> ApproveRequest(int id)
		{
			try
			{
				var user = await _userManager.GetUserAsync(User);
				var leaveRequest = await _repo.LeaveRequests.Find(q=> q.Id == id);
				string employeeId =  leaveRequest.RequestingEmployeeId;
				int leaveTypeId = leaveRequest.LeaveTypeId;
				var allocation = await _repo.LeaveAllocations.Find
					(
						q=>q.EmployeeId == employeeId 
						&& q.LeaveTypeId == leaveTypeId 
						&& q.Period == DateTime.Now.Year
					);

				int daysRequested = (int)(leaveRequest.EndDate - leaveRequest.StartDate).TotalDays;
				allocation.NumberOfDays -= daysRequested;

				leaveRequest.Approved = true;
				leaveRequest.ApprovedBy = user;
				leaveRequest.DateActioned = DateTime.Now;
				_repo.LeaveRequests.Update(leaveRequest);
				_repo.LeaveAllocations.Update(allocation);
				await _repo.Save();

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
				var leaveRequest =await _repo.LeaveRequests.Find(q=>q.Id == id);
				leaveRequest.Approved = false;
				leaveRequest.ApprovedBy = user;
				leaveRequest.DateActioned = DateTime.Now;
				_repo.LeaveRequests.Update(leaveRequest);
				return RedirectToAction(nameof(Index));

			}
			catch (Exception ex)
			{
				return RedirectToAction(nameof(Index));
			}
			
		}

		public async Task<ActionResult> Create()
		{
			var leaveTypes = await _repo.LeaveTypes.FindAll();
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
				var leaveTypes = await _repo.LeaveTypes.FindAll();
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
				var allocation = await _repo.LeaveAllocations.Find
					(
						q=>q.EmployeeId == employee.Id 
						&& q.LeaveTypeId == model.LeaveTypeId 
						&& q.Period == DateTime.Now.Year
					);
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
				await _repo.LeaveRequests.Create(leaveRequest);
				await _repo.Save();

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
			var leaveRequest = await _repo.LeaveRequests.Find(q=>q.Id == id);
			leaveRequest.Cancelled = true;
			_repo.LeaveRequests.Update(leaveRequest);
			await _repo.Save();
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

		protected override void Dispose(bool disposing)
		{
			_repo.Dispose();
			base.Dispose(disposing);
		}
	}
}
