using Leave_management.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Leave_management.Contracts
{
	public interface IUnitOfWork : IDisposable
	{
		IGenericRepository<LeaveType> LeaveTypes { get; }
		IGenericRepository<LeaveAllocation> LeaveAllocations { get;}
		IGenericRepository<LeaveRequest> LeaveRequests { get;}
		Task Save();
	}
}
