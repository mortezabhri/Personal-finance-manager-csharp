// PersonalFinanceManager.cs
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.Json;

namespace PersonalFinanceManager
{
    // Models
    public abstract class Transaction
    {
        public Guid Id { get; set; } = Guid.NewGuid();
        public decimal Amount { get; set; }
        public DateTime Date { get; set; }
        public string Description { get; set; }
        public string title { get; set; }
        public string UserEmail { get; set; }
    }

    public class Income : Transaction
    {
        public string Source { get; set; }
    }

    public class Expense : Transaction
    {
        public string Category { get; set; }
    }

    public class User
    {
        public string Username { get; set; }
        public string Email { get; set; }
        public bool Active { get; set; }
    }

    public class Category
    {
        public string Name { get; set; }
    }

    // Helper
    public static class Utils
    {
        public static void SaveToFile<T>(string path, T data)
        {
            var json = JsonSerializer.Serialize(
                data,
                new JsonSerializerOptions { WriteIndented = true }
            );
            File.WriteAllText(path, json);
        }
        public static T? LoadFromFile<T>(string path)
        {
            if (!File.Exists(path))
                return default;
            var json = File.ReadAllText(path);
            return JsonSerializer.Deserialize<T>(json);
        }
    }

    // Services
    public class UserService
    {
        private List<User> users = new();
        public string CurrentUserEmail { get; private set; }

        public UserService() => Load();

        public void AddUser(User user)
        {
            users.Add(user);
            Save();
        }

        public void UpdateUserActive(string email, bool status)
        {
            var user = users.FirstOrDefault(u => u.Email == email);
            if (user != null)
            {
                user.Active = status;
                Save();
                Console.WriteLine($"User is {status}");
            }
            else
            {
                Console.WriteLine("User not found with provided email.");
            }
        }

        public List<User> GetAllUsers() => users;

        private void Load() =>
            users = Utils.LoadFromFile<List<User>>("users.json") ?? new List<User>();

        private void Save() => Utils.SaveToFile("users.json", users);
    }

    public class TransactionService
    {
        private List<Transaction> transactions = new();

        public TransactionService() => Load();

        public void AddTransaction(Transaction t)
        {
            transactions.Add(t);
            Save();
        }

        public void DeleteTransaction(Guid id)
        {
            transactions.RemoveAll(t => t.Id == id);
            Save();
        }

        public List<Transaction> GetAll(string userEmail) =>
            transactions.Where(t => t.UserEmail == userEmail).ToList();

        private void Load() =>
            transactions =
                Utils.LoadFromFile<List<Transaction>>("transactions.json")
                ?? new List<Transaction>();

        private void Save() => Utils.SaveToFile("transactions.json", transactions);
    }

    public class CategoryService
    {
        private List<Category> categories = new()
        {
            new Category { Name = "Ghaza" },
            new Category { Name = "Haml o Naghl" },
            new Category { Name = "Ghaboz" },
            new Category { Name = "Kharid" },
            new Category { Name = "Sargarmi" },
            new Category { Name = "Sayer" },
        };

        public CategoryService() => Load();

        public void AddCategory(string name)
        {
            if (!categories.Any(c => c.Name == name))
            {
                categories.Add(new Category { Name = name });
                Save();
            }
        }

        public List<Category> GetAll() => categories;

        private void Load()
        {
            var data = Utils.LoadFromFile<List<Category>>("categories.json");
            if (data != null)
                categories = data;
        }

        private void Save() => Utils.SaveToFile("categories.json", categories);
    }

    // -----------------Main Program---------------------
    class Program
    {
        static UserService userService = new();
        static TransactionService transactionService = new();
        static CategoryService categoryService = new();

        static void Main()
        {
            while (true)
            {
                Console.Clear();
                Console.WriteLine("=== Personal Finance Manager ===");
                Console.WriteLine("1. Modiriyat Karbaran");
                Console.WriteLine("2. Modiriyat Tarakonesh-ha");
                Console.WriteLine("3. Daste-bandi-ha");
                Console.WriteLine("4. Gozaresh-giri");
                Console.WriteLine("5. Khorooj");

                var choice = Console.ReadLine();
                switch (choice)
                {
                    case "1":
                        ManageUsers();
                        break;
                    case "2":
                        ManageTransactions();
                        break;
                    case "3":
                        ManageCategories();
                        break;
                    case "4":
                        ShowReports();
                        break;
                    case "5":
                        return;
                }
            }
        }

        static void ManageUsers()
        {
            Console.Clear();
            Console.WriteLine("-- Modiriyat Karbaran --");
            Console.WriteLine("1. Ezafe kardan Karbar Jadid");
            Console.WriteLine("2. Namayesh Karbaran");
            Console.WriteLine("3. Taghir Karbar faal");
            var choice = Console.ReadLine();
            switch (choice)
            {
                case "1":
                    Console.Write("Nam Karbari: ");
                    var username = Console.ReadLine();
                    Console.Write("Email: ");
                    var email = Console.ReadLine();
                    userService.AddUser(
                        new User
                        {
                            Username = username,
                            Email = email,
                            Active = false,
                        }
                    );
                    break;
                case "2":
                    foreach (var user in userService.GetAllUsers())
                        Console.WriteLine(
                            $"username : {user.Username} --- email : {user.Email} --- active : {user.Active}"
                        );
                    Console.ReadKey();
                    break;
                case "3":
                    Console.Write("Email Karbar: ");
                    var activeEmail = Console.ReadLine();
                    Console.Write("Vaziat Karbar: ");
                    var statusUser = Console.ReadLine();
                    bool status = false;
                    if (statusUser == "true")
                    {
                        status = true;
                    }
                    else
                    {
                        status = false;
                    }
                    userService.UpdateUserActive(activeEmail, status);
                    Console.ReadKey();
                    break;
            }
        }

