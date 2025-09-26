namespace Application.Features.ValueFeature.Queries.ExportCsvValue;

public class ExportCsvValueHandler : ExportCsvHandler<Value, ExportCsvValueQuery>, IQueryHandler<ExportCsvValueQuery, Response<string>>
{
    public ExportCsvValueHandler(IReadRepository<Value> readRepository)
        : base(readRepository)
    {
    }

    public override Expression<Func<Value, List<object>>> Selector => value => new List<object>
    {
        value.Id,
        value.Name,
        value.ValueNumber,
        value.CreatedAt,
        value.UpdatedAt!,
        value.StatusId
    };

    public override Func<IQueryable<Value>, IIncludableQueryable<Value, object>>? Include => null;

    public override string[] Headers => new[]
    {
        "Id",
        "Name",
        "Value Number",
        "Created At",
        "Updated At",
        "Status Id"
    };

    protected override Expression<Func<Value, bool>>? FilterPredicate(ExportCsvValueQuery request)
    {
        // Convert DateTime parameters to UTC to avoid PostgreSQL timezone issues
        var createdFromUtc = request.CreatedFrom?.Kind == DateTimeKind.Unspecified
            ? DateTime.SpecifyKind(request.CreatedFrom.Value, DateTimeKind.Utc)
            : request.CreatedFrom?.ToUniversalTime();

        var createdToUtc = request.CreatedTo?.Kind == DateTimeKind.Unspecified
            ? DateTime.SpecifyKind(request.CreatedTo.Value, DateTimeKind.Utc)
            : request.CreatedTo?.ToUniversalTime();

        return value => !value.IsDeleted &&
            (!createdFromUtc.HasValue || value.CreatedAt >= createdFromUtc) &&
            (!createdToUtc.HasValue || value.CreatedAt <= createdToUtc);
    }

    public async Task<Response<string>> Handle(ExportCsvValueQuery request, CancellationToken cancellationToken = default)
    {
        try
        {
            var csvData = await HandleBase(request, cancellationToken);

            if (csvData == null)
            {
                return Response<string>.Success(string.Empty);
            }

            var csvString = Convert.ToBase64String(csvData);
            return Response<string>.Success(csvString);
        }
        catch (Exception)
        {
            return ErrorsMessage.SystemError.ToErrorMessage<string>(string.Empty);
        }
    }
}
