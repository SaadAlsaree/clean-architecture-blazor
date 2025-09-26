using Application.Features.ValueFeature.Queries.Shared;
using Microsoft.AspNetCore.Components.Web;
using MudBlazor;
using WebApp.Services;
using WebApp.Components.Pages;

namespace WebApp.ViewModels;

/// <summary>
/// ViewModel for Value Management page
/// </summary>
public class ValueManagementViewModel
{
    private readonly IValueManagementService _valueService;
    private readonly IDialogService _dialogService;
    private readonly ISnackbar _snackbar;
    private Action? _stateHasChanged;

    public ValueManagementViewModel(
        IValueManagementService valueService,
        IDialogService dialogService,
        ISnackbar snackbar)
    {
        _valueService = valueService;
        _dialogService = dialogService;
        _snackbar = snackbar;
    }

    public void SetStateHasChanged(Action stateHasChanged)
    {
        _stateHasChanged = stateHasChanged;
    }

    // Properties
    private bool _isLoading = false;
    public bool IsLoading
    {
        get => _isLoading;
        set
        {
            _isLoading = value;
            _stateHasChanged?.Invoke();
        }
    }

    private string _searchTerm = string.Empty;
    public string SearchTerm
    {
        get => _searchTerm;
        set
        {
            _searchTerm = value;
            _stateHasChanged?.Invoke();
        }
    }

    private string _quickSearchString = string.Empty;
    public string QuickSearchString
    {
        get => _quickSearchString;
        set
        {
            _quickSearchString = value;
            _stateHasChanged?.Invoke();
        }
    }

    private int? _selectedStatus = null;
    public int? SelectedStatus
    {
        get => _selectedStatus;
        set
        {
            _selectedStatus = value;
            _stateHasChanged?.Invoke();
        }
    }

    private int _totalItems = 0;
    public int TotalItems
    {
        get => _totalItems;
        set
        {
            _totalItems = value;
            _stateHasChanged?.Invoke();
        }
    }

    private bool _hasError = false;
    public bool HasError
    {
        get => _hasError;
        set
        {
            _hasError = value;
            _stateHasChanged?.Invoke();
        }
    }

    private string _errorMessage = string.Empty;
    public string ErrorMessage
    {
        get => _errorMessage;
        set
        {
            _errorMessage = value;
            _stateHasChanged?.Invoke();
        }
    }

    // Quick filter for client-side filtering
    public Func<ValueViewModel, bool> QuickFilter => x =>
    {
        if (string.IsNullOrWhiteSpace(QuickSearchString))
            return true;

        if (x.Name.Contains(QuickSearchString, StringComparison.OrdinalIgnoreCase))
            return true;

        if (x.ValueNumber.ToString().Contains(QuickSearchString))
            return true;

        if (x.StatusName.Contains(QuickSearchString, StringComparison.OrdinalIgnoreCase))
            return true;

        return false;
    };

    /// <summary>
    /// Initialize the ViewModel
    /// </summary>
    public async Task InitializeAsync()
    {
        TotalItems = 0;
        HasError = false;
        ErrorMessage = string.Empty;
        IsLoading = false;
        await Task.CompletedTask;
    }

    /// <summary>
    /// Load server data for the grid
    /// </summary>
    public async Task<GridData<ValueViewModel>> LoadServerDataAsync(GridState<ValueViewModel> state)
    {
        Console.WriteLine($"LoadServerDataAsync called - Page: {state.Page}, PageSize: {state.PageSize}");
        IsLoading = true;

        try
        {
            var searchTerm = !string.IsNullOrEmpty(QuickSearchString) ? QuickSearchString : SearchTerm;
            Console.WriteLine($"SearchTerm: {searchTerm}, SelectedStatus: {SelectedStatus}");
            var result = await _valueService.LoadValuesAsync(state, searchTerm, SelectedStatus);

            TotalItems = result.TotalItems;
            Console.WriteLine($"Result - TotalItems: {result.TotalItems}, ItemsCount: {result.Items.Count()}");
            return result;
        }
        finally
        {
            IsLoading = false;
        }
    }

