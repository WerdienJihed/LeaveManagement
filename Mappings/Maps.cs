using AutoMapper;
using Leave_management.Data;
using Leave_management.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Leave_management.Mappings
{
	public class Maps : Profile
	{
		public Maps()
		{
			CreateMap<LeaveType, DetailsLeaveTypeViewModel>().ReverseMap();
			CreateMap<LeaveType, CreateLeaveTypeViewModel>().ReverseMap();
			CreateMap<LeaveHistory, LeaveHistoryViewModel>().ReverseMap();
			CreateMap<LeaveAllocation, LeaveAllocationViewModel>().ReverseMap();
			CreateMap<Employee, EmployeeViewModel>().ReverseMap();
		}
	}
}