        static void ManageTransactions()
        {
            Console.Clear();
            Console.WriteLine("-- Modiriyat Tarakonesh-ha --");
            Console.WriteLine("1. Sabt Daramad");
            Console.WriteLine("2. Sabt Hazine");
            Console.WriteLine("3. Namayesh hame Tarakonesh-ha");
            Console.WriteLine("4. Hazf Tarakonesh");
            var choice = Console.ReadLine();
            switch (choice)
            {
                case "1":
                    Console.Write("Mablagh: ");
                    var incomeAmount = decimal.Parse(Console.ReadLine());
                    Console.Write("Manba: ");
                    var source = Console.ReadLine();
                    Console.Write("Tozihat: ");
                    var incomeDesc = Console.ReadLine();
                    transactionService.AddTransaction(
                        new Income
                        {
                            Amount = incomeAmount,
                            Source = source,
                            Date = DateTime.Now,
                            Description = incomeDesc,
                            title = "income",
                            UserEmail = userService.CurrentUserEmail,
                        }
                    );
                    break;
                case "2":
                    Console.Write("Mablagh: ");
                    var expAmount = decimal.Parse(Console.ReadLine());
                    Console.Write("Dastebandi: ");
                    var category = Console.ReadLine();
                    Console.Write("Tozihat: ");
                    var expDesc = Console.ReadLine();
                    transactionService.AddTransaction(
                        new Expense
                        {
                            Amount = expAmount,
                            Category = category,
                            Date = DateTime.Now,
                            Description = expDesc,
                            title = "expence",
                            UserEmail = userService.CurrentUserEmail,
                        }
                    );
                    break;
                case "3":
                    var transactions = transactionService.GetAll(userService.CurrentUserEmail);

                    foreach (var t in transactions)
                        Console.WriteLine(
                            $"ID : {t.Id} --- date : {t.Date.ToShortDateString()} --- amount : {t.Amount} --- title : {t.title} --- description : {t.Description}"
                        );

                    Console.ReadKey();
                    break;
                case "4":
                    Console.Write("ID Tarakonesh: ");
                    var id = Guid.Parse(Console.ReadLine());
                    transactionService.DeleteTransaction(id);
                    break;
            }
        }

        static void ManageCategories()
        {
            Console.Clear();
            Console.WriteLine("-- Modiriyat Dastebandi-ha --");
            Console.WriteLine("1. Namayesh Dastebandi-ha");
            Console.WriteLine("2. Ezafe kardan Dastebandi Jadid");
            var choice = Console.ReadLine();
            switch (choice)
            {
                case "1":
                    foreach (var cat in categoryService.GetAll())
                        Console.WriteLine(cat.Name);
                    Console.ReadKey();
                    break;
                case "2":
                    Console.Write("Nam Dastebandi: ");
                    var name = Console.ReadLine();
                    categoryService.AddCategory(name);
                    break;
            }
        }

        static void ShowReports()
        {
            Console.Clear();
            Console.WriteLine("-- Gozaresh-giri --");
            var transactions = transactionService.GetAll(userService.CurrentUserEmail);

            // Monthly Summary
            var month = DateTime.Now.Month;
            var year = DateTime.Now.Year;
            var monthly = transactions.Where(t => t.Date.Month == month && t.Date.Year == year);
            var incomeTotal = monthly.OfType<Income>().Sum(i => i.Amount);
            var expenseTotal = monthly.OfType<Expense>().Sum(e => e.Amount);
            Console.WriteLine($"Kholase Mah: Daramad: {incomeTotal} | Hazine: {expenseTotal}");

            // Category Summary
            var categorySummary = monthly
                .OfType<Expense>()
                .GroupBy(e => e.Category)
                .Select(g => new { Category = g.Key, Total = g.Sum(x => x.Amount) });
            Console.WriteLine("Hazine bar asas Dastebandi:");
            foreach (var item in categorySummary)
                Console.WriteLine($"{item.Category}: {item.Total}");

            // Daily Balance
            var daily = monthly
                .GroupBy(t => t.Date.Date)
                .Select(g => new
                {
                    Date = g.Key,
                    Balance = g.OfType<Income>().Sum(i => i.Amount)
                        - g.OfType<Expense>().Sum(e => e.Amount),
                })
                .OrderBy(r => r.Date);

            Console.WriteLine("Balance Roozane:");
            foreach (var d in daily)
                Console.WriteLine($"{d.Date.ToShortDateString()}: {d.Balance}");

            // Pie Chart (Income vs Expense)
            Console.WriteLine("\nChart: Daramad vs Hazine");
            decimal total = incomeTotal + expenseTotal;
            if (total == 0)
            {
                Console.WriteLine("Hich Daramad ya Hazine baraye mah mojood nist.");
            }
            else
            {
                int width = 30;
                double incomePercent = (double)incomeTotal / (double)total;
                int incomeChars = (int)(incomePercent * width);
                int expenseChars = width - incomeChars;

                Console.Write("[");

                Console.ForegroundColor = ConsoleColor.Green;
                Console.Write(new string('■', incomeChars));
                Console.ResetColor();

                Console.ForegroundColor = ConsoleColor.Red;
                Console.Write(new string('■', expenseChars));
                Console.ResetColor();

                Console.WriteLine("]");

                Console.WriteLine($"Sabz (Daramad): {incomeTotal} ({incomePercent:P0})");
                Console.WriteLine($"Ghermez (Hazine): {expenseTotal} ({1 - incomePercent:P0})");
            }

            Console.ReadKey();
        }
    }
}