    /// <summary>
    /// Search values
    /// </summary>
    public async Task SearchValuesAsync(MudDataGrid<ValueViewModel> dataGrid)
    {
        Console.WriteLine($"SearchValuesAsync called - SearchTerm: {SearchTerm}, SelectedStatus: {SelectedStatus}, QuickSearchString: {QuickSearchString}");
        await dataGrid?.ReloadServerData()!;
    }

    /// <summary>
    /// Handle search key up event
    /// </summary>
    public async Task OnSearchKeyUpAsync(KeyboardEventArgs e, MudDataGrid<ValueViewModel> dataGrid)
    {
        Console.WriteLine($"OnSearchKeyUpAsync called - Key: {e.Key}");
        if (e.Key == "Enter")
        {
            await SearchValuesAsync(dataGrid);
        }
    }

    /// <summary>
    /// Refresh data
    /// </summary>
    public async Task RefreshDataAsync(MudDataGrid<ValueViewModel> dataGrid)
    {
        Console.WriteLine("RefreshDataAsync called - clearing filters");
        SearchTerm = string.Empty;
        QuickSearchString = string.Empty;
        SelectedStatus = null;
        await dataGrid?.ReloadServerData()!;
    }

    /// <summary>
    /// Open create dialog
    /// </summary>
    public async Task<bool> OpenCreateDialogAsync()
    {
        try
        {
            var parameters = new DialogParameters();
            var options = new DialogOptions() { CloseButton = true, MaxWidth = MaxWidth.Small, FullWidth = true };

            var dialog = await _dialogService.ShowAsync<ValueCreateDialog>("Create New Value", parameters, options);
            var result = await dialog.Result;

            if (result != null && !result.Canceled && result.Data != null)
            {
                _snackbar.Add("Value created successfully!", Severity.Success);
                return true; // Indicates refresh is needed
            }
        }
        catch (Exception ex)
        {
            _snackbar.Add($"Error opening dialog: {ex.Message}", Severity.Error);
        }
        return false;
    }

    /// <summary>
    /// Open edit dialog
    /// </summary>
    public async Task<bool> OpenEditDialogAsync(ValueViewModel value)
    {
        try
        {
            var parameters = new DialogParameters
            {
                ["Value"] = value
            };
            var options = new DialogOptions() { CloseButton = true, MaxWidth = MaxWidth.Small, FullWidth = true };

            var dialog = await _dialogService.ShowAsync<ValueEditDialog>("Edit Value", parameters, options);
            var result = await dialog.Result;

            if (result != null && !result.Canceled && result.Data != null)
            {
                _snackbar.Add("Value updated successfully!", Severity.Success);
                return true; // Indicates refresh is needed
            }
        }
        catch (Exception ex)
        {
            _snackbar.Add($"Error opening edit dialog: {ex.Message}", Severity.Error);
        }
        return false;
    }

    /// <summary>
    /// Delete value
    /// </summary>
    public async Task DeleteValueAsync(Guid id, MudDataGrid<ValueViewModel> dataGrid)
    {
        var result = await _dialogService.ShowMessageBox(
            "Delete Value",
            "Are you sure you want to delete this value?",
            yesText: "Delete", cancelText: "Cancel");

        if (result == true)
        {
            try
            {
                var deleteResult = await _valueService.DeleteValueAsync(id);
                if (deleteResult.Succeeded)
                {
                    await dataGrid?.ReloadServerData()!;
                }
            }
            catch (Exception ex)
            {
                _snackbar.Add($"Error deleting value: {ex.Message}", Severity.Error);
            }
        }
    }

    /// <summary>
    /// Get status color for display
    /// </summary>
    public Color GetStatusColor(int statusId)
    {
        return statusId switch
        {
            0 => Color.Warning, // Unverified
            1 => Color.Success, // Verified
            2 => Color.Error, // Rejected
            _ => Color.Default
        };
    }

    /// <summary>
    /// Retry loading data
    /// </summary>
    public async Task RetryLoadAsync(MudDataGrid<ValueViewModel> dataGrid)
    {
        HasError = false;
        ErrorMessage = string.Empty;
        await dataGrid?.ReloadServerData()!;
    }
}
