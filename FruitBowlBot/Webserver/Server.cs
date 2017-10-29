using System;
using System.Net;
using System.Text;
using System.Threading;

namespace FruitBowlBot.Webserver
{
	//fuck this
	[System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1001:TypesThatOwnDisposableFieldsShouldBeDisposable")]
	public class Server 
	{
		private readonly HttpListener _listener = new HttpListener();
		private readonly Func<HttpListenerRequest, string> _response;

		public Server(string[] prefixes, Func<HttpListenerRequest, string> method)
		{
			if (!HttpListener.IsSupported)
				throw new NotSupportedException("Needs Windows XP SP2, Server 2003 or later.");
			
			//sets prefixses
			if (prefixes == null || prefixes.Length == 0)
				throw new ArgumentException("prefixes");
			foreach (string s in prefixes)
			{
				_listener.Prefixes.Add(s);
				Console.WriteLine(s);
			}

			_response = method ?? throw new ArgumentException("method");
			_listener.Start();
		}

		public Server(Func<HttpListenerRequest, string> method, params string[] prefixes)
            : this(prefixes, method) { }


		public void Run()
		{
			ThreadPool.QueueUserWorkItem((o) =>
			{
				Console.WriteLine("Webserver running...");
				try
				{
					while (_listener.IsListening)
					{
						ThreadPool.QueueUserWorkItem((c) =>
						{
							var ctx = c as HttpListenerContext;
							try
							{
								string rstr = _response(ctx.Request);
								byte[] buf = Encoding.UTF8.GetBytes(rstr);
								ctx.Response.ContentLength64 = buf.Length;
								ctx.Response.OutputStream.Write(buf, 0, buf.Length);
							}
							catch { } // suppress any exceptions
							finally
							{
								// always close the stream
								ctx.Response.OutputStream.Close();
							}
						}, _listener.GetContext());
					}
				}
				catch { } // suppress any exceptions
			});
		}

		public static string SendResponse(HttpListenerRequest request)
		{
			if (Convert.ToBoolean(JefBot.Bot.settings["debug"]))
				Console.WriteLine(request.RawUrl);

			return GenerateHTML(request);
		}

		public static string GenerateHTML(HttpListenerRequest req)
		{
			//init output
			string output = "";

			//read index template



			//make plugin list
			string plugins = "";
			foreach (var item in JefBot.Bot._plugins)
			{
				string temp = ""; 
				temp += "<div class=\"plugin\"><p>";
				temp += $"<h1>{item.PluginName}</h1>";
				temp += $"<div class=\"command\">{String.Join("; ",item.Command)}</div>";
				temp += $"<div class=\"aliases\">{String.Join("; ",item.Aliases)}</div>";
				temp += $"{String.Join("; ", item.Help)}";
				temp += "</p></div>";
				plugins += temp;
			}

			//replace correct part with plugins

			output += plugins;
			return output;
		}

		public void Stop()
		{
			_listener.Stop();
			_listener.Close();
		}
	}
}

