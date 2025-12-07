using FormBuilder.Core.Models;
using Microsoft.EntityFrameworkCore;
using System.Security.Cryptography;
using System.Text;

public static class adminDataSeeder
{
    public static void Seed(AkhmanageItContext context)
    {
        context.Database.Migrate(); // يضمن عمل أي Migration

        // إضافة User "anas" لو مش موجود
        if (!context.TblUsers.Any(u => u.Username == "anas"))
        {
            string password = "Admin@123"; // باسورد مؤقت
            string hashedPassword = HashPassword(password);

            context.TblUsers.Add(new TblUser
            {
                Username = "anas",
                Password = hashedPassword
                // أي Role أو GroupId نتركه فارغ
            });

            context.SaveChanges();
        }
    }


    // دالة SHA512
    private static string HashPassword(string password)
    {
        using (SHA512 sha = SHA512.Create())
        {
            byte[] bytes = Encoding.UTF8.GetBytes(password);
            byte[] hash = sha.ComputeHash(bytes);
            StringBuilder sb = new StringBuilder(128);
            foreach (byte b in hash)
                sb.Append(b.ToString("x2"));
            return sb.ToString();
        }
    }
}
