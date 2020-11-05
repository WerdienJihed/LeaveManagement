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

		public async Task<bool> CheckAllocation(int leaveTypeId, string employeeId)
		{
			var period = DateTime.Now.Year;
			var allocations = await FindAll();
			return allocations.Where(q => q.EmployeeId == employeeId && q.LeaveTypeId == leaveTypeId && q.Period == period).Any();
		}

		public async Task<bool> Create(LeaveAllocation entity)
		{
			_db.LeaveAllocations.Add(entity);
			return await Save();
		}

		public async Task<bool> Delete(LeaveAllocation entity)
		{
			_db.LeaveAllocations.Remove(entity);
			return await Save();
		}

		public async Task<ICollection<LeaveAllocation>> FindAll()
		{
			return await _db.LeaveAllocations
				.Include(q => q.LeaveType)
				.Include(q=>q.Employee)
				.ToListAsync();
		}

		public async Task<LeaveAllocation> FindById(int id)
		{
			return await _db.LeaveAllocations
				.Include(q => q.LeaveType)
				.Include(q => q.Employee)
				.FirstOrDefaultAsync(q=> q.Id == id);
		}

		public async Task<ICollection<LeaveAllocation>> GetLeaveAllocationByEmployee(string id)
		{
			var period = DateTime.Now.Year;
			var leaveAllocations = await FindAll();
			return leaveAllocations.Where(q => q.EmployeeId == id && q.Period == period).ToList();
		}

		public async Task<LeaveAllocation> GetLeaveAllocationByEmployeeAndType(string employeeId, int leaveTypeId)
		{
			var period = DateTime.Now.Year;
			var leaveAllocations = await FindAll();
			return leaveAllocations.FirstOrDefault(q => q.EmployeeId == employeeId && q.Period == period && q.LeaveTypeId == leaveTypeId);
		}

		public async Task<bool> IsExists(int id)
		{
			return await _db.LeaveAllocations.AnyAsync(l => l.Id == id);
		}

		public async Task<bool> Save()
		{
			int changes = await _db.SaveChangesAsync();
			return changes > 0;

		}

		public async Task<bool> Update(LeaveAllocation entity)
		{
			_db.LeaveAllocations.Update(entity);
			return await Save();
		}
	}
}

