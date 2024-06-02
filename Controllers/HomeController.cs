using FilmKurdu.Models;
using FilmKurdu.Models.ViewModel;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Web;
using System.Web.Mvc;
using System.Web.Security;
using System.Linq;


namespace FilmKurdu.Controllers
{
    public class HomeController : Controller
    {
        private readonly string _connectionString = ConfigurationManager.ConnectionStrings["DB_Context"].ConnectionString;

        public ActionResult Index()
        {
            return RedirectToAction("HomePage");
        }

        public ActionResult HomePage()
        {
            List<MediaItem> combinedList = new List<MediaItem>();

            List<Movies> movieList = GetAllMovies();
            foreach (var movie in movieList)
            {
                combinedList.Add(CreateMediaItem(movie, "Movie"));
            }

            List<Series> seriesList = GetAllSeries();
            foreach (var series in seriesList)
            {
                combinedList.Add(CreateMediaItem(series, "Series"));
            }

            return View(combinedList);
        }

        [HttpGet]
        public ActionResult Register()
        {
            return View();
        }

        [HttpPost]
        public ActionResult Register(Users model)
        {
            if (ModelState.IsValid)
            {
                if (model.Username.Length < 3 || model.Username.Length > 16)
                {
                    ViewBag.Message = "Username must be between 3 and 16 characters.";
                    return View(model);
                }
                if (model.Password.Length < 8 || model.Password.Length > 32)
                {
                    ViewBag.Message = "Password must be between 8 and 32 characters.";
                    return View(model);
                }

                var existingUser = GetUserByUsernameOrEmail(model.Username, model.Mail);

                if (existingUser != null)
                {
                    ViewBag.Message = "Username or E-Mail already exists.";
                    return View(model);
                }

                if (model.Username.ToLower() == "admin")
                {
                    model.IsAdmin = true;
                }

                CreateUser(model);
                return RedirectToAction("Login", "Home");
            }

            return View(model);
        }

        public ActionResult Login()
        {
            return View();
        }

        [HttpPost]
        public ActionResult Login(string usernameOrEmail, string password)
        {
            if (string.IsNullOrEmpty(usernameOrEmail) || string.IsNullOrEmpty(password))
            {
                ViewBag.Message = "Please fill out all fields.";
                return View();
            }

            if (ModelState.IsValid)
            {
                var user = GetUserByCredentials(usernameOrEmail, password);

                if (user != null)
                {
                    SetUserCookies(user);

                    if (user.Username == "admin")
                    {
                        return RedirectToAction("Index", "Admin");
                    }

                    FormsAuthentication.SetAuthCookie(user.Username, false);
                    return RedirectToAction("HomePage", "Home");
                }
                {
                    ViewBag.Message = "Invalid username or password.";
                }
            }

            return View();
        }

        public ActionResult Logout()
        {
            RemoveUserCookies();
            return RedirectToAction("Login", "Home");
        }

        public ActionResult SearchPage(string searchKeyword)
        {
            if (string.IsNullOrEmpty(searchKeyword))
            {
                return RedirectToAction("HomePage");
            }
            else
            {
                List<Movies> filteredMovies = GetFilteredMovies(searchKeyword);
                List<Series> filteredSeries = GetFilteredSeries(searchKeyword);

                List<MediaItem> combinedList = new List<MediaItem>();

                foreach (var movie in filteredMovies)
                {
                    combinedList.Add(CreateMediaItem(movie, "Movie"));
                }

                foreach (var series in filteredSeries)
                {
                    combinedList.Add(CreateMediaItem(series, "Series"));
                }

                return View(combinedList);
            }
        }


        public ActionResult ProfilePage()
        {
            if (HttpContext.Request.Cookies["UserId"] != null)
            {
                int userId = Convert.ToInt32(HttpContext.Request.Cookies["UserId"].Value);
                var user = GetUserById(userId);
                return View(user);
            }
            else
            {
                return RedirectToAction("Login", "Home");
            }
        }


        #region Database Operations

