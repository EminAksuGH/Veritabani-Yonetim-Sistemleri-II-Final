using FilmKurdu.Models;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Web;
using System.Web.Mvc;
using System.Web.Security;

namespace FilmKurdu.Controllers
{
    public class AdminController : Controller
    {
        private readonly string _connectionString = ConfigurationManager.ConnectionStrings["DB_Context"].ConnectionString;

        public ActionResult Index()
        {
            if (HttpContext.Request.Cookies["UserId"] != null && HttpContext.Request.Cookies["Username"].Value == "admin")
            {
                List<Users> usersList = GetAllUsers();
                return View(usersList);
            }
            else
            {
                return RedirectToAction("Login", "Home");
            }
        }

        public ActionResult CreateUser()
        {
            return View();
        }

        [HttpPost]
        public ActionResult CreateUser(Users user)
        {
            if (ModelState.IsValid)
            {
                CreateUserInDatabase(user);
                return RedirectToAction("Index");
            }
            return View(user);
        }

        public ActionResult UpdateUser1()
        {
            List<Users> usersList = GetAllUsers();
            return View(usersList);
        }

        public ActionResult UpdateUser2(int? userId)
        {
            Users user = GetUserById(userId);
            return View(user);
        }

        [HttpPost]
        public ActionResult UpdateUser(Users updatedUser)
        {
            UpdateUserInDatabase(updatedUser);
            return RedirectToAction("Index", "Admin");
        }

        public ActionResult DeleteUser()
        {
            List<Users> usersList = GetAllUsers();
            return View(usersList);
        }

        public ActionResult DeleteUser2(int? userId)
        {
            DeleteUserFromDatabase(userId);
            return RedirectToAction("Index");
        }

        #region Database Operations

        private List<Users> GetAllUsers()
        {
            var users = new List<Users>();
            using (var connection = new SqlConnection(_connectionString))
            {
                using (var command = new SqlCommand("GetAllUsers", connection))
                {
                    command.CommandType = CommandType.StoredProcedure;
                    connection.Open();
                    using (var reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            users.Add(new Users
                            {
                                ID = (int)reader["ID"],
                                Username = reader["Username"].ToString(),
                                Mail = reader["Mail"].ToString(),
                                Password = reader["Password"].ToString()
                            });
                        }
                    }
                }
            }
            return users;
        }

        private Users GetUserById(int? userId)
        {
            Users user = null;
            if (userId != null)
            {
                using (var connection = new SqlConnection(_connectionString))
                {
                    using (var command = new SqlCommand("GetUserById", connection))
                    {
                        command.CommandType = CommandType.StoredProcedure;
                        command.Parameters.AddWithValue("@UserId", userId);
                        connection.Open();
                        using (var reader = command.ExecuteReader())
                        {
                            if (reader.Read())
                            {
                                user = new Users
                                {
                                    ID = (int)reader["ID"],
                                    Username = reader["Username"].ToString(),
                                    Mail = reader["Mail"].ToString(),
                                    Password = reader["Password"].ToString()
                                };
                            }
                        }
                    }
                }
            }
            return user;
        }

        private void CreateUserInDatabase(Users user)
        {
            using (var connection = new SqlConnection(_connectionString))
            {
                using (var command = new SqlCommand("CreateUser", connection))
                {
                    command.CommandType = CommandType.StoredProcedure;
                    command.Parameters.AddWithValue("@Username", user.Username);
                    command.Parameters.AddWithValue("@Mail", user.Mail);
                    command.Parameters.AddWithValue("@Password", user.Password);
                    connection.Open();
                    command.ExecuteNonQuery();
                }
            }
        }

        private void UpdateUserInDatabase(Users updatedUser)
        {
            using (var connection = new SqlConnection(_connectionString))
            {
                using (var command = new SqlCommand("UpdateUser", connection))
                {
                    command.CommandType = CommandType.StoredProcedure;
                    command.Parameters.AddWithValue("@UserId", updatedUser.ID);
                    command.Parameters.AddWithValue("@Username", updatedUser.Username);
                    command.Parameters.AddWithValue("@Mail", updatedUser.Mail);
                    command.Parameters.AddWithValue("@Password", updatedUser.Password);
                    connection.Open();
                    command.ExecuteNonQuery();
                }
            }
        }

        private void DeleteUserFromDatabase(int? userId)
        {
            if (userId != null)
            {
                using (var connection = new SqlConnection(_connectionString))
                {
                    using (var command = new SqlCommand("DeleteUser", connection))
                    {
                        command.CommandType = CommandType.StoredProcedure;
                        command.Parameters.AddWithValue("@UserId", userId);
                        connection.Open();
                        command.ExecuteNonQuery();
                    }
                }
            }
        }

        public ActionResult AddMovie()
        {
            return View();
        }

        [HttpPost]
        public ActionResult AddMovie(Movies movie)
        {
            if (ModelState.IsValid)
            {
                AddMoviesToDatabase(movie);
                return RedirectToAction("Index");
            }
            return View(movie);
        }

        private void AddMoviesToDatabase(Movies movie)  
        {
            using (var connection = new SqlConnection(_connectionString))
            {
                using (var command = new SqlCommand("AddMovie", connection))
                {
                    command.CommandType = CommandType.StoredProcedure;
                    command.Parameters.AddWithValue("@Name", movie.Name);
                    command.Parameters.AddWithValue("@Description", movie.Description);
                    command.Parameters.AddWithValue("@Genre", movie.Genre);
                    command.Parameters.AddWithValue("@Genre2", movie.Genre2);
                    command.Parameters.AddWithValue("@Image", movie.Image);
                    command.Parameters.AddWithValue("@ReleaseDate", movie.ReleaseDate);
                    command.Parameters.AddWithValue("@Stars", movie.Stars);
                    command.Parameters.AddWithValue("@Score", movie.Score);
                    connection.Open();
                    command.ExecuteNonQuery();
                }
            }
        }



        public ActionResult AddSeries()
        {
            return View();
        }

        [HttpPost]
        public ActionResult AddSeries(Series series)
        {
            if (ModelState.IsValid)
            {
                AddSeriesToDatabase(series);
                return RedirectToAction("Index");
            }
            return View(series);
        }

        private void AddSeriesToDatabase(Series series)
        {
            using (var connection = new SqlConnection(_connectionString))
            {
                using (var command = new SqlCommand("AddSeries", connection))
                {
                    command.CommandType = CommandType.StoredProcedure;
                    command.Parameters.AddWithValue("@Name", series.Name);
                    command.Parameters.AddWithValue("@Description", series.Description);
                    command.Parameters.AddWithValue("@Genre", series.Genre);
                    command.Parameters.AddWithValue("@Genre2", series.Genre2);
                    command.Parameters.AddWithValue("@Image", series.Image);
                    command.Parameters.AddWithValue("@ReleaseDate", series.ReleaseDate);
                    command.Parameters.AddWithValue("@Stars", series.Stars);
                    command.Parameters.AddWithValue("@Score", series.Score);
                    connection.Open();
                    command.ExecuteNonQuery();
                }
            }
        }
        #endregion
    }
}
