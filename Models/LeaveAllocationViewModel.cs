using Microsoft.AspNetCore.Mvc.Rendering;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace Leave_management.Models
{
	public class LeaveAllocationViewModel
	{
		public int Id { get; set; }
		public int NumberOfDays { get; set; }
		public DateTime DateCreated { get; set; }
		public int Period { get; set; }
		public EmployeeViewModel Employee { get; set; }
		public string EmployeeId { get; set; }
		public LeaveTypeViewModel LeaveType { get; set; }
		public int LeaveTypeId { get; set; }
	}

	public class CreateLeaveAllocationViewModel
	{
		public int NumberUpdated { get; set; }
		public List<LeaveTypeViewModel> LeaveTypeViewModels { get; set; }
	}
	public class EditLeaveAllocationViewModel
	{
		public int Id { get; set; }
		[Display(Name ="Number Of Days")]
		public int NumberOfDays { get; set; }
		public EmployeeViewModel Employee { get; set; }
		public string EmployeeId { get; set; }
		public LeaveTypeViewModel LeaveType { get; set; }
	}
	public class ViewAllocationViewModel
	{
		public EmployeeViewModel  Employee { get; set; }
		public string EmployeeId { get; set; }
		public List<LeaveAllocationViewModel> leaveAllocations { get; set; }
	}
}
