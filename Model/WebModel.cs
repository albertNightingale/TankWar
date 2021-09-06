using System;
using System.Collections.Generic;
using System.Text;

namespace Model
{
    /// <summary>
    /// A simple container class representing one player in one game
    /// </summary>
    public class PlayerModel
    {
        public readonly string Name;
        public readonly int Score;
        public readonly int Accuracy;
        public PlayerModel(string n, int s, int a)
        {
            Name = n;
            Score = s;
            Accuracy = a;
        }
    }


    /// <summary>
    /// A simple container class representing one game and its players
    /// </summary>
    public class GameModel
    {
        public readonly int ID;
        public readonly int Duration;
        private Dictionary<string, PlayerModel> players;

        public GameModel(int id, int d)
        {
            Duration = d;
            players = new Dictionary<string, PlayerModel>();
        }

        /// <summary>
        /// Adds a player to the game, if not contained in the list
        /// </summary>
        /// <param name="name">The player's name</param>
        /// <param name="score">The player's score</param>
        /// <param name="accuracy">The player's accuracy</param>
        public void AddPlayer(string name, int score, int accuracy)
        {
            if (!players.ContainsKey(name))
                players.Add(name, new PlayerModel(name, score, accuracy));
        }

        /// <summary>
        /// Returns the players in this game
        /// </summary>
        /// <returns></returns>
        public Dictionary<string, PlayerModel> GetPlayers()
        {
            return players;
        }

    }

    /// <summary>
    /// A simple container class representing the information about one player's session in one game
    /// </summary>
    public class SessionModel
    {
        public readonly int GameID;
        public readonly int Duration;
        public readonly int Score;
        public readonly int Accuracy;

        public SessionModel(int gid, int dur, int score, int acc)
        {
            GameID = gid;
            Duration = dur;
            Score = score;
            Accuracy = acc;
        }
    }
}
