
namespace Shared.Response.Columns;

public class NoteColumn
{
    public static string id = "Id";
    public static string title = "Title";
    public static string content = "Content";
    public static string createdAt = "Created At";
    public static string updatedAt = "updatedAt";
   
   

    public static string GetPropertyHeaderName(string propertyName)
    {
        switch (propertyName)
        {
            case nameof(id):
                return id;
            case nameof(title):
                return title;
            case nameof(content):
                return content;
            case nameof(createdAt):
                return createdAt;
            case nameof(updatedAt):
                return updatedAt;
            default:
                return "";
        }
    }

    public static bool GetPropertyIsHidden(string propertyName)
    {
        switch (propertyName)
        {
            case nameof(id):
                return true;
            case nameof(title): 
            case nameof(content):
            case nameof(createdAt):
            case nameof(updatedAt):
                return false;
            default:
                return true;
        }
    }




    public static DataType GetPropertyDataType(string propertyName)
    {
        switch (propertyName)
        {
            case nameof(id):
                return DataType.Number;
            case nameof(title):
                return DataType.String;
            case nameof(content):
                return DataType.String;
            case nameof(createdAt):
                return DataType.DateTime;
            case nameof(updatedAt):
                return DataType.DateTime;
            default:
                return DataType.String;
        }
    }

}
