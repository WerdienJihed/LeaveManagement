using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Leave_management.Models;

namespace Leave_management.Data
{
	public class ApplicationDbContext : IdentityDbContext
	{
		public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
			: base(options)
		{
		}

		public DbSet<Employee> employees { get; set; }
		public DbSet<LeaveHistory> LeaveHistories { get; set; }
		public DbSet<LeaveType> LeaveTypes { get; set; }
		public DbSet<LeaveAllocation> LeaveAllocations { get; set; }
		public DbSet<Leave_management.Models.LeaveTypeViewModel> DetailsLeaveTypeViewModel { get; set; }
		public DbSet<Leave_management.Models.EmployeeViewModel> EmployeeViewModel { get; set; }
		public DbSet<Leave_management.Models.LeaveAllocationViewModel> LeaveAllocationViewModel { get; set; }
		public DbSet<Leave_management.Models.EditLeaveAllocationViewModel> EditLeaveAllocationViewModel { get; set; }
	}
}
