using Application.Features.ValueFeature.Commands.CreateValue;
using Application.Features.ValueFeature.Commands.UpdateValue;
using Application.Features.ValueFeature.Queries.Shared;
using Domain.Common;
using MudBlazor;

namespace WebApp.Services;

/// <summary>
/// Service interface for Value management operations
/// </summary>
public interface IValueManagementService
{
    /// <summary>
    /// Loads values with pagination and filtering
    /// </summary>
    Task<GridData<ValueViewModel>> LoadValuesAsync(GridState<ValueViewModel> state, string searchTerm, int? statusId);

    /// <summary>
    /// Creates a new value
    /// </summary>
    Task<Response<Guid>> CreateValueAsync(CreateValueCommand command);

    /// <summary>
    /// Updates an existing value
    /// </summary>
    Task<Response<Guid>> UpdateValueAsync(UpdateValueCommand command);

    /// <summary>
    /// Deletes a value by ID
    /// </summary>
    Task<Response<bool>> DeleteValueAsync(Guid id);

    /// <summary>
    /// Gets a value by ID
    /// </summary>
    Task<Response<ValueViewModel>> GetValueByIdAsync(Guid id);

    /// <summary>
    /// Refreshes the data grid
    /// </summary>
    Task RefreshDataAsync();
}
