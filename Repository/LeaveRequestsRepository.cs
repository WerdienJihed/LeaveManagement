using Leave_management.Contracts;
using Leave_management.Data;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Leave_management.Repository
{
	public class LeaveRequestsRepository : ILeaveRequestRepository
	{
		private readonly ApplicationDbContext _db;
		public LeaveRequestsRepository(ApplicationDbContext db)
		{
			_db = db;
		}
		public async Task<bool> Create(LeaveRequest entity)
		{
			 await _db.LeaveRequests.AddAsync(entity);
			return await Save();
		}

		public async Task<bool> Delete(LeaveRequest entity)
		{
			_db.LeaveRequests.Remove(entity);
			return await Save();
		}

		public async Task<ICollection<LeaveRequest>> FindAll()
		{
			return await _db.LeaveRequests
				.Include(q=>q.RequestingEmployee)
				.Include(q=>q.ApprovedBy)
				.Include(q=>q.LeaveType)
				.ToListAsync();
		}

		public async Task<LeaveRequest> FindById(int id)
		{
			return await _db.LeaveRequests
				.Include(q => q.RequestingEmployee)
				.Include(q => q.ApprovedBy)
				.Include(q => q.LeaveType)
				.FirstOrDefaultAsync(q=>q.Id == id);
		}

		public async Task<ICollection<LeaveRequest>> GetLeaveRequestsByEmployee(string employeeId)
		{
			var leaveRequests = await FindAll();
			return leaveRequests.Where(q=> q.RequestingEmployeeId == employeeId).ToList();
		}

		public async Task<bool> IsExists(int id)
		{
			return await _db.LeaveRequests.AnyAsync(l => l.Id == id);
		}

		public async Task<bool> Save()
		{
			int changes = await _db.SaveChangesAsync();
			return changes > 0;

		}

		public async Task<bool> Update(LeaveRequest entity)
		{
			_db.LeaveRequests.Update(entity);
			return await Save();
		}
		
	}
}
