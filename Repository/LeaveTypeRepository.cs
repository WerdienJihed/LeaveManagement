using Leave_management.Contracts;
using Leave_management.Data;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Leave_management.Repository
{
	public class LeaveTypeRepository : ILeaveTypeRepository
	{
		private readonly ApplicationDbContext _db;
		public LeaveTypeRepository(ApplicationDbContext db)
		{
			_db = db;
		}
		public async Task<bool> Create(LeaveType entity)
		{
			await _db.LeaveTypes.AddAsync(entity);
			return await Save();
		}

		public async Task<bool> Delete(LeaveType entity)
		{
			_db.LeaveTypes.Remove(entity);
			return await Save();
		}

		public async Task<ICollection<LeaveType>> FindAll()
		{
			return await _db.LeaveTypes.ToListAsync();
		}

		public async Task<LeaveType> FindById(int id)
		{
			return await _db.LeaveTypes.FindAsync(id);
		}

		public async Task<ICollection<LeaveType>> GetEmployeesByLeaveType(int id)
		{
			throw new NotImplementedException();
		}

		public async Task<bool> IsExists(int id)
		{
			return await _db.LeaveTypes.AnyAsync(l => l.Id == id);
		}

		public async Task<bool> Save()
		{
			int changes = await _db.SaveChangesAsync();
			return changes > 0; 
			
		}

		public async Task<bool> Update(LeaveType entity)
		{
			_db.LeaveTypes.Update(entity);
			return await Save();
		}
	}
}
