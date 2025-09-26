using Application.Features.ValueFeature.Commands.CreateValue;
using Application.Features.ValueFeature.Commands.UpdateValue;
using Application.Features.ValueFeature.Queries.GetListValue;
using Application.Features.ValueFeature.Queries.Shared;
using Application.Contracts.Persistence.Common;
using Cortex.Mediator;
using Domain.Common;
using MudBlazor;

namespace WebApp.Services;

/// <summary>
/// Service implementation for Value management operations
/// </summary>
public class ValueManagementService : IValueManagementService
{
    private readonly IMediator _mediator;
    private readonly ISnackbar _snackbar;

    public ValueManagementService(IMediator mediator, ISnackbar snackbar)
    {
        _mediator = mediator ?? throw new ArgumentNullException(nameof(mediator));
        _snackbar = snackbar ?? throw new ArgumentNullException(nameof(snackbar));
    }

    /// <summary>
    /// Loads values with pagination and filtering
    /// </summary>
    public async Task<GridData<ValueViewModel>> LoadValuesAsync(GridState<ValueViewModel> state, string searchTerm, int? statusId)
    {
        try
        {
            var query = new GetListValueQuery
            {
                Page = state.Page + 1,
                PageSize = (byte)state.PageSize,
                Name = searchTerm,
                StatusId = statusId
            };

            Console.WriteLine($"Loading values - Page: {query.Page}, PageSize: {query.PageSize}, SearchTerm: {searchTerm}, StatusId: {statusId}");
            var result = await _mediator.SendQueryAsync<GetListValueQuery, Response<PagedResult<ValueViewModel>>>(query);
            Console.WriteLine($"Query result - Succeeded: {result.Succeeded}, Message: {result.Message}");

            if (result.Succeeded)
            {
                var pagedResult = result.Data;
                return new GridData<ValueViewModel>
                {
                    Items = pagedResult?.Data?.ToList() ?? new List<ValueViewModel>(),
                    TotalItems = pagedResult?.TotalCount ?? 0
                };
            }
            else
            {
                _snackbar.Add(result.Message, Severity.Error);
                return new GridData<ValueViewModel> { Items = new List<ValueViewModel>(), TotalItems = 0 };
            }
        }
        catch (Exception ex)
        {
            _snackbar.Add($"Error loading data: {ex.Message}", Severity.Error);
            return new GridData<ValueViewModel> { Items = new List<ValueViewModel>(), TotalItems = 0 };
        }
    }

    /// <summary>
    /// Creates a new value
    /// </summary>
    public async Task<Response<Guid>> CreateValueAsync(CreateValueCommand command)
    {
        try
        {
            var result = await _mediator.SendCommandAsync<CreateValueCommand, Response<Guid>>(command);

            if (result.Succeeded)
            {
                _snackbar.Add("Value created successfully!", Severity.Success);
            }
            else
            {
                _snackbar.Add(result.Message, Severity.Error);
            }

            return result;
        }
        catch (Exception ex)
        {
            _snackbar.Add($"Error creating value: {ex.Message}", Severity.Error);
            return Response<Guid>.Fail(new MessageResponse { Message = ex.Message, Code = "CREATE_ERROR" });
        }
    }

    /// <summary>
    /// Updates an existing value
    /// </summary>
    public async Task<Response<Guid>> UpdateValueAsync(UpdateValueCommand command)
    {
        try
        {
            var result = await _mediator.SendCommandAsync<UpdateValueCommand, Response<Guid>>(command);

            if (result.Succeeded)
            {
                _snackbar.Add("Value updated successfully!", Severity.Success);
            }
            else
            {
                _snackbar.Add(result.Message, Severity.Error);
            }

            return result;
        }
        catch (Exception ex)
        {
            _snackbar.Add($"Error updating value: {ex.Message}", Severity.Error);
            return Response<Guid>.Fail(new MessageResponse { Message = ex.Message, Code = "UPDATE_ERROR" });
        }
    }

    /// <summary>
    /// Deletes a value by ID
    /// </summary>
    public async Task<Response<bool>> DeleteValueAsync(Guid id)
    {
        try
        {
            // Note: You'll need to implement a Delete command in the Application layer
            // For now, we'll return a placeholder response
            _snackbar.Add("Delete functionality to be implemented", Severity.Warning);
            await Task.CompletedTask; // To satisfy async requirement
            return Response<bool>.Success(true);
        }
        catch (Exception ex)
        {
            _snackbar.Add($"Error deleting value: {ex.Message}", Severity.Error);
            return Response<bool>.Fail(new MessageResponse { Message = ex.Message, Code = "DELETE_ERROR" });
        }
    }

    /// <summary>
    /// Gets a value by ID
    /// </summary>
    public async Task<Response<ValueViewModel>> GetValueByIdAsync(Guid id)
    {
        try
        {
            // Note: You'll need to implement a GetById query in the Application layer
            // For now, we'll return a placeholder response
            _snackbar.Add("Get by ID functionality to be implemented", Severity.Warning);
            await Task.CompletedTask; // To satisfy async requirement
            return Response<ValueViewModel>.Fail(new MessageResponse { Message = "Not implemented yet", Code = "NOT_IMPLEMENTED" });
        }
        catch (Exception ex)
        {
            _snackbar.Add($"Error getting value: {ex.Message}", Severity.Error);
            return Response<ValueViewModel>.Fail(new MessageResponse { Message = ex.Message, Code = "GET_ERROR" });
        }
    }

    /// <summary>
    /// Refreshes the data grid
    /// </summary>
    public async Task RefreshDataAsync()
    {
        // This method can be used to trigger data refresh
        // Implementation depends on your specific needs
        await Task.CompletedTask;
    }
}
