
using System.Reflection;

namespace Shared.Response.Columns;

public static class GenerateDataTableColumn<T>
{
    public static List<TableColumn> GetTableColumns()
    {
        var dataTableColumns = new List<TableColumn>();

        Type type = typeof(T);
        FieldInfo[] fields = type.GetFields();

        foreach (FieldInfo fi in fields)
        {
            dataTableColumns.Add(new TableColumn
            {
                Field = fi.Name,
                HeaderName = GetPropertyHeaderName(type, fi.Name),
                Hide = GetPropertyIsHidden(type, fi.Name),
                PropertyType = GetPropertyDataType(type, fi.Name),
            }); ;
        }

        return dataTableColumns;
    }

        private static string GetPropertyHeaderName(Type type, string propertyName)
    {
        string propertyDescription = "";
        var descriptionMethod = type.GetMethod(nameof(GetPropertyHeaderName));
        if (descriptionMethod is not null)
        {
            var descriptionObjValue = descriptionMethod.Invoke(null, new object[] { propertyName });
            if (descriptionObjValue is not null)
                propertyDescription = (string)descriptionObjValue;
        }

        return propertyDescription;
    }
    private static bool GetPropertyIsHidden(Type type, string propertyName)
    {
        bool propertyHidden = true;
        var hiddenMethod = type.GetMethod(nameof(GetPropertyIsHidden));
        if (hiddenMethod is not null)
        {
            var parameters = hiddenMethod.GetParameters();
            var param =  new object[] { propertyName };
            var hiddenObjValue = hiddenMethod.Invoke(null, param);
            if (hiddenObjValue is not null)
                propertyHidden = (bool)hiddenObjValue;
        }

        return propertyHidden;
    }



    private static DataType GetPropertyDataType(Type type, string propertyName)
    {
        DataType propertyType = DataType.String;
        var dataTypeMethod = type.GetMethod(nameof(GetPropertyDataType));
        if (dataTypeMethod is not null)
        {
            var dataTypeObjValue = dataTypeMethod.Invoke(null, new object[] { propertyName });

            if (dataTypeObjValue is not null)
                propertyType = (DataType)dataTypeObjValue;
        }

        return propertyType;
    }

}