        private List<Movies> GetAllMovies()
        {
            var movies = new List<Movies>();
            using (var connection = new SqlConnection(_connectionString))
            {
                using (var command = new SqlCommand("GetAllMovies", connection))
                {
                    command.CommandType = CommandType.StoredProcedure;
                    connection.Open();
                    using (var reader = command.ExecuteReader())
                    {
                        while (reader.Read())

                        {
                            movies.Add(new Movies
                            {
                                ID = (int)reader["ID"],
                                Name = reader["Name"].ToString(),
                                Description = reader["Description"].ToString(),
                                Genre = reader["Genre"].ToString(),
                                Genre2 = reader["Genre2"].ToString(),
                                Image = reader["Image"].ToString(),
                                ReleaseDate = (DateTime)reader["ReleaseDate"],
                                Stars = reader["Stars"].ToString(),
                                Score = float.TryParse(reader["Score"].ToString(), out float scoreValue) ? scoreValue : 0.0f
                            });
                        }
                    }
                }
            }
            return movies;
        }

        private List<Series> GetAllSeries()
        {
            var series = new List<Series>();
            using (var connection = new SqlConnection(_connectionString))
            {
                using (var command = new SqlCommand("GetAllSeries", connection))
                {
                    command.CommandType = CommandType.StoredProcedure;
                    connection.Open();
                    using (var reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            series.Add(new Series
                            {
                                ID = (int)reader["ID"],
                                Name = reader["Name"].ToString(),
                                Description = reader["Description"].ToString(),
                                Genre = reader["Genre"].ToString(),
                                Genre2 = reader["Genre2"].ToString(),
                                Image = reader["Image"].ToString(),
                                ReleaseDate = (DateTime)reader["ReleaseDate"],
                                Stars = reader["Stars"].ToString(),
                                Score = float.TryParse(reader["Score"].ToString(), out float scoreValue) ? scoreValue : 0.0f
                            });
                        }
                    }
                }
            }
            return series;
        }

