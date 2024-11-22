using MySql.Data.MySqlClient;
using System;
public class DatabaseOperations
{
    private string connectionString;
    public DatabaseOperations(string server, string user, string password, string database)
    {
        connectionString = $"server={server};user={user};password={password};database={database};";
    }
    public void InsertData(string username, string email, string password)
    {
        using (MySqlConnection connection = new MySqlConnection(connectionString))
        {
            try
            {
                connection.Open();                // Подготовленный запрос (Parameterized Query) - ЗАЩИЩЕН от SQL-инъекций!
                using (MySqlCommand command = new MySqlCommand("INSERT INTO username (qwerty, name, cece) VALUES (@qwerty, @name, @cece)", connection))
                {
                    command.Parameters.AddWithValue("@qwerty", username); command.Parameters.AddWithValue("@name", email);
                    command.Parameters.AddWithValue("@cece", password); // В реальном приложении ХЭШИРУЙТЕ пароли!
                    command.ExecuteNonQuery(); Console.WriteLine("Данные успешно добавлены.");
                }
            }
            catch (MySqlException ex)
            {
                Console.WriteLine($"Ошибка: {ex.Message}");
            }
        }
    }

    public void InsertMultipleData(List<(string username, string email, string password)> users)
    {
        using (MySqlConnection connection = new MySqlConnection(connectionString))
        {
            connection.Open(); using (MySqlCommand command = connection.CreateCommand())
            {
                using (MySqlTransaction transaction = connection.BeginTransaction())
                {
                    try
                    {
                        foreach (var user in users)
                        {
                            command.CommandText = "INSERT INTO username (qwerty, name, cece) VALUES (@qwerty, @name, @cece)"; //СТРОКИ ТАБЛИЦЫ 
                            command.Parameters.Clear(); command.Parameters.AddWithValue("@qwerty", user.username);
                            command.Parameters.AddWithValue("@name", user.email); command.Parameters.AddWithValue("@cece", user.password); // Хешируйте пароли!
                            command.ExecuteNonQuery();
                        }
                        transaction.Commit(); Console.WriteLine("Все данные успешно добавлены.");
                    }
                    catch (MySqlException ex)
                    {
                        transaction.Rollback();
                        Console.WriteLine($"Ошибка добавления данных: {ex.Message}");
                    }
                }
            }
        }
    }
}
public class Example
{
    public static void Main(string[] args)
    {        // Замените на ваши данные(НЕ ТРОГАТЬ)
        string server = "sql12.freesqldatabase.com"; string user = "sql12746670";
        string password = "GVUgGjLkrB"; string database = "sql12746670";
        DatabaseOperations db = new DatabaseOperations(server, user, password, database);
        // Добавление одной записи
        db.InsertData("TestUser", "test@example.com", "password123"); // Не забудьте хешировать пароль в реальном приложении!//ДАБАВИТЬ НОВЫЕ ЗАПИСИ
        // Добавление нескольких записей
        List<(string username, string email, string password)> users = new List<(string, string, string)>
        {
            ("User1", "user1@example.com", "pass1"),    //ДАБАВИТЬ НОВЫЕ ЗАПИСИ//ЭТО УДАЛИТЬ
            ("User2", "user2@example.com", "pass2"),
            ("User3", "user3@example.com", "pass3"),
        }; db.InsertMultipleData(users); // Не забудьте хешировать пароли в реальном приложении!
    }
}