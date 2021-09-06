using Model;
using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Text;

namespace Control
{
    public class DBManager
    {
        private static readonly string connection_info = 
            "server=atr.eng.utah.edu;" + 
            "database=cs3500_u1278169;" + 
            "uid=cs3500_u1278169;" + 
            "password=ps9database"; 

        /// <summary>
        /// Gather data for games from database
        /// </summary>
        /// <returns></returns>
        public static Dictionary<int, GameModel> GetGamesData()
        {
            Dictionary<int, GameModel> gamesdata = new Dictionary<int, GameModel>();
            using (MySqlConnection conn = new MySqlConnection(connection_info))
            {
                try
                {
                    // Open a connection
                    conn.Open();

                    // Create a command
                    MySqlCommand command = conn.CreateCommand();
                    command.CommandText = "select Player.player_name, Player.score, Player.accuracy, Duration, GameID from Game " + 
                        " inner join Player on Game.GameID = Player.Game_In";

                    // Execute the command and cycle through the DataReader object
                    using (MySqlDataReader reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            // create objects
                            if (!gamesdata.ContainsKey((int)reader["GameID"])) // if game not yet exist, then create game
                                gamesdata.Add((int)reader["GameID"], new GameModel((int)reader["GameID"], Convert.ToInt32(reader["Duration"])));
                            // add players to the game's list
                            gamesdata[(int)reader["GameID"]].AddPlayer(reader["player_name"].ToString(), (int)reader["score"], (int)reader["Accuracy"]);
                        }
                    }
                }
                catch (Exception e)
                {
                    Console.WriteLine(e.Message);
                }
            }
            return gamesdata; 
        }

        /// <summary>
        /// Gather data for players from database
        /// </summary>
        /// <returns></returns>
        public static List<SessionModel> GetPlayerData(string player_name)
        {
            List<SessionModel> sessiondata = new List<SessionModel>();
            using (MySqlConnection conn = new MySqlConnection(connection_info))
            {
                try
                {
                    // Open a connection
                    conn.Open();

                    // Create a command
                    MySqlCommand command = conn.CreateCommand();
                    command.CommandText = "select GameID, Duration, score, accuracy from Player " +
                        "inner join Game on Game.GameID = Player.Game_In " +
                        "where Player.player_name = \"" +  player_name + "\"";

                    // Execute the command and cycle through the DataReader object
                    using (MySqlDataReader reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            sessiondata.Add(
                                new SessionModel((int)reader["GameID"], 
                                Convert.ToInt32(reader["Duration"]), 
                                (int)reader["score"], 
                                (int)reader["accuracy"])
                            ); 
                        }
                    }
                }
                catch (Exception e)
                {
                    Console.WriteLine(e.Message);
                }
            }
            return sessiondata;
        }

        public static void SaveData(World world)
        {
            using (MySqlConnection conn = new MySqlConnection(connection_info))
            {
                try  // store game to database
                {
                    // Open a connection
                    conn.Open();

                    // Create a command
                    MySqlCommand command = conn.CreateCommand();
                    command.CommandText =
                        "insert into Game(Duration) values(" + ServerController.TimeSpan + ")";

                    command.ExecuteNonQuery();
                }
                catch (Exception e)
                {
                    Console.WriteLine(e.Message);
                }
            }

            int recent_gameid = -1;

            using (MySqlConnection conn = new MySqlConnection(connection_info))
            {
                try  // try to read the id from database  
                {
                    // Open a connection
                    conn.Open();

                    // Create a command
                    MySqlCommand command = conn.CreateCommand();
                    command.CommandText =
                        "SELECT GameID FROM Game ORDER BY GameID DESC LIMIT 1";
                    command.ExecuteNonQuery();

                    // Execute the command and cycle through the DataReader object
                    using (MySqlDataReader reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            recent_gameid = Convert.ToInt32(reader["GameID"]);
                        }
                    }
                }
                catch (Exception e)
                {
                    Console.WriteLine(e.Message);
                }
            }

            using (MySqlConnection conn = new MySqlConnection(connection_info))
            {
                try  // store players to database based on the id
                {
                    // Open a connection
                    conn.Open();

                    lock (world)
                    {
                        foreach (Tanks t in world.TankList.Values)
                        {
                            Console.WriteLine("Saving " + t.Name + " to the database! ");
                            // Create a command
                            MySqlCommand command = conn.CreateCommand();
                            command.CommandText =
                                "insert into Player (player_name, score, accuracy, Game_In) " +
                                "values (\"" + t.Name + "\", " + t.Score + ", " + t.getAccuracy() + ", " + recent_gameid + ")";

                            command.ExecuteNonQuery();
                        }
                    }
                }
                catch (Exception e)
                {
                    Console.WriteLine(e.Message);
                }
            }
        }
    }
}
