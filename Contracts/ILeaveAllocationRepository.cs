using Leave_management.Data;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Leave_management.Contracts
{
	public interface ILeaveAllocationRepository : IRepositoryBase<LeaveAllocation>
    {
        Task<bool> CheckAllocation(int leaveTypeId, string employeeId);
        Task<ICollection<LeaveAllocation>> GetLeaveAllocationByEmployee(string imployeeId);
        Task<LeaveAllocation> GetLeaveAllocationByEmployeeAndType(string imployeeId, int leaveTypeId);
    }
}
