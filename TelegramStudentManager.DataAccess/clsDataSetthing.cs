using System;

namespace TelegramStudentManager.DataAccess
{
    public class clsDataSetthing
    {
        public static string connectionString =
            Environment.GetEnvironmentVariable("ConnectionStrings__DefaultConnection")
            ?? "Host=aws-1-eu-west-1.pooler.supabase.com;" +
               "Port=5432;" +
               "Database=postgres;" +
               "Username=postgres.lucisablddsjlvlyknzk;" +
               "Password=abukdi6187@.com;" +
               "SSL Mode=Require;" +
               "Trust Server Certificate=True;";
    }
}