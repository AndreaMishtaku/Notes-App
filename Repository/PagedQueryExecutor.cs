using Dapper;
using Shared.Enums;
using Shared.Request;
using Shared.Response;
using System.Data;
using System.Reflection;
using System.Text;

namespace Repository;

public class PagedQueryExecutor<T>
{
    private readonly IDbConnection _dbConnection;
    private readonly string _baseQuery;

    public PagedQueryExecutor(IDbConnection dbConnection,string baseQuery)
    {
        _dbConnection = dbConnection ?? throw new ArgumentNullException(nameof(dbConnection)); ;
        _baseQuery = baseQuery ?? throw new ArgumentNullException(nameof(baseQuery));
    }

    public async Task<PagedResult<T>> GetPagedResultAsync(int pageNumber, int pageSize, Filter[] filters = null)
    {
        if (pageNumber < 1 || pageSize < 1)
        {
            throw new ArgumentException("Page number and page size must be greater than 0.");
        }

        var offset = (pageNumber - 1) * pageSize;

        var sqlBuilder = new StringBuilder(_baseQuery);

        if (filters != null && filters.Any())
        {
            sqlBuilder.Append(" where ");
            for (var i= 0;i < filters.Length;i++)
            { 
                AddFilterCondition(sqlBuilder, filters[i]);
                if (i < filters.Length - 1)
                {
                    sqlBuilder.Append(" and ");
                }
                
            }
           
        }

        var dataQuery = $"{sqlBuilder} order by id offset {offset} rows fetch next {pageSize} rows only";

        var countQuery = $"SELECT COUNT(*) FROM ({dataQuery}) AS CountQuery";

        var count = await _dbConnection.ExecuteScalarAsync<int>(countQuery);

        var totalPages = (int)Math.Ceiling((double)count / pageSize);

        return new PagedResult<T>
        {
            RowCount = count,
            Page = pageNumber,
            PageSize = pageSize,
            Rows = (List<T>)await _dbConnection.QueryAsync<T>(dataQuery),
            HasNext = pageNumber < totalPages,
            HasPrevious = pageNumber > 1,
            TotalPages = totalPages
        };
    }


    private void AddFilterCondition(StringBuilder sqlBuilder, Filter filter)
    {
        var propertyName = filter.Field;

        switch (filter.Operation)
        {
            case Operation.Contains:
                sqlBuilder.Append($"{propertyName} like '%{filter.Value}%'");
                break;

            case Operation.Equals:
                sqlBuilder.Append($"{propertyName}='{filter.Value}'");
                break;

            case Operation.GreaterThan:
                sqlBuilder.Append($"{propertyName} > '{filter.Value}'");
                break;

            case Operation.LessThan:
                sqlBuilder.Append($"{propertyName} < '{filter.Value}'");
                break;

            default:
                throw new NotSupportedException($"Unsupported operator: {filter.Operation}");
        }
    }

 
}
