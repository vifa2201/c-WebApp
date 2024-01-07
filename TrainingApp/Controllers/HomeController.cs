using System.Diagnostics;
using System;
using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.Sqlite;
using TrainingApp.Models;
using TrainingApp.Models.ViewModels;
namespace TrainingApp.Controllers;

public class HomeController : Controller
{
    private readonly ILogger<HomeController> _logger;

    public HomeController(ILogger<HomeController> logger)
    {
        _logger = logger;
    }

//renderar vyn till index
    public IActionResult Index()
    {
        var trainingListViewModel = GetAllTrainings();
        return View(trainingListViewModel);
    }

    public IActionResult Privacy()
    {
        return View();
    }
    //läser data från formuläret 
    public JsonResult ReadForm(int id)
    {
        var todo = GetById(id);
        return Json(todo);
    }
    //Lägger till träning
    public RedirectResult Insert(TrainingModel training)
    {
        //koppling till databas
        using (SqliteConnection con =
        new SqliteConnection("Data Source=db.sqlite"))

        {
            using (var tableCmd = con.CreateCommand())
            {
                //öppnar databas och exekverar insert fråga
                con.Open();
                tableCmd.CommandText = $"INSERT INTO training (Type, Duration, Distance, Comment, Date) VALUES ('{training.Type}', {training.Duration}, {training.Distance}, '{training.Comment}', '{training.Date.ToString("yyyy-MM-dd HH:mm:ss")}')";
                try
                {
                    tableCmd.ExecuteNonQuery();

                    // Meddelande vid lyckad 
                    TempData["SuccessMessage"] = "Träningspass tillagd";
                }

                // felmeddelande
                catch (Exception ex)
                {
                    Console.WriteLine(ex.Message);

                    // Lägger till ett felmeddelande
                    TempData["ErrorMessage"] = "Ett fel uppstod vid försök att lägga till träningspasset.";
                }
            }
            return Redirect("http://localhost:5015/");
        }
    }

    //hämtar alla träning
    internal TrainingViewModel GetAllTrainings()
    {
        //skapar lista utifrån trainingmodel
        List<TrainingModel> trainingList = new();
         //databaskoppling
        using (SqliteConnection con = new SqliteConnection("Data Source=db.sqlite"))
        using (var tableCmd = con.CreateCommand())
        {
            con.Open();
            //sql fråga
            tableCmd.CommandText = "SELECT * FROM training ORDER BY Date DESC";

            using (var reader = tableCmd.ExecuteReader())
            {
                if (reader.HasRows)
                {
                    while (reader.Read())
                    {
                        //skaper träningsmodell av datan
                        trainingList.Add(new TrainingModel
                        {
                            Id = reader.GetInt32(0),
                            Type = reader.GetString(1),
                            Duration = reader.GetInt32(2),
                            Distance = reader.GetInt32(3),
                            Comment = reader.GetString(4)
                        });
                    }
                }
                else
                {
                    // Om det inte finns några rader, returnera en tom lista
                    return new TrainingViewModel { TrainingList = trainingList };
                }
            }
        }
        // Om det inte finns några rader, returnera en tom lista
        return new TrainingViewModel { TrainingList = trainingList };

    }
    //hämta en baserad på id
    internal TrainingModel GetById(int id)
    {
        //initerar ny modell
        TrainingModel training = new TrainingModel();
        //databaskoppling
        using (var connection = new SqliteConnection("Data Source=db.sqlite"))
        {
            connection.Open();
            using (var tableCmd = connection.CreateCommand())
            {
                //häntar träningpass med efterfrågat id
                tableCmd.CommandText = "SELECT * FROM training WHERE Id = @Id";
                tableCmd.Parameters.AddWithValue("@Id", id);

                using (var reader = tableCmd.ExecuteReader())
                {
                    if (reader.HasRows)
                    {
                        reader.Read();
                        training.Id = reader.GetInt32(0);
                        training.Type = reader.GetString(1);
                        training.Duration = reader.GetInt32(2);
                        training.Distance = reader.GetInt32(3);
                        training.Comment = reader.GetString(4);

                        return training;
                    }
                    else
                    {
                        return null; // Returnera null om ingen post hittades
                    }
                }
            }
        }
    }

    //radera träning
    [HttpPost]
    public IActionResult Delete(int id)
    {
        using (SqliteConnection con = new SqliteConnection("Data Source=db.sqlite"))
        using (var tableCmd = con.CreateCommand())
        {
            con.Open();
            //sql fråga med DELETE
            tableCmd.CommandText = "DELETE FROM training WHERE Id = @Id";
            tableCmd.Parameters.AddWithValue("@Id", id);

            try
            {
                //vid lyckad delete förfrågan
                tableCmd.ExecuteNonQuery();


            }
            catch (Exception ex)
            {
                //skriver ut felmeddelande
                Console.WriteLine(ex.Message);
            }
        }

        // Omdirigera till Index efter att posten har raderats
        return RedirectToAction("Index");
    }

    //uppdatera träning: 
    public RedirectResult Update(TrainingModel training)
    {
        using (var con = new SqliteConnection("Data Source=db.sqlite"))
        {
            con.Open();
            using (var tableCmd = con.CreateCommand())
            {

                //sql fråga med Update
                tableCmd.CommandText = "UPDATE training SET Type = @Type, Duration = @Duration, Distance = @Distance, Comment = @Comment WHERE Id = @Id";

                // Parametrar mot SQL injections
                tableCmd.Parameters.AddWithValue("@Type", training.Type);
                tableCmd.Parameters.AddWithValue("@Duration", training.Duration);
                tableCmd.Parameters.AddWithValue("@Distance", training.Distance);
                tableCmd.Parameters.AddWithValue("@Comment", training.Comment);
                tableCmd.Parameters.AddWithValue("@Id", training.Id);


                try
                {
                    //vid lyckad förfrågan
                    tableCmd.ExecuteNonQuery();
                     TempData["SuccessMessage"] = "Träningspasset uppdaterades framgångsrikt!";
                }
                catch (Exception ex)
                {
                    //skriver ut errror
                    Console.WriteLine(ex.Message);
                }
            }
            return Redirect("http://localhost:5015/");
        }
    }

}
