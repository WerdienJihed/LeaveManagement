using Leave_management.Data;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Leave_management.Contracts
{
	public interface ILeaveTypeRepository : IRepositoryBase<LeaveType>
    {
        Task<ICollection<LeaveType>> GetEmployeesByLeaveType(int id);
    }
}
