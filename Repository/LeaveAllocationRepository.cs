using Leave_management.Contracts;
using Leave_management.Data;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Leave_management.Repository
{
	public class LeaveAllocationRepository : ILeaveAllocationRepository
	{
		private readonly ApplicationDbContext _db;
		public LeaveAllocationRepository (ApplicationDbContext db)
		{
			_db = db;
		}

		public bool CheckAllocation(int leaveTypeId, string employeeId)
		{
			var period = DateTime.Now.Year;
			return FindAll()
				.Where(q => q.EmployeeId == employeeId && q.LeaveTypeId == leaveTypeId && q.Period == period)
				.Any();
		}

		public bool Create(LeaveAllocation entity)
		{
			_db.LeaveAllocations.Add(entity);
			return Save();
		}

		public bool Delete(LeaveAllocation entity)
		{
			_db.LeaveAllocations.Remove(entity);
			return Save();
		}

		public ICollection<LeaveAllocation> FindAll()
		{
			return _db.LeaveAllocations
				.Include(q => q.LeaveType)
				.Include(q=>q.Employee)
				.ToList();
		}

		public LeaveAllocation FindById(int id)
		{
			return _db.LeaveAllocations
				.Include(q => q.LeaveType)
				.Include(q => q.Employee)
				.FirstOrDefault(q=> q.Id == id);
		}

		public ICollection<LeaveAllocation> GetLeaveAllocationByEmployee(string id)
		{
			var period = DateTime.Now.Year;
			return FindAll()
				.Where(q => q.EmployeeId == id && q.Period == period)
				.ToList();
		}

		public bool IsExists(int id)
		{
			return _db.LeaveAllocations.Any(l => l.Id == id);
		}

		public bool Save()
		{
			int changes = _db.SaveChanges();
			return changes > 0;

		}

		public bool Update(LeaveAllocation entity)
		{
			_db.LeaveAllocations.Update(entity);
			return Save();
		}
	}
}

