using Convertly.Application.Account.Dtos;
using Convertly.Application.Common;

namespace Convertly.Application.Account;

public interface IAccountService
{
    Task<ApiResponse<object>> ChangePasswordAsync(ChangePasswordRequest request, CancellationToken cancellationToken);

    Task<ApiResponse<object>> DeleteAccountAsync(DeleteAccountRequest request, CancellationToken cancellationToken);
}
