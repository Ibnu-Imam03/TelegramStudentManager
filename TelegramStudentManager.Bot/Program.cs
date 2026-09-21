using System.Net.Http.Json;

using Telegram.Bot;
using Telegram.Bot.Polling;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;
using Telegram.Bot.Types.ReplyMarkups;

// ============================================================
// BOT CONFIGURATION
// ============================================================

var botToken = "8887991036:AAHUBWKI7j188zqIefWvj1TBYTBdHFlfsGw";

var bot = new TelegramBotClient(botToken);

var httpClient = new HttpClient();

// ============================================================
// BOT START
// ============================================================

var me = await bot.GetMe();

Console.WriteLine($"Bot started: @{me.Username}");
Console.WriteLine("Waiting for messages...");

using CancellationTokenSource cts = new();

bot.StartReceiving(
    updateHandler: HandleUpdateAsync,
    errorHandler: HandleErrorAsync,
    receiverOptions: new ReceiverOptions
    {
        AllowedUpdates = []
    },
    cancellationToken: cts.Token
);

Console.WriteLine("Press Enter to stop the bot.");

await Task.Delay(Timeout.Infinite, cts.Token);


// ============================================================
// HANDLE UPDATE
// ============================================================

async Task HandleUpdateAsync(
    ITelegramBotClient bot,
    Update update,
    CancellationToken cancellationToken)
{
    try
    {
        // ====================================================
        // CALLBACK QUERY
        // ====================================================

        if (update.CallbackQuery != null)
        {
            await HandleStudentButton(
                bot,
                update.CallbackQuery,
                cancellationToken);

            return;
        }

        // ====================================================
        // MESSAGE
        // ====================================================

        if (update.Message == null)
            return;

        if (update.Message.Text == null)
            return;

        string messageText =
            update.Message.Text.Trim();

        // ====================================================
        // /start
        // ====================================================

        if (messageText == "/start")
        {
            string miniAppUrl =
                "https://marshy-mural-feel.ngrok-free.dev";

            InlineKeyboardMarkup keyboard =
                new InlineKeyboardMarkup(
                    InlineKeyboardButton.WithWebApp(
                        "🚀 Open Student Manager",
                        new WebAppInfo
                        {
                            Url = miniAppUrl
                        }
                    )
                );

            await bot.SendMessage(
                chatId: update.Message.Chat.Id,
                text:
                    "👋 <b>Welcome to Student Manager!</b>\n\n" +
                    "Manage your students easily from Telegram.\n\n" +
                    "Tap the button below to get started.",
                parseMode: ParseMode.Html,
                replyMarkup: keyboard,
                cancellationToken: cancellationToken
            );

            return;
        }

        // ====================================================
        // /list
        // ====================================================

        if (messageText == "/list")
        {
            await GetStudents(
                bot,
                update.Message.Chat.Id,
                cancellationToken);

            return;
        }
    }
    catch (Exception ex)
    {
        Console.WriteLine(
            $"Update Error: {ex.Message}");
    }
}


// ============================================================
// GET STUDENTS
// ============================================================

async Task GetStudents(
    ITelegramBotClient bot,
    long chatId,
    CancellationToken cancellationToken)
{
    try
    {
        string apiUrl =
            "https://telegram-student-manager-1.onrender.com/api/Students";

        List<Student>? students =
            await httpClient.GetFromJsonAsync<List<Student>>(
                apiUrl,
                cancellationToken);

        if (students == null || students.Count == 0)
        {
            await bot.SendMessage(
                chatId: chatId,
                text: "❌ No students found.",
                cancellationToken: cancellationToken
            );

            return;
        }

        List<List<InlineKeyboardButton>> buttons =
            new List<List<InlineKeyboardButton>>();

        foreach (Student student in students)
        {
            buttons.Add(
                new List<InlineKeyboardButton>
                {
                    InlineKeyboardButton.WithCallbackData(
                        $"👤 {student.FullName}",
                        $"student_{student.StudentID}")
                });
        }

        InlineKeyboardMarkup keyboard =
            new InlineKeyboardMarkup(buttons);

        await bot.SendMessage(
            chatId: chatId,
            text:
                "📚 <b>Students</b>\n\n" +
                "Select a student:",
            parseMode: ParseMode.Html,
            replyMarkup: keyboard,
            cancellationToken: cancellationToken
        );
    }
    catch (Exception ex)
    {
        Console.WriteLine(
            $"API Error: {ex.Message}");

        await bot.SendMessage(
            chatId: chatId,
            text:
                "❌ Could not connect to the Student API.",
            cancellationToken: cancellationToken
        );
    }
}


// ============================================================
// HANDLE STUDENT BUTTON
// ============================================================

async Task HandleStudentButton(
    ITelegramBotClient bot,
    CallbackQuery callbackQuery,
    CancellationToken cancellationToken)
{
    if (callbackQuery.Data == null)
        return;

    if (!callbackQuery.Data.StartsWith("student_"))
        return;

    string idText =
        callbackQuery.Data.Replace("student_", "");

    if (!int.TryParse(idText, out int studentID))
        return;

    try
    {
        string apiUrl =
            $"https://telegram-student-manager-1.onrender.com/api/Students/{studentID}";

        Student? student =
            await httpClient.GetFromJsonAsync<Student>(
                apiUrl,
                cancellationToken);

        if (student == null)
        {
            await bot.AnswerCallbackQuery(
                callbackQuery.Id,
                "Student not found.",
                cancellationToken: cancellationToken
            );

            return;
        }

        await bot.AnswerCallbackQuery(
            callbackQuery.Id,
            cancellationToken: cancellationToken
        );

        string response =
            "👤 <b>Student Information</b>\n\n" +
            $"🆔 ID: {student.StudentID}\n" +
            $"👤 Name: {student.FullName}\n" +
            $"🎓 Student Number: {student.StudentNumber}\n" +
            $"🏢 Department: {student.Department}\n" +
            $"📅 Year: {student.Year}\n" +
            $"📱 Phone: {student.Phone}";

        await bot.SendMessage(
            chatId: callbackQuery.Message!.Chat.Id,
            text: response,
            parseMode: ParseMode.Html,
            cancellationToken: cancellationToken
        );
    }
    catch (Exception ex)
    {
        Console.WriteLine(
            $"API Error: {ex.Message}");

        await bot.AnswerCallbackQuery(
            callbackQuery.Id,
            "❌ Could not connect to the Student API.",
            cancellationToken: cancellationToken
        );
    }
}


// ============================================================
// ERROR HANDLER
// ============================================================

Task HandleErrorAsync(
    ITelegramBotClient bot,
    Exception exception,
    CancellationToken cancellationToken)
{
    Console.WriteLine(
        $"Telegram Error: {exception.Message}");

    return Task.CompletedTask;
}


// ============================================================
// STUDENT CLASS
// ============================================================

public class Student
{
    public int StudentID { get; set; }

    public string FullName { get; set; }

    public string StudentNumber { get; set; }

    public string Department { get; set; }

    public int Year { get; set; }

    public string Phone { get; set; }
}