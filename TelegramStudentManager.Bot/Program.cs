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
// CHECK BOT CONNECTION
// ============================================================

var me = await bot.GetMe();

Console.WriteLine($"Bot started: @{me.Username}");
Console.WriteLine("Waiting for messages...");


// ============================================================
// CANCELLATION TOKEN
// ============================================================

using CancellationTokenSource cts = new();


// ============================================================
// START RECEIVING TELEGRAM UPDATES
// ============================================================

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


// Keep the bot running
await Task.Delay(Timeout.Infinite, cts.Token);


// ============================================================
// HANDLE TELEGRAM UPDATES
// ============================================================

async Task HandleUpdateAsync(
    ITelegramBotClient bot,
    Update update,
    CancellationToken cancellationToken)
{
    // --------------------------------------------------------
    // HANDLE INLINE BUTTON CLICK
    // --------------------------------------------------------

    if (update.CallbackQuery is { } callbackQuery)
    {
        await HandleStudentButton(
            bot,
            callbackQuery,
            cancellationToken);

        return;
    }


    // --------------------------------------------------------
    // MAKE SURE UPDATE CONTAINS A MESSAGE
    // --------------------------------------------------------

    if (update.Message is not { } message)
        return;


    // --------------------------------------------------------
    // MAKE SURE MESSAGE CONTAINS TEXT
    // --------------------------------------------------------

    if (message.Text is not { } messageText)
        return;


    Console.WriteLine(
        $"Received: {messageText} from {message.Chat.Id}");


    // ========================================================
    // /start
    // ========================================================

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
            chatId: message.Chat.Id,

            text:
                "👋 <b>Welcome to Student Manager!</b>\n\n" +
                "Manage your students easily from Telegram.\n\n" +
                "Tap the button below to get started.",

            parseMode: ParseMode.Html,

            replyMarkup: keyboard,

            cancellationToken: cancellationToken
        );
    }


    // ========================================================
    // /list
    // ========================================================

    else if (messageText == "/list")
    {
        await GetStudents(
            bot,
            message.Chat.Id,
            cancellationToken);
    }
}


// ============================================================
// GET ALL STUDENTS
// ============================================================

async Task GetStudents(
    ITelegramBotClient bot,
    long chatId,
    CancellationToken cancellationToken)
{
    try
    {
        // ----------------------------------------------------
        // API URL
        // ----------------------------------------------------

        string apiUrl =
            "http://localhost:5266/api/Students";


        // ----------------------------------------------------
        // CALL API
        // ----------------------------------------------------

        List<Student>? students =
            await httpClient.GetFromJsonAsync<List<Student>>(
                apiUrl,
                cancellationToken);


        // ----------------------------------------------------
        // CHECK IF STUDENTS EXIST
        // ----------------------------------------------------

        if (students == null || students.Count == 0)
        {
            await bot.SendMessage(
                chatId: chatId,

                text: "❌ No students found.",

                cancellationToken: cancellationToken
            );

            return;
        }


        // ----------------------------------------------------
        // CREATE INLINE BUTTONS
        // ----------------------------------------------------

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


        // ----------------------------------------------------
        // CREATE KEYBOARD
        // ----------------------------------------------------

        InlineKeyboardMarkup keyboard =
            new InlineKeyboardMarkup(buttons);


        // ----------------------------------------------------
        // SEND STUDENT LIST
        // ----------------------------------------------------

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
    // --------------------------------------------------------
    // CHECK CALLBACK DATA
    // --------------------------------------------------------

    if (callbackQuery.Data == null)
        return;


    if (!callbackQuery.Data.StartsWith("student_"))
        return;


    // --------------------------------------------------------
    // GET STUDENT ID
    // --------------------------------------------------------

    string idText =
        callbackQuery.Data.Replace("student_", "");


    if (!int.TryParse(idText, out int studentID))
        return;


    try
    {
        // ----------------------------------------------------
        // API URL
        // ----------------------------------------------------

        string apiUrl =
            $"http://localhost:5266/api/Students/{studentID}";


        // ----------------------------------------------------
        // GET STUDENT FROM API
        // ----------------------------------------------------

        Student? student =
            await httpClient.GetFromJsonAsync<Student>(
                apiUrl,
                cancellationToken);


        // ----------------------------------------------------
        // CHECK STUDENT
        // ----------------------------------------------------

        if (student == null)
        {
            await bot.AnswerCallbackQuery(
                callbackQuery.Id,

                "Student not found.",

                cancellationToken: cancellationToken
            );

            return;
        }


        // ----------------------------------------------------
        // ANSWER BUTTON CLICK
        // ----------------------------------------------------

        await bot.AnswerCallbackQuery(
            callbackQuery.Id,

            cancellationToken: cancellationToken
        );


        // ----------------------------------------------------
        // CREATE STUDENT INFORMATION
        // ----------------------------------------------------

        string response =
            "👤 <b>Student Information</b>\n\n" +

            $"🆔 ID: {student.StudentID}\n" +

            $"👤 Name: {student.FullName}\n" +

            $"🎓 Student Number: {student.StudentNumber}\n" +

            $"🏢 Department: {student.Department}\n" +

            $"📅 Year: {student.Year}\n" +

            $"📱 Phone: {student.Phone}";


        // ----------------------------------------------------
        // SEND STUDENT INFORMATION
        // ----------------------------------------------------

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
// HANDLE BOT ERRORS
// ============================================================

Task HandleErrorAsync(
    ITelegramBotClient bot,
    Exception exception,
    CancellationToken cancellationToken)
{
    Console.WriteLine(
        $"Bot Error: {exception.Message}");

    return Task.CompletedTask;
}


// ============================================================
// STUDENT MODEL
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