        private List<Movies> GetFilteredMovies(string searchKeyword)
        {
            var filteredMovies = new List<Movies>();
            using (var connection = new SqlConnection(_connectionString))
            {
                using (var command = new SqlCommand("FilterMovies", connection))
                {
                    command.CommandType = CommandType.StoredProcedure;
                    command.Parameters.AddWithValue("@SearchKeyword", "%" + searchKeyword + "%");
                    connection.Open();
                    using (var reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            filteredMovies.Add(new Movies
                            {
                                ID = (int)reader["ID"],
                                Name = reader["Name"].ToString(),
                                Description = reader["Description"].ToString(),
                                Genre = reader["Genre"].ToString(),
                                Genre2 = reader["Genre2"].ToString(),
                                Image = reader["Image"].ToString(),
                                ReleaseDate = (DateTime)reader["ReleaseDate"],
                                Stars = reader["Stars"].ToString(),
                                Score = float.TryParse(reader["Score"].ToString(), out float scoreValue) ? scoreValue : 0.0f
                            });
                        }
                    }
                }
            }
            return filteredMovies;
        }


        private List<Series> GetFilteredSeries(string searchKeyword)
        {
            var filteredSeries = new List<Series>();
            using (var connection = new SqlConnection(_connectionString))
            {
                using (var command = new SqlCommand("FilterSeries", connection))
                {
                    command.CommandType = CommandType.StoredProcedure;
                    command.Parameters.AddWithValue("@SearchKeyword", "%" + searchKeyword + "%");
                    connection.Open();
                    using (var reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            filteredSeries.Add(new Series
                            {
                                ID = (int)reader["ID"],
                                Name = reader["Name"].ToString(),
                                Description = reader["Description"].ToString(),
                                Genre = reader["Genre"].ToString(),
                                Genre2 = reader["Genre2"].ToString(),
                                Image = reader["Image"].ToString(),
                                ReleaseDate = (DateTime)reader["ReleaseDate"],
                                Stars = reader["Stars"].ToString(),
                                Score = float.TryParse(reader["Score"].ToString(), out float scoreValue) ? scoreValue : 0.0f
                            });
                        }
                    }
                }
            }
            return filteredSeries;
        }


        public ActionResult RandomMovie()
        {
            List<MediaItem> combinedList = new List<MediaItem>();

            List<Movies> moviesList = GetAllMovies();
            List<Series> seriesList = GetAllSeries();

            combinedList.AddRange(moviesList.Select(movie => CreateMediaItem(movie, "Movie")));
            combinedList.AddRange(seriesList.Select(series => CreateMediaItem(series, "Series")));

            Random random = new Random();
            int randomIndex = random.Next(0, combinedList.Count);

            MediaItem randomMediaItem = combinedList[randomIndex];
            if (randomMediaItem.Type == "Movie")
            {
                return RedirectToAction("MoviePage", new { movieId = randomMediaItem.ID });
            }
            else if (randomMediaItem.Type == "Series")
            {
                return RedirectToAction("SeriesPage", new { seriesId = randomMediaItem.ID });
            }
            else
            {
                return RedirectToAction("HomePage");
            }
        }


        public ActionResult MoviePage(int? movieId)
        {
            int userId;
            if (HttpContext.Request.Cookies["UserId"] != null)
            {
                userId = Convert.ToInt32(HttpContext.Request.Cookies["UserId"].Value);
                ViewBag.IsInWatchlist = IsMovieInWatchlist(userId, movieId ?? 0);
            }
            else
            {
                return RedirectToAction("Login", "Home");
            }

            if (movieId == null)
            {
                return RedirectToAction("HomePage");
            }

            Movies movie = GetMovieById(movieId.Value);

            if (movie == null)
            {
                return RedirectToAction("HomePage");
            }

            List<Reviews> reviewsList = GetReviewsByMovieId(movieId.Value);
            List<Users> usersList = new List<Users>();

            foreach (var review in reviewsList)
            {
                Users user = GetUserById(review.userID);
                if (user != null)
                {
                    usersList.Add(user);
                }
            }

            ReviewView viewModel = new ReviewView
            {
                Movie = movie,
                Reviews = reviewsList,
                Users = usersList
            };

            return View(viewModel);
        }

        private Movies GetMovieById(int movieId)
        {
            using (var connection = new SqlConnection(_connectionString))
            {
                using (var command = new SqlCommand("GetMovieById", connection))
                {
                    command.CommandType = CommandType.StoredProcedure;
                    command.Parameters.AddWithValue("@MovieID", movieId);
                    connection.Open();
                    using (var reader = command.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            return new Movies
                            {
                                ID = (int)reader["ID"],
                                Name = reader["Name"].ToString(),
                                Description = reader["Description"].ToString(),
                                Genre = reader["Genre"].ToString(),
                                Genre2 = reader["Genre2"].ToString(),
                                Image = reader["Image"].ToString(),
                                ReleaseDate = (DateTime)reader["ReleaseDate"],
                                Stars = reader["Stars"].ToString(),
                                Score = float.TryParse(reader["Score"].ToString(), out float scoreValue) ? scoreValue : 0.0f
                            };
                        }
                    }
                }
            }
            return null;
        }

        private List<Reviews> GetReviewsByMovieId(int movieId)
        {
            var reviewsList = new List<Reviews>();
            using (var connection = new SqlConnection(_connectionString))
            {
                using (var command = new SqlCommand("GetReviewsByMovieId", connection))
                {
                    command.CommandType = CommandType.StoredProcedure;
                    command.Parameters.AddWithValue("@MovieID", movieId);
                    connection.Open();
                    using (var reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            reviewsList.Add(new Reviews
                            {
                                ID = (int)reader["ID"],
                                userID = (int)reader["userID"],
                                movieID = (int)reader["movieID"],
                                ReviewText = reader["Review"].ToString()
                            });
                        }
                    }
                }
            }
            return reviewsList;
        }



        public ActionResult SeriesPage(int? seriesId)
        {
            if (seriesId == null)
            {
                return RedirectToAction("HomePage");
            }

            Series series = GetSeriesById(seriesId.Value);

            if (series == null)
            {
                return RedirectToAction("HomePage");
            }

            List<Reviews2> reviewsList = GetReviewsBySeriesId(seriesId.Value);
            List<Users> usersList = new List<Users>();

            foreach (var review in reviewsList)
            {
                Users user = GetUserById(review.userID);
                if (user != null)
                {
                    usersList.Add(user);
                }
            }

            ReviewView2 viewModel = new ReviewView2
            {
                Series = series,
                Reviews = reviewsList,
                Users = usersList
            };

            return View(viewModel);
        }

        private Series GetSeriesById(int seriesId)
        {
            using (var connection = new SqlConnection(_connectionString))
            {
                using (var command = new SqlCommand("GetSeriesById", connection))
                {
                    command.CommandType = CommandType.StoredProcedure;
                    command.Parameters.AddWithValue("@SeriesID", seriesId);
                    connection.Open();
                    using (var reader = command.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            return new Series
                            {
                                ID = (int)reader["ID"],
                                Name = reader["Name"].ToString(),
                                Description = reader["Description"].ToString(),
                                Genre = reader["Genre"].ToString(),
                                Genre2 = reader["Genre2"].ToString(),
                                Image = reader["Image"].ToString(),
                                ReleaseDate = (DateTime)reader["ReleaseDate"],
                                Stars = reader["Stars"].ToString(),
                                Score = float.TryParse(reader["Score"].ToString(), out float scoreValue) ? scoreValue : 0.0f
                            };
                        }
                    }
                }
            }
            return null;
        }

        private List<Reviews2> GetReviewsBySeriesId(int seriesId)
        {
            var reviewsList = new List<Reviews2>();
            using (var connection = new SqlConnection(_connectionString))
            {
                using (var command = new SqlCommand("GetReviewsBySeriesId", connection))
                {
                    command.CommandType = CommandType.StoredProcedure;
                    command.Parameters.AddWithValue("@SeriesID", seriesId);
                    connection.Open();
                    using (var reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            reviewsList.Add(new Reviews2
                            {
                                ID = (int)reader["ID"],
                                userID = (int)reader["userID"],
                                seriesID = (int)reader["seriesID"],
                                ReviewText = reader["Review"].ToString()
                            });
                        }
                    }
                }
            }
            return reviewsList;
        }



        private Users GetUserByUsernameOrEmail(string username, string email)
        {
            using (var connection = new SqlConnection(_connectionString))
            {
                using (var command = new SqlCommand("GetUserByUsernameOrEmail", connection))
                {
                    command.CommandType = CommandType.StoredProcedure;
                    command.Parameters.AddWithValue("@Username", username);
                    command.Parameters.AddWithValue("@Email", email);
                    connection.Open();
                    using (var reader = command.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            return new Users
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
            return null;
        }



        public ActionResult Watchlist()
        {
            if (HttpContext.Request.Cookies["UserId"] != null)
            {
                int userId = Convert.ToInt32(HttpContext.Request.Cookies["UserId"].Value);
                List<Movies> movies = GetMoviesByUserId(userId);
                return View(movies);
            }
            else
            {
                return RedirectToAction("Login", "Home");
            }
        }

        public ActionResult AddWatchList(int? movieId)
        {
            if (HttpContext.Request.Cookies["UserId"] != null && movieId.HasValue)
            {
                int userId = Convert.ToInt32(HttpContext.Request.Cookies["UserId"].Value);
                bool isAlreadyAdded = IsMovieInWatchlist(userId, movieId.Value);

                if (isAlreadyAdded)
                {
                    ViewBag.Message = "This movie is already on your list.";
                }
                else
                {
                    AddMovieToWatchlist(userId, movieId.Value);
                }
                ViewBag.IsInWatchlist = IsMovieInWatchlist(userId, movieId??0);
                return RedirectToAction("MoviePage", "Home", new { movieId = movieId.Value });
            }
            else
            {
                return RedirectToAction("Login", "Home");
            }
        }

        public ActionResult RemoveFromWatchList(int? movieId)
        {
            if (HttpContext.Request.Cookies["UserId"] != null && movieId.HasValue)
            {
                int userId = Convert.ToInt32(HttpContext.Request.Cookies["UserId"].Value);
                RemoveMovieFromWatchlist(userId, movieId.Value);
            }
            return RedirectToAction("MoviePage", "Home", new { movieId = movieId.Value });
        }


        private List<Movies> GetMoviesByUserId(int userId)
        {
            var movies = new List<Movies>();
            using (var connection = new SqlConnection(_connectionString))
            {
                using (var command = new SqlCommand("GetMoviesByUserId", connection))
                {
                    command.CommandType = CommandType.StoredProcedure;
                    command.Parameters.AddWithValue("@UserId", userId);
                    connection.Open();
                    using (var reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            movies.Add(new Movies
                            {
                                ID = (int)reader["ID"],
                                Name = reader["Name"].ToString(),
                                Image = reader["Image"].ToString(),
                            });
                        }
                    }
                }
            }
            return movies;
        }

        private bool IsMovieInWatchlist(int userId, int movieId)
        {
            using (var connection = new SqlConnection(_connectionString))
            {
                using (var command = new SqlCommand("IsMovieInWatchlist", connection))
                {
                    command.CommandType = CommandType.StoredProcedure;
                    command.Parameters.AddWithValue("@UserId", userId);
                    command.Parameters.AddWithValue("@MovieId", movieId);
                    connection.Open();
                    int count = (int)command.ExecuteScalar();
                    return count > 0;
                }
            }
        }

        private void AddMovieToWatchlist(int userId, int movieId)
        {
            using (var connection = new SqlConnection(_connectionString))
            {
                using (var command = new SqlCommand("AddMovieToWatchlist", connection))
                {
                    command.CommandType = CommandType.StoredProcedure;
                    command.Parameters.AddWithValue("@UserId", userId);
                    command.Parameters.AddWithValue("@MovieId", movieId);
                    connection.Open();
                    command.ExecuteNonQuery();
                }
            }
        }

        private void RemoveMovieFromWatchlist(int userId, int movieId)
        {
            using (var connection = new SqlConnection(_connectionString))
            {
                using (var command = new SqlCommand("RemoveMovieFromWatchlist", connection))
                {
                    command.CommandType = CommandType.StoredProcedure;
                    command.Parameters.AddWithValue("@UserId", userId);
                    command.Parameters.AddWithValue("@MovieId", movieId);
                    connection.Open();
                    command.ExecuteNonQuery();
                }
            }
        }



        public ActionResult Reviews()
        {
            if (HttpContext.Request.Cookies["UserId"] != null)
            {
                int userId = Convert.ToInt32(HttpContext.Request.Cookies["UserId"].Value);
                var userReviews = GetReviewsByUserId(userId);

                return View(userReviews);
            }
            else
            {
                return RedirectToAction("Login");
            }
        }

        public ActionResult Reviews2()
        {
            if (HttpContext.Request.Cookies["UserId"] != null)
            {
                int userId = Convert.ToInt32(HttpContext.Request.Cookies["UserId"].Value);
                var userReviews = GetReviewsByUserId2(userId);

                return View(userReviews);
            }
            else
            {
                return RedirectToAction("Login");
            }
        }

        [HttpPost]
        public ActionResult AddReview(Reviews rev)
        {
            if (HttpContext.Request.Cookies["UserId"] != null)
            {
                if (!string.IsNullOrEmpty(rev.Review))
                {
                    int userId = Convert.ToInt32(HttpContext.Request.Cookies["UserId"].Value);
                    AddReviewToDb(userId, rev.movieID, rev.Review);

                    return RedirectToAction("MoviePage", new { movieId = rev.movieID });
                }
                else
                {
                    ViewBag.Error = "You have to type something.";
                    return RedirectToAction("MoviePage", new { movieId = rev.movieID });
                }
            }
            else
            {
                return RedirectToAction("Login", "Home");
            }
        }


        [HttpPost]
        public ActionResult AddReview2(Reviews2 rev)
        {
            if (HttpContext.Request.Cookies["UserId"] != null)
            {
                if (!string.IsNullOrEmpty(rev.Review))
                {
                    int userId = Convert.ToInt32(HttpContext.Request.Cookies["UserId"].Value);
                    AddReviewToDb2(userId, rev.seriesID, rev.Review);

                    return RedirectToAction("SeriesPage", new { seriesId = rev.seriesID });
                }
                else
                {
                    ViewBag.Error = "You have to type something.";
                    return RedirectToAction("SeriesPage", new { seriesId = rev.seriesID });
                }
            }
            else
            {
                return RedirectToAction("Login", "Home");
            }
        }

        public ActionResult DeleteReview(int? reviewId)
        {
            if (HttpContext.Request.Cookies["UserId"] != null && reviewId.HasValue)
            {
                int userId = Convert.ToInt32(HttpContext.Request.Cookies["UserId"].Value);

                if (DeleteReviewFromDb(reviewId.Value, userId))
                {
                    TempData["SuccessMessage"] = "Your review successfully removed.";
                }
                else
                {
                    TempData["ErrorMessage"] = "Can't find your review :(";
                }

                return RedirectToAction("Reviews");
            }
            else
            {
                return RedirectToAction("Login");
            }
        }

        #region Database Operations

        private List<ReviewViewModel> GetReviewsByUserId(int userId)
        {
            var reviewsList = new List<ReviewViewModel>();

            using (var connection = new SqlConnection(_connectionString))
            {
                using (var command = new SqlCommand("GetReviewsByUserId", connection))
                {
                    command.CommandType = CommandType.StoredProcedure;
                    command.Parameters.AddWithValue("@UserId", userId);

                    connection.Open();
                    using (var reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            var review = new Reviews
                            {
                                ID = (int)reader["ID"],
                                userID = (int)reader["userID"],
                                movieID = (int)reader["movieID"],
                                Review = reader["Review"].ToString()
                            };

                            var movie = new Movies
                            {
                                ID = (int)reader["ID"],
                                Name = reader["Name"].ToString(),
                                Description = reader["Description"].ToString(),
                                Genre = reader["Genre"].ToString(),
                                Genre2 = reader["Genre2"].ToString(),
                                Image = reader["Image"].ToString(),
                                ReleaseDate = (DateTime)reader["ReleaseDate"],
                                Stars = reader["Stars"].ToString(),
                                Score = float.TryParse(reader["Score"].ToString(), out float scoreValue) ? scoreValue : 0.0f
                            };
                            reviewsList.Add(new ReviewViewModel { Review = review, Movie = movie,});
                        }
                    }
                }
            }

            return reviewsList;
        }

        private void AddReviewToDb(int userId, int movieId, string reviewText)
        {
            using (var connection = new SqlConnection(_connectionString))
            {
                using (var command = new SqlCommand("AddReview", connection))
                {
                    command.CommandType = CommandType.StoredProcedure;
                    command.Parameters.AddWithValue("@UserId", userId);
                    command.Parameters.AddWithValue("@MovieId", movieId);
                    command.Parameters.AddWithValue("@ReviewText", reviewText);

                    connection.Open();
                    command.ExecuteNonQuery();
                }
            }
        }


        private void AddReviewToDb2(int userId, int seriesId, string reviewText)
        {
            using (var connection = new SqlConnection(_connectionString))
            {
                using (var command = new SqlCommand("AddReview2", connection))
                {
                    command.CommandType = CommandType.StoredProcedure;
                    command.Parameters.AddWithValue("@UserId", userId);
                    command.Parameters.AddWithValue("@SeriesId", seriesId);
                    command.Parameters.AddWithValue("@ReviewText", reviewText);

                    connection.Open();
                    command.ExecuteNonQuery();
                }
            }
        }

        private bool DeleteReviewFromDb(int reviewId, int userId)
        {
            using (var connection = new SqlConnection(_connectionString))
            {
                using (var command = new SqlCommand("DeleteReview", connection))
                {
                    command.CommandType = CommandType.StoredProcedure;
                    command.Parameters.AddWithValue("@ReviewId", reviewId);
                    command.Parameters.AddWithValue("@UserId", userId);

                    connection.Open();
                    return command.ExecuteNonQuery() > 0;
                }
            }
        }



        private List<ReviewViewModel2> GetReviewsByUserId2(int userId)
        {
            var reviewsList = new List<ReviewViewModel2>();

            using (var connection = new SqlConnection(_connectionString))
            {
                using (var command = new SqlCommand("GetReviewsByUserId2", connection))
                {
                    command.CommandType = CommandType.StoredProcedure;
                    command.Parameters.AddWithValue("@UserId", userId);

                    connection.Open();
                    using (var reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            var review = new Reviews2
                            {
                                ID = (int)reader["ID"],
                                userID = (int)reader["userID"],
                                seriesID = (int)reader["seriesID"],
                                Review = reader["Review"].ToString()
                            };

                            var series = new Series
                            {
                                ID = (int)reader["ID"],
                                Name = reader["Name"].ToString(),
                                Description = reader["Description"].ToString(),
                                Genre = reader["Genre"].ToString(),
                                Genre2 = reader["Genre2"].ToString(),
                                Image = reader["Image"].ToString(),
                                ReleaseDate = (DateTime)reader["ReleaseDate"],
                                Stars = reader["Stars"].ToString(),
                                Score = float.TryParse(reader["Score"].ToString(), out float scoreValue) ? scoreValue : 0.0f
                            };
                            reviewsList.Add(new ReviewViewModel2 { Review = review, Series = series,});
                        }
                    }
                }
            }

            return reviewsList;
        }




        private Users GetUserByCredentials(string usernameOrEmail, string password)
        {
            using (var connection = new SqlConnection(_connectionString))
            {
                using (var command = new SqlCommand("GetUserByCredentials", connection))
                {
                    command.CommandType = CommandType.StoredProcedure;
                    command.Parameters.AddWithValue("@UsernameOrEmail", usernameOrEmail);
                    command.Parameters.AddWithValue("@Password", password);
                    connection.Open();
                    using (var reader = command.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            return new Users
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
            return null;
        }
        private Users GetUserById(int userId)
        {
            using (var connection = new SqlConnection(_connectionString))
            {
                using (var command = new SqlCommand("GetUserById", connection))
                {
                    command.CommandType = CommandType.StoredProcedure;
                    command.Parameters.AddWithValue("@UserID", userId);
                    connection.Open();
                    using (var reader = command.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            return new Users
                            {
                                ID = (int)reader["ID"],
                                Username = reader["Username"].ToString(),
                                Mail = reader["Mail"].ToString(),
                                Image = reader["Image"].ToString()
                            };
                        }
                    }
                }
            }
            return null;
        }

        private void CreateUser(Users user)
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

        [HttpPost]
        public JsonResult ChangePassword(int userId, string newPassword)
        {
            try
            {
                using (SqlConnection connection = new SqlConnection(_connectionString))
                {
                    using (SqlCommand command = new SqlCommand("ChangePassword", connection))
                    {
                        command.CommandType = CommandType.StoredProcedure;
                        command.Parameters.AddWithValue("@UserID", userId);
                        command.Parameters.AddWithValue("@Password", newPassword);
                        connection.Open();
                        command.ExecuteNonQuery();
                    }
                }

                return Json(new { success = true });
            }
            catch (Exception)
            {
                return Json(new { success = false, message = "Passwords must be between 8 and 32 characters in length and include at least one special character." });
            }
        }



        [HttpPost]
        public JsonResult ChangeImage(int userId, string newImage)
        {
            try
            {
                using (SqlConnection connection = new SqlConnection(_connectionString))
                {
                    using (SqlCommand command = new SqlCommand("ChangeImage", connection))
                    {
                        command.CommandType = CommandType.StoredProcedure;
                        command.Parameters.AddWithValue("@UserID", userId);
                        command.Parameters.AddWithValue("@Image", newImage);
                        connection.Open();
                        command.ExecuteNonQuery();
                    }
                }

                return Json(new { success = true });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = ex.ToString() });
            }
        }


        public ActionResult Movies()
        {
            List<Movies> moviesList = GetAllMovies();
            return View(moviesList);
        }

        public ActionResult Series()
        {
            List<Series> seriesList = GetAllSeries();
            return View(seriesList);
        }



        public ActionResult Categories(string genre1, string genre2)
        {
            List<Movies> filteredMovies;
            List<Series> filteredSeries;

            List<MediaItem> combinedList = new List<MediaItem>();

            if (string.IsNullOrEmpty(genre1) && string.IsNullOrEmpty(genre2))
            {
                filteredMovies = GetAllMovies();
                filteredSeries = GetAllSeries();
            }
            else
            {
                filteredMovies = GetMoviesByGenres(genre1, genre2);
                filteredSeries = GetSeriesByGenres(genre1, genre2);
            }

            foreach (var movie in filteredMovies)
            {
                MediaItem mediaItem = new MediaItem
                {
                    Type = "Movie",
                    ID = movie.ID,
                    Name = movie.Name,
                    Description = movie.Description,
                    Genre = movie.Genre,
                    Genre2 = movie.Genre2,
                    Image = movie.Image,
                    ReleaseDate = movie.ReleaseDate.ToString("dd MMM yyyy"),
                    Stars = movie.Stars,
                    Score = movie.Score
                };

                combinedList.Add(mediaItem);
            }

            foreach (var series in filteredSeries)
            {
                MediaItem mediaItem = new MediaItem
                {
                    Type = "Series",
                    ID = series.ID,
                    Name = series.Name,
                    Description = series.Description,
                    Genre = series.Genre,
                    Genre2 = series.Genre2,
                    Image = series.Image,
                    ReleaseDate = series.ReleaseDate.ToString("dd MMM yyyy"),
                    Stars = series.Stars,
                    Score = series.Score
                };

                combinedList.Add(mediaItem);
            }

            return View(combinedList);
        }


        public List<Movies> GetMoviesByGenres(string genre1, string genre2)
        {
            using (var connection = new SqlConnection(_connectionString))
            {
                var command = new SqlCommand("GetMoviesByGenres", connection)
                {
                    CommandType = CommandType.StoredProcedure
                };
                command.Parameters.AddWithValue("@Genre1", (object)genre1 ?? DBNull.Value);
                command.Parameters.AddWithValue("@Genre2", (object)genre2 ?? DBNull.Value);

                connection.Open();
                var reader = command.ExecuteReader();
                var movies = new List<Movies>();
                while (reader.Read())
                {
                    movies.Add(new Movies
                    {
                        ID = (int)reader["ID"],
                        Name = (string)reader["Name"],
                        Genre = (string)reader["Genre"],
                        Genre2 = (string)reader["Genre2"],
                        ReleaseDate = (DateTime)reader["ReleaseDate"],
                        Stars = (string)reader["Stars"],
                        Score = float.TryParse(reader["Score"].ToString(), out float scoreValue) ? scoreValue : 0.0f
                    });
                }
                return movies;
            }
        }

        public List<Series> GetSeriesByGenres(string genre1, string genre2)
        {
            using (var connection = new SqlConnection(_connectionString))
            {
                var command = new SqlCommand("GetSeriesByGenres", connection)
                {
                    CommandType = CommandType.StoredProcedure
                };
                command.Parameters.AddWithValue("@Genre1", (object)genre1 ?? DBNull.Value);
                command.Parameters.AddWithValue("@Genre2", (object)genre2 ?? DBNull.Value);

                connection.Open();
                var reader = command.ExecuteReader();
                var series = new List<Series>();
                while (reader.Read())
                {
                    series.Add(new Series
                    {
                        ID = (int)reader["ID"],
                        Name = (string)reader["Name"],
                        Genre = (string)reader["Genre"],
                        Genre2 = (string)reader["Genre2"],
                        ReleaseDate = (DateTime)reader["ReleaseDate"],
                        Stars = (string)reader["Stars"],
                        Score = float.TryParse(reader["Score"].ToString(), out float scoreValue) ? scoreValue : 0.0f
                    });
                }
                return series;
            }
        }


        public ActionResult CheckTableChanges()
        {
            bool hasChanges = false;

            int previousCount = 0;
            if (System.IO.File.Exists(Server.MapPath("~/App_Data/MovieCount.txt")))
            {
                previousCount = Convert.ToInt32(System.IO.File.ReadAllText(Server.MapPath("~/App_Data/MovieCount.txt")));
            }

            var movies = GetAllMovies();
            int currentCount = movies.Count;

            if (currentCount != previousCount)
            {
                hasChanges = true;
                System.IO.File.WriteAllText(Server.MapPath("~/App_Data/MovieCount.txt"), currentCount.ToString());
            }

            return Json(hasChanges, JsonRequestBehavior.AllowGet);
        }



        private void SetUserCookies(Users user)
        {
            HttpCookie cookie = new HttpCookie("UserID", user.ID.ToString());
            HttpCookie cookie2 = new HttpCookie("Username", user.Username);
            HttpCookie cookie3 = new HttpCookie("Mail", user.Mail);

            cookie.Expires = DateTime.Now.AddDays(1);
            cookie2.Expires = DateTime.Now.AddDays(1);
            cookie3.Expires = DateTime.Now.AddDays(1);

            Response.Cookies.Add(cookie);
            Response.Cookies.Add(cookie2);
            Response.Cookies.Add(cookie3);
        }

        private void RemoveUserCookies()
        {
            Response.Cookies["UserID"].Expires = DateTime.Now.AddDays(-1);
            Response.Cookies["Username"].Expires = DateTime.Now.AddDays(-1);
            Response.Cookies["Mail"].Expires = DateTime.Now.AddDays(-1);
        }

        private MediaItem CreateMediaItem(dynamic item, string type)
        {
            return new MediaItem
            {
                Type = type,
                ID = item.ID,
                Name = item.Name,
                Description = item.Description,
                Genre = item.Genre,
                Genre2 = item.Genre2,
                Image = item.Image,
                ReleaseDate = item.ReleaseDate.ToString("dd MMM yyyy"),
                Stars = item.Stars,
                Score = item.Score
            };
        }

        #endregion
    }
} 
#endregion