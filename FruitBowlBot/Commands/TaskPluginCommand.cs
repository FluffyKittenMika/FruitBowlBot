using Discord;
using Discord.WebSocket;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TwitchLib;
using TwitchLib.Models.Client;
using MySql.Data;
using MySql.Data.MySqlClient;
using System;

namespace JefBot.Commands
{
    internal class TaskPluginCommand : IPluginCommand
    {
        public string PluginName => "Task";
        public string Command => "task";
		public IEnumerable<string> Help => new[] { "!task add/remove/get {name} {message} [minutes] (repeats a message every X sec (default 5 min))" };
        public IEnumerable<string> Aliases => new string[0];
        public bool Loaded { get; set; } = true;

        public async Task<string> Action(Message message)
        {
            string res = null;
            await Task.Run(() => { res = Handle(message); }).ConfigureAwait(false);
            return res;
        }

        public string Handle(Message message)
        {
            return null;
        }
        
        public string AddTask(string taskname, string taskresult, string channel, int waittime = 500)
        {
            using(MySqlConnection con = new MySqlConnection(Bot.SQLConnectionString))
	        {
                con.Open();
                MySqlCommand cmd = con.CreateCommand();
                cmd.CommandText = @"INSERT INTO `TaskScheduler` (`CHANNEL`, `LASTRUN`, `TASKNAME`, `TASKRESULT`, `NEXTRUNTIME`)
                                    VALUES (@channel, CURRENT_TIMESTAMP, @taskname, @taskresult, @waittime)";
                cmd.Parameters.AddWithValue("@taskname", taskname);
                cmd.Parameters.AddWithValue("@taskresult", taskresult);
                cmd.Parameters.AddWithValue("@channel", channel);
                cmd.Parameters.AddWithValue("@waittime", waittime);
                var res = cmd.ExecuteNonQuery();
                if (res == -1)
	            {
                    return $"Query returned -1, might be an error, task might not have been added";
	            }else
	            {
                    return $"{taskname} has been added.";
	            }
	        }
        }
		

        public string RemoveTask(string taskname, string channel)
        {
            using(MySqlConnection con = new MySqlConnection(Bot.SQLConnectionString))
            {
                con.Open();
                MySqlCommand cmd = con.CreateCommand();
                cmd.CommandText = @"DELETE FROM `TaskScheduler` WHERE `TASKNAME` = @tasknam AND `CHANNEL` = @channel ";
                cmd.Parameters.AddWithValue("@taskname", taskname);
                cmd.Parameters.AddWithValue("@channel", channel);
                var res = cmd.ExecuteNonQuery();
                if (res == -1)
                {
                    return "Could not remove task";
                }else
                {
					return "Task removed";
                }
            }
        }

        public string GetTasks(string channel)
		{
			using (MySqlConnection con = new MySqlConnection(Bot.SQLConnectionString))
			{
				con.Open();
				MySqlCommand cmd = con.CreateCommand();
				cmd.CommandText = @"SELECT * FROM `TaskScheduler` WHERE `CHANNEL` = @channel";
				cmd.Parameters.AddWithValue("@channel", channel);

				//we'll just fetch all info for convinience
				using (MySqlDataReader reader = cmd.ExecuteReader())
				{
					string res = "Result " + Environment.NewLine;
					res += "---------------------------";
					while (reader.Read())
					{
						var ID = reader.GetInt32(reader.GetOrdinal("ID"));
						var LASTRUN = reader.GetTimeSpan(reader.GetOrdinal("LASTRUN"));
						var TASKNAME = reader.GetString(reader.GetOrdinal("TASKNAME"));
						var TASKRESULT = reader.GetString(reader.GetOrdinal("TASKRESULT"));
						var NEXTRUNTIME = reader.GetInt32(reader.GetOrdinal("NEXTRUNTIME"));

						res += $"{ID}: {TASKNAME} : interval {NEXTRUNTIME}{Environment.NewLine}";
					}
					res += "---------------------------";
					return res;
				}
			}
		}



    }
}
